using Microsoft.AspNetCore.Http;

using Hrpc;

namespace Vibe.Util.AspNet;

public static class HrpcExt
{
    public static Func<HttpContext, Task> WrapHrpcRequest<T, V>(Func<T, Task<V>> handler)
        where T : Google.Protobuf.IMessage<T>, new()
        where V : Google.Protobuf.IMessage<V>, new()
    {
        return async (ctx) =>
        {
            if (ctx.Request.Method.ToLower() != "post")
            {
                await ctx.BadRequest();
                return;
            }
                Console.WriteLine("we are here");

            byte[] bytes = new byte[0];
            await ctx.Request.Body.ReadAsync(bytes);
            var req = Proto.Unmarshal<T>(bytes, 200);

            try
            {

                var res = await handler(req);
                ctx.Response.StatusCode = 200;
                ctx.Response.Headers.Add("Content-Type", "application/hrpc");
                await ctx.Response.BodyWriter.WriteAsync(Proto.Marshal(res));
            }
            catch (Exception e)
            {
                ctx.Response.StatusCode = 500;
                throw;
            }
        };
    }
}


public static class ErrorsExt
{
    // todo: HRPC-ify these
    // todo: when we do that, put it in hrpc.net

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