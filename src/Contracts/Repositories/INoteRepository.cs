using Contracts.Filters;
using Domain;

namespace Contracts.Repositories
{
	public interface INoteRepository : IEntityRepository<Note, Filter>
	{
	}
}