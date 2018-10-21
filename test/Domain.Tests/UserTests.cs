namespace Domain.Tests
{
	public class UserTests : EntityTests<User, User.Builder>
	{
		protected override User.Builder CreateBuilder()
		{
			return new User.Builder();
		}
	}
}