using System.Globalization;
using System.Numerics;

namespace CodingChallenge.Application.NFT.Base;

public static class ValidationExtensions
{
    public const string HexadecimalPrefix = "0x";
    public static bool IsHex(this string hexStringRaw)
    {
        if (string.IsNullOrEmpty(hexStringRaw))
            return false;
        if (!hexStringRaw.StartsWith(HexadecimalPrefix))
            return false;

        var hexString = hexStringRaw.Substring(2);
        return BigInteger.TryParse(
            hexString,
            NumberStyles.AllowHexSpecifier, null, out var number);

    }
}
