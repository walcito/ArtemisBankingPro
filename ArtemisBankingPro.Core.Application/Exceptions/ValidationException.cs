using FluentValidation.Results;

namespace ArtemisBankingPro.Core.Application.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("Se produjeron uno o más errores de validación.")
    {
        Errors = failures.Select(f => f.ErrorMessage).ToList();
    }
}
