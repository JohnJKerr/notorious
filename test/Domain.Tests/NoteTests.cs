using System;
using System.Security.Cryptography;
using Xunit;

namespace Domain.Tests
{
	public class NoteTests : EntityTests<Note, Note.Builder>
	{
		protected override Note.Builder CreateBuilder()
		{
			return new Note.Builder();
		}

		[Fact]
		public void WithContent_before_Build_sets_Content()
		{
			// arrange
			var builder = CreateBuilder();
			var content = Guid.NewGuid().ToString();
			
			// act
			var output = builder
				.WithContent(content)
				.Build();
			
			// assert
			Assert.Equal(content, output.Content);
		}

		[Fact]
		public void SetContent_sets_Content()
		{
			// arrange
			var entity = CreateBuilder().Build();
			var content = Guid.NewGuid().ToString();
			
			// act
			entity.SetContent(content);
			
			// assert
			Assert.Equal(content, entity.Content);
		}
	}
}