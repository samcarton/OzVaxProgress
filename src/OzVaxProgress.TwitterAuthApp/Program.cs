using System;
using System.Threading.Tasks;
using CommandLine;
using Tweetinvi;

namespace OzVaxProgress.TwitterAuthApp
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (opts) =>
                    {
                        try
                        {
                            await AuthorizeAsync(opts.Key, opts.Secret);
                            return 0;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            return -3; // Unhandled error
                        }
                    },
                    errs => Task.FromResult(-1));
        }

        private static async Task AuthorizeAsync(string key, string secret)
        {
            var appClient = new TwitterClient(key, secret);
            var authenticationRequest = await appClient.Auth.RequestAuthenticationUrlAsync();
            
            Console.WriteLine($"Visit this URL to obtain a PIN code using your bot account: {authenticationRequest.AuthorizationURL}");
            
            Console.WriteLine();
            Console.WriteLine("Please enter the code and press enter:");
            var pinCode = Console.ReadLine();

            var userCredentials = await appClient.Auth.RequestCredentialsFromVerifierCodeAsync(pinCode, authenticationRequest);

            Console.WriteLine();
            Console.WriteLine("Save these codes:");
            Console.WriteLine($"Access token: {userCredentials.AccessToken}");
            Console.WriteLine($"Access token secret: {userCredentials.AccessTokenSecret}");
        }
    }
}
