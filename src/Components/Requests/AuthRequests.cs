using Harmony.Auth.V1;

namespace Vibe.Components;

public partial class RequestsHandler
{
    public async Task<BeginAuthResponse> BeginAuth(BeginAuthRequest request)
    {
        return new BeginAuthResponse()
        {
            AuthId = "test"
        };
    }

    public async Task<NextStepResponse> NextStep(NextStepRequest request)
    {
        var choice = new AuthStep.Types.Choice();
        choice.Title = "initial";
        choice.Options.Add("login");
        choice.Options.Add("signup");

        return new NextStepResponse()
        {
            Step = new AuthStep()
            {
                CanGoBack = false,
                Choice = choice,
            }
        };
    }
}
