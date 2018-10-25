using Contracts.Filters;
using Domain;
using Storage;

namespace Repositories
{
	public class UserRepository : EntityRepository<User, Filter>
	{
		public UserRepository(MongoStorageClient client) : base(client)
		{
		}
	}
}