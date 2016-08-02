using POGOProtos.Inventory.Item;

namespace PokeGoBot.Core.CrossCutting.Classes
{
    public class ItemPerCount
    {
        public ItemId Item { get; set; }
        public int Count { get; set; }
        public bool Unseen { get; set; }
    }
}
