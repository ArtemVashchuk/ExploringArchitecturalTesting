using System.Collections.Generic;
using Domain.Exceptions.Base;

namespace Application.Exceptions;

public sealed class ValidationException(Dictionary<string, string[]> errors)
    : BadRequestException("Validation errors occurred")
{
    public Dictionary<string, string[]> Errors { get; } = errors;
}