using System.Collections.Generic;
using Domain;
using MongoDB.Driver;

namespace Storage.Tests
{
	public class UserStorageTests : MongoStorageClientTests<User>
	{
		protected override User GenerateEntity()
		{
			return new User.Builder()
				.Build();
		}

		protected override IEnumerable<User> GetFromCollectionProperty()
		{
			return Client.Users
				.Find(Builders<User>.Filter.Empty)
				.ToList();
		}
	}
}