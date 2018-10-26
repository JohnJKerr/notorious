using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
	public abstract class Entity
	{
		private List<Tag> _tags;
		private List<UpdateAudit> _updateAudits;
		
		protected Entity()
		{
			Id = Guid.NewGuid();
			_tags = new List<Tag>();
			_updateAudits = new List<UpdateAudit>();
		}
		
		public Guid Id { get; private set; }
		public IEnumerable<Tag> Tags => _tags;
		public IEnumerable<UpdateAudit> UpdateAudits => _updateAudits;

		public void AddTag(Tag tag)
		{
			if(_tags.Contains(tag)) throw new ArgumentException($"Entity already has tag with ID: {tag.Id}");
			_tags.Add(tag);
		}

		public void RemoveTag(Tag tag)
		{
			if(!_tags.Contains(tag)) throw new ArgumentException($"Entity does not have tag with ID: {tag.Id}");
			_tags.Remove(tag);
		}

		public void AddUpdateAudit(User user, DateTime date)
		{
			_updateAudits.Add(new UpdateAudit(user, date));
		}

		public abstract class Builder<TEntity>
			where TEntity : Entity
		{
			private readonly List<Tag> _tags = new List<Tag>();
			
			public Builder<TEntity> WithTags(params Tag[] tags)
			{
				_tags.AddRange(tags);
				return this;
			}

			private void AddTags(TEntity entity)
			{
				_tags
					.ToList()
					.ForEach(entity.AddTag);
			}

			protected void SetBaseProperties(TEntity entity)
			{
				AddTags(entity);
			}
			
			public abstract TEntity Build();
		}
	}
}