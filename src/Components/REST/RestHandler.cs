using System.Reflection;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Vibe.Util.AspNet;

namespace Vibe.Components;

public class RestHandler : BaseHandler
{
    public async Task UploadFile(HttpContext ctx)
    {
        await ctx.Unimplemented();
    }

    public async Task DownloadFile(HttpContext ctx)
    {
        await ctx.Unimplemented();
    }

    public async Task AboutServer(HttpContext ctx)
    {
        var version = typeof(RestHandler).Assembly
            .GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))
            as AssemblyInformationalVersionAttribute;

        var o = new JObject();

        o.Add("serverName", "Vibe");
        o.Add("version", version?.InformationalVersion);
        // todo: set these via configuration
        o.Add("aboutServer", "some server information goes here");
        o.Add("messageOfTheDay", "it's cold outside");

        ctx.Response.StatusCode = 200;
        await ctx.Response.WriteAsync(JsonConvert.SerializeObject(o));
    }
}
