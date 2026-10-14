# 🚀 ECommerce Management API - Guia Prático de C# e .NET no Mercado de Trabalho

Esta aplicação foi desenvolvida com o objetivo de servir como um **material de estudo prático, completo e profissional**, demonstrando exatamente como sistemas corporativos modernos são arquitetados e construídos no mercado de trabalho utilizando **.NET 8** e **C#**.

---

## 🏛️ 1. Arquitetura da Solução: Clean Architecture (Arquitetura Limpa)

A solução está organizada seguindo os princípios de **Clean Architecture** (Arquitetura Limpa / Onion Architecture), proposta por Robert C. Martin (Uncle Bob). O objetivo principal é manter o **domínio do negócio independente de bibliotecas externas, bancos de dados ou frameworks de UI**.

```text
               ┌──────────────────────────────────────────┐
               │         ECommerceApi.Api (Web)           │
               └────────────────────┬─────────────────────┘
                                    │
               ┌────────────────────▼─────────────────────┐
               │    ECommerceApi.Infrastructure (DB)      │
               └────────────────────┬─────────────────────┘
                                    │
               ┌────────────────────▼─────────────────────┐
               │     ECommerceApi.Application (CQRS)      │
               └────────────────────┬─────────────────────┘
                                    │
               ┌────────────────────▼─────────────────────┐
               │        ECommerceApi.Domain (Core)        │
               └──────────────────────────────────────────┘
```

### Papel de Cada Projeto / Camada:

1. **`ECommerceApi.Domain` (O Coração da Aplicação)**:
   - **O que contém**: Entidades (`Product`, `Order`, `OrderItem`), Enums (`OrderStatus`), Value Objects, Interfaces de Repositório (`IProductRepository`) e a classe base `Result`.
   - **Regra de ouro**: Não possui **nenhuma dependência de outros projetos ou bibliotecas externas**. Toda regra de negócio essencial fica aqui.

2. **`ECommerceApi.Application` (Casos de Uso / Regras de Negócio da Aplicação)**:
   - **O que contém**: Commands, Queries, DTOs, Handlers do MediatR, Validadores FluentValidation e Pipeline Behaviors.
   - **Depende apenas de**: `ECommerceApi.Domain`.

3. **`ECommerceApi.Infrastructure` (Detalhes Técnicos e Persistência)**:
   - **O que contém**: Implementação do `DbContext` do Entity Framework Core, mapeamentos fluentes (`IEntityTypeConfiguration`), repositórios concretos e Migrations para o SQLite.
   - **Depende de**: `ECommerceApi.Domain` e `ECommerceApi.Application`.

4. **`ECommerceApi.Api` (Ponto de Entrada REST / Apresentação)**:
   - **O que contém**: Controllers, Middlewares HTTP (ex: tratamento global de exceções), Injeção de Dependências (`Program.cs`) e documentação Swagger UI.
   - **Depende de**: `ECommerceApi.Application` e `ECommerceApi.Infrastructure`.

5. **`ECommerceApi.UnitTests` (Qualidade e Testes)**:
   - Contém testes unitários automatizados com **xUnit**, **FluentAssertions** e **Moq**.

---

## 💡 2. Padrões de Projeto (Design Patterns) do Mercado

### 🔹 2.1 CQRS (Command Query Responsibility Segregation) com MediatR
No mercado, separar operações de **escrita/alteração** (Commands) de operações de **leitura** (Queries) traz alta escalabilidade e clareza de código.
- **Commands**: Alteram estado (ex: `CreateProductCommand`, `PayOrderCommand`).
- **Queries**: Somente lêem dados sem efeitos colaterais (ex: `GetProductByIdQuery`).
- **MediatR**: Atua como um barramento em memória (Mediator Pattern), desacoplando totalmente a Controller dos Handlers que executam a lógica.

### 🔹 2.2 Result Pattern (Sem exceções para erros de negócio)
Lançar `throw new Exception()` para regras de negócio (ex: estoque insuficiente ou pedido não encontrado) é uma má prática de mercado, pois lança exceções que pesam na memória (stack trace) e tornam o código imprevisível.
- Utiliza-se a classe `Result` / `Result<T>` para indicar expressamente se a operação obteve **Sucesso** ou **Falha (com `Error`)**.

### 🔹 2.3 Pipeline Behavior com FluentValidation
Em vez de validar DTOs manualmente dentro dos Handlers ou Controllers, utiliza-se o `ValidationPipelineBehavior` do MediatR.
- Qualquer requisição passa automaticamente pelo validador do FluentValidation antes mesmo de chegar ao Handler. Se houver falhas de validação, a requisição é interrompida imediatamente.

### 🔹 2.4 Repository & Unit of Work Patterns
- **Repository**: Abstrai o acesso ao banco de dados (`IProductRepository`). A camada de aplicação manipula dados sem saber se eles vêm do SQLite, SQL Server ou MongoDB.
- **Unit of Work**: Garante que múltiplas operações de escrita sejam salvas em uma única transação atômica (`SaveChangesAsync`).

### 🔹 2.5 Global Exception Middleware (RFC 7807)
Qualquer erro inesperado não tratado é capturado pelo `GlobalExceptionHandlingMiddleware`, garantindo que o cliente receba uma resposta limpa e padronizada no formato **ProblemDetails (RFC 7807)** com HTTP Status Codes adequados (`400 Bad Request`, `404 Not Found`, `500 Internal Server Error`).

---

## 🛠️ 3. Como Executar a Aplicação

### Pré-requisitos
- **.NET 8 SDK** ou superior instalado.

### Passos para Rodar a API:

1. **Clonar ou navegar até o diretório do projeto**:
   ```bash
   cd /caminho/do/projeto
   ```

2. **Compilar a solução**:
   ```bash
   dotnet build
   ```

3. **Executar a API**:
   ```bash
   dotnet run --project src/ECommerceApi.Api/ECommerceApi.Api.csproj
   ```

4. **Acessar a documentação do Swagger**:
   Abra o navegador em:
   ```text
   http://localhost:5000 (ou a URL indicada no terminal)
   ```
   > Ao iniciar, a aplicação cria o banco de dados SQLite (`ecommerce.db`) e realiza o *Seeding* com produtos pré-cadastrados (ex: Notebook Gamer, Teclado Mecânico, etc.).

---

## 🧪 4. Como Executar os Testes Unitários

A solução conta com **9 testes unitários** testando regras do domínio e fluxos dos handlers da aplicação.

Para rodar todos os testes, execute:
```bash
dotnet test
```

Saída esperada:
```text
Passed!  - Failed: 0, Passed: 9, Skipped: 0, Total: 9
```

---

## 📖 5. Exemplo de Fluxo Completo para Estudo

1. **Consultar Produtos Disponíveis**:
   - Faça um `GET /api/products` no Swagger para ver a lista de produtos semeados e seus IDs.

2. **Criar um Pedido (`POST /api/orders`)**:
   - Envie um JSON informando o e-mail do cliente e os itens/quantidades desejados:
   ```json
   {
     "customerEmail": "aluno@estudo.com",
     "items": [
       {
         "productId": "<GUID_DO_PRODUTO>",
         "quantity": 2
       }
     ]
   }
   ```
   - *O sistema deduzirá automaticamente o estoque do produto e criará o pedido com status `Pending`.*

3. **Pagar o Pedido (`POST /api/orders/{id}/pay`)**:
   - Informe o ID do pedido criado para alterar o status do pedido para `Paid`.

---

## 📚 Conclusão

Esta aplicação exemplifica o padrão de arquitetura e código utilizado por empresas de tecnologia de grande e médio porte ao desenvolverem APIs RESTful em C# e .NET. Bons estudos!
