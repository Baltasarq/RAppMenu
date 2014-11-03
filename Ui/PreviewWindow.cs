using System.Drawing;
using System.Windows.Forms;

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
            foreach(MenuComponent mc in this.Document.Root.MenuComponents) {
                var menuOption = new ToolStripMenuItem( mc.Name );

                if ( mc is Separator ) {
                    mUser.DropDownItems.Add( new ToolStripSeparator() );
                }
                else {
                    mUser.DropDownItems.Add( menuOption );
                }
            }
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

