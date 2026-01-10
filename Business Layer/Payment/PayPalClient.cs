

namespace Business_Layer.Payment;

using PayPalCheckoutSdk.Core;
using PayPalHttp;

public class PayPalClient
{
    public static PayPalEnvironment Environment(string cliendId,string cliendSecret)
    {
        return new SandboxEnvironment(cliendId, cliendSecret);
    }

    public static HttpClient Client(string cliendId, string cliendSecret)
    {
        return new PayPalHttpClient(Environment(cliendId,cliendSecret));
    }
}
