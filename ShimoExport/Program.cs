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

			Application.Run(new MainForm());
		}
	}
}
