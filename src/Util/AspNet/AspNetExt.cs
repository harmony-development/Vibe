using Microsoft.AspNetCore.Http;

namespace Vibe.Util.AspNet;

public static class AspNetExt
{
    // todo: HRPC-ify these

    public static Task BadRequest(this HttpContext ctx)
    {
        ctx.Response.StatusCode = 400;
        return ctx.Response.WriteAsync("400 bad request");
    }

    public static Task Unauthorized(this HttpContext ctx)
    {
        ctx.Response.StatusCode = 401;
        return ctx.Response.WriteAsync("401 unauthorized");
    }

    public static Task Unimplemented(this HttpContext ctx)
    {
        ctx.Response.StatusCode = 501;
        return ctx.Response.WriteAsync("501 unimplmented");
    }
}