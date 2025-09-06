# Guia de Testes de Integração - OData API

## Pré-requisitos

### Instalar Pacote InMemory Database
**Execute este comando na pasta do projeto de testes:**
```bash
cd ODataAPI.Tests
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

## Código Completo da Classe de Testes

**Copie este código completo para seu arquivo `EmpresasIntegrationTests.cs`:**

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace ODataAPI.Tests;

public class EmpresasIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public EmpresasIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_Empresas_DeveRetornarSucesso()
    {
        var response = await _client.GetAsync("/odata/Empresas");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("@odata.context", content);
        Assert.Contains("value", content);
    }

    [Fact]
    public async Task Post_Empresa_DeveSerCriadaComSucesso()
    {
        var empresaRequest = CreateEmpresaRequest("Empresa Teste", "12345678000199");
        var json = JsonSerializer.Serialize(empresaRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/odata/Empresas", content);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var empresa = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        Assert.Equal("Empresa Teste", empresa.GetProperty("Nome").GetString());
        Assert.Equal("12345678000199", empresa.GetProperty("CNPJ").GetString());
    }

    [Fact]
    public async Task Get_EmpresaById_DeveRetornarEmpresaCorreta()
    {
        var empresaId = await CreateEmpresaAndGetId("Empresa Busca", "98765432000188");
        
        var response = await _client.GetAsync($"/odata/Empresas({empresaId})");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var empresa = JsonSerializer.Deserialize<JsonElement>(content);
        
        Assert.Equal(empresaId, empresa.GetProperty("Id").GetInt32());
        Assert.Equal("Empresa Busca", empresa.GetProperty("Nome").GetString());
        Assert.Equal("98765432000188", empresa.GetProperty("CNPJ").GetString());
    }

    [Fact]
    public async Task Put_Empresa_DeveAtualizarTodosOsCampos()
    {
        var empresaId = await CreateEmpresaAndGetId("Empresa Original", "11111111000111");
        
        var empresaAtualizada = CreateEmpresaRequest("Empresa Atualizada", "22222222000122", empresaId);
        var json = JsonSerializer.Serialize(empresaAtualizada);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/odata/Empresas({empresaId})", content);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        await AssertEmpresaWasUpdated(empresaId, "Empresa Atualizada", "22222222000122");
    }

    [Fact]
    public async Task Patch_Empresa_DeveAtualizarApenasPropriedadeEnviada()
    {
        var empresaId = await CreateEmpresaAndGetId("Empresa Patch", "33333333000133");
        
        var patchData = new { Nome = "Nome Atualizado via Patch" };
        var json = JsonSerializer.Serialize(patchData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync($"/odata/Empresas({empresaId})", content);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        await AssertEmpresaProperty(empresaId, "Nome", "Nome Atualizado via Patch");
        await AssertEmpresaProperty(empresaId, "CNPJ", "33333333000133");
    }

    [Fact]
    public async Task Delete_Empresa_DeveRemoverEmpresa()
    {
        var empresaId = await CreateEmpresaAndGetId("Empresa Delete", "44444444000144");
        
        var deleteResponse = await _client.DeleteAsync($"/odata/Empresas({empresaId})");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        
        var getResponse = await _client.GetAsync($"/odata/Empresas({empresaId})");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Get_EmpresaInexistente_DeveRetornarNotFound()
    {
        var response = await _client.GetAsync("/odata/Empresas(99999)");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_EmpresaInexistente_DeveRetornarNotFound()
    {
        var empresaRequest = CreateEmpresaRequest("Empresa Inexistente", "99999999000199", 99999);
        var json = JsonSerializer.Serialize(empresaRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/odata/Empresas(99999)", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_EmpresaInexistente_DeveRetornarNotFound()
    {
        var response = await _client.DeleteAsync("/odata/Empresas(99999)");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static object CreateEmpresaRequest(string nome, string cnpj, int? id = null)
    {
        var empresa = new { Nome = nome, CNPJ = cnpj };
        return id.HasValue ? new { Id = id.Value, Nome = nome, CNPJ = cnpj } : empresa;
    }

    private async Task<int> CreateEmpresaAndGetId(string nome, string cnpj)
    {
        var empresaRequest = CreateEmpresaRequest(nome, cnpj);
        var json = JsonSerializer.Serialize(empresaRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/odata/Empresas", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var empresa = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        return empresa.GetProperty("Id").GetInt32();
    }

    private async Task AssertEmpresaWasUpdated(int empresaId, string expectedNome, string expectedCnpj)
    {
        var response = await _client.GetAsync($"/odata/Empresas({empresaId})");
        var content = await response.Content.ReadAsStringAsync();
        var empresa = JsonSerializer.Deserialize<JsonElement>(content);
        
        Assert.Equal(expectedNome, empresa.GetProperty("Nome").GetString());
        Assert.Equal(expectedCnpj, empresa.GetProperty("CNPJ").GetString());
    }

    private async Task AssertEmpresaProperty(int empresaId, string propertyName, string expectedValue)
    {
        var response = await _client.GetAsync($"/odata/Empresas({empresaId})");
        var content = await response.Content.ReadAsStringAsync();
        var empresa = JsonSerializer.Deserialize<JsonElement>(content);
        
        Assert.Equal(expectedValue, empresa.GetProperty(propertyName).GetString());
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}
```

---

## Como Foi Construído - Explicação Detalhada

### 1. Configuração Base da Classe
```csharp
public class EmpresasIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public EmpresasIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));
        _client = _factory.CreateClient();
    }
}
```
**Por que**: Configura ambiente isolado usando InMemory database para testes rápidos e independentes.

## Testes Implementados

### 1. GET - Listar Empresas
```csharp
[Fact]
public async Task Get_Empresas_DeveRetornarSucesso()
```
**Objetivo**: Verifica se o endpoint GET /odata/Empresas retorna status 200 e estrutura OData válida.
**Por que**: Garante que a API está funcionando e retornando dados no formato OData correto.

### 2. POST - Criar Empresa
```csharp
[Fact]
public async Task Post_Empresa_DeveSerCriadaComSucesso()
```
**Objetivo**: Testa criação de nova empresa via POST, verificando status 201 e dados retornados.
**Por que**: Valida que novas empresas podem ser criadas corretamente através da API.

### 3. GET por ID - Buscar Empresa Específica
```csharp
[Fact]
public async Task Get_EmpresaById_DeveRetornarEmpresaCorreta()
```
**Objetivo**: Verifica se é possível buscar uma empresa específica pelo ID.
**Por que**: Testa a funcionalidade de busca individual, essencial para operações CRUD.

### 4. PUT - Atualização Completa
```csharp
[Fact]
public async Task Put_Empresa_DeveAtualizarTodosOsCampos()
```
**Objetivo**: Testa atualização completa de uma empresa, verificando se todos os campos são atualizados.
**Por que**: Garante que o PUT funciona corretamente substituindo todos os dados da entidade.

### 5. PATCH - Atualização Parcial
```csharp
[Fact]
public async Task Patch_Empresa_DeveAtualizarApenasPropriedadeEnviada()
```
**Objetivo**: Verifica se apenas as propriedades enviadas são atualizadas, mantendo as demais inalteradas.
**Por que**: Testa funcionalidade específica do OData para atualizações parciais eficientes.

### 6. DELETE - Remover Empresa
```csharp
[Fact]
public async Task Delete_Empresa_DeveRemoverEmpresa()
```
**Objetivo**: Testa remoção de empresa e verifica se ela não pode mais ser encontrada.
**Por que**: Completa o CRUD testando a operação de exclusão.

### 7. Cenários de Erro - Recursos Inexistentes
```csharp
[Fact]
public async Task Get_EmpresaInexistente_DeveRetornarNotFound()

[Fact]
public async Task Put_EmpresaInexistente_DeveRetornarNotFound()

[Fact]
public async Task Delete_EmpresaInexistente_DeveRetornarNotFound()
```
**Objetivo**: Verifica se a API retorna 404 (Not Found) para recursos que não existem.
**Por que**: Garante tratamento adequado de erros e comportamento consistente da API.

## Métodos Auxiliares (DRY)

### CreateEmpresaRequest
```csharp
private static object CreateEmpresaRequest(string nome, string cnpj, int? id = null)
```
**Por que**: Elimina duplicação na criação de objetos empresa, centralizando a lógica.

### CreateEmpresaAndGetId
```csharp
private async Task<int> CreateEmpresaAndGetId(string nome, string cnpj)
```
**Por que**: Reutiliza lógica comum de criar empresa e extrair ID para outros testes.

### AssertEmpresaWasUpdated
```csharp
private async Task AssertEmpresaWasUpdated(int empresaId, string expectedNome, string expectedCnpj)
```
**Por que**: Padroniza verificações de atualização, evitando código repetitivo.

### AssertEmpresaProperty
```csharp
private async Task AssertEmpresaProperty(int empresaId, string propertyName, string expectedValue)
```
**Por que**: Permite verificação específica de propriedades individuais de forma reutilizável.

## Executando os Testes

**⚠️ IMPORTANTE: Execute os comandos na pasta do projeto de testes:**
```bash
cd ODataAPI.Tests
```

### Todos os testes
```bash
dotnet test
```

### Com verbosidade para debug
```bash
dotnet test --verbosity normal
```

### Apenas testes de integração
```bash
dotnet test --filter "FullyQualifiedName~EmpresasIntegrationTests"
```

### Executar teste específico
```bash
dotnet test --filter "Get_Empresas_DeveRetornarSucesso"
```

## Configuração do Ambiente

### Program.cs - Detecção de Ambiente de Teste
```csharp
if (string.IsNullOrEmpty(connectionString) || builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
```
**Por que**: Automaticamente usa InMemory database em testes, evitando dependência do PostgreSQL.

## Princípios Aplicados

### Clean Code
- **Nomes descritivos**: Cada teste tem nome que explica exatamente o que faz
- **Métodos pequenos**: Cada teste tem uma única responsabilidade
- **Código limpo**: Sem duplicação, fácil de ler e manter

### KISS (Keep It Simple, Stupid)
- **Lógica simples**: Testes diretos sem complexidade desnecessária
- **Fluxo linear**: Arrange → Act → Assert de forma clara
- **Foco no essencial**: Testa apenas o que é necessário

### DRY (Don't Repeat Yourself)
- **Métodos auxiliares**: Código comum extraído para reutilização
- **Configuração centralizada**: Setup único para todos os testes
- **Zero duplicação**: Cada lógica implementada apenas uma vez

## Benefícios

✅ **Cobertura completa**: Testa todo o CRUD + cenários de erro
✅ **Execução rápida**: InMemory database acelera os testes
✅ **Isolamento**: Cada teste é independente
✅ **Manutenibilidade**: Código limpo e bem estruturado
✅ **Confiabilidade**: Testes reais que validam comportamento da API
✅ **Documentação viva**: Testes servem como documentação do comportamento esperado