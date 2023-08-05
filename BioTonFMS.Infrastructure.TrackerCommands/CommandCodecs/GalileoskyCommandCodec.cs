using System.Text;
using Azure;
using BioTonFMS.Common.Constants;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.TrackerProtocolSpecific.TrackerMessages;

namespace BioTonFMS.TrackerProtocolSpecific.CommandCodecs;

public class GalileoskyCommandCodec : ICommandCodec
{
    private const byte CommandNumberTag = 0xE0;
    private const byte CommandTextTag = 0xE1;
    private const byte BinaryDataTag = 0xEB;
    private const byte ImeiTag = 0x03;
    private const byte ExternalIdTag = 0x04;

    public byte[] EncodeCommand(Tracker tracker, int commandId, string commandText)
    {
        // описание протокола
        // http://base.galileosky.com/articles/#!docs-publication/galileosky-protocol/a/h2__1673588740
        var result = new List<byte>(16) { ImeiTag };

        byte[] imei = Encoding.UTF8.GetBytes(tracker.Imei);
        result.AddRange(imei);

        result.Add(ExternalIdTag);
        byte[] externalId = BitConverter.GetBytes((ushort)tracker.ExternalId);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(externalId);
        result.AddRange(externalId);

        result.Add(CommandNumberTag);
        byte[] cmdNum = BitConverter.GetBytes(commandId);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(cmdNum);
        result.AddRange(cmdNum);

        result.Add(CommandTextTag);
        byte[] cmdText = Encoding.UTF8.GetBytes(commandText);
        result.Add((byte)cmdText.Length);
        result.AddRange(cmdText);

        byte[] length = BitConverter.GetBytes((ushort)result.Count);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(length);
        result.Insert(0, Galileosky.HeaderTag);
        result.InsertRange(1, length);

        ushort crc = Galileosky.GetCrc(result.ToArray(), result.Count);
        byte[] crcBytes = BitConverter.GetBytes(crc);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(crcBytes);
        result.AddRange(crcBytes);

        return result.ToArray();
    }

    public CommandResponseInfo DecodeCommand(byte[] commandResponse)
    {
        CommandResponseInfo responseInfo = new ();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var i = 0;
        i += Galileosky.HeaderLength;
        while (i < commandResponse.Length - Galileosky.CheckSumLength)
        {
            switch (commandResponse[i])
            {
                case ImeiTag:
                    responseInfo.Imei = commandResponse[(i+1)..((i+1) + 15)].ParseToString();
                    i += 15;
                    break;
                case ExternalIdTag:
                    responseInfo.ExternalId = commandResponse[(i+1)..((i+1) + 2)].ParseToInt();
                    i += 2;
                    break;
                case CommandNumberTag:
                    responseInfo.CommandId = commandResponse[(i+1)..((i+1) + 4)].ParseToInt();
                    i += 4;
                    break;
                case CommandTextTag:
                    // В следующем за тегом байте (i + 1) содержится длина текста
                    // Сам текст начинается ещё на байт дальше, поэтому i + 2
                    responseInfo.ResponseText = Encoding.GetEncoding("windows-1251").GetString(
                        commandResponse[(i + 2)..(i + 2 + commandResponse[i + 1])]);
                    i += commandResponse[i + 1] + 1;
                    break;
                case BinaryDataTag:
                    // Тут то же самое, просто в строку не переводим
                    responseInfo.ResponseBinary = commandResponse[(i + 2)..(i + 2 + commandResponse[i + 1])];
                    break;
            }

            i++;
        }

        return responseInfo;
    }
}