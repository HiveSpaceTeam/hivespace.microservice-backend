using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }

    private readonly List<UserAddress> _addresses = [];
    public IReadOnlyCollection<UserAddress> Addresses => _addresses;

    // Default constructor for EF Core and object initializer
    public ApplicationUser() { }

    public ApplicationUser(string userName) : base(userName)
    {
    }

    public void SetGender(Gender? gender)
    {
        Gender = gender;
    }

    public void SetDateOfBirth(DateTime? dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void AddAddress(UserAddress address)
    {
        _ = address ?? throw new ArgumentNullException(nameof(address));

        // If this is the first address, make it default
        if (!_addresses.Any())
        {
            address.SetAsDefault();
        }

        _addresses.Add(address);
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new ArgumentException($"Address with ID {addressId} not found", nameof(addressId));

        var wasDefault = address.IsDefault;
        _addresses.Remove(address);

        // If we removed the default address and there are other addresses, make the first one default
        if (wasDefault && _addresses.Any())
        {
            _addresses.First().SetAsDefault();
        }
    }

    public void SetDefaultAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new ArgumentException($"Address with ID {addressId} not found", nameof(addressId));

        // Only update if this address is not already default
        if (!address.IsDefault)
        {
            // Set all addresses as non-default first
            foreach (var addr in _addresses.Where(a => a.IsDefault))
            {
                addr.SetAsNonDefault();
            }

            // Set the specified address as default
            address.SetAsDefault();
        }
    }

    public UserAddress? GetAddress(Guid addressId)
    {
        return _addresses.FirstOrDefault(a => a.Id == addressId);
    }
}
