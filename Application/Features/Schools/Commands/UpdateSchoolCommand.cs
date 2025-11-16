using ABCSharedLibrary.Models.Requests.School;
using ABCSharedLibrary.Wrappers;
using Application.Pipelines;
using MediatR;

namespace Application.Features.Schools.Commands
{
    public class UpdateSchoolCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public UpdateSchoolRequest SchoolRequest { get; set; }
    }

    public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public UpdateSchoolCommandHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
        {
            var school = await _schoolService.GetByIdAsync(request.SchoolRequest.Id);

            if (school is not null)
            {
                school.Name = request.SchoolRequest.Name;
                school.EstablishedDate = request.SchoolRequest.EstablishedDate;

                var updatedSchoolId = await _schoolService.UpdateAsync(school);

                return await ResponseWrapper<int>.SuccessAsync(data: updatedSchoolId, "School updated successfully");
            }

            return await ResponseWrapper.FailAsync("School update failed");
        }
    }
}
