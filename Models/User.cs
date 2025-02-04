namespace ProvisionAPI.Models
{
	public class User
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime LastModifyDate { get; set; }
		public DateTime PasswordExpiry { get; set; }
	}
}
