using Domain.Abstractions;
using Domain.Entities;

namespace Infrastructure.Repositories;

public sealed class WebinarRepository(ApplicationDbContext dbContext) : IWebinarRepository
{
    public void Insert(Webinar webinar) => dbContext.Set<Webinar>().Add(webinar);
}