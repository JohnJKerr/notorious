using System;
using Xunit;

namespace Domain.Tests
{
	public class TagTests : EntityTests<Tag, Tag.Builder>
	{
		protected override Tag.Builder CreateBuilder()
		{
			return new Tag.Builder(Guid.NewGuid().ToString());
		}

		[Fact]
		public void Build_sets_Name()
		{
			// arrange
			var name = Guid.NewGuid().ToString();
			var builder = new Tag.Builder(name);
			
			// act
			var output = builder.Build();
			
			// assert
			Assert.Equal(name, output.Name);
		}
	}
}