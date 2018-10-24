using System;
using System.Linq;
using Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Tag = Domain.Tag;

namespace Storage
{
	public sealed class MongoStorageClient
	{
		private readonly IMongoDatabase _database;

		public MongoStorageClient(string connectionString, string dbName)
		{
			MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(1);
			var client = new MongoClient(connectionString);
			_database = client.GetDatabase(dbName);
			RegisterMappings();
		}

		public IMongoCollection<T> Set<T>() => _database.GetCollection<T>(typeof(T).Name);
		public IMongoCollection<Note> Notes => Set<Note>();
		public IMongoCollection<Tag> Tags => Set<Tag>();
		public IMongoCollection<User> Users => Set<User>();

		private void RegisterMappings()
		{
			if (BsonClassMap.GetRegisteredClassMaps().Any()) return;

			BsonClassMap.RegisterClassMap<Entity>(cm =>
			{
				cm.AutoMap();
				cm.SetIsRootClass(true);
				cm.MapField("_tags")
					.SetElementName(nameof(Entity.Tags));
			});

			BsonClassMap.RegisterClassMap<Note>();
		}
	}
}