using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for a menu entry.
    /// </summary>
    public class MenuTreeNode: MenuComponentTreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.TreeNodes.MenuEntryTreeNode"/> class.
		/// </summary>
		/// <param name="text">The entry's text.</param>
		/// <param name="parent">The menu component parent.</param>
        public MenuTreeNode(string text, Core.MenuComponents.Menu parent)
			:base( text, new Core.MenuComponents.Menu( text, parent ) )
        {
            this.Init();
        }

		protected MenuTreeNode(RootMenu rme)
			:base( rme.Name, rme )
		{
            this.Init();
		}

        private void Init()
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addmenu" ).ImageIndex;
        }
    }
}
