using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public Gender? Gender { get; private set; }
    public DateTime? DateOfBirth { get; private set; }

    private readonly List<UserAddress> _addresses = [];
    public IReadOnlyCollection<UserAddress> Addresses => _addresses;
}
