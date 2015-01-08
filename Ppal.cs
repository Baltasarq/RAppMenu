using System;
using System.Diagnostics;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Ui;

namespace RAppMenu {
	public class Ppal {
		[STAThread]
		public static void Main()
		{
			Form mainForm = null;

			try {
                AppInfo.BuildLog();

				mainForm = new MainWindow();
             //   mainForm = new Ui.MenuComponentGuiEditors.FunctionGuiEditors.CsvEditor();
             //   mainForm = new Ui.MenuComponentGuiEditors.FunctionGuiEditors.FunctionCallsGuiEditor(
             //       new RAppMenu.Core.MenuComponents.Function( "f", new Core.MenuComponents.RootMenu( new MenuDesign() ) ) );
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
