using System;
using Contracts.Filters;
using Domain;
using Repositories;

namespace Repository.Tests
{
	public class UserRepositoryTests : EntityRepositoryTests<UserRepository, User, Filter>
	{
		protected override UserRepository CreateRepository() => new UserRepository(Client);
		protected override Filter CreateFilter() => new Filter();

		protected override User CreateEntity()
		{
			return new User.Builder()
				.Build();
		}
	}
}