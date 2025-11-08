using ABCSharedLibrary.Models.Requests.Identity;
using ABCSharedLibrary.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands
{
    public class CreateRoleCommand : IRequest<IResponseWrapper>
    {
        public CreateRoleRequest CreateRoleRequest { get; set; }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public CreateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleName = await _roleService.CreateAsync(request.CreateRoleRequest);

            return await ResponseWrapper<string>.SuccessAsync(data: roleName, message: "Role created");
        }
    }
}
