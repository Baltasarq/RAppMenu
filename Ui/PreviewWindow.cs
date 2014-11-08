using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui {
    public class PreviewWindow: Form {
        public PreviewWindow(Document doc, Icon icon)
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
                    IList<MenuComponent> menuComponents = graphicMenu.MenuComponents;
                    var items = new List<GraphMenuUtils.GraphicsMenuTable.GraphMenuItemData>();
                    var subMenu = (ToolStripMenuItem) mUser.DropDownItems.Add( mc.Name );

                    foreach(ImageMenuEntry submc in menuComponents) {
                        items.Add(
                            new GraphMenuUtils.GraphicsMenuTable.GraphMenuItemData(
                                submc.ImagePath,
                                submc.ImageToolTip,
                                submc.Function )
                        );
                    }

                    GraphMenuUtils.GraphicsMenuTable.AddGraphMenuTable( subMenu, items );
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

            // Built menu
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
        public Document Document {
            get {
                return this.document;
            }
        }

        private Document document;
    }
}

