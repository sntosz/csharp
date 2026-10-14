using ECommerceApi.Application.DTOs;
using ECommerceApi.Application.Orders.Commands;
using ECommerceApi.Application.Orders.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Api.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento e fluxo de vida dos pedidos.
/// </summary>
public class OrdersController : BaseApiController
{
    /// <summary>
    /// Retorna a lista de todos os pedidos cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orders = await Mediator.Send(new GetAllOrdersQuery(), cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Retorna os detalhes de um pedido pelo seu ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cria um novo pedido no sistema e deduz a quantidade do estoque dos produtos.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Realiza o pagamento de um pedido pendente.
    /// </summary>
    [HttpPost("{id:guid}/pay")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pay(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new PayOrderCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
