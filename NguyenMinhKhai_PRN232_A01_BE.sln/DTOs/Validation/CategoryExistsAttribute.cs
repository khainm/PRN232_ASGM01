using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs.Validation
{
    public class CategoryExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var categoryId = (int)value;
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;

            var categoryExists = dbContext.Categories.Any(c => c.CategoryId == categoryId);
            if (!categoryExists)
                return new ValidationResult("Category does not exist");

            return ValidationResult.Success;
        }
    }
} 