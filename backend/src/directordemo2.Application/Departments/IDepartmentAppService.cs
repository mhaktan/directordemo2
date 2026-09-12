using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.Departments.Dto;

namespace directordemo2.Departments
{
    public interface IDepartmentAppService : IAsyncCrudAppService<
        DepartmentDto,
        long,
        PagedDepartmentResultRequestDto,
        CreateDepartmentDto,
        DepartmentDto>
    {
        decimal? GetStats(DepartmentStatsInput input);
        Task<DepartmentReportDto> GetReportData(long id);
    }
}
