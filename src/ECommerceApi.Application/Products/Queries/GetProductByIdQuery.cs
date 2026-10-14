using ECommerceApi.Application.DTOs;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Repositories;
using MediatR;

namespace ECommerceApi.Application.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<ProductDto>(new Error("Product.NotFound", $"Produto com ID '{request.Id}' não foi encontrado."));
        }

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
