using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Xunit;
using Tag = Domain.Tag;

namespace Storage.Tests
{
	public class TagStorageTests : MongoStorageClientTests<Tag>
	{
		protected override Tag GenerateEntity()
		{
			return new Tag.Builder(Guid.NewGuid().ToString())
				.Build();
		}

		protected override IEnumerable<Tag> GetFromCollectionProperty()
		{
			return Client.Tags
				.Find(Builders<Tag>.Filter.Empty)
				.ToList();
		}
	}
}