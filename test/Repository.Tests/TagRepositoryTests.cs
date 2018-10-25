using System;
using Contracts.Filters;
using Domain;
using Repositories;

namespace Repository.Tests
{
	public class TagRepositoryTests : EntityRepositoryTests<TagRepository, Tag, Filter>
	{
		protected override TagRepository CreateRepository() => new TagRepository(Client);
		protected override Filter CreateFilter() => new Filter();

		protected override Tag CreateEntity()
		{
			return new Tag.Builder(Guid.NewGuid().ToString())
				.Build();
		}
	}
}