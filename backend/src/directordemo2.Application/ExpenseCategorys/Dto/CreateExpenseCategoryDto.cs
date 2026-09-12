using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace directordemo2.ExpenseCategorys.Dto
{
    [AutoMapTo(typeof(Entities.ExpenseCategory))]
    public class CreateExpenseCategoryDto
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

    }
}