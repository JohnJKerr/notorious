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

		public void AddTag(Tag tag)
		{
			if(_tags.Contains(tag)) throw new ArgumentException($"Entity already has tag with ID: {tag.Id}");
			_tags.Add(tag);
		}

		protected void AddTags(params Tag[] tags)
		{
			tags
				.ToList()
				.ForEach(AddTag);
		}

		public void RemoveTag(Tag tag)
		{
			if(!_tags.Contains(tag)) throw new ArgumentException($"Entity does not have tag with ID: {tag.Id}");
			_tags.Remove(tag);
		}

		public abstract class Builder<TEntity>
			where TEntity : Entity
		{
			private readonly List<Tag> _tags = new List<Tag>();

			protected Tag[] Tags => _tags.ToArray();
			
			public Builder<TEntity> WithTags(params Tag[] tags)
			{
				_tags.AddRange(tags);
				return this;
			}
			
			public abstract TEntity Build();
		}
	}
}