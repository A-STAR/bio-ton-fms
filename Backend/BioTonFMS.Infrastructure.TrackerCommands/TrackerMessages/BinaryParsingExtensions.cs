using BioTonFMS.Domain.TrackerMessages;
using System.Text;

namespace BioTonFMS.TrackerProtocolSpecific.TrackerMessages;

internal static class BinaryParsingExtensions
{
    internal static string ParseToString(this byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    internal static int ParseToInt(this byte[] bytes)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes.Length switch
        {
            1 => bytes[0],
            2 => BitConverter.ToUInt16(bytes, 0),
            4 => BitConverter.ToInt32(bytes, 0),
            _ => throw new ArgumentException("Array must contain 1, 2 or 4 bytes")
        };
    }

    internal static double ParseToDouble(this byte[] bytes)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes.Length switch
        {
            4 => BitConverter.ToSingle(bytes, 0),
            8 => BitConverter.ToDouble(bytes, 0),
            _ => throw new ArgumentException("Array must contain 4 or 8 bytes")
        };
    }

    internal static DateTime ParseToDateTime(this byte[] bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Array must contain 4 bytes");

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        var seconds = BitConverter.ToUInt32(bytes, 0);
        return DateTime.UnixEpoch.AddSeconds(seconds);
    }

    internal static Velocity ParseVelocity(this byte[] bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Array must contain 4 bytes");

        return new Velocity
        {
            Speed = ParseToInt(bytes[..2]) / (double)10,
            Direction = ParseToInt(bytes[2..]) / (double)10
        };
    }

    internal static Coordinates ParseCoords(this byte[] bytes)
    {
        if (bytes.Length != 9)
            throw new ArgumentException("Array must contain 9 bytes");

        var coords = new Coordinates
        {
            Longitude = ParseToInt(bytes[1..5]) / (double)1000000,
            Latitude = ParseToInt(bytes[5..]) / (double)1000000,
            SatNumber = bytes[0] & 0b00001111,
            Correctness = (CoordCorrectnessEnum)((bytes[0] & 0b11110000) >> 4)
        };

        if (!Enum.IsDefined(coords.Correctness)) coords.Correctness = CoordCorrectnessEnum.Incorrect;

        return coords;
    }

    internal static CanLog ParseCanLog(this byte[] bytes)

    {
        if (bytes.Length != 4)
            throw new ArgumentException("Array must contain 4 bytes");

        return new CanLog
        {
            CoolantTemperature = bytes[1] - 40,
            EngineSpeed = ParseToInt(bytes[2..]) / 8,
            FuelLevel = bytes[0] * 4 / 10
        };
    }
}

internal class Coordinates
{
    internal CoordCorrectnessEnum Correctness { get; set; }
    internal int SatNumber { get; set; }
    internal double Longitude { get; set; }
    internal double Latitude { get; set; }
}

internal class CanLog
{
    internal int FuelLevel { get; set; }
    internal int CoolantTemperature { get; set; }
    internal int EngineSpeed { get; set; }
}

internal class Velocity
{
    internal double Speed { get; set; }
    internal double Direction { get; set; }
}