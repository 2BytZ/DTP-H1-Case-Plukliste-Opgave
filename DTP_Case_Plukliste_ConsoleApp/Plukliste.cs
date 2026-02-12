using FKTV_DAL;

namespace Plukliste;

public class Pluklist
{
    public string? Name { get; set; }
    public string? Forsendelse { get; set; }
    public string? Adresse { get; set; }
    public List<Item> Lines { get; set; } = new List<Item>();
    public void AddItem(Item item) => Lines.Add(item);
}

public class Item
{
    public string ProductID { get; set; }
    public string Title { get; set; }
    public ItemType Type { get; set; }
    public int Amount { get; set; }
    public int AmountInStorage { get; set; }
}

public enum ItemType
{
    Fysisk, Print
}