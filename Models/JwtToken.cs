namespace ProvisionAPI.Models
{
	public class JwtToken
	{
		public string Token { get; set; }
		public RefreshToken refreshToken { get; set; }
	}
	public class RefreshToken
	{
		public int ID { get; set; }
		public string Token { get; set; }
		public string JwtId { get; set; }
		public int UserId { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime ExpiryDate { get; set; }
		public bool Used { get; set; }
	}
}
