# O que é uma API OData e por que ela é importante

## 🔹 O que é uma API OData?

O **OData (Open Data Protocol)** é um **padrão aberto** criado
inicialmente pela Microsoft que define uma forma de expor e consumir
dados por meio de serviços RESTful.\
Ele padroniza **como** acessar e manipular dados via HTTP, usando
convenções de URL, query strings e formatos de resposta (JSON, XML).

Com OData, você não precisa reinventar filtros, paginação, ordenação ou
projeções: o protocolo já define isso de forma uniforme.

------------------------------------------------------------------------

## 🔹 Por que é importante?

Porque ele: - **Padroniza** o acesso a dados entre sistemas diferentes.\
- **Reduz o trabalho manual** de criar endpoints customizados para cada
filtro/consulta.\
- **Permite interoperabilidade** entre clientes e servidores,
independentemente da linguagem usada.

------------------------------------------------------------------------

## 🔹 Benefícios e vantagens do OData

1.  **Filtros e queries prontos**
    -   Suporte nativo a `$filter`, `$select`, `$orderby`, `$expand`,
        `$top`, `$skip`.\

    -   Exemplo:

        ``` http
        GET /odata/Empresas?$filter=Nome eq 'Delta Tech'&$select=Nome,CNPJ
        ```
2.  **Paginação padrão**
    -   Basta usar `$top` e `$skip` para buscar dados paginados.
3.  **Expansão de relacionamentos (`$expand`)**
    -   Inclui entidades relacionadas em uma única chamada.\

    -   Exemplo:

        ``` http
        GET /odata/Empresas?$expand=Funcionarios
        ```
4.  **Alinhado ao REST**
    -   Usa métodos HTTP padrão: `GET`, `POST`, `PUT`, `PATCH`,
        `DELETE`.
5.  **Documentação automática (metadata)**
    -   O endpoint `/odata/$metadata` fornece um contrato XML da sua
        API.
6.  **Integração com ferramentas e BI**
    -   Power BI, Excel, Tableau e outros podem consumir OData
        diretamente.
7.  **Menos código repetitivo**
    -   Você foca na lógica de negócio, o OData resolve filtros e
        queries.

------------------------------------------------------------------------

## 🔹 Grandes empresas e produtos que usam OData

-   **Microsoft**
    -   **Microsoft Graph API** (Office 365, Teams, Outlook, OneDrive).\
    -   **Dynamics 365** (ERP/CRM).\
    -   **Azure Data Catalog**.\
-   **SAP** -- Expõe serviços via OData (SAP Gateway).\
-   **Salesforce** -- Suporte OData para relatórios e analytics.\
-   **Tableau** e **Power BI** -- Consomem OData diretamente.\
-   **IBM** e **Oracle** -- Integram OData em algumas soluções.

------------------------------------------------------------------------

👉 **Resumo:**\
OData é importante porque **padroniza consultas e manipulação de dados,
reduz código repetitivo e garante compatibilidade com ferramentas de
mercado**.
