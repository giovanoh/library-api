using System.ComponentModel.DataAnnotations;

namespace Library.API.Validation;

/// <summary>
/// Validates that a date is in the past (not later than the current date).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PastDateAttribute : ValidationAttribute
{
    public PastDateAttribute() : base("The {0} cannot be in the future.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }
        if (value is DateTime dateValue)
        {
            return dateValue.Date <= DateTime.Today;
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name);
    }
}