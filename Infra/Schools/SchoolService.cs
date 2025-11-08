using Application.Features.Schools;
using Domain.Entities;
using Infra.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infra.Schools
{
    public class SchoolService : ISchoolService
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public SchoolService(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public async Task<int> CreateAsync(School school)
        {
            await _applicationDBContext.Schools.AddAsync(school);
            await _applicationDBContext.SaveChangesAsync();

            return school.Id;
        }

        public async Task<int> DeleteAsync(School school)
        {
            _applicationDBContext.Schools.Remove(school);

            await _applicationDBContext.SaveChangesAsync();

            return school.Id;
        }

        public async Task<List<School>> GetAllAsync()
        {
            return await _applicationDBContext.Schools.ToListAsync();
        }

        public async Task<School> GetByIdAsync(int schoolId)
        {
            var school = await _applicationDBContext.Schools.Where(e => e.Id == schoolId).FirstOrDefaultAsync();

            return school;
        }

        public async Task<School> GetByNameAsync(string name)
        {
            var school = await _applicationDBContext.Schools.Where(e => e.Name == name).FirstOrDefaultAsync();

            return school;
        }

        public async Task<int> UpdateAsync(School school)
        {
            _applicationDBContext.Schools.Update(school);

            await _applicationDBContext.SaveChangesAsync();

            return school.Id;
        }
    }
}
