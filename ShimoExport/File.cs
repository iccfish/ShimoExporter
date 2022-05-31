namespace ShimoExport
{
	using System;
	using System.Collections.Generic;

	class File
	{
		public string Guid { get; set; }

		public string Type { get; set; }
		public string SubType { get; set; }
		public string Name { get; set; }
		public List<File> SubItems { get; set; }
		public DateTime UpdatedAt { get; set; }
		public bool IsShortcut { get; set; }

		public ShortcutSource ShortcutSource { get; set; }
	}

	class Space
	{
		public string Guid { get; set; }
		public string Name { get; set; }
		public long UpdatedAt { get; set; }
	}

	class ShortcutSource
	{
		public string Guid { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }

		// 1 ... 1 文件夹
		// 2 ... -4 excel
		// 2 ... -2 doc
		public int Type { get; set; }
		public ItemType SubType { get; set; }

		public (string type, string subtype) GetOriginalType()
		{
			var val = (int)SubType;
			if (val > -999 && val < 3) return (SubType.ToString().ToLower(), null);

			if (val == 3) return ("img", "jpg");

			return (SubType.ToString().ToLower(), SubType.ToString().ToLower());
		}
	}

}
