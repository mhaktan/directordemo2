using AutoMapper;
using directordemo2.Entities;
using directordemo2.ExpenseCategorys.Dto;

namespace directordemo2.ExpenseCategorys
{
    public class ExpenseCategoryMapProfile : Profile
    {
        public ExpenseCategoryMapProfile()
        {
            CreateMap<ExpenseCategory, ExpenseCategoryDto>();
            CreateMap<CreateExpenseCategoryDto, ExpenseCategory>();
            CreateMap<ExpenseCategoryDto, ExpenseCategory>();
        }
    }
}
