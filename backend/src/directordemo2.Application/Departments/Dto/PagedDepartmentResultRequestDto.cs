using System;
using Abp.Application.Services.Dto;

namespace directordemo2.Departments.Dto
{
    public class PagedDepartmentResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? AnnualBudget { get; set; }
        public bool? IsActive { get; set; }
        public decimal? AnnualBudgetFrom { get; set; }
        public decimal? AnnualBudgetTo { get; set; }
    }
}
