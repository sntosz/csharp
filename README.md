# Sistema de Gerenciamento de Biblioteca Universitária

Projeto prático desenvolvido para a disciplina de **Programação Orientada a Objetos** do curso de **Engenharia de Software**.

---

## 👥 Integrantes da Equipe

- **Joao Ortiz**
- **Ryan Soler**
- **Vinicius Durau**
- **Gustavo Santos**

---

## 📖 Descrição do Sistema

O **Sistema de Gerenciamento de Biblioteca Universitária** é uma Web API RESTful construída em **C#** e **ASP.NET Core (.NET 10)** para gerenciar o acervo de materiais de uma biblioteca (Livros e Revistas).

O sistema resolve problemas de desorganização manual, permitindo:
- Cadastrar livros e revistas com regras de negócio específicas;
- Consultar todo o acervo ou buscar materiais individuais por ID;
- Atualizar e excluir materiais do acervo;
- Consultar a descrição e o prazo padrão de empréstimo de forma dinâmica (Polimorfismo);
- Realizar simulação de empréstimo apenas para materiais com essa capacidade (Interface `IEmprestimo`).

---

## 💻 Tecnologias Utilizadas

- **C# 14 / .NET 10**
- **ASP.NET Core Web API** (Controllers)
- **Swashbuckle.AspNetCore** (Swagger / OpenAPI)
- **Armazenamento em memória:** `Dictionary<int, MaterialBiblioteca>`
- **Padrão de Injeção de Dependência:** Registro de repositório como `Singleton`
- **CORS:** Configuração de política `PermitirFrontend` liberada para integração

---

## 🧠 Conceitos Teóricos Explicados (Guia para a Prova)

Este projeto foi estruturado para exercitar e fixar os conceitos fundamentais de Orientação a Objetos e Arquitetura de Software:

### 1. Herança
- **O que é:** Mecanismo pelo qual uma classe filha (subclasse) herda campos, propriedades e métodos de uma classe pai (superclasse).
- **No projeto:** `Livro` e `Revista` herdam de `MaterialBiblioteca`. As propriedades comuns `Id`, `Titulo` e `AnoPublicacao` foram definidas apenas uma vez na classe pai.

### 2. Classe Abstrata (`abstract`)
- **O que é:** Uma classe que serve como molde conceitual e não pode ser instanciada diretamente via `new`. Pode conter tanto métodos concretos (com código) quanto métodos abstratos (apenas a assinatura).
- **No projeto:** `MaterialBiblioteca` é abstrata (`public abstract class MaterialBiblioteca`). Não faz sentido existir no acervo um "material genérico", apenas livros ou revistas concretos.

### 3. Métodos Virtuais (`virtual`) e Abstratos (`abstract`) com `override`
- **`abstract`:** Obriga todas as classes filhas não-abstratas a implementarem o método. No projeto: `public abstract int CalcularPrazo();`. Cada material é forçado a definir seu prazo (`Livro` retorna 14, `Revista` retorna 7).
- **`virtual`:** Fornece uma implementação padrão na classe base que as classes filhas podem opcionalmente sobrescrever usando `override`. No projeto: `public virtual string Descricao()` tem um formato base, e `Livro` e `Revista` sobrescrevem para exibir dados específicos (Autor/Edição).

### 4. Polimorfismo
- **O que é:** Capacidade de tratar objetos de diferentes classes derivadas por meio de uma referência comum da classe base, executando o comportamento correto em tempo de execução sem condicionais (`if/else` ou `switch`).
- **No projeto:** No endpoint `GET /api/Materiais/{id}/descricao`, chamamos:
  ```csharp
  material.CalcularPrazo();
  ```
  Se `material` for um `Livro`, o C# executa o método do `Livro` (14 dias). Se for uma `Revista`, executa o método da `Revista` (7 dias). A Controller não possui nenhum `if (material is Livro)`.

### 5. Interfaces e Operador `is not`
- **O que é:** Um contrato que define comportamentos/capacidades específicas que qualquer classe pode assinar, permitindo desacoplar capacidades de hierarquias de herança.
- **No projeto:** A interface `IEmprestimo` possui o método `string Emprestar();`. Apenas `Livro` implementa `IEmprestimo`, enquanto `Revista` não. No endpoint `POST /api/Materiais/{id}/emprestar`, usamos o operador de padrão de tipo:
  ```csharp
  if (material is not IEmprestimo materialEmprestavel)
  {
      return BadRequest(new { mensagem = "Este tipo de material não possui capacidade de empréstimo." });
  }
  return Ok(materialEmprestavel.Emprestar());
  ```

### 6. Repository Pattern e Dicionário em Memória
- **O que é:** Padrão de projeto que abstrai a persistência e o acesso aos dados da camada de controle/aplicação. A Controller apenas conhece a interface `IMaterialRepository`.
- **No projeto:** `MaterialRepository` gerencia um `Dictionary<int, MaterialBiblioteca>` privado. Polimorfismo se aplica na coleção: o dicionário armazena o tipo-base `MaterialBiblioteca`, comportando simultaneamente livros e revistas na mesma coleção.
- **Geração de ID:** O repositório calcula `_materiais.Keys.Max() + 1`, garantindo que exclusões não causem colisões de identificador.

### 7. Injeção de Dependência e Singleton
- **O que é:** O container de serviços do ASP.NET Core cria e gerencia a vida útil das dependências. A Controller recebe `IMaterialRepository` pelo construtor.
- **No projeto:** Registrado no `Program.cs` com:
  ```csharp
  builder.Services.AddSingleton<IMaterialRepository, MaterialRepository>();
  ```
  O ciclo de vida **Singleton** garante que exista exatamente uma única instância do repositório durante todo o ciclo de vida da API, mantendo os dados na memória entre diferentes requisições HTTP.

---

## 📁 Estrutura do Projeto

```
MinhaBiblioteca/
│
├── Controllers/
│   └── MateriaisController.cs
│
├── Models/
│   ├── IEmprestimo.cs
│   ├── MaterialBiblioteca.cs
│   ├── Livro.cs
│   └── Revista.cs
│
├── Repositories/
│   ├── IMaterialRepository.cs
│   └── MaterialRepository.cs
│
├── Program.cs
└── MinhaBiblioteca.csproj
```

---

## 🚀 Como Executar o Projeto

1. Certifique-se de possuir o SDK do .NET 10 instalado:
   ```bash
   dotnet --version
   ```

2. Entre na pasta da aplicação:
   ```bash
   cd MinhaBiblioteca
   ```

3. Restaure as dependências e compile o projeto:
   ```bash
   dotnet build
   ```

4. Inicie o servidor:
   ```bash
   dotnet run
   ```

5. Abra o navegador e acesse a interface interativa do Swagger:
   ```
   http://localhost:5000/swagger
   # ou https://localhost:7000/swagger (dependendo das portas informadas no console)
   ```

---

## 📡 Principais Endpoints

| Método | Rota | Descrição | Status de Sucesso |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/Materiais` | Lista todos os materiais do acervo | `200 OK` |
| **GET** | `/api/Materiais/{id}` | Busca material por ID | `200 OK` ou `404 Not Found` |
| **POST** | `/api/Materiais/livro` | Cadastra um novo livro | `201 Created` |
| **POST** | `/api/Materiais/revista` | Cadastra uma nova revista | `201 Created` |
| **PUT** | `/api/Materiais/livro/{id}` | Atualiza os dados de um livro | `200 OK` ou `404 Not Found` |
| **PUT** | `/api/Materiais/revista/{id}` | Atualiza os dados de uma revista | `200 OK` ou `404 Not Found` |
| **DELETE** | `/api/Materiais/{id}` | Remove um material por ID | `204 No Content` ou `404 Not Found` |
| **GET** | `/api/Materiais/{id}/descricao` | Exibe tipo, descrição polimórfica e prazo | `200 OK` ou `404 Not Found` |
| **POST** | `/api/Materiais/{id}/emprestar` | Realiza empréstimo se implementar `IEmprestimo` | `200 OK`, `400 Bad Request` ou `404` |

---

## 🧪 Guia Passo a Passo dos 20 Testes no Swagger

Abra o Swagger (`/swagger`) e execute cada teste clicando em **"Try it out"**, preenchendo os parâmetros e clicando em **"Execute"**:

| Nº | Teste / Ação | Endpoint e Parâmetros / Body | Resultado Esperado |
| :---: | :--- | :--- | :--- |
| **1** | Listar materiais iniciais | `GET /api/Materiais` | `200 OK` com a lista contendo o livro inicial (ID 1) e a revista inicial (ID 2). |
| **2** | Buscar material existente | `GET /api/Materiais/1` | `200 OK` com os dados do livro "Clean Code". |
| **3** | Buscar material inexistente | `GET /api/Materiais/999` | `404 Not Found` com mensagem amigável. |
| **4** | Cadastrar livro válido | `POST /api/Materiais/livro`<br>Body:<br>`{"titulo":"O Programador Pragmático","anoPublicacao":1999,"autor":"Andrew Hunt","paginas":352}` | `201 Created` com o livro cadastrado e `id: 3`. |
| **5** | Cadastrar revista válida | `POST /api/Materiais/revista`<br>Body:<br>`{"titulo":"Revista Tech","anoPublicacao":2026,"edicao":20,"mesPublicacao":11}` | `201 Created` com a revista cadastrada e `id: 4`. |
| **6** | Validação: Livro sem título | `POST /api/Materiais/livro`<br>Body:<br>`{"titulo":" ","anoPublicacao":2020,"autor":"Autor X","paginas":100}` | `400 Bad Request` com a mensagem: *"O título do material é obrigatório e não pode ser vazio."* |
| **7** | Validação: Livro com páginas 0 | `POST /api/Materiais/livro`<br>Body:<br>`{"titulo":"Livro Zero","anoPublicacao":2020,"autor":"Autor Y","paginas":0}` | `400 Bad Request` com a mensagem: *"O número de páginas deve ser maior que 0."* |
| **8** | Validação: Revista com edição 0 | `POST /api/Materiais/revista`<br>Body:<br>`{"titulo":"Revista Z","anoPublicacao":2020,"edicao":0,"mesPublicacao":5}` | `400 Bad Request` com a mensagem: *"A revista deve possuir edição maior que 0."* |
| **9** | Validação: Revista com mês 13 | `POST /api/Materiais/revista`<br>Body:<br>`{"titulo":"Revista Mes Invalido","anoPublicacao":2020,"edicao":1,"mesPublicacao":13}` | `400 Bad Request` com a mensagem: *"O mês de publicação da revista deve estar entre 1 e 12."* |
| **10** | Atualizar livro existente | `PUT /api/Materiais/livro/1`<br>Body:<br>`{"titulo":"Clean Code 2ª Edição","anoPublicacao":2009,"autor":"Robert C. Martin","paginas":480}` | `200 OK` com a mensagem: *"Livro atualizado com sucesso."* |
| **11** | Atualizar revista existente | `PUT /api/Materiais/revista/2`<br>Body:<br>`{"titulo":"Tecnologia & IA","anoPublicacao":2025,"edicao":16,"mesPublicacao":7}` | `200 OK` com a mensagem: *"Revista atualizada com sucesso."* |
| **12** | Atualizar material inexistente | `PUT /api/Materiais/livro/999`<br>Body:<br>`{"titulo":"Inexistente","anoPublicacao":2020,"autor":"A","paginas":50}` | `404 Not Found` com mensagem de material não encontrado. |
| **13** | Excluir material existente | `DELETE /api/Materiais/1` | `204 No Content`. |
| **14** | Excluir material inexistente | `DELETE /api/Materiais/999` | `404 Not Found`. |
| **15** | Polimorfismo: Descrição de livro | `GET /api/Materiais/3/descricao` (ou livro cadastrado) | `200 OK` com `"tipo": "Livro"` e `"prazoEmprestimo": 14`. |
| **16** | Polimorfismo: Descrição de revista | `GET /api/Materiais/2/descricao` | `200 OK` com `"tipo": "Revista"` e `"prazoEmprestimo": 7`. |
| **17** | Interface: Empréstimo de livro | `POST /api/Materiais/3/emprestar` (ou ID do livro existente) | `200 OK` com o texto: *"O livro '...' pode ser emprestado por 14 dias."* |
| **18** | Interface: Empréstimo de revista | `POST /api/Materiais/2/emprestar` | `400 Bad Request` com mensagem: *"Este tipo de material não possui capacidade de empréstimo."* |
| **19** | Teste de IDs diferentes | Cadastre múltiplos livros e revistas em sequência via POST | Cada item criado recebe um `id` sequencial exclusivo e incremental. |
| **20** | Teste de não reutilização de ID após exclusão | Exclua o item com maior ID (ex: DELETE ID 4) e cadastre um novo item | O novo ID gerado é 5 (calculado via `Max() + 1`), evitando colisão de IDs reutilizados. |
