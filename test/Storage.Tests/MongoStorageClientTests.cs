using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using MongoDB.Driver;
using Xunit;
using Tag = Domain.Tag;

namespace Storage.Tests
{
	[Collection("Database Integration Tests")]
	public abstract class MongoStorageClientTests<TEntity> : IDisposable
		where TEntity : Entity
	{
		private const string ConnectionString = "mongodb://localhost:27017";
		private const string DbName = "notorious-storage-integ-tests";
		protected readonly MongoStorageClient Client;

		protected MongoStorageClientTests()
		{
			Client = new MongoStorageClient(ConnectionString, DbName);
		}

		public void Dispose()
		{
			var mongoClient = new MongoClient(ConnectionString);
			mongoClient.DropDatabase(DbName);
		}

		[Fact]
		public void Set_returns_a_Collection_of_TEntity()
		{
			// arrange
			GenerateAndInsertEntity();

			// act
			var entities = Get();

			// assert
			Assert.True(entities.Any());
		}

		[Fact]
		public void Set_returns_entities_with_Id_set()
		{
			// arrange
			var entity = GenerateAndInsertEntity();

			// act
			var getEntity = Get(entity.Id).First();

			// assert
			Assert.Equal(entity.Id, getEntity.Id);
		}

		[Fact]
		public void Set_returns_entities_with_Tags_set()
		{
			// arrange
			var entity = GenerateAndInsertEntity(GenerateTags());

			// act
			var getEntity = Get(entity.Id).First();

			// assert
			var entityIds = entity.Tags.Select(t => t.Id);
			var getEntityIds = getEntity.Tags.Select(t => t.Id);
			Assert.Equal(entityIds, getEntityIds);
		}

		[Fact]
		public void Collection_property_returns_a_collection()
		{
			// arrange
			GenerateAndInsertEntity();
			
			// act
			var output = GetFromCollectionProperty();
			
			// assert
			Assert.True(output.Any());
		}

		protected abstract TEntity GenerateEntity();

		protected TEntity GenerateAndInsertEntity(params Tag[] tags)
		{
			var entity = GenerateEntity();
			tags
				.ToList()
				.ForEach(t => entity.AddTag(t));
			Client.Set<TEntity>().InsertOne(entity);
			return entity;
		}

		protected abstract IEnumerable<TEntity> GetFromCollectionProperty();

		/// <summary>
		/// Returns entities via the Set method
		/// </summary>
		/// <param name="ids">When not specified returns all entities</param>
		/// <returns></returns>
		private IEnumerable<TEntity> Get(params Guid[] ids)
		{
			var filter = Builders<TEntity>.Filter.Empty;
			if (ids.Any()) filter = filter & Builders<TEntity>.Filter.In(e => e.Id, ids);
			return Client.Set<TEntity>()
				.Find(filter)
				.ToList();
		}

		private Tag[] GenerateTags(int numberToGenerate = 10)
		{
			Tag GenerateTag()
			{
				return new Tag.Builder(Guid.NewGuid().ToString())
					.Build();
			}

			return Enumerable.Range(0, numberToGenerate)
				.ToList()
				.Select(_ => GenerateTag())
				.ToArray();
		}
	}
}