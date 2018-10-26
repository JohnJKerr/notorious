using System;
using System.Linq;
using Xunit;

namespace Domain.Tests
{
	public abstract class EntityTests<TEntity, TBuilder>
		where TEntity : Entity
		where TBuilder : Entity.Builder<TEntity>
	{
		protected abstract TBuilder CreateBuilder();

		[Fact]
		public void Builder_Build_sets_Id()
		{
			// arrange
			var builder = CreateBuilder();

			// act
			var entity = builder.Build();

			// assert
			Assert.NotEqual(Guid.Empty, entity.Id);
		}

		[Fact]
		public void WithTags_before_Build_sets_Tags()
		{
			// arrange
			var builder = CreateBuilder();
			var tags = GenerateTags();

			// act
			var output = builder
				.WithTags(tags)
				.Build();

			// assert
			Assert.Equal(tags, output.Tags);
		}

		[Fact]
		public void AddTag_adds_Tag_to_Tags()
		{
			// arrange
			var output = CreateBuilder()
				.Build();
			var tag = GenerateTag();

			// act
			output.AddTag(tag);

			// assert
			Assert.Contains(tag, output.Tags);
		}

		[Fact]
		public void AddTag_throws_ArgumentException_if_Tags_already_contains_Tag()
		{
			// arrange
			var tag = GenerateTag();
			var output = CreateBuilder()
				.WithTags(tag)
				.Build();

			// act/assert
			Assert.Throws<ArgumentException>(() => output.AddTag(tag));
		}

		[Fact]
		public void RemoveTag_removes_Tag_from_Tags()
		{
			// arrange
			var output = CreateBuilder()
				.WithTags(GenerateTags())
				.Build();
			var tagToRemove = output.Tags.First();

			// act
			output.RemoveTag(tagToRemove);

			// assert
			Assert.DoesNotContain(tagToRemove, output.Tags);
		}

		[Fact]
		public void RemoveTag_throws_ArgumentException_when_Tags_does_not_contain_Tag()
		{
			// arrange
			var entity = CreateBuilder().Build();
			var tag = GenerateTag();

			// act/assert
			Assert.Throws<ArgumentException>(() => entity.RemoveTag(tag));
		}

		private static Tag GenerateTag()
		{
			return new Tag.Builder(Guid.NewGuid().ToString())
				.Build();
		}

		private static Tag[] GenerateTags(int numberToGenerate = 5)
		{
			return Enumerable.Range(0, numberToGenerate)
				.Select(_ => GenerateTag())
				.ToArray();
		}

		private static User GenerateUser()
		{
			return new User.Builder()
				.Build();
		}
	}
}