using ABCSharedLibrary.Models.Requests.Identity;
using ABCSharedLibrary.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands
{
    public class UpdateRolePermissionsCommand : IRequest<IResponseWrapper>
    {
        public UpdateRolePermissionsRequest UpdateRequest { get; set; }
    }

    public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public UpdateRolePermissionsCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            var msg = await _roleService.UpdatePermissionsAsync(request.UpdateRequest);

            return await ResponseWrapper<string>.SuccessAsync(message: msg);
        }
    }
}
