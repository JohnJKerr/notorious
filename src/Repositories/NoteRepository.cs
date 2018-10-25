using Contracts.Filters;
using Domain;
using Storage;

namespace Repositories
{
	public class NoteRepository : EntityRepository<Note, Filter>
	{
		public NoteRepository(MongoStorageClient client) : base(client)
		{
		}
	}
}