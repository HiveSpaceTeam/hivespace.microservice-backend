using Identity.API.Data;
using Identity.API.Interfaces;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Services;

public class UserAddressService : IUserAddressService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAddressService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name
            ?? throw new InvalidOperationException("User is not authenticated");
    }

    public async Task<IEnumerable<UserAddressResponseDto>> GetUserAddressesAsync()
    {
        var userId = GetCurrentUserId();
        var addresses = await _context.Set<UserAddress>()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.CreatedAt)
            .ToListAsync();

        return addresses.Select(MapToResponseDto);
    }

    public async Task<UserAddressResponseDto?> GetUserAddressAsync(Guid addressId)
    {
        var userId = GetCurrentUserId();
        var address = await _context.Set<UserAddress>()
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId)
            ?? throw new ArgumentException($"User with ID {userId} not found", nameof(userId));

        return MapToResponseDto(address);
    }

    public async Task<UserAddressResponseDto> CreateUserAddressAsync(CreateUserAddressDto createDto)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new ArgumentException($"User with ID {userId} not found", nameof(userId));

        var address = new UserAddress(
            createDto.FullName,
            createDto.Street,
            createDto.Ward,
            createDto.District,
            createDto.Province,
            createDto.Country,
            createDto.ZipCode,
            createDto.PhoneNumber,
            createDto.IsDefault);

        // Handle default logic more efficiently
        if (createDto.IsDefault || !user.Addresses.Any())
        {
            // Use a single query to update all existing addresses
            var existingAddresses = user.Addresses.Where(a => a.IsDefault).ToList();
            foreach (var existingAddress in existingAddresses)
            {
                existingAddress.SetAsNonDefault();
            }
            address.SetAsDefault();
        }

        user.AddAddress(address);
        await _context.SaveChangesAsync();

        return MapToResponseDto(address);
    }

    public async Task<UserAddressResponseDto> UpdateUserAddressAsync(Guid addressId, UpdateUserAddressDto updateDto)
    {
        var userId = GetCurrentUserId();
        // Get user with addresses to ensure we have the full context
        var user = await _context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new ArgumentException($"User with ID {userId} not found", nameof(userId));

        var address = user.Addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new ArgumentException($"Address with ID {addressId} not found for user {userId}", nameof(addressId));

        address.UpdateAddress(
            updateDto.FullName,
            updateDto.Street,
            updateDto.Ward,
            updateDto.District,
            updateDto.Province,
            updateDto.Country,
            updateDto.ZipCode,
            updateDto.PhoneNumber);

        await _context.SaveChangesAsync();

        return MapToResponseDto(address);
    }

    public async Task SetDefaultAddressAsync(Guid addressId)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new ArgumentException($"User with ID {userId} not found", nameof(userId));

        // Check if address exists before setting as default
        if (!user.Addresses.Any(a => a.Id == addressId))
            throw new ArgumentException($"Address with ID {addressId} not found for user {userId}", nameof(addressId));

        user.SetDefaultAddress(addressId);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAddressAsync(Guid addressId)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new ArgumentException($"User with ID {userId} not found", nameof(userId));

        // Check if address exists before removing
        if (!user.Addresses.Any(a => a.Id == addressId))
            throw new ArgumentException($"Address with ID {addressId} not found for user {userId}", nameof(addressId));

        user.RemoveAddress(addressId);
        await _context.SaveChangesAsync();
    }

    private static UserAddressResponseDto MapToResponseDto(UserAddress address)
    {
        return new UserAddressResponseDto
        {
            Id = address.Id,
            FullName = address.FullName,
            Street = address.Street,
            Ward = address.Ward,
            District = address.District,
            Province = address.Province,
            Country = address.Country,
            ZipCode = address.ZipCode,
            PhoneNumber = address.PhoneNumber,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt,
            UpdatedAt = address.UpdatedAt,
            FullAddress = address.GetFullAddress()
        };
    }
}
