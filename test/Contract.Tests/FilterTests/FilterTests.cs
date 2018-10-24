using Contracts.Filters;
using Xunit;

namespace Contract.Tests.FilterTests
{
	public class FilterTests
	{
		[Fact]
		public void Constructor_defaults_Take_to_10()
		{
			// act
			var filter = new Filter();
			
			// assert
			Assert.Equal(10, filter.Take);
		}

		[Fact]
		public void Constructor_defaults_Skip_to_0()
		{
			// act
			var filter = new Filter();
			
			// assert
			Assert.Equal(0, filter.Skip);
		}
	}
}