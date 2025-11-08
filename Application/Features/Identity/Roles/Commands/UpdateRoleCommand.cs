using ABCSharedLibrary.Models.Requests.Identity;
using ABCSharedLibrary.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<IResponseWrapper>
    {
        public UpdateRoleRequest UpdateRoleRequest { get; set; }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleName = await _roleService.UpdateAsync(request.UpdateRoleRequest);

            return await ResponseWrapper<string>.SuccessAsync(data: roleName, message: $"Role {request.UpdateRoleRequest.Name} updated to {roleName}");
        }
    }
}
