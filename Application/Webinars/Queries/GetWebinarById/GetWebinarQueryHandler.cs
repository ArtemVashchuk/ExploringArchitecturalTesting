using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Dapper;
using Domain.Exceptions;

namespace Application.Webinars.Queries.GetWebinarById;

internal sealed class GetWebinarQueryHandler(IDbConnection dbConnection)
    : IQueryHandler<GetWebinarByIdQuery, WebinarResponse>
{
    public async Task<WebinarResponse> Handle(
        GetWebinarByIdQuery request,
        CancellationToken cancellationToken)
    {
        const string sql = @"SELECT * FROM ""Webinars"" WHERE ""Id"" = @WebinarId";

        var webinar = await dbConnection.QueryFirstOrDefaultAsync<WebinarResponse>(
            sql,
            new { request.WebinarId });

        if (webinar is null)
        {
            throw new WebinarNotFoundException(request.WebinarId);
        }

        return webinar;
    }
}