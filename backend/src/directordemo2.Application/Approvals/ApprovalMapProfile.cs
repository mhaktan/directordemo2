using AutoMapper;
using directordemo2.Approvals.Dto;
using directordemo2.Entities;

namespace directordemo2.Approvals
{
    public class ApprovalMapProfile : Profile
    {
        public ApprovalMapProfile()
        {
            CreateMap<ApprovalRecord, ApprovalRecordDto>();
            CreateMap<StatusChangeLog, StatusChangeLogDto>();
        }
    }
}
