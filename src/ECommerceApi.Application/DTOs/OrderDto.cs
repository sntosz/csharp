namespace ECommerceApi.Application.DTOs;

public record OrderDto(
    Guid Id,
    string CustomerEmail,
    string Status,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
