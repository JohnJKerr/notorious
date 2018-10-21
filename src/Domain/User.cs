namespace Domain
{
	public class User : Entity
	{
		public class Builder : Entity.Builder<User>
		{
			public override User Build()
			{
				var user = new User();
				SetBaseProperties(user);
				return user;
			}
		}
	}
}