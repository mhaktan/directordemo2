using System;
using Abp.Application.Services.Dto;

namespace directordemo2.Suppliers.Dto
{
    public class PagedSupplierResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
    }
}
