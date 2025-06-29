using Identity.API.Interfaces;
using Identity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[Authorize(Policy = "RequireIdentityFullAccessScope")]
[ApiController]
[Route("api/v1/users/address")]
public class UserAddressController : ControllerBase
{
    private readonly IUserAddressService _userAddressService;

    public UserAddressController(IUserAddressService userAddressService)
    {
        _userAddressService = userAddressService;
    }

    /// <summary>
    /// Get all addresses for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserAddressResponseDto>>> GetUserAddresses()
    {
        var addresses = await _userAddressService.GetUserAddressesAsync();
        return Ok(addresses);
    }

    /// <summary>
    /// Get a specific address by ID
    /// </summary>
    [HttpGet("{addressId:guid}")]
    public async Task<ActionResult<UserAddressResponseDto>> GetUserAddress(Guid addressId)
    {
        var address = await _userAddressService.GetUserAddressAsync(addressId);
        return address != null ? Ok(address) : NotFound();
    }

    /// <summary>
    /// Create a new address for the current user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserAddressResponseDto>> CreateUserAddress([FromBody] CreateUserAddressDto createDto)
    {
        var address = await _userAddressService.CreateUserAddressAsync(createDto);
        return CreatedAtAction(nameof(GetUserAddress), new { addressId = address.Id }, address);
    }

    /// <summary>
    /// Update an existing address
    /// </summary>
    [HttpPut("{addressId:guid}")]
    public async Task<ActionResult<UserAddressResponseDto>> UpdateUserAddress(Guid addressId, [FromBody] UpdateUserAddressDto updateDto)
    {
        var address = await _userAddressService.UpdateUserAddressAsync(addressId, updateDto);
        return Ok(address);
    }

    /// <summary>
    /// Set an address as default
    /// </summary>
    [HttpPut("{addressId:guid}/default")]
    public async Task<ActionResult> SetDefaultAddress(Guid addressId)
    {
        await _userAddressService.SetDefaultAddressAsync(addressId);
        return NoContent();
    }

    /// <summary>
    /// Delete an address
    /// </summary>
    [HttpDelete("{addressId:guid}")]
    public async Task<ActionResult> DeleteUserAddress(Guid addressId)
    {
        await _userAddressService.DeleteUserAddressAsync(addressId);
        return NoContent();
    }
} 