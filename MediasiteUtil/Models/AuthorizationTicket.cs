using System;

namespace MediasiteUtil.Models
{
	public class AuthorizationTicket
	{
		public string TicketId { get; set; }
		public string Username { get; set; }
		public string ClientIpAddress { get; set; }
		public string Owner { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime ExpirationTime { get; set; }
		public string ResourceId { get; set; }
		public int MinutesToLive { get; set; }
	}
}