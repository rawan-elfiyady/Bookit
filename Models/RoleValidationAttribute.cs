using System.ComponentModel.DataAnnotations;

public class RoleValidationAttribute : ValidationAttribute
{
    private static readonly string[] AllowedRoles = { "Admin", "User", "Librarian" };

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string role && AllowedRoles.Contains(role))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult(ErrorMessage);
    }
}
