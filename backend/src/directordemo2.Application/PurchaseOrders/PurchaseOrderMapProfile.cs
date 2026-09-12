using AutoMapper;
using directordemo2.Entities;
using directordemo2.PurchaseOrders.Dto;

namespace directordemo2.PurchaseOrders
{
    public class PurchaseOrderMapProfile : Profile
    {
        public PurchaseOrderMapProfile()
        {
            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreatePurchaseOrderDto, PurchaseOrder>();
            CreateMap<PurchaseOrderDto, PurchaseOrder>();
        }
    }
}
