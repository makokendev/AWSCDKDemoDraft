using CommandLine;

namespace CodingChallenge.Console;

public class CommandLineOptions
{
    [Option('i', "read-inline", Required = false, HelpText = "Reads either a single json element, or an array of json elements representing transactions as an argument.")]
    public string InlineJson { get; set; }
    [Option('f', "read-file", Required = false, HelpText = "Reads either a single json element, or an array of json elements representing transactions from the file in the specified location.")]
    public string FilePath { get; set; }

    [Option('n', "nft", Required = false, HelpText = "Returns ownership information for the nft with the given id.")]
    public string TokenId { get; set; }
    [Option('w', "wallet", Required = false, HelpText = "Lists all NFTs currently owned by the wallet of the given address.")]
    public string WalletId { get; set; }
    [Option('r', "reset", Required = false, HelpText = "Deletes all data previously processed by the program.")]
    public bool Reset { get; set; }

}
