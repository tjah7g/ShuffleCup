namespace GameApi.Models
{
    public class PlayerScore
    {
        public int Id { get; set; }
        public string PlayerName { get; set; } = string.Empty; // Wajib
        public int Score { get; set; } // Wajib
        public int Level { get; set; } = 1; // TAMBAHAN: Level pemain
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Otomatis
    }
}