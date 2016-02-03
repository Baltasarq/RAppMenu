using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui {
    /// <summary>
    /// This is the preview window.
    /// This small window just shows the menu the user is building.
    /// </summary>
    public class PreviewWindow: Form {
        /// <summary>
        /// Initializes a new instance of the <see cref="RWABuilder.Ui.PreviewWindow"/> class.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/>object, containing the menu.</param>
        /// <param name="icon">The icon for the window.</param>
        public PreviewWindow(MenuDesign doc, Icon icon)
        {
            this.document = doc;
			this.errors = new StringBuilder();
            this.Build( icon );
        }

        private void BuildUserMenu(ToolStripMenuItem mUser)
        {
			this.BuildUserSubMenu( mUser, this.Document.Root );
        }

		private void BuildUserSubMenu(ToolStripMenuItem mUser, Core.MenuComponents.Menu menuComponent)
		{
			foreach(MenuComponent mc in menuComponent.MenuComponents) {
                var graphicMenu = mc as GraphicMenu;
                var menu = mc as Core.MenuComponents.Menu;
				var pmc = mc as PdfFile;
				var fmc = mc as Function;

				if ( graphicMenu != null ) {
					var subMenu = (ToolStripMenuItem) mUser.DropDownItems.Add( graphicMenu.Name );
					this.BuildUserGraphicSubMenu( subMenu, graphicMenu );
				}
				else
				if ( menu != null ) {
					var subMenu = (ToolStripMenuItem) mUser.DropDownItems.Add( mc.Name );
					this.BuildUserSubMenu( subMenu, menu );
				}
				else
				if ( pmc != null ) {
					string fileName = mc.Name;
					
					// Full path or not?
					if ( Path.GetDirectoryName( fileName ) == string.Empty ) {
						Console.WriteLine( "PDF file PATH to be combined because: " + fileName );								
								fileName = Path.Combine( LocalStorageManager.PdfFolder, mc.Name );
						Console.WriteLine( "PDF file PATH combined: " + fileName );
					}

					// Pdf
					if ( pmc != null
 					  && !File.Exists( fileName ) )
					{
						errors.AppendFormat( "Missing PDF file: '{0}' in '{1}' at '{2}'",
							fileName, pmc.Name, pmc.GetPathAsString() );
						errors.AppendLine();
					}

					mUser.DropDownItems.Add( new ToolStripMenuItem( Path.GetFileName( fileName ) ) );
					Trace.WriteLine( DateTime.Now + ": Added PDF: " + fileName );
				}
				else
				if ( fmc != null ) {
					mUser.DropDownItems.Add( new ToolStripMenuItem( fmc.Caption ) );
					Trace.WriteLine( DateTime.Now + ": Added function: " + fmc.Caption );
				}				
				else
				if ( mc is Separator ) {
					mUser.DropDownItems.Add( new ToolStripSeparator() );
					Trace.WriteLine( DateTime.Now + ": Added separator" );
				}
				else {
					mUser.DropDownItems.Add( new ToolStripMenuItem( mc.Name ) );
					Trace.WriteLine( DateTime.Now + ": Added other: " + mc.Name );
				}
			}

			return;
		}

        private void BuildUserGraphicSubMenu(ToolStripMenuItem subMenu, GraphicMenu graphicMenu)
        {
			Trace.WriteLine( DateTime.Now + ": Building graphic menu..." );

            IList<MenuComponent> menuComponents = graphicMenu.MenuComponents;
            var items = new List<GraphMenuUtils.GraphicsMenuTable.GraphMenuItemData>();

            // Build the list of images
            foreach (GraphicEntry submc in menuComponents)
            {
				string fileName = submc.ImagePath;

				if ( Path.GetDirectoryName( fileName ) == string.Empty ) {
					fileName = Path.Combine( LocalStorageManager.GraphsFolder, submc.ImagePath );
				}

				if ( !File.Exists( fileName ) ) {
					this.errors.AppendFormat( "Missing graphic file: '{0}' in '{1}' at '{2}'",
					                    fileName, submc.Name, submc.GetPathAsString() );
					this.errors.AppendLine();
				} else {
	                items.Add(
	                    new GraphMenuUtils.GraphicsMenuTable.GraphMenuItemData(
	                        fileName,
	                        submc.ImageToolTip,
	                        submc.Function )
	                );
				}
            }

            // Build the menu
			try {
				var grphMenu = GraphMenuUtils.GraphicsMenuTable.AddGraphMenuTable( subMenu, items );
				grphMenu.SizeMode = GraphMenuUtils.GraphicsMenuTable.SizeModeStyle.ZoomImage;

				// Set properties, if needed
				grphMenu.ItemHeight = graphicMenu.ImageHeight;
				grphMenu.ItemWidth = graphicMenu.ImageWidth;
				grphMenu.NumColumns = graphicMenu.MinimumNumberOfColumns;
			} catch(Exception exc) {
				Trace.WriteLine( DateTime.Now + ": ERROR creating graphic menu: " + exc.Message );
			}

            return;
        }

        private void BuildMainMenu()
        {
			Trace.WriteLine( DateTime.Now + ": Building main menu" );

            // File menu
            var opQuit = new ToolStripMenuItem( "&Quit" );
            opQuit.ShortcutKeys = Keys.Control | Keys.Q;
            opQuit.Click += (sender, e) => this.Close();
            opQuit.Image =
                UserAction.ImageList.Images[ UserAction.LookUp( "quit" ).ImageIndex ];

            var mFile = new ToolStripMenuItem( "&File" );
			mFile.DropDownItems.Add( opQuit );

			// Build user's menu root
			var mRoot = new ToolStripMenuItem( "RWizard Applications" );

            // Build user's menu
            var mUser = new ToolStripMenuItem( this.Document.Root.Name );
			mRoot.DropDownItems.Add( mUser );
            this.BuildUserMenu( mUser );

            // Main menu
            var mMain = new MenuStrip();
            mMain.Items.AddRange( new ToolStripItem[] {
                mFile, mRoot
            });
            mMain.Dock = DockStyle.Top;
            this.Controls.Add( mMain );
            this.MainMenuStrip = mMain;
        }

		private void BuildErrorsPanel()
		{
			Trace.WriteLine( DateTime.Now + ": Building errors panel" );

			this.pnlErrors = new GroupBox();
			this.pnlErrors.SuspendLayout();
			this.pnlErrors.Hide();
			this.Controls.Add( this.pnlErrors );
			this.pnlErrors.Dock = DockStyle.Fill;
			this.pnlErrors.Text = "Errors";
			this.pnlErrors.Font = new Font( this.pnlErrors.Font, FontStyle.Bold );

			this.lbErrors = new ListBox();
			this.lbErrors.Font = new Font( this.lbErrors.Font, FontStyle.Regular );
			this.lbErrors.ForeColor = Color.DarkRed;
			this.lbErrors.Dock = DockStyle.Fill;
			this.pnlErrors.Controls.Add( this.lbErrors );
			this.pnlErrors.ResumeLayout( false );
		}

        private void Build(Icon icon)
        {
			Trace.WriteLine( DateTime.Now + ": Begin creating preview menu" );
			Trace.Indent();

			this.BuildErrorsPanel();
			this.BuildMainMenu();

			if ( errors.Length > 0 ) {
				string[] errorList = this.errors.ToString().Split( '\n' );
				this.lbErrors.Items.AddRange( errorList );
				this.pnlErrors.Show();
			}

            this.MinimizeBox = this.MaximizeBox = false;
            this.Icon = icon;
            this.Text = AppInfo.Name + " preview";
            this.MinimumSize = new Size( 600, 400 );

			Trace.Unindent();
			Trace.WriteLine( DateTime.Now + ": Finished creating preview menu" );
        }

        /// <summary>
        /// Gets the document this preview window is showing.
        /// </summary>
        /// <value>The <see cref="Document"/> object.</value>
        public MenuDesign Document {
            get {
                return this.document;
            }
        }

        private MenuDesign document;
		private GroupBox pnlErrors;
		private ListBox lbErrors;
		private StringBuilder errors;
    }
}

