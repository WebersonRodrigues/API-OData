# Exemplos de Teste da API OData

## Como Executar a API

```bash
cd caminho\OData\ODataAPI
dotnet run
```

A API estará disponível em: `https://localhost:5240` (a porta será exibida no console)

## Glossário OData - Palavras Reservadas

### **Operadores de Filtro ($filter)**

| Operador | Significado | Exemplo | Descrição |
|----------|-------------|---------|------------|
| `eq` | **Equal** (igual) | `Preco eq 1000` | Preço igual a 1000 |
| `ne` | **Not Equal** (diferente) | `Categoria ne 'Livros'` | Categoria diferente de 'Livros' |
| `gt` | **Greater Than** (maior que) | `Preco gt 500` | Preço maior que 500 |
| `ge` | **Greater or Equal** (maior ou igual) | `Preco ge 500` | Preço maior ou igual a 500 |
| `lt` | **Less Than** (menor que) | `Preco lt 1000` | Preço menor que 1000 |
| `le` | **Less or Equal** (menor ou igual) | `Preco le 1000` | Preço menor ou igual a 1000 |
| `and` | **E** (operador lógico) | `Preco gt 100 and Preco lt 500` | Preço entre 100 e 500 |
| `or` | **OU** (operador lógico) | `Categoria eq 'Livros' or Categoria eq 'Games'` | Categoria Livros OU Games |
| `not` | **NÃO** (negação) | `not (Preco gt 1000)` | NÃO preço maior que 1000 |

### **Funções de String**

| Função | Significado | Exemplo | Descrição |
|--------|-------------|---------|------------|
| `contains` | **Contém** | `contains(Nome,'Tech')` | Nome contém a palavra 'Tech' |
| `startswith` | **Começa com** | `startswith(Telefone,'(11)')` | Telefone começa com '(11)' |
| `endswith` | **Termina com** | `endswith(Email,'.com')` | Email termina com '.com' |
| `tolower` | **Minúsculo** | `tolower(Nome)` | Converte nome para minúsculo |
| `toupper` | **Maiúsculo** | `toupper(Nome)` | Converte nome para maiúsculo |
| `length` | **Tamanho** | `length(Nome) gt 10` | Nome com mais de 10 caracteres |

### **Funções de Data**

| Função | Significado | Exemplo | Descrição |
|--------|-------------|---------|------------|
| `year` | **Ano** | `year(DataCriacao) eq 2024` | Ano da data de criação é 2024 |
| `month` | **Mês** | `month(DataCriacao) eq 12` | Mês da data de criação é dezembro |
| `day` | **Dia** | `day(DataCriacao) eq 15` | Dia da data de criação é 15 |
| `hour` | **Hora** | `hour(DataCriacao) eq 14` | Hora da data de criação é 14h |
| `minute` | **Minuto** | `minute(DataCriacao) eq 30` | Minuto da data de criação é 30 |

### **Parâmetros de Query**

| Parâmetro | Significado | Exemplo | Descrição |
|-----------|-------------|---------|------------|
| `$filter` | **Filtrar** | `$filter=Preco gt 1000` | Filtra registros por condição |
| `$orderby` | **Ordenar** | `$orderby=Preco desc` | Ordena por preço decrescente |
| `$top` | **Primeiros N** | `$top=5` | Retorna apenas os primeiros 5 registros |
| `$skip` | **Pular N** | `$skip=10` | Pula os primeiros 10 registros |
| `$select` | **Selecionar campos** | `$select=Nome,Preco` | Retorna apenas os campos Nome e Preço |
| `$expand` | **Expandir relacionamentos** | `$expand=Loja` | Inclui dados da entidade relacionada Loja |
| `$count` | **Contar registros** | `$count=true` | Inclui contagem total de registros |

### **Modificadores de Ordenação**

| Modificador | Significado | Exemplo | Descrição |
|-------------|-------------|---------|------------|
| `asc` | **Ascendente** (padrão) | `$orderby=Nome asc` | Ordem crescente (A-Z, 1-9) |
| `desc` | **Descendente** | `$orderby=Preco desc` | Ordem decrescente (Z-A, 9-1) |

---

## Testes Básicos

### 1. Verificar se a API está funcionando

**Metadata da API:**
```
GET https://localhost:5240/odata/$metadata
```

### 2. Listar Todas as Entidades

**Empresas:**
```
GET https://localhost:5240/odata/Empresas
```

**Lojas:**
```
GET https://localhost:5240/odata/Lojas
```

**Produtos:**
```
GET https://localhost:5240/odata/Produtos
```

## Exemplos de Filtros OData

### Filtros por Preço
```
# Produtos com preço maior que R$ 1000
GET /odata/Produtos?$filter=Preco gt 1000

# Produtos com preço entre R$ 200 e R$ 2000
GET /odata/Produtos?$filter=Preco ge 200 and Preco le 2000

# Produtos com preço menor que R$ 500
GET /odata/Produtos?$filter=Preco lt 500
```

### Filtros por Texto
```
# Empresas que contém "Tech" no nome
GET /odata/Empresas?$filter=contains(Nome,'Tech')

# Produtos da categoria "Eletrônicos"
GET /odata/Produtos?$filter=Categoria eq 'Eletrônicos'

# Lojas com telefone que começa com "(11)"
GET /odata/Lojas?$filter=startswith(Telefone,'(11)')
```

### Filtros por Data
```
# Empresas criadas em 2019
GET /odata/Empresas?$filter=year(DataCriacao) eq 2019

# Produtos cadastrados nos últimos 6 meses
GET /odata/Produtos?$filter=DataCadastro ge 2024-06-01T00:00:00Z

# Lojas abertas depois de 2020
GET /odata/Lojas?$filter=DataAbertura gt 2020-01-01T00:00:00Z
```

## Exemplos de Ordenação

```
# Produtos ordenados por preço (crescente)
GET /odata/Produtos?$orderby=Preco

# Produtos ordenados por preço (decrescente)
GET /odata/Produtos?$orderby=Preco desc

# Empresas ordenadas por nome
GET /odata/Empresas?$orderby=Nome

# Múltiplos critérios: por categoria e depois por preço
GET /odata/Produtos?$orderby=Categoria,Preco desc
```

## Exemplos de Paginação

```
# Primeiros 5 produtos
GET /odata/Produtos?$top=5

# Pular os primeiros 10 e pegar os próximos 5
GET /odata/Produtos?$skip=10&$top=5

# Contar total de registros
GET /odata/Produtos?$count=true

# Paginação com contagem
GET /odata/Produtos?$top=10&$skip=0&$count=true
```

## Exemplos de Seleção de Campos

```
# Apenas nome e preço dos produtos
GET /odata/Produtos?$select=Nome,Preco

# Apenas nome e CNPJ das empresas
GET /odata/Empresas?$select=Nome,CNPJ

# Campos específicos das lojas
GET /odata/Lojas?$select=Nome,Endereco,Telefone
```

## Exemplos de Expansão (Relacionamentos)

### Expansão Simples
```
# Empresas com suas lojas
GET /odata/Empresas?$expand=Lojas

# Lojas com seus produtos
GET /odata/Lojas?$expand=Produtos

# Produtos com informações da loja
GET /odata/Produtos?$expand=Loja
```

### Expansão Aninhada (Múltiplos Níveis)
```
# Empresas → Lojas → Produtos
GET /odata/Empresas?$expand=Lojas($expand=Produtos)

# Produtos → Loja → Empresa
GET /odata/Produtos?$expand=Loja($expand=Empresa)

# Lojas com produtos filtrados
GET /odata/Lojas?$expand=Produtos($filter=Preco gt 500)
```

## Consultas Complexas Combinadas

> **💡 Dica:** Nos exemplos abaixo, cada parâmetro está explicado em detalhes para facilitar o entendimento!

### Exemplo 1: E-commerce Avançado
```
# Top 5 produtos mais caros da categoria Eletrônicos com informações da loja
GET /odata/Produtos?$filter=Categoria eq 'Eletrônicos'&$orderby=Preco desc&$top=5&$expand=Loja&$select=Nome,Preco,Loja
```

**Explicação detalhada:**
- `$filter=Categoria eq 'Eletrônicos'` → **Filtra** apenas produtos da categoria 'Eletrônicos'
- `$orderby=Preco desc` → **Ordena** por preço em ordem **decrescente** (mais caro primeiro)
- `$top=5` → **Limita** o resultado aos **primeiros 5** produtos
- `$expand=Loja` → **Inclui** os dados da loja relacionada a cada produto
- `$select=Nome,Preco,Loja` → **Retorna apenas** os campos Nome, Preço e dados da Loja

### Exemplo 2: Relatório de Lojas
```
# Lojas de São Paulo com produtos em estoque, ordenadas por nome
GET /odata/Lojas?$filter=contains(Endereco,'SP')&$expand=Produtos($filter=QuantidadeEstoque gt 0)&$orderby=Nome
```

**Explicação detalhada:**
- `$filter=contains(Endereco,'SP')` → **Filtra** lojas que **contêm** 'SP' no endereço
- `$expand=Produtos(...)` → **Inclui** os produtos relacionados a cada loja
- `$filter=QuantidadeEstoque gt 0` → **Dentro do expand**, filtra apenas produtos com estoque **maior que** 0
- `$orderby=Nome` → **Ordena** as lojas por nome em ordem **crescente**

### Exemplo 3: Dashboard Executivo
```
# Empresas com lojas e contagem de produtos
GET /odata/Empresas?$expand=Lojas($expand=Produtos;$count=true)&$select=Nome,CNPJ
```

**Explicação detalhada:**
- `$expand=Lojas(...)` → **Inclui** as lojas relacionadas a cada empresa
- `$expand=Produtos` → **Dentro das lojas**, inclui os produtos relacionados
- `$count=true` → **Adiciona** a contagem total de produtos em cada loja
- `$select=Nome,CNPJ` → **Retorna apenas** Nome e CNPJ das empresas (+ dados expandidos)

## Exemplos de CRUD (Create, Read, Update, Delete)

### CREATE (POST)

**Criar Nova Empresa:**
```http
POST /odata/Empresas
Content-Type: application/json

{
  "Nome": "Inovação Tech",
  "CNPJ": "33.444.555/0001-66",
  "Endereco": "Av. Inovação, 1000",
  "DataCriacao": "2024-01-15T00:00:00"
}
```

**Criar Nova Loja:**
```http
POST /odata/Lojas
Content-Type: application/json

{
  "Nome": "Tech Store Campinas",
  "Endereco": "Shopping Campinas",
  "Telefone": "(19) 3333-4444",
  "DataAbertura": "2024-02-01T00:00:00",
  "EmpresaId": 1
}
```

**Criar Novo Produto:**
```http
POST /odata/Produtos
Content-Type: application/json

{
  "Nome": "Tablet Samsung",
  "Descricao": "Tablet Android 10 polegadas",
  "Preco": 899.99,
  "QuantidadeEstoque": 30,
  "Categoria": "Eletrônicos",
  "DataCadastro": "2024-01-20T00:00:00",
  "LojaId": 1
}
```

### UPDATE (PUT)

**Atualizar Produto:**
```http
PUT /odata/Produtos(1)
Content-Type: application/json

{
  "Id": 1,
  "Nome": "Smartphone Galaxy S24",
  "Descricao": "Smartphone Android com 256GB",
  "Preco": 1499.99,
  "QuantidadeEstoque": 45,
  "Categoria": "Eletrônicos",
  "DataCadastro": "2023-08-01T00:00:00",
  "LojaId": 1
}
```

### DELETE

**Deletar Produto:**
```http
DELETE /odata/Produtos(3)
```

## Testando com curl

### Listar Produtos
```bash
curl -X GET "https://localhost:5240/odata/Produtos" -k
```

### Criar Produto
```bash
curl -X POST "https://localhost:5240/odata/Produtos" \
  -H "Content-Type: application/json" \
  -d '{
    "Nome": "Mouse Gamer",
    "Descricao": "Mouse óptico para jogos",
    "Preco": 149.99,
    "QuantidadeEstoque": 75,
    "Categoria": "Acessórios",
    "DataCadastro": "2024-01-25T00:00:00",
    "LojaId": 1
  }' -k
```

### Filtrar e Expandir
```bash
curl -X GET "https://localhost:5240/odata/Produtos?\$filter=Preco gt 1000&\$expand=Loja" -k
```

## Casos de Uso Práticos

### 1. Catálogo de Produtos
```
# Produtos disponíveis com informações da loja
GET /odata/Produtos?$filter=QuantidadeEstoque gt 0&$expand=Loja&$select=Nome,Preco,QuantidadeEstoque,Loja&$orderby=Categoria,Nome
```

**Explicação:**
- `$filter=QuantidadeEstoque gt 0` → Apenas produtos **em estoque** (quantidade > 0)
- `$expand=Loja` → Inclui **dados da loja** onde o produto está
- `$select=Nome,Preco,QuantidadeEstoque,Loja` → Campos **específicos** retornados
- `$orderby=Categoria,Nome` → Ordena por **categoria primeiro**, depois por **nome**

### 2. Relatório de Vendas por Loja
```
# Lojas com produtos e valores
GET /odata/Lojas?$expand=Produtos&$select=Nome,Endereco,Produtos
```

**Explicação:**
- `$expand=Produtos` → Inclui **todos os produtos** de cada loja
- `$select=Nome,Endereco,Produtos` → Retorna **nome e endereço** da loja + **produtos**

### 3. Busca de Produtos
```
# Buscar produtos por nome
GET /odata/Produtos?$filter=contains(tolower(Nome),'smartphone')&$select=Nome,Preco,Categoria
```

**Explicação:**
- `contains(tolower(Nome),'smartphone')` → Busca **'smartphone'** no nome (case-insensitive)
- `tolower(Nome)` → Converte o nome para **minúsculo** antes da busca
- `$select=Nome,Preco,Categoria` → Retorna apenas **campos essenciais**

### 4. Dashboard Gerencial
```
# Visão geral: empresas, lojas e produtos
GET /odata/Empresas?$expand=Lojas($expand=Produtos($select=Nome,Preco);$select=Nome,Endereco)&$select=Nome,CNPJ
```

**Explicação (expansão aninhada complexa):**
- `$expand=Lojas(...)` → Inclui **lojas** de cada empresa
- `$expand=Produtos($select=Nome,Preco)` → **Dentro das lojas**, inclui produtos com apenas Nome e Preço
- `$select=Nome,Endereco` → **Das lojas**, retorna apenas Nome e Endereço
- `$select=Nome,CNPJ` → **Das empresas**, retorna apenas Nome e CNPJ

## Exemplos de Consultas por Cenário

### **E-commerce**
```
# Produtos em promoção (preço < 500) ordenados por popularidade
GET /odata/Produtos?$filter=Preco lt 500&$orderby=QuantidadeEstoque desc&$expand=Loja&$select=Nome,Preco,QuantidadeEstoque,Loja
```

### **Relatórios Gerenciais**
```
# Empresas com mais de 2 lojas, incluindo contagem de produtos
GET /odata/Empresas?$expand=Lojas($expand=Produtos;$count=true)&$filter=Lojas/$count gt 2
```

### **Busca Avançada**
```
# Produtos caros (>1000) OU da categoria Informática, com dados da loja
GET /odata/Produtos?$filter=Preco gt 1000 or Categoria eq 'Informática'&$expand=Loja&$orderby=Preco desc
```

### **Filtros por Data**
```
# Produtos cadastrados este ano, agrupados por categoria
GET /odata/Produtos?$filter=year(DataCadastro) eq 2024&$orderby=Categoria,DataCadastro desc
```

---

## Dicas de Troubleshooting

1. **Erro de CORS:** Certifique-se que o CORS está configurado no Program.cs
2. **Erro de Conexão BD:** Verifique se o PostgreSQL está rodando na porta 5432
3. **Erro 404:** Verifique se a URL está correta com `/odata/` no início
4. **Erro de Serialização:** Use `Content-Type: application/json` nos POSTs
5. **Caracteres Especiais:** Use encoding UTF-8 para acentos e caracteres especiais
6. **Aspas em Filtros:** Use aspas simples para strings: `'Eletrônicos'` não `"Eletrônicos"`

## Próximos Passos

1. Teste todos os exemplos acima
2. Crie seus próprios filtros e consultas
3. Experimente com diferentes combinações de $filter, $orderby, $expand
4. Teste operações CRUD em todas as entidades
5. Explore o documento $metadata para entender a estrutura completa