using AutoMapper;
using directordemo2.Entities;
using directordemo2.PurchaseRequestItems.Dto;

namespace directordemo2.PurchaseRequestItems
{
    public class PurchaseRequestItemMapProfile : Profile
    {
        public PurchaseRequestItemMapProfile()
        {
            CreateMap<PurchaseRequestItem, PurchaseRequestItemDto>();
            CreateMap<CreatePurchaseRequestItemDto, PurchaseRequestItem>();
            CreateMap<PurchaseRequestItemDto, PurchaseRequestItem>();
        }
    }
}
