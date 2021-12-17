using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Vibe.Util.AspNet;

public class VibeRouter
{
    private Dictionary<string, (bool, Action<Task>)> _streamRoutes = new();
    private Dictionary<string, (bool, Action<Task>)> _requestRoutes = new();

    private readonly VibeConfig _config;

    public VibeRouter(VibeConfig config)
    {
        _config = config;
    }

    public void Initialize(IApplicationBuilder app)
    {
        var routes = new RouteBuilder(app);

        if (_config.Deployment == Deployment.SingleProcess || _config.Deployment == Deployment.REST)
        {
            routes.MapPost(
                "/_harmony/media/upload",
                context => context.Unimplemented()
            );

            routes.MapGet(
                "/_harmony/media/download/{file_id}",
                context => context.Unimplemented()
            );

            routes.MapGet(
                "/_harmony/about",
                context => context.Unimplemented()
            );
        }

        routes.MapVerb(
            HttpMethod.Patch.Method, "/{*url}",
            context => context.BadRequest()
        );

        routes.MapVerb(
            HttpMethod.Put.Method, "/{*url}",
            context => context.BadRequest()
        );

        routes.MapVerb(
            HttpMethod.Get.Method, "/{*url}",
            ctx =>
            {
                if (!_streamRoutes.TryGetValue(ctx.Request.Path, out var info))
                    return ctx.BadRequest();

                var (needsAuth, action) = info;

                if (needsAuth && !ctx.Request.Headers.ContainsKey("Authorization"))
                    return ctx.Unauthorized();

                return Task.CompletedTask;
            }
        );

        routes.MapVerb(
            HttpMethod.Post.Method, "/{*url}",
            ctx =>
            {
                if (!_requestRoutes.TryGetValue(ctx.Request.Path, out var info))
                    return ctx.BadRequest();

                var (needsAuth, action) = info;

                if (needsAuth && !ctx.Request.Headers.ContainsKey("Authorization"))
                    return ctx.Unauthorized();

                return Task.CompletedTask;
            }
        );

        app.UseRouter(routes.Build());
    }
}