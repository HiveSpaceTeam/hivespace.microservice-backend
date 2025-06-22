namespace Identity.API.Models.Dtos.Request;

public class ChangePasswordRequestDto
{
    public string Password { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
