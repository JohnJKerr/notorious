using System;

namespace Contracts.Providers
{
	public interface IDateTimeProvider
	{
		DateTime Now { get; }
	}
}