using System;
using Providers;
using Xunit;

namespace Provider.Tests
{
	public class DateTimeProviderTests
	{
		private readonly DateTimeProvider _provider;

		public DateTimeProviderTests()
		{
			_provider = new DateTimeProvider();
		}

		[Fact]
		public void Now_returns_UtcNow_within_10ms()
		{
			// arrange
			var expected = DateTime.UtcNow;
			
			// act
			var now = _provider.Now;
			
			// assert
			var msDifference = (now - expected).TotalMilliseconds;
			Assert.True(msDifference < 10);
		}
	}
}