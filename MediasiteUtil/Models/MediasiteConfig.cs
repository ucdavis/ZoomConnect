namespace MediasiteUtil.Models
{
	/// <summary>
	/// Summary description for MediasiteConfig
	/// </summary>
	public class MediasiteConfig
	{
		public MediasiteConfig()
		{
		}

		public string Endpoint { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string ApiKey { get; set; }
		public string RootFolderId { get; set; }
		public string PlayerId { get; set; }
		public string RecorderUsername { get; set; }
		public string RecorderPassword { get; set; }
	}
}