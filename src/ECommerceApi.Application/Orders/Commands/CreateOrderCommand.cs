using ECommerceApi.Application.DTOs;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace ECommerceApi.Application.Orders.Commands;

public record CreateOrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderCommand(
    string CustomerEmail,
    List<CreateOrderItemRequest> Items) : IRequest<Result<OrderDto>>;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("O e-mail do cliente é obrigatório.")
            .EmailAddress().WithMessage("E-mail com formato inválido.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("O Id do produto é obrigatório.");
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        });
    }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.CustomerEmail);

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
            {
                return Result.Failure<OrderDto>(new Error("Product.NotFound", $"Produto com ID '{item.ProductId}' não foi encontrado."));
            }

            if (!product.IsActive)
            {
                return Result.Failure<OrderDto>(new Error("Product.Inactive", $"O produto '{product.Name}' está inativo."));
            }

            var stockResult = product.DeductStock(item.Quantity);
            if (stockResult.IsFailure)
            {
                return Result.Failure<OrderDto>(stockResult.Error);
            }

            _productRepository.Update(product);

            var addItemResult = order.AddItem(product.Id, product.Name, item.Quantity, product.Price);
            if (addItemResult.IsFailure)
            {
                return Result.Failure<OrderDto>(addItemResult.Error);
            }
        }

        await _orderRepository.AddAsync(order, cancellationToken);
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
