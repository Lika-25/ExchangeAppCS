namespace Exchange_appl.Models
{
    public partial class Exchangeoffer
    {
        public int Id { get; set; }

        public int ItemOfferedId { get; set; }

        public int ItemRequestedId { get; set; }  // Убираем ? для запроса null

        public int ReceiverId { get; set; }

        public string Status { get; set; } = "Waiting"; // Значение по умолчанию

        public virtual Item ItemOffered { get; set; } = null!;
        public virtual Item ItemRequested { get; set; } = null!;  // Убираем ? для запроса null

        public virtual User Receiver { get; set; } = null!;
        public virtual User Sender { get; set; } = null!;  // Добавляем поле для отправителя

        // Список допустимых статусов
        public static readonly string[] ValidStatuses = { "Waiting", "Accepted", "Rejected" };

        // Проверка и установка допустимого статуса
        public void SetStatus(string newStatus)
        {
            if (!ValidStatuses.Contains(newStatus))
            {
                throw new ArgumentException($"Недопустимый статус: {newStatus}");
            }
            Status = newStatus;
        }
    }
}
