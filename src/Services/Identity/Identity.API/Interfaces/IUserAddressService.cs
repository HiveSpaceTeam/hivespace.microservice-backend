using Identity.API.Models;
using Identity.API.Models.Dtos.Request;
using Identity.API.Models.Dtos.Responses;

namespace Identity.API.Interfaces;

public interface IUserAddressService
{
    Task<IEnumerable<UserAddressResponseDto>> GetUserAddressesAsync();
    Task<UserAddressResponseDto?> GetUserAddressAsync(Guid addressId);
    Task<UserAddressResponseDto> CreateUserAddressAsync(CreateUserAddressDto createDto);
    Task<UserAddressResponseDto> UpdateUserAddressAsync(Guid addressId, UpdateUserAddressDto updateDto);
    Task SetDefaultAddressAsync(Guid addressId);
    Task DeleteUserAddressAsync(Guid addressId);
}
