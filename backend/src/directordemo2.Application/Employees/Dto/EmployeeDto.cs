using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace directordemo2.Employees.Dto
{
    [AutoMapFrom(typeof(Entities.Employee))]
    public class EmployeeDto : EntityDto<long>
    {
        public string RegistrationNumber { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; }

        public long UserId { get; set; }

        public long DepartmentId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}