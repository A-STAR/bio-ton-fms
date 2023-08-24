using System.Collections;
using System.Text;

namespace BioTonFMS.Infrastructure.Extensions;

public static class BitArrayExtensions
{
    public static string GetBitString(this BitArray bits)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < bits.Count; i++)
        {
            char c = bits[i] ? '1' : '0';
            sb.Append(c);
        }

        return sb.ToString();
    }
}