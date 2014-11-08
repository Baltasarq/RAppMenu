using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui {
    /// <summary>
    /// This is the preview window.
    /// This small window just shows the menu the user is building.
    /// </summary>
    public class PreviewWindow: Form {
        /// <summary>
        /// Initializes a new instance of the <see cref="RAppMenu.Ui.PreviewWindow"/> class.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/>object, containing the menu.</param>
        /// <param name="icon">The icon for the window.</param>
        public PreviewWindow(DesignOfUserMenu doc, Icon icon)
        {
            this.document = doc;
            this.Build( icon );
        }

        private void BuildUserMenu(ToolStripMenuItem mUser)
        {
			this.BuildUserSubMenu( mUser, this.Document.Root );
        }

		private void BuildUserSubMenu(ToolStripMenuItem mUser, Core.MenuComponents.Menu menuComponent)
		{
			foreach(MenuComponent mc in menuComponent.MenuComponents) {
                var graphicMenu = mc as ImagesMenu;
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
					mUser.DropDownItems.Add( new ToolStripMenuItem( mc.Name ) );
				}
			}

			return;
		}

        private void BuildUserGraphicSubMenu(ToolStripMenuItem subMenu, ImagesMenu graphicMenu)
        {
            IList<MenuComponent> menuComponents = graphicMenu.MenuComponents;
            var items = new List<GraphMenuUtils.GraphicsMenuTable.GraphMenuItemData>();

            // Build the list of images
            foreach (ImageMenuEntry submc in menuComponents)
            {
                items.Add(
                    new GraphMenuUtils.GraphicsMenuTable.GraphMenuItemData(
                        submc.ImagePath,
                        submc.ImageToolTip,
                        submc.Function )
                );
            }

            // Build the menu
            GraphMenuUtils.GraphicsMenuTable.AddGraphMenuTable( subMenu, items );
        }

        private void BuildMenu()
        {
            // File menu
            var opQuit = new ToolStripMenuItem( "&Quit" );
            opQuit.ShortcutKeys = Keys.Control | Keys.Q;
            opQuit.Click += (sender, e) => this.Close();
            opQuit.Image =
                UserAction.ImageList.Images[ UserAction.LookUp( "quit" ).ImageIndex ];

            var mFile = new ToolStripMenuItem( "&File" );
            mFile.DropDownItems.Add( opQuit );

            // Build user's menu
            var mUser = new ToolStripMenuItem( this.Document.Root.Name );
            this.BuildUserMenu( mUser );

            // Main menu
            var mMain = new MenuStrip();
            mMain.Items.AddRange( new ToolStripItem[] {
                mFile, mUser
            });
            mMain.Dock = DockStyle.Top;
            this.Controls.Add( mMain );
            this.MainMenuStrip = mMain;
        }

        private void Build(Icon icon)
        {
            this.BuildMenu();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MinimizeBox = this.MaximizeBox = false;
            this.Icon = icon;
            this.Text = AppInfo.Name + " preview";
            this.MinimumSize = new Size( 320, 240 );
        }

        /// <summary>
        /// Gets the document this preview window is showing.
        /// </summary>
        /// <value>The <see cref="Document"/> object.</value>
        public DesignOfUserMenu Document {
            get {
                return this.document;
            }
        }

        private DesignOfUserMenu document;
    }
}

