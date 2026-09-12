using AutoMapper;
using directordemo2.Entities;
using directordemo2.Employees.Dto;

namespace directordemo2.Employees
{
    public class EmployeeMapProfile : Profile
    {
        public EmployeeMapProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<EmployeeDto, Employee>();
        }
    }
}
