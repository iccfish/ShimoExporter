using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShimoExport
{
	using System.Windows.Forms;

	class Program
	{
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			try
			{
				SetProcessDPIAware();
			}
			catch (Exception)
			{
			}

			Application.Run(new MainForm());
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern bool SetProcessDPIAware();
	}
}
