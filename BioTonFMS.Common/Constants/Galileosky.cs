namespace BioTonFMS.Common.Constants;

public static class Galileosky
{
    public const byte HeaderTag = 0x01;
    public const int CheckSumLength = 2;
    public const int HeaderLength = 3;
    
    public static ushort GetCrc(byte[] buf, int len)
    {
        ushort crc = 0xFFFF;

        for (var pos = 0; pos < len; pos++)
        {
            crc ^= buf[pos];

            for (var i = 8; i != 0; i--)
            {
                if ((crc & 0x0001) != 0)
                {
                    crc >>= 1;
                    crc ^= 0xA001;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }

        return crc;
    }
}