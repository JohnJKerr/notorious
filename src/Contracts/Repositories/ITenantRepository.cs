using System;

namespace Contracts.Repositories
{
	public interface ITenantRepository
	{
		void SetTenancy(Guid userId);
	}
}