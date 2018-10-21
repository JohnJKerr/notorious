using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
	public abstract class Entity
	{
		private readonly List<Tag> _tags;
		
		protected Entity()
		{
			Id = Guid.NewGuid();
			_tags = new List<Tag>();
		}
		
		public Guid Id { get; }
		public IEnumerable<Tag> Tags => _tags;
		public User CreatedByUser { get; private set; }
		public DateTime CreatedDate { get; private set; }

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

		public void InitialiseAudit(User createdBy, DateTime createdDate)
		{
			CreatedByUser = createdBy;
			CreatedDate = createdDate;
		}

		public abstract class Builder<TEntity>
			where TEntity : Entity
		{
			private readonly List<Tag> _tags = new List<Tag>();
			private bool _hasAudit = false;
			private User _createdByUser;
			private DateTime _createdDate;
			
			public Builder<TEntity> WithTags(params Tag[] tags)
			{
				_tags.AddRange(tags);
				return this;
			}

			public Builder<TEntity> WithAudit(User createdBy, DateTime createdDate)
			{
				_hasAudit = true;
				_createdByUser = createdBy;
				_createdDate = createdDate;
				return this;
			}
			
			private void AddTags(TEntity entity)
			{
				_tags
					.ToList()
					.ForEach(entity.AddTag);
			}

			private void InitialiseAudit(TEntity entity)
			{
				if (!_hasAudit) return;
				entity.InitialiseAudit(_createdByUser, _createdDate);
			}

			protected void SetBaseProperties(TEntity entity)
			{
				AddTags(entity);
				InitialiseAudit(entity);
			}
			
			public abstract TEntity Build();
		}
	}
}