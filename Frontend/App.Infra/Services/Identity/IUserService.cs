using ABCSharedLibrary.Models.Requests.User;
using ABCSharedLibrary.Models.Responses.User;
using ABCSharedLibrary.Wrappers;

namespace App.Infra.Services.Identity
{
    public interface IUserService
    {
        Task<IResponseWrapper<string>> UpdateUserAsync(UpdateUserRequest request);
        Task<IResponseWrapper<string>> ChangeUserPasswordAsync(ChangePasswordRequest request);
        Task<IResponseWrapper<List<UserResponse>>> GetUsersAsync();
        Task<IResponseWrapper<UserResponse>> GetByIdAsync(string userId);
        Task<IResponseWrapper<string>> RegisterUserAsync(CreateUserRequest request);
        Task<IResponseWrapper<List<UserRoleResponse>>> GetUserRolesAsync(string userId);
        Task<IResponseWrapper<string>> UpdateUserRolesAsync(string userId, UserRolesRequest request);
        Task<IResponseWrapper<string>> ChangeUserStatusAsync(ChangeUserStatusRequest request);
    }
}
