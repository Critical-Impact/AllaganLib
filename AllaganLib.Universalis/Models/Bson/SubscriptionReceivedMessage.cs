using System.Collections.Generic;
using AllaganLib.Universalis.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace AllaganLib.Universalis.Models.Bson;

public class SubscriptionReceivedMessage
{
    public UniversalisWebsocketService.EventType EventType
    {
        get
        {
            return this.Event switch
            {
                "listings/add" => UniversalisWebsocketService.EventType.ListingsAdd,
                "listings/remove" => UniversalisWebsocketService.EventType.ListingsRemove,
                "sales/add" => UniversalisWebsocketService.EventType.SalesAdd,
                "sales/remove" => UniversalisWebsocketService.EventType.SalesRemove,
                _ => UniversalisWebsocketService.EventType.Unknown
            };
        }
    }

    [BsonElement("event")]
    public string Event { get; set; }

    [BsonElement("item")]
    public uint Item { get; set; }

    [BsonElement("world")]
    public uint World { get; set; }

    [BsonElement("listings")]
    public List<Listing> Listings { get; set; }

    [BsonElement("sales")]
    public List<Sale> Sales { get; set; }
}
