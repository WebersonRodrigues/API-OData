using Microsoft.AspNetCore.Hosting;
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
        var empresa = new { Nome = nome, CNPJ = cnpj, Endereco = "Endereco Padrao" };
        return id.HasValue ? new { Id = id.Value, Nome = nome, CNPJ = cnpj, Endereco = "Endereco Padrao" } : empresa;
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