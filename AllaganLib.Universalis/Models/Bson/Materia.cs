using MongoDB.Bson.Serialization.Attributes;

namespace AllaganLib.Universalis.Models.Bson;

public class Materia
{
    [BsonElement("slotID")]
    public int SlotID { get; set; }

    [BsonElement("materiaID")]
    public int MateriaID { get; set; }
}
