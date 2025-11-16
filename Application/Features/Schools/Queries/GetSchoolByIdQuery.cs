using ABCSharedLibrary.Models.Responses.School;
using ABCSharedLibrary.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Queries
{
    public class GetSchoolByIdQuery : IRequest<IResponseWrapper>
    {
        public int SchoolId { get; set; }
    }

    public class GetSchoolByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public GetSchoolByIdQueryHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
        {
            var school = await _schoolService.GetByIdAsync(request.SchoolId);

            if (school is not null)
            {
                return await ResponseWrapper<SchoolResponse>.SuccessAsync(data: school.Adapt<SchoolResponse>());
            }

            return await ResponseWrapper.FailAsync("School get by id failed");
        }
    }
}
