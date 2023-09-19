using System.Globalization;
using BioTonFMS.Domain;
using BioTonFMS.TrackerProtocolSpecific.CommandCodecs;
using FluentAssertions;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.TrackerCommandTests.CodecsTests;

public class GalileoskyCommandCodecTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GalileoskyCommandCodecTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region Decode

    [Theory]
    [InlineData(
        "01 91 00 03 38 36 38 32 30 34 30 30 35 36 34 37 38 33 38 04 32 00 E0 00 00 00 00 E1 77 " +
        "44 65 76 35 30 20 53 6F 66 74 3D 32 32 33 20 50 61 63 6B 3D 31 31 36 20 54 6D " +
        "44 74 3D 30 30 3A 32 34 3A 31 34 20 31 2E 30 31 2E 30 30 20 50 65 72 3D 31 30 " +
        "20 4E 61 76 3D 32 35 35 20 4C 61 74 3D 30 2E 30 30 30 30 30 30 20 4C 6F 6E 3D " +
        "30 2E 30 30 30 30 30 30 20 53 70 64 3D 30 2E 30 20 48 44 4F 50 3D 30 2E 30 20 " +
        "53 61 74 43 6E 74 3D 30 20 41 3D 30 2E 30 30 97 95",
        "Dev50 Soft=223 Pack=116 TmDt=00:24:14 1.01.00 Per=10 Nav=255 " +
        "Lat=0.000000 Lon=0.000000 Spd=0.0 HDOP=0.0 SatCnt=0 A=0.00",
        null)]
    [InlineData(
        "01 08 01 03 38 36 38 32 30 34 30 30 35 36 34 37 38 33 38 04 32 00 E0 00 00 00 00 E1 77 " +
        "44 65 76 35 30 20 53 6F 66 74 3D 32 32 33 20 50 61 63 6B 3D 31 31 36 20 54 6D " +
        "44 74 3D 30 30 3A 32 34 3A 31 34 20 31 2E 30 31 2E 30 30 20 50 65 72 3D 31 30 " +
        "20 4E 61 76 3D 32 35 35 20 4C 61 74 3D 30 2E 30 30 30 30 30 30 20 4C 6F 6E 3D " +
        "30 2E 30 30 30 30 30 30 20 53 70 64 3D 30 2E 30 20 48 44 4F 50 3D 30 2E 30 20 " +
        "53 61 74 43 6E 74 3D 30 20 41 3D 30 2E 30 30 " +
        "EB 77 " +
        "44 65 76 35 30 20 53 6F 66 74 3D 32 32 33 20 50 61 63 6B 3D 31 31 36 20 54 6D " +
        "44 74 3D 30 30 3A 32 34 3A 31 34 20 31 2E 30 31 2E 30 30 20 50 65 72 3D 31 30 " +
        "20 4E 61 76 3D 32 35 35 20 4C 61 74 3D 30 2E 30 30 30 30 30 30 20 4C 6F 6E 3D " +
        "30 2E 30 30 30 30 30 30 20 53 70 64 3D 30 2E 30 20 48 44 4F 50 3D 30 2E 30 20 " +
        "53 61 74 43 6E 74 3D 30 20 41 3D 30 2E 30 30 " +
        "97 95",
        "Dev50 Soft=223 Pack=116 TmDt=00:24:14 1.01.00 Per=10 Nav=255 " +
        "Lat=0.000000 Lon=0.000000 Spd=0.0 HDOP=0.0 SatCnt=0 A=0.00",
        new byte[]
        {
            68, 101, 118, 53, 48, 32, 83, 111, 102, 116, 61, 50, 50,
            51, 32, 80, 97, 99, 107, 61, 49, 49, 54, 32, 84, 109, 68, 116, 61,
            48, 48, 58, 50, 52, 58, 49, 52, 32, 49, 46, 48, 49, 46, 48, 48,
            32, 80, 101, 114, 61, 49, 48, 32, 78, 97, 118, 61, 50, 53, 53, 32,
            76, 97, 116, 61, 48, 46, 48, 48, 48, 48, 48, 48, 32, 76, 111, 110,
            61, 48, 46, 48, 48, 48, 48, 48, 48, 32, 83, 112, 100, 61, 48, 46,
            48, 32, 72, 68, 79, 80, 61, 48, 46, 48, 32, 83, 97, 116, 67, 110,
            116, 61, 48, 32, 65, 61, 48, 46, 48, 48
        })]
    public void Decode_ValidData_ShouldDecodeCorrectly(
        string response, string expectedText, byte[] expectedBinary)
    {
        var codec = new GalileoskyCommandCodec();
        byte[] bytes = response.Split()
            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
            .ToArray();

        CommandResponseInfo decoded = codec.DecodeCommand(bytes);

        decoded.ResponseText.Should().Be(expectedText);
        decoded.ResponseBinary.Should().Equal(expectedBinary);
    }

    /*    [Theory]
        [InlineData("34 30 30 35 36 34 37 38 33 38 04 32 00 77 44 65 76 35 63 " +
                    "6B 3D 31 31 36 20 54 6D 53 61 74 43 6E 74 3D 30 20 41")]
        public void Decode_InvalidData_ShouldNotLoop(string response)
        {
            var codec = new GalileoskyCommandCodec();
            byte[] bytes = response.Split()
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                .ToArray();

            var decoded = codec.DecodeCommand(bytes);


            decoded.ResponseText.Should().Be("");
            //decoded.ResponseBynaryInfo.Should().Equal(Array.Empty<byte>());
            throw new NotImplementedException();
        }*/

    #endregion

    [Theory]
    [InlineData("status",
        "1 20 0 3 38 36 38 32 30 34 30 30 35 36 34 37 38 33 " +
        "38 4 7B 0 E0 5 0 0 0 E1 6 73 74 61 74 75 73 67 0D")]
    public void Encode_ValidData_ShouldEncodeCorrectly(string cmd, string expected)
    {
        var codec = new GalileoskyCommandCodec();

        var tracker = new Tracker
        {
            ExternalId = 123,
            Imei = "868204005647838"
        };

        byte[] encoded = codec.EncodeCommand(tracker, 5, cmd);
        _testOutputHelper.WriteLine(encoded.Length.ToString());

        var result = string.Join(' ', encoded.Select(x => x.ToString("X")));
        _testOutputHelper.WriteLine(result);

        byte[] expectedBytes = expected.Split()
            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
            .ToArray();

        encoded.Length.Should().Be(expectedBytes.Length);
        encoded.Should().Equal(expectedBytes);
    }
}