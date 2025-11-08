using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Queries
{
    public class GetSchoolsQuery : IRequest<IResponseWrapper> { }

    public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public GetSchoolsQueryHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
        {
            var schools = await _schoolService.GetAllAsync();

            if (schools?.Count > 0)
            {
                return await ResponseWrapper<List<SchoolResponse>>.SuccessAsync(data: schools.Adapt<List<SchoolResponse>>());
            }

            return await ResponseWrapper.FailAsync("Schools wer not found");
        }
    }
}
