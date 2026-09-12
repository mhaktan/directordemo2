using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.ExpenseCategorys.Dto;

namespace directordemo2.ExpenseCategorys
{
    public interface IExpenseCategoryAppService : IAsyncCrudAppService<
        ExpenseCategoryDto,
        long,
        PagedExpenseCategoryResultRequestDto,
        CreateExpenseCategoryDto,
        ExpenseCategoryDto>
    {
        Task<ExpenseCategoryReportDto> GetReportData(long id);
    }
}
