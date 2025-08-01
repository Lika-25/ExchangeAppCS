namespace Exchange_appl
{
    public class Item
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? ExchangeCategory { get; set; } 
        public string? Image { get; set; }
        public string? AuthorPhone { get; set; }
    }
}
