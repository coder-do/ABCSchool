using ABCSharedLibrary.Models.Requests.Identity;
using ABCSharedLibrary.Models.Responses.Identity;
using ABCSharedLibrary.Wrappers;

namespace App.Infra.Services.Identity
{
    public interface IRoleService
    {
        Task<IResponseWrapper<List<RoleResponse>>> GetRolesAsync();
        Task<IResponseWrapper<string>> CreateAsync(CreateRoleRequest request);
        Task<IResponseWrapper<string>> UpdateAsync(UpdateRoleRequest request);
        Task<IResponseWrapper<string>> DeleteAsync(string roleId);
        Task<IResponseWrapper<RoleResponse>> GetRoleWithoutPermissionsAsync(string roleId);
        Task<IResponseWrapper<RoleResponse>> GetRoleWithPermissionsAsync(string roleId);
        Task<IResponseWrapper<string>> UpdatePermissionsAsync(UpdateRolePermissionsRequest request);
    }
}
