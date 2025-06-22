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
    
    // Navigation property for the user
    public string UserId { get; private set; } = string.Empty;
    public ApplicationUser User { get; private set; } = null!;

    // Private constructor for EF Core
    private UserAddress() { }

    public UserAddress(
        string fullName,
        string street,
        string ward,
        string district,
        string province,
        string country,
        string? zipCode = null,
        string? phoneNumber = null,
        bool isDefault = false)
    {
        Id = Guid.NewGuid();
        SetFullName(fullName);
        SetStreet(street);
        SetWard(ward);
        SetDistrict(district);
        SetProvince(province);
        SetCountry(country);
        SetZipCode(zipCode);
        SetPhoneNumber(phoneNumber);
        IsDefault = isDefault;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));
        
        FullName = fullName.Trim();
        UpdateModifiedTime();
    }

    public void SetStreet(string street)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        
        Street = street.Trim();
        UpdateModifiedTime();
    }

    public void SetWard(string ward)
    {
        if (string.IsNullOrWhiteSpace(ward))
            throw new ArgumentException("Ward cannot be empty", nameof(ward));
        
        Ward = ward.Trim();
        UpdateModifiedTime();
    }

    public void SetDistrict(string district)
    {
        if (string.IsNullOrWhiteSpace(district))
            throw new ArgumentException("District cannot be empty", nameof(district));
        
        District = district.Trim();
        UpdateModifiedTime();
    }

    public void SetProvince(string province)
    {
        if (string.IsNullOrWhiteSpace(province))
            throw new ArgumentException("Province cannot be empty", nameof(province));
        
        Province = province.Trim();
        UpdateModifiedTime();
    }

    public void SetCountry(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));
        
        Country = country.Trim();
        UpdateModifiedTime();
    }

    public void SetZipCode(string? zipCode)
    {
        ZipCode = zipCode?.Trim();
        UpdateModifiedTime();
    }

    public void SetPhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber?.Trim();
        UpdateModifiedTime();
    }

    public void SetAsDefault()
    {
        if (!IsDefault)
        {
            IsDefault = true;
            UpdateModifiedTime();
        }
    }

    public void SetAsNonDefault()
    {
        if (IsDefault)
        {
            IsDefault = false;
            UpdateModifiedTime();
        }
    }

    public void UpdateAddress(
        string fullName,
        string street,
        string ward,
        string district,
        string province,
        string country,
        string? zipCode = null,
        string? phoneNumber = null)
    {
        // Only update if values are different to avoid unnecessary modifications
        if (FullName != fullName.Trim())
            SetFullName(fullName);
        
        if (Street != street.Trim())
            SetStreet(street);
        
        if (Ward != ward.Trim())
            SetWard(ward);
        
        if (District != district.Trim())
            SetDistrict(district);
        
        if (Province != province.Trim())
            SetProvince(province);
        
        if (Country != country.Trim())
            SetCountry(country);
        
        if (ZipCode != zipCode?.Trim())
            SetZipCode(zipCode);
        
        if (PhoneNumber != phoneNumber?.Trim())
            SetPhoneNumber(phoneNumber);
    }

    private void UpdateModifiedTime()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string GetFullAddress()
    {
        var parts = new List<string>
        {
            Street,
            Ward,
            District,
            Province,
            Country
        };

        var address = string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        
        if (!string.IsNullOrWhiteSpace(ZipCode))
        {
            address += $" {ZipCode}";
        }

        return address;
    }
}
