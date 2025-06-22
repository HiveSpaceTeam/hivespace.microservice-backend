namespace Identity.API.Models;

public record CreateUserAddressDto
{
    public string FullName { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? ZipCode { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsDefault { get; init; }
}

public record UpdateUserAddressDto
{
    public string FullName { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? ZipCode { get; init; }
    public string? PhoneNumber { get; init; }
}

public record UserAddressResponseDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? ZipCode { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsDefault { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public string FullAddress { get; init; } = string.Empty;
} 