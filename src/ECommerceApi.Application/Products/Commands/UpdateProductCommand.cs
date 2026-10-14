using ECommerceApi.Application.DTOs;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace ECommerceApi.Application.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price) : IRequest<Result<ProductDto>>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O Id do produto é obrigatório.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome do produto é obrigatório.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("O preço deve ser maior que zero.");
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<ProductDto>(new Error("Product.NotFound", $"Produto com ID '{request.Id}' não foi encontrado."));
        }

        product.UpdateInfo(request.Name, request.Description, request.Price);
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);

        return Result.Success(dto);
    }
}
