using AutoMapper;
using directordemo2.Entities;
using directordemo2.Quotations.Dto;

namespace directordemo2.Quotations
{
    public class QuotationMapProfile : Profile
    {
        public QuotationMapProfile()
        {
            CreateMap<Quotation, QuotationDto>();
            CreateMap<CreateQuotationDto, Quotation>();
            CreateMap<QuotationDto, Quotation>();
        }
    }
}
