using Contracts.Filters;
using Domain;
using Repositories;

namespace Repository.Tests
{
	public class NoteRepositoryTests : EntityRepositoryTests<NoteRepository, Note, Filter>
	{
		protected override NoteRepository CreateRepository() => new NoteRepository(Client);
		protected override Filter CreateFilter() => new Filter();

		protected override Note CreateEntity()
		{
			return new Note.Builder()
				.Build();
		}
	}
}