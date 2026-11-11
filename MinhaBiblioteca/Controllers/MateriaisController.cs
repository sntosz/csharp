using Microsoft.AspNetCore.Mvc;
using MinhaBiblioteca.Models;
using MinhaBiblioteca.Repositories;

namespace MinhaBiblioteca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MateriaisController : ControllerBase
{
    private readonly IMaterialRepository _repository;

    public MateriaisController(IMaterialRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult ObterTodos()
    {
        var materiais = _repository.ObterTodos();
        return Ok(materiais);
    }

    [HttpGet("{id}")]
    public IActionResult ObterPorId(int id)
    {
        var material = _repository.ObterPorId(id);
        if (material == null)
        {
            return NotFound(new { mensagem = $"Material com ID {id} não foi encontrado." });
        }

        return Ok(material);
    }

    [HttpPost("livro")]
    public IActionResult CadastrarLivro([FromBody] Livro livro)
    {
        if (livro == null)
        {
            return BadRequest(new { mensagem = "Dados do livro não fornecidos." });
        }

        if (string.IsNullOrWhiteSpace(livro.Titulo))
        {
            return BadRequest(new { mensagem = "O título do material é obrigatório e não pode ser vazio." });
        }

        if (livro.AnoPublicacao < 0)
        {
            return BadRequest(new { mensagem = "O ano de publicação não pode ser menor que 0." });
        }

        if (string.IsNullOrWhiteSpace(livro.Autor))
        {
            return BadRequest(new { mensagem = "O livro deve possuir autor informado." });
        }

        if (livro.Paginas <= 0)
        {
            return BadRequest(new { mensagem = "O número de páginas deve ser maior que 0." });
        }

        var livroCriado = _repository.Adicionar(livro);
        return CreatedAtAction(nameof(ObterPorId), new { id = livroCriado.Id }, livroCriado);
    }

    [HttpPost("revista")]
    public IActionResult CadastrarRevista([FromBody] Revista revista)
    {
        if (revista == null)
        {
            return BadRequest(new { mensagem = "Dados da revista não fornecidos." });
        }

        if (string.IsNullOrWhiteSpace(revista.Titulo))
        {
            return BadRequest(new { mensagem = "O título do material é obrigatório e não pode ser vazio." });
        }

        if (revista.AnoPublicacao < 0)
        {
            return BadRequest(new { mensagem = "O ano de publicação não pode ser menor que 0." });
        }

        if (revista.Edicao <= 0)
        {
            return BadRequest(new { mensagem = "A revista deve possuir edição maior que 0." });
        }

        if (revista.MesPublicacao < 1 || revista.MesPublicacao > 12)
        {
            return BadRequest(new { mensagem = "O mês de publicação da revista deve estar entre 1 e 12." });
        }

        var revistaCriada = _repository.Adicionar(revista);
        return CreatedAtAction(nameof(ObterPorId), new { id = revistaCriada.Id }, revistaCriada);
    }

    [HttpPut("livro/{id}")]
    public IActionResult AtualizarLivro(int id, [FromBody] Livro livro)
    {
        if (livro == null)
        {
            return BadRequest(new { mensagem = "Dados do livro não fornecidos." });
        }

        if (string.IsNullOrWhiteSpace(livro.Titulo))
        {
            return BadRequest(new { mensagem = "O título do material é obrigatório e não pode ser vazio." });
        }

        if (livro.AnoPublicacao < 0)
        {
            return BadRequest(new { mensagem = "O ano de publicação não pode ser menor que 0." });
        }

        if (string.IsNullOrWhiteSpace(livro.Autor))
        {
            return BadRequest(new { mensagem = "O livro deve possuir autor informado." });
        }

        if (livro.Paginas <= 0)
        {
            return BadRequest(new { mensagem = "O número de páginas deve ser maior que 0." });
        }

        bool atualizado = _repository.Atualizar(id, livro);
        if (!atualizado)
        {
            return NotFound(new { mensagem = $"Material com ID {id} não foi encontrado." });
        }

        return Ok(new { mensagem = "Livro atualizado com sucesso." });
    }

    [HttpPut("revista/{id}")]
    public IActionResult AtualizarRevista(int id, [FromBody] Revista revista)
    {
        if (revista == null)
        {
            return BadRequest(new { mensagem = "Dados da revista não fornecidos." });
        }

        if (string.IsNullOrWhiteSpace(revista.Titulo))
        {
            return BadRequest(new { mensagem = "O título do material é obrigatório e não pode ser vazio." });
        }

        if (revista.AnoPublicacao < 0)
        {
            return BadRequest(new { mensagem = "O ano de publicação não pode ser menor que 0." });
        }

        if (revista.Edicao <= 0)
        {
            return BadRequest(new { mensagem = "A revista deve possuir edição maior que 0." });
        }

        if (revista.MesPublicacao < 1 || revista.MesPublicacao > 12)
        {
            return BadRequest(new { mensagem = "O mês de publicação da revista deve estar entre 1 e 12." });
        }

        bool atualizado = _repository.Atualizar(id, revista);
        if (!atualizado)
        {
            return NotFound(new { mensagem = $"Material com ID {id} não foi encontrado." });
        }

        return Ok(new { mensagem = "Revista atualizada com sucesso." });
    }

    [HttpDelete("{id}")]
    public IActionResult ExcluirMaterial(int id)
    {
        bool removido = _repository.Remover(id);
        if (!removido)
        {
            return NotFound(new { mensagem = $"Material com ID {id} não foi encontrado." });
        }

        return NoContent();
    }

    [HttpGet("{id}/descricao")]
    public IActionResult ObterDescricao(int id)
    {
        var material = _repository.ObterPorId(id);
        if (material == null)
        {
            return NotFound(new { mensagem = $"Material com ID {id} não foi encontrado." });
        }

        return Ok(new
        {
            id = material.Id,
            tipo = material.GetType().Name,
            descricao = material.Descricao(),
            prazoEmprestimo = material.CalcularPrazo()
        });
    }

    [HttpPost("{id}/emprestar")]
    public IActionResult Emprestar(int id)
    {
        var material = _repository.ObterPorId(id);
        if (material == null)
        {
            return NotFound(new { mensagem = $"Material com ID {id} não foi encontrado." });
        }

        if (material is not IEmprestimo materialEmprestavel)
        {
            return BadRequest(new { mensagem = "Este tipo de material não possui capacidade de empréstimo." });
        }

        string mensagem = materialEmprestavel.Emprestar();
        return Ok(mensagem);
    }
}
