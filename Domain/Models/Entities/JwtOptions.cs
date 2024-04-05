namespace Domain.Models.Entities
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;

        public DateTime Expires { get; set; } = DateTime.UtcNow.AddMinutes(60);
    }
}
