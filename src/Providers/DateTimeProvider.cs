using System;
using Contracts.Providers;

namespace Providers
{
	public class DateTimeProvider : IDateTimeProvider
	{
		public DateTime Now => DateTime.UtcNow;
	}
}