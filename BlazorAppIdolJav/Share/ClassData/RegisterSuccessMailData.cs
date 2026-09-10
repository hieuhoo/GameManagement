namespace GameManagement.Share.ClassData
{
	public class RegisterSuccessMailData
	{
		public required string FullName { get; set; }
		public required string UserName { get; set; }
		public required DateTime RegistrationDate { get; set; }
		public required string Email { get; set; }

	}
}
