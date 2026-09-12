using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace directordemo2.Suppliers.Dto
{
    [AutoMapFrom(typeof(Entities.Supplier))]
    public class SupplierDto : EntityDto<long>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string TaxNumber { get; set; }

        public string ContactPerson { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}