namespace FKTV.Models.Items
{
    public class Item
    {
        public string ProductID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; } //remove itemtype in favor of string if errors occour
        public int Amount { get; set; }
    }
}
