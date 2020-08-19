using System;

namespace MediasiteUtil.Models
{
	public class Recorder
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string SerialNumber { get; set; }
		public string Version { get; set; }
		public string WebServiceUrl { get; set; }
		public string EncryptionKey { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string LicenseStatus { get; set; }
	}
}