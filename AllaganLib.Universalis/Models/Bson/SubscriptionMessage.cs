using MongoDB.Bson.Serialization.Attributes;

namespace AllaganLib.Universalis.Models.Bson;

public class SubscriptionMessage(string @event, string channel)
{
    [BsonElement("event")]
    public string Event { get; } = @event;

    [BsonElement("channel")]
    public string Channel { get; } = channel;
}
