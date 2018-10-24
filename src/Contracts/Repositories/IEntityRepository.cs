using System;
using System.Collections.Generic;
using Contracts.Filters;
using Domain;

namespace Contracts.Repositories
{
	public interface IEntityRepository<TEntity, TFilter>
		where TEntity : Entity
		where TFilter : Filter
	{
		TEntity GetById(Guid id);
		IList<TEntity> GetByIds(params Guid[] ids);
		IList<TEntity> Get(TFilter filter);
		void Add(TEntity entity);
		void Update(TEntity entity);
		void Delete(TEntity entity);
		void SaveChanges();
	}
}