using System.IO;
using System.Text;
using System.Drawing;
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
				if ( mc is Separator ) {
					mUser.DropDownItems.Add( new ToolStripSeparator() );
				}
				else {
					var pmc = mc as PdfFile;
					string fileName = Path.Combine( AppInfo.PdfFolder, mc.Name );

					// Pdf
					if ( pmc != null
					  && !File.Exists( fileName ) )
					{
						errors.AppendFormat( "Missing PDF file: '{0}' in '{1}' at '{2}'",
						                    fileName, pmc.Name, pmc.GetPathAsString() );
						errors.AppendLine();
					}

					// Function
					mUser.DropDownItems.Add( new ToolStripMenuItem( mc.Name ) );
				}
			}

			return;
		}

        private void BuildUserGraphicSubMenu(ToolStripMenuItem subMenu, GraphicMenu graphicMenu)
        {
            IList<MenuComponent> menuComponents = graphicMenu.MenuComponents;
            var items = new List<GraphMenuUtils.GraphicsMenuTable.GraphMenuItemData>();

            // Build the list of images
            foreach (GraphicEntry submc in menuComponents)
            {
				string fileName = Path.Combine( AppInfo.GraphsFolder, submc.ImagePath );

				if ( !File.Exists( fileName ) ) {
					errors.AppendFormat( "Missing graphic file: '{0}' in '{1}' at '{2}'",
					                    fileName, submc.Name, submc.GetPathAsString() );
					errors.AppendLine();
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
            var grphMenu = GraphMenuUtils.GraphicsMenuTable.AddGraphMenuTable( subMenu, items );
			grphMenu.SizeMode = GraphMenuUtils.GraphicsMenuTable.SizeModeStyle.ZoomImage;

            // Set properties, if needed
            grphMenu.ItemHeight = graphicMenu.ImageHeight;
            grphMenu.ItemWidth = graphicMenu.ImageWidth;
            grphMenu.NumColumns = graphicMenu.MinimumNumberOfColumns;

            return;
        }

        private void BuildMainMenu()
        {
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

