using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.Suppliers.Dto;

namespace directordemo2.Suppliers
{
    public interface ISupplierAppService : IAsyncCrudAppService<
        SupplierDto,
        long,
        PagedSupplierResultRequestDto,
        CreateSupplierDto,
        SupplierDto>
    {
        Task<SupplierReportDto> GetReportData(long id);
    }
}
