namespace Identity.API.Models.Dtos.Request;

public class CreateUserRequestDto
{
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
