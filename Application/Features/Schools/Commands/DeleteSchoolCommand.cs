using ABCSharedLibrary.Wrappers;
using MediatR;

namespace Application.Features.Schools.Commands
{
    public class DeleteSchoolCommand : IRequest<IResponseWrapper>
    {
        public int SchoolId { get; set; }
    }

    public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public DeleteSchoolCommandHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
        {
            var school = await _schoolService.GetByIdAsync(request.SchoolId);

            if (school is not null)
            {
                var deletedId = await _schoolService.DeleteAsync(school);

                return await ResponseWrapper<int>.SuccessAsync(data: deletedId, "School deleted successfully");
            }

            return await ResponseWrapper.FailAsync("School delete failed");
        }
    }
}
