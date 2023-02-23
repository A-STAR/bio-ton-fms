﻿using System.Linq.Expressions;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Expressions;
using BioTonFMS.Telematica;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests;

public class MessageProcessingTests
{
    [Fact]
    public void CalculateFallBackValues()
    {
        var message = new TrackerMessage()
        {
            Id = 1, Tags = new List<MessageTag>()
            {
                new MessageTagDouble()
                {
                    TagType = 4, IsFallback = false, TrackerTagId = 1, Value = 22
                }
            }
        };
        var previousMessage = new TrackerMessage()
        {
            Id = 1, Tags = new List<MessageTag>()
            {
                new MessageTagDouble()
                {
                    TagType = 4, IsFallback = false, TrackerTagId = 1, Value = 11
                },
                new MessageTagDouble()
                {
                    TagType = 4, IsFallback = true, TrackerTagId = 2, Value = 111
                },
                new MessageTagDouble()
                {
                    TagType = 4, IsFallback = false, TrackerTagId = 3, Value = 1111
                }
            }
        };
        MessageProcessing.CalculateFallBackValues(message, previousMessage);

        var tags1 = message.Tags.Where(t => t.TrackerTagId == 1).ToArray();
        tags1.Length.Should().Be(1);
        tags1[0].IsFallback.Should().BeFalse();
        tags1[0].TagType.Should().Be(4);
        (tags1[0] as MessageTagDouble)!.Value.Should().Be(22);

        var tags2 = message.Tags.Where(t => t.TrackerTagId == 2).ToArray();
        tags2.Length.Should().Be(1);
        tags2[0].IsFallback.Should().BeTrue();
        tags2[0].TagType.Should().Be(4);
        (tags2[0] as MessageTagDouble)!.Value.Should().Be(111);

        var tags3 = message.Tags.Where(t => t.TrackerTagId == 3).ToArray();
        tags3.Length.Should().Be(1);
        tags3[0].IsFallback.Should().BeTrue();
        tags3[0].TagType.Should().Be(4);
        (tags3[0] as MessageTagDouble)!.Value.Should().Be(1111);
    }

    [Fact]
    public void BuildSensors()
    {
        var trackers = new Tracker[]
        {
            new()
            {
                Id = 111, Sensors = new List<Sensor>()
                {
                    new()
                    {
                        Id = 222, Formula = "a"
                    }
                }
            }
        };
        var trackerTags = new TrackerTag[]
        {
            new()
            {
                Name = "a", DataType = TagDataTypeEnum.Double
            }
        };
        var result = trackers.BuildSensors(trackerTags).ToArray();
        result.Length.Should().Be(1);
        result[0].Item1.Should().Be(111);
        result[0].Item2.Length.Should().Be(1);
        result[0].Item2[0].Item1.Should().Be(222.ToString());
        result[0].Item2[0].Item2.Should().NotBeNull();
        result[0].Item2[0].Item2!.ToString().Should().Be("a => IIF(a.IsFallback, null, a.Value)");
    }

    [Fact]
    public void CalculateSensors()
    {
        var parameterA = Expression.Parameter(typeof( TagData<double> ), "a");
        var parametersA = new[]
        {
            parameterA
        };
        var parameterD = Expression.Parameter(typeof( TagData<double> ), "d");
        var parametersD = new[]
        {
            parameterD
        };
        var sensorExpressions = new List<(string, Expression?)>
        {
            (
                "b",
                Expression.Lambda(Expression.Property(parameterA, "Value"), parametersA)
            ),
            (
                "c",
                Expression.Lambda(Expression.Property(parameterA, "IsFallback"), parametersA)
            ),
            (
                "e",
                Expression.Lambda(Expression.Property(parameterD, "Value"), parametersD)
            ),
            (
                "f",
                Expression.Lambda(Expression.Property(parameterD, "IsFallback"), parametersD)
            )
        };
        var messageTags = new MessageTag[]
        {
            new MessageTagDouble()
            {
                TrackerTagId = 123, TagType = 4, IsFallback = false, Value = 345
            },
            new MessageTagDouble()
            {
                TrackerTagId = 124, TagType = 4, IsFallback = true, Value = 543
            }
        };
        var tagNameById = new Dictionary<int, string>
        {
            {
                123, "a"
            },
            {
                124, "d"
            }
        };

        var result = sensorExpressions.CalculateSensors(messageTags, tagNameById).ToArray();

        result.Length.Should().Be(4);

        result[0].Item1.Should().Be("b");
        result[0].Item2.Should().NotBeNull();
        result[0].Item2!.Should().Be(345);

        result[1].Item1.Should().Be("c");
        result[1].Item2.Should().NotBeNull();
        result[1].Item2!.Should().Be(false);

        result[2].Item1.Should().Be("e");
        result[2].Item2.Should().NotBeNull();
        result[2].Item2!.Should().Be(543);

        result[3].Item1.Should().Be("f");
        result[3].Item2.Should().NotBeNull();
        result[3].Item2!.Should().Be(true);
    }

    [Fact]
    public void UpdateSensorTags()
    {
        var messages = new[]
        {
            new TrackerMessage()
            {
                TrId = 111, Tags = new List<MessageTag>()
                {
                    new MessageTagDouble()
                    {
                        TagType = 4, IsFallback = false, TrackerTagId = 123, Value = 22
                    },
                    new MessageTagDouble()
                    {
                        TagType = 4, IsFallback = false, SensorId = 12332, Value = 2345
                    }
                }
            }
        };
        var trackers = new Tracker[]
        {
            new()
            {
                Id = 111, Sensors = new List<Sensor>()
                {
                    new()
                    {
                        Id = 222, Formula = "a + const2"
                    }
                }
            }
        };
        var trackerTags = new TrackerTag[]
        {
            new()
            {
                Id = 123, Name = "a", DataType = TagDataTypeEnum.Double
            }
        };


        messages.UpdateSensorTags(trackers, trackerTags);

        messages.Length.Should().Be(1);
        messages[0].Tags.Count.Should().Be(2);

        messages[0].Tags[0].Should().BeOfType<MessageTagDouble>();
        messages[0].Tags[0].SensorId.Should().BeNull();
        messages[0].Tags[0].TrackerTagId.Should().Be(123);

        messages[0].Tags[1].Should().BeOfType<MessageTagDouble>();
        messages[0].Tags[1].TrackerTagId.Should().BeNull();
        messages[0].Tags[1].SensorId.Should().Be(222);
        (messages[0].Tags[1] as MessageTagDouble)!.Value.Should().Be(24);
    }
}
