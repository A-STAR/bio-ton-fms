using System.Globalization;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;
using FluentAssertions;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.GalileoskyTests;

public class GalileoskyMessageHandlerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GalileoskyMessageHandlerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    public static IEnumerable<object[]> Files =>
        new List<object[]>
        {
            new object[]
            {
                "[BIO-81] Расшифровка_пакет_а_JD_с_CAN_данными.docx",
                "./GalileoskyTests/TestCases/1-message.txt",
                false
            },
            new object[]
            {
                "[BIO-81] Расшифровка_пакета_сигнализация.docx",
                "./GalileoskyTests/TestCases/2-message.txt"
            },
            new object[]
            {
                "[BIO-81] Расшифровка пакетов 3 легковые.docx",
                "./GalileoskyTests/TestCases/3-message.txt"
            },
            new object[]
            {
                "[BIO-81] Расшифровка пакетов 2 легковые.docx",
                "./GalileoskyTests/TestCases/4-message.txt"
            }
        };
    
    [Theory, MemberData(nameof(Files))]
    public void HandleMessage_ValidCheckSumFromFile_ShouldPublishMessage(
        string title, string messagePath, bool shouldPublish = true)
    {
        _testOutputHelper.WriteLine(title);

        var busMock = new MessageBusMock();
        var handler = new GalileoskyProtocolMessageHandler(busMock);

        var bytes = File.ReadAllLines(messagePath)
            .SelectMany(x => x.Split())
            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
            .ToArray();

        var resp = handler.HandleMessage(bytes).Select(x => x.ToString("X"));
        
        _testOutputHelper.WriteLine($"Response for tracker: {string.Join(' ', resp)}");

        busMock.Messages.Count.Should().Be(shouldPublish ? 1 : 0);
    }
}