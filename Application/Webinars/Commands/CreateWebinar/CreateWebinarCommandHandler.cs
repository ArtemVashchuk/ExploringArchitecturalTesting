using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.Webinars.Commands.CreateWebinar;

internal sealed class CreateWebinarCommandHandler(
    IWebinarRepository webinarRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateWebinarCommand, Guid>
{
    public async Task<Guid> Handle(CreateWebinarCommand request, CancellationToken cancellationToken)
    {
        var webinar = new Webinar(Guid.NewGuid(), request.Name, request.ScheduledOn);

        webinarRepository.Insert(webinar);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return webinar.Id;
    }
}