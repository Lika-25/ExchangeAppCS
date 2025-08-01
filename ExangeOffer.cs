namespace Exchange_appl
{
    public class ExchangeOffer
    {
        public int Id { get; set; }
        public int ItemOfferedId { get; set; }
        public int ItemRequestedId { get; set; }
        public int ReceiverId { get; set; }
        public string? Status { get; set; }
    }
}


