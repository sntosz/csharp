using ECommerceApi.Application.DTOs;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace ECommerceApi.Application.Orders.Commands;

public record PayOrderCommand(Guid OrderId) : IRequest<Result<OrderDto>>;

public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
{
    public PayOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("O Id do pedido é obrigatório.");
    }
}

public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure<OrderDto>(new Error("Order.NotFound", $"Pedido com ID '{request.OrderId}' não encontrado."));
        }

        var result = order.Pay();
        if (result.IsFailure)
        {
            return Result.Failure<OrderDto>(result.Error);
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToDto(order));
    }

    private static OrderDto MapToDto(Order order) =>
        new(
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
}
