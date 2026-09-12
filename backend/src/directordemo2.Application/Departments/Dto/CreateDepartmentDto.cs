using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace directordemo2.Departments.Dto
{
    [AutoMapTo(typeof(Entities.Department))]
    public class CreateDepartmentDto
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public decimal? AnnualBudget { get; set; }

        public bool IsActive { get; set; }

    }
}