using System.Globalization;
using BioTonFMS.Domain;
using BioTonFMS.TrackerCommands.Codecs;
using FluentAssertions;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.CodecsTests;

public class GalileoskyCommandCodecTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GalileoskyCommandCodecTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(
        "03 38 36 38 32 30 34 30 30 35 36 34 37 38 33 38 04 32 00 E0 00 00 00 00 E1 77 " +
        "44 65 76 35 30 20 53 6F 66 74 3D 32 32 33 20 50 61 63 6B 3D 31 31 36 20 54 6D " +
        "44 74 3D 30 30 3A 32 34 3A 31 34 20 31 2E 30 31 2E 30 30 20 50 65 72 3D 31 30 " +
        "20 4E 61 76 3D 32 35 35 20 4C 61 74 3D 30 2E 30 30 30 30 30 30 20 4C 6F 6E 3D " +
        "30 2E 30 30 30 30 30 30 20 53 70 64 3D 30 2E 30 20 48 44 4F 50 3D 30 2E 30 20 " +
        "53 61 74 43 6E 74 3D 30 20 41 3D 30 2E 30 30 97 95",
        "Dev50 Soft=223 Pack=116 TmDt=00:24:14 1.01.00 Per=10 Nav=255 " +
        "Lat=0.000000 Lon=0.000000 Spd=0.0 HDOP=0.0 SatCnt=0 A=0.00")]
    public void Decode_ValidData_ShouldDecodeCorrectly(string response, string expected)
    {
        var codec = new GalileoskyCommandCodec();
        byte[] bytes = response.Split()
            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
            .ToArray();

        var decoded = codec.Decode(bytes);

        decoded.ResponseText.Should().Be(expected);
    }
    
    [Fact]
    public void Encode_ValidData_ShouldEncodeCorrectly()
    {
        var codec = new GalileoskyCommandCodec();

        var tracker = new Tracker
        {
            ExternalId = 0,
            Imei = "868204005647838"
        };

        byte[] encoded = codec.Encode(tracker, "status");
        _testOutputHelper.WriteLine(encoded.Length.ToString());

        var result = string.Join(' ',encoded.Select(x => x.ToString("X")));
        _testOutputHelper.WriteLine(result);

        encoded.Length.Should().Be(37);
    }
}