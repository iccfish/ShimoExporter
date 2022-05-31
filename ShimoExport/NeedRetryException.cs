namespace ShimoExport
{
	using System;

	class NeedRetryException : Exception
	{
		public int Timeout { get; set; }

		public NeedRetryException(int timeout) : base("操作过于频繁，需要等待")
		{
			Timeout = timeout;
		}
	}
}
