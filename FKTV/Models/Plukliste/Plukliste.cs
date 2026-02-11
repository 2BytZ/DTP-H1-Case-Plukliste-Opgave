using FKTV.Models.Items;

namespace FKTV.Models.Plukliste
{
    public class Plukliste
    {
        public string? Name { get; set; }
        public string? Forsendelse { get; set; }
        public string? Adresse { get; set; }
        public List<Item> Lines { get; set; } = new List<Item>();
        public void AddItem(Item item) => Lines.Add(item);
    }
}
