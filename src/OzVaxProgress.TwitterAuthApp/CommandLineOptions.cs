using CommandLine;

namespace OzVaxProgress.TwitterAuthApp
{
    public class CommandLineOptions
    {
        [Option(shortName: 'k', longName: "key", Required = true, HelpText = "Consumer key.")]
        public string Key { get; set; }
        
        [Option(shortName: 's', longName: "secret", Required = true, HelpText = "Consumer secret.")]
        public string Secret { get; set; }
    }
}