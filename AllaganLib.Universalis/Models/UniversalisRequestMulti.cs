using System.Collections.Generic;

namespace AllaganLib.Universalis.Models;

public class UniversalisRequestMulti
{
    public string[] itemIDs { internal get; set; }

    public Dictionary<string, UniversalisRequestSingle> items { internal get; set; }
}
