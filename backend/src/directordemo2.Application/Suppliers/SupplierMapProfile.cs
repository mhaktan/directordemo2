using AutoMapper;
using directordemo2.Entities;
using directordemo2.Suppliers.Dto;

namespace directordemo2.Suppliers
{
    public class SupplierMapProfile : Profile
    {
        public SupplierMapProfile()
        {
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>();
            CreateMap<SupplierDto, Supplier>();
        }
    }
}
