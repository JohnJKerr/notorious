using System;

namespace Domain
{
	public class UpdateAudit
	{
		internal UpdateAudit(User user, DateTime date)
		{
			User = user;
			Date = date;
		}
		
		public User User { get; private set; }
		public DateTime Date { get; private set; }
	}
}