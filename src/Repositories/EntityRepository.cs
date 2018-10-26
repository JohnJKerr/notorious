using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.Filters;
using Contracts.Providers;
using Contracts.Repositories;
using Domain;
using MongoDB.Driver;
using Storage;

namespace Repositories
{
	public abstract class EntityRepository<TEntity, TFilter> : IEntityRepository<TEntity, TFilter>, ITenantRepository
		where TEntity : Entity
		where TFilter : Filter
	{
		private readonly MongoStorageClient _client;
		private readonly List<TEntity> _inserts;
		private readonly List<TEntity> _updates;
		private readonly List<TEntity> _deletions;
		protected virtual FilterDefinition<TEntity> DefaultReadFilter => Builders<TEntity>.Filter.Empty;
		protected virtual FilterDefinition<TEntity> DefaultWriteFilter => Builders<TEntity>.Filter.Empty;
		protected Guid TenantUserId;

		protected EntityRepository(MongoStorageClient client)
		{
			_client = client;
			_inserts = new List<TEntity>();
			_updates = new List<TEntity>();
			_deletions = new List<TEntity>();
		}

		private IMongoCollection<TEntity> Collection
		{
			get
			{
				if(TenantUserId == default(Guid)) 
					throw new InvalidOperationException("Cannot access data collections without setting tenancy");
				return _client.Set<TEntity>();
			}
		}

		public TEntity GetById(Guid id)
		{
			var filter = DefaultReadFilter & Builders<TEntity>.Filter.Eq(e => e.Id, id);
			return Collection
				.Find(filter)
				.FirstOrDefault();
		}

		public IList<TEntity> GetByIds(params Guid[] ids)
		{
			if(!ids.Any()) throw new ArgumentException("Must provide ids in order to get by ids");
			var filter = DefaultReadFilter & Builders<TEntity>.Filter.In(e => e.Id, ids);
			return Collection
				.Find(filter)
				.ToList();
		}

		public IList<TEntity> Get(TFilter filter)
		{
			var queryFilter = DefaultReadFilter & MapFilter(filter);
			return Collection
				.Find(queryFilter)
				.Skip(filter.Skip)
				.Limit(filter.Take)
				.ToList();
		}

		public void Add(TEntity entity) => AddToModifierCollection(_inserts, entity);
		public void Update(TEntity entity) => AddToModifierCollection(_updates, entity);
		public void Delete(TEntity entity) => AddToModifierCollection(_deletions, entity);

		public void SaveChanges()
		{
			ProcessDeletions();
			ProcessInserts();
			ProcessUpdates();
		}

		public void SetTenancy(Guid userId)
		{
			TenantUserId = userId;
		}

		protected virtual FilterDefinition<TEntity> MapFilter(TFilter filter)
		{
			return Builders<TEntity>.Filter.Empty;
		}

		private void ProcessDeletions()
		{
			if (!_deletions.Any()) return;
			var deletionIds = _deletions.Select(e => e.Id);
			var filter = DefaultWriteFilter & Builders<TEntity>.Filter.In(e => e.Id, deletionIds);
			Collection.DeleteMany(filter);
			_deletions.Clear();
		}

		private void ProcessInserts()
		{
			if (!_inserts.Any()) return;
			Collection.InsertMany(_inserts);
			_inserts.Clear();
		}

		private void ProcessUpdates()
		{
			if (!_updates.Any()) return;

			void ReplaceEntity(TEntity entity)
			{
				var filter = DefaultWriteFilter & Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
				Collection.ReplaceOne(filter, entity);
			}
			
			_updates.ForEach(ReplaceEntity);
			_updates.Clear();
		}

		private static void AddToModifierCollection(IList<TEntity> collection, TEntity entity)
		{
			if (collection.Contains(entity)) return;
			collection.Add(entity);
		}
	}
}