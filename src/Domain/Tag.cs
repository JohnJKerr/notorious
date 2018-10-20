namespace Domain
{
	public class Tag : Entity
	{
		private Tag(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public class Builder : Entity.Builder<Tag>
		{
			private readonly string _name;
			
			public Builder(string name)
			{
				_name = name;
			}
			
			public override Tag Build()
			{
				var tag = new Tag(_name);
				tag.AddTags(Tags);
				return tag;
			}
		}
	}
}