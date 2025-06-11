namespace Identity.API.Models;

public class UserAddress
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string Ward { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string Province { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string? ZipCode { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
}
