namespace ShimoExport
{
	using System;
	using System.Collections.Generic;

	class File
	{
		public DateTime CreatedAt { get; set; }
		public string Guid { get; set; }
		public int Id { get; set; }
		public string InviteCode { get; set; }
		public bool IsFolder { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public User User { get; set; }
		public List<File> SubItems { get; set; }
		public DateTime UpdatedAt { get; set; }
		public bool IsShortcut { get; set; }
		public string ContentUrl { get; set; }
	}
}
