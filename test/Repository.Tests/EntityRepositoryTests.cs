using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.Filters;
using Domain;
using MongoDB.Driver;
using Repositories;
using Storage;
using Xunit;
using Tag = Domain.Tag;

namespace Repository.Tests
{
	[Collection("Database Integration Tests")]
	public abstract class EntityRepositoryTests<TRepository, TEntity, TFilter> : IDisposable
		where TRepository : EntityRepository<TEntity, TFilter>
		where TEntity : Entity
		where TFilter : Filter
	{
		private const string ConnectionString = "mongodb://localhost:27017";
		private const string DbName = "notorious-repository-integ-tests";

		protected readonly MongoStorageClient Client;
		protected readonly TRepository Repository;
		protected abstract TRepository CreateRepository();
		protected abstract TFilter CreateFilter();

		protected EntityRepositoryTests()
		{
			Client = new MongoStorageClient(ConnectionString, DbName);
			Repository = CreateRepository();
		}

		public void Dispose()
		{
			var mongoClient = new MongoClient(ConnectionString);
			mongoClient.DropDatabase(DbName);
		}

		[Fact]
		public void GetById_returns_the_correct_Entity()
		{
			// arrange
			var entity = CreateAndSaveEntity();

			// act
			var getEntity = Repository.GetById(entity.Id);

			// assert
			Assert.Equal(entity, getEntity, new EntityComparer());
		}

		[Fact]
		public void GetById_returns_null_when_entity_does_not_exist()
		{
			// act
			var getEntity = Repository.GetById(Guid.NewGuid());

			// assert
			Assert.Null(getEntity);
		}

		[Fact]
		public void GetByIds_returns_the_correct_Entities()
		{
			// arrange
			var entities = CreateAndSaveEntities();
			var entityIds = entities.Select(e => e.Id).ToArray();

			// act
			var getEntities = Repository.GetByIds(entityIds);

			// assert
			Assert.Equal(entities, getEntities, new EntityComparer());
		}

		[Fact]
		public void GetByIds_throws_ArgumentException_when_no_ids_are_provided()
		{
			// act/assert
			Assert.Throws<ArgumentException>(() => Repository.GetByIds());
		}

		[Fact]
		public void Get_with_Filter_Take_set_returns_specified_number_of_Entities()
		{
			// arrange
			CreateAndSaveEntities();
			const int numberToTake = 5;
			var filter = CreateFilter();
			filter.Take = numberToTake;

			// act
			var getEntities = Repository.Get(filter);

			// assert
			Assert.Equal(numberToTake, getEntities.Count);
		}

		[Fact]
		public void Get_with_Filter_Skip_set_skips_the_specified_number_of_Entities()
		{
			// arrange
			CreateAndSaveEntities();
			const int numberToSkip = 5;
			var filter = CreateFilter();
			filter.Skip = numberToSkip;
			var getWithoutSkip = Repository.Get(CreateFilter());
			var expected = getWithoutSkip.Skip(5);
			
			// act
			var getWithSkip = Repository.Get(filter);
			
			// assert
			Assert.Equal(expected, getWithSkip, new EntityComparer());
		}

		[Fact]
		public void Get_returns_empty_list_when_entities_do_not_exist()
		{
			// act
			var getEntities = Repository.Get(CreateFilter());
			
			// assert
			Assert.Empty(getEntities);
		}

		[Fact]
		public void Add_before_SaveChanges_adds_Entity_to_Collection()
		{
			// arrange
			var entity = CreateEntity();
			
			// act
			Repository.Add(entity);
			Repository.SaveChanges();
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.Equal(entity, getEntity, new EntityComparer());
		}

		[Fact]
		public void Add_same_Entity_twice_before_SaveChanges_does_not_insert_duplicate()
		{
			// arrange
			var entity = CreateEntity();
			var entityCount = GetEntityCount();
			
			// act
			Repository.Add(entity);
			Repository.Add(entity);
			Repository.SaveChanges();
			
			// assert
			Assert.Equal(entityCount + 1, GetEntityCount());
		}

		[Fact]
		public void Add_Entity_twice_before_SaveChanges_inserts_the_most_recent_version()
		{
			throw new NotImplementedException();
		}

		[Fact]
		public void Add_before_SaveChanges_calls_InitialiseCreateAudit_on_Entities()
		{
			// arrange
			var entity = CreateEntity();
			
			// act
			Repository.Add(entity);
			Repository.SaveChanges();
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.NotEqual(default(DateTime), getEntity.CreatedDate);
		}

		[Fact]
		public void SaveChanges_twice_after_Add_does_not_add_two_Entities_to_Collection()
		{
			// arrange
			var entityCount = GetEntityCount();
			var entity = CreateEntity();

			// act
			Repository.Add(entity);
			Repository.SaveChanges();
			Repository.SaveChanges();
			
			// assert
			Assert.Equal(entityCount + 1, GetEntityCount());
		}

		[Fact]
		public void Add_without_SaveChanges_does_not_add_Entity_to_Collection()
		{
			// arrange
			var entity = CreateEntity();
			
			// act
			Repository.Add(entity);
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.Null(getEntity);
		}

		[Fact]
		public void Delete_before_SaveChanges_removes_Entity_from_Collection()
		{
			// arrange
			var entity = CreateAndSaveEntity();
			
			// act
			Repository.Delete(entity);
			Repository.SaveChanges();
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.Null(getEntity);
		}

		[Fact]
		public void Delete_without_SaveChanges_does_not_remove_Entity_from_Collection()
		{
			// arrange
			var entity = CreateAndSaveEntity();
			
			// act
			Repository.Delete(entity);
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.NotNull(getEntity);
		}

		[Fact]
		public void Update_before_SaveChanges_updates_the_Entity_in_the_Collection()
		{
			// arrange
			var entity = CreateAndSaveEntity();
			AddTagToEntity(entity);
			
			// act
			Repository.Update(entity);
			Repository.SaveChanges();
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.Equal(entity.Tags, getEntity.Tags, new EntityComparer());
		}

		[Fact]
		public void Update_before_SaveChanges_calls_InitialiseUpdateAudit_on_Entities()
		{
			// arrange
			var entity = CreateAndSaveEntity();
			AddTagToEntity(entity);
			
			// act
			Repository.Update(entity);
			Repository.SaveChanges();
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.NotEqual(default(DateTime), getEntity.LastModifiedDate);
		}

		[Fact]
		public void Update_without_SaveChanges_does_not_update_the_Entity_in_the_Collection()
		{
			// arrange
			var entity = CreateAndSaveEntity();
			AddTagToEntity(entity);
			
			// act
			Repository.Update(entity);
			
			// assert
			var getEntity = Repository.GetById(entity.Id);
			Assert.NotEqual(entity.Tags, getEntity.Tags, new EntityComparer());
		}
		
		[Fact]
		public void Update_Entity_twice_before_SaveChanges_updates_to_the_most_recent_version()
		{
			throw new NotImplementedException();
		}

		[Fact]
		public void SaveChanges_without_collection_modifier_does_not_throw()
		{
			// act
			var exception = Record.Exception(() => Repository.SaveChanges());
			
			// assert
			Assert.Null(exception);
		}

		protected abstract TEntity CreateEntity();

		private TEntity CreateAndSaveEntity()
		{
			var entity = CreateEntity();
			Repository.Add(entity);
			Repository.SaveChanges();
			return entity;
		}

		private IList<TEntity> CreateAndSaveEntities(int numberToCreate = 10)
		{
			var entities = Enumerable.Range(0, numberToCreate)
				.ToList()
				.Select(_ => CreateAndSaveEntity())
				.ToList();
			return entities;
		}

		private void AddTagToEntity(TEntity entity)
		{
			var tag = new Tag.Builder(Guid.NewGuid().ToString())
				.Build();
			entity.AddTag(tag);
		}
		
		private long GetEntityCount() => Client.Set<TEntity>().CountDocuments(Builders<TEntity>.Filter.Empty);

		private class EntityComparer : IEqualityComparer<Entity>
		{
			public bool Equals(Entity x, Entity y)
			{
				return x.Id.Equals(y.Id);
			}

			public int GetHashCode(Entity obj)
			{
				throw new NotImplementedException();
			}
		}
	}
}