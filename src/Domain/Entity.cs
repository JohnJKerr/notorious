using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
	public abstract class Entity
	{
		private List<Tag> _tags;
		private bool _hasCreateAudit;
		
		protected Entity()
		{
			Id = Guid.NewGuid();
			_tags = new List<Tag>();
		}
		
		public Guid Id { get; private set; }
		public IEnumerable<Tag> Tags => _tags;
		public User CreatedByUser { get; private set; }
		public DateTime CreatedDate { get; private set; }
		public User LastModifiedByUser { get; private set; }
		public DateTime LastModifiedDate { get; private set; }

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

		public void InitialiseCreateAudit(User createdBy, DateTime createdDate)
		{
			_hasCreateAudit = true;
			CreatedByUser = createdBy;
			CreatedDate = createdDate;
		}

		public void InitialiseUpdateAudit(User modifiedBy, DateTime modifiedDate)
		{
			if(!_hasCreateAudit) throw new InvalidOperationException();
			LastModifiedByUser = modifiedBy;
			LastModifiedDate = modifiedDate;
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

			public Builder<TEntity> WithCreateAudit(User createdBy, DateTime createdDate)
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

			private void InitialiseCreateAudit(TEntity entity)
			{
				if (!_hasAudit) return;
				entity.InitialiseCreateAudit(_createdByUser, _createdDate);
			}

			protected void SetBaseProperties(TEntity entity)
			{
				AddTags(entity);
				InitialiseCreateAudit(entity);
			}
			
			public abstract TEntity Build();
		}
	}
}