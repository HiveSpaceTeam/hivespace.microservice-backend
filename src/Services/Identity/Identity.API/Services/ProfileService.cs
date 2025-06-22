using Identity.API.Interfaces;
using Identity.API.Models.Dtos.Request;
using Identity.API.Models.Dtos.Responses;

namespace Identity.API.Services;

public class ProfileService : IProfileService
{
    public Task ChangePassword(ChangePasswordRequestDto requestDto)
    {
        throw new NotImplementedException();
    }

    public Task<SignupResponseDto> CreateUserAsync(CreateUserRequestDto requestDto)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserInfoAsync(UpdateUserRequestDto param)
    {
        throw new NotImplementedException();
    }
}
