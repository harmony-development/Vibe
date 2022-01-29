using Microsoft.AspNetCore.Http;

using Harmony.Auth.V1;

namespace Vibe.Util.AspNet;

public partial class VibeRouter
{
    private Dictionary<string, (bool, Func<HttpContext, Task>)> _streamRoutes = new();

    private void initStreamRoutes()
    {

    }

    private Dictionary<string, (bool, Func<HttpContext, Task>)> _requestRoutes = new();

    private void initRequestRoutes()
    {
        _requestRoutes.Add("/protocol.auth.v1.AuthService/BeginAuth", (false, HrpcExt.WrapHrpcRequest<BeginAuthRequest, BeginAuthResponse>(_requests.BeginAuth)));
        _requestRoutes.Add("/protocol.auth.v1.AuthService/NextStep", (false, HrpcExt.WrapHrpcRequest<NextStepRequest, NextStepResponse>(_requests.NextStep)));
    }
}