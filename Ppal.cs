using System;
using System.Diagnostics;
using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Ui;

namespace RWABuilder {
	public class Ppal {
		[STAThread]
		public static void Main()
		{
			Form mainForm = null;

			try {
				AppInfo.BuildLog();
				AppInfo.GetMainAppInfo();

				mainForm = new MainWindow();
				Application.EnableVisualStyles();
				Application.Run( mainForm );
			}
			catch(Exception exc) {
                if ( mainForm != null ) {
                    mainForm.Close();
                }

                MessageBox.Show( null, exc.Message, AppInfo.Name );
                Trace.WriteLine( "[CRITICAL] " + DateTime.Now + ": " + exc.Message );
                Trace.WriteLine( exc.StackTrace );
			}
            finally {
                AppInfo.CloseLog();
            }
		}
	}
}
