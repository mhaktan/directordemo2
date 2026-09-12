using AutoMapper;
using directordemo2.Entities;
using directordemo2.Departments.Dto;

namespace directordemo2.Departments
{
    public class DepartmentMapProfile : Profile
    {
        public DepartmentMapProfile()
        {
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<DepartmentDto, Department>();
        }
    }
}
