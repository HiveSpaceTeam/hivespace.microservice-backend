using Identity.API.Models.Dtos.Request;
using Identity.API.Models.Dtos.Responses;

namespace Identity.API.Interfaces;

public interface IProfileService
{
    Task<SignupResponseDto> CreateUserAsync(CreateUserRequestDto requestDto);
    Task UpdateUserInfoAsync(UpdateUserRequestDto param);
    Task ChangePassword(ChangePasswordRequestDto requestDto);
}
