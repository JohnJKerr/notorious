namespace Contracts.Filters
{
	public class Filter
	{
		public Filter()
		{
			Take = 10;
		}
		
		public int Skip { get; set; }
		public int Take { get; set; }
	}
}