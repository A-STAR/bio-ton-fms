using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.Telematica.Data.Mapping;

internal static class CsvMessageTagsConverter
{
    public static IList<MessageTag> ConvertToMessageTags(this IEnumerable<MessageTagCsv> csvMessageTags, TrackerMessage message)
    {
        List<MessageTag> tagList = new List<MessageTag>();
        foreach (var csvMessageTag in csvMessageTags)
        {
            MessageTag messageTag;
            switch (csvMessageTag.TagType) 
            {
                case TagDataTypeEnum.Double:
                    messageTag = new MessageTagDouble() { Value = csvMessageTag.DoubleValue!.Value, TagType = TagDataTypeEnum.Double };
                    break;
                case TagDataTypeEnum.Boolean:
                    messageTag = new MessageTagBoolean() { Value = csvMessageTag.BooleanValue!.Value, TagType = TagDataTypeEnum.Boolean };
                    break;
                case TagDataTypeEnum.String:
                    messageTag = new MessageTagString() { Value = csvMessageTag.StringValue!, TagType = TagDataTypeEnum.String };
                    break;
                case TagDataTypeEnum.DateTime:
                    messageTag = new MessageTagDateTime() { Value = csvMessageTag.DateTimeValue!.Value.ToUniversalTime(), TagType = TagDataTypeEnum.DateTime };
                    break;
                case TagDataTypeEnum.Byte:
                    messageTag = new MessageTagByte() { Value = csvMessageTag.ByteValue!.Value, TagType = TagDataTypeEnum.Byte };
                    break;
                case TagDataTypeEnum.Integer:
                    messageTag = new MessageTagInteger() { Value = csvMessageTag.IntegerValue!.Value, TagType = TagDataTypeEnum.Integer };
                    break;
                default:
                    throw new NotImplementedException();
            }
            messageTag.TrackerMessage = message;
            messageTag.TrackerTagId = csvMessageTag.TrackerTagId;
            messageTag.IsFallback = csvMessageTag.IsFallback;

            tagList.Add(messageTag);
        }

        return tagList;
    }
}
