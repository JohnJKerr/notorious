using System.Collections.Generic;
using System.Linq;
using Domain;
using MongoDB.Driver;
using Xunit;

namespace Storage.Tests
{
	public class NoteStorageTests : MongoStorageClientTests<Note>
	{
		protected override Note GenerateEntity()
		{
			return new Note.Builder()
				.Build();
		}

		protected override IEnumerable<Note> GetFromCollectionProperty()
		{
			return Client.Notes
				.Find(Builders<Note>.Filter.Empty)
				.ToList();
		}
	}
}