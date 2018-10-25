using Contracts.Filters;
using Domain;
using Storage;

namespace Repositories
{
	public class TagRepository : EntityRepository<Tag, Filter>
	{
		public TagRepository(MongoStorageClient client) : base(client)
		{
		}
	}
}