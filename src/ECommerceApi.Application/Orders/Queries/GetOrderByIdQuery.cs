using ECommerceApi.Application.DTOs;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Repositories;
using MediatR;

namespace ECommerceApi.Application.Orders.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
        {
            return Result.Failure<OrderDto>(new Error("Order.NotFound", $"Pedido com ID '{request.Id}' não foi encontrado."));
        }

        var dto = new OrderDto(
            order.Id,
            order.CustomerEmail,
            order.Status.ToString(),
            order.TotalAmount,
            order.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice)).ToList(),
            order.CreatedAt,
            order.UpdatedAt);

        return Result.Success(dto);
    }
}
