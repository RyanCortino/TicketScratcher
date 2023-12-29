using TicketScratcher.Server.Infrastructure.Identity;

namespace TicketScratcher.Server.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapIdentityApi<ApplicationUser>();
    }
}
