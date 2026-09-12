using AutoMapper;
using directordemo2.Entities;
using directordemo2.PurchaseRequests.Dto;

namespace directordemo2.PurchaseRequests
{
    public class PurchaseRequestMapProfile : Profile
    {
        public PurchaseRequestMapProfile()
        {
            CreateMap<PurchaseRequest, PurchaseRequestDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreatePurchaseRequestDto, PurchaseRequest>();
            CreateMap<PurchaseRequestDto, PurchaseRequest>();
        }
    }
}
