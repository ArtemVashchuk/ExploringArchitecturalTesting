using System;
using Domain.Exceptions.Base;

namespace Domain.Exceptions;

public sealed class WebinarNotFoundException(Guid webinarId)
    : NotFoundException($"The webinar with the identifier {webinarId} was not found.");