using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

using Vibe.Components;

namespace Vibe.Util.AspNet;

public partial class VibeRouter
{
    private readonly RequestsHandler _requests;
    private readonly StreamHandler _streams;
    private readonly RestHandler _rest;

    private readonly VibeConfig _config;

    public VibeRouter(VibeConfig config,    
        RequestsHandler requests, StreamHandler streams, RestHandler rest)
    {
        _config = config;

        _requests = requests;
        _streams = streams;
        _rest = rest;

        // todo: there must be a better way of doing this...
        initRequestRoutes();
        initStreamRoutes();
    }

    public void Initialize(IApplicationBuilder app)
    {
        var routes = new RouteBuilder(app);

        if (_config.Deployment == Deployment.SingleProcess || _config.Deployment == Deployment.REST)
        {
            routes.MapPost(
                "/_harmony/media/upload",
                context => _rest.UploadFile(context)
            );

            routes.MapGet(
                "/_harmony/media/download/{file_id}",
                context => _rest.DownloadFile(context)
            );

            routes.MapGet(
                "/_harmony/about",
                context => _rest.AboutServer(context)
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

                return action(ctx);
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

                // todo: should we try-catch here?
                return action(ctx);
            }
        );

        app.UseRouter(routes.Build());
    }
}