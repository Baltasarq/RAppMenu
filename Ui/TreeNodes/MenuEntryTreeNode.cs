using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for a menu entry.
    /// </summary>
    public class MenuEntryTreeNode: MenuComponentTreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.TreeNodes.MenuEntryTreeNode"/> class.
		/// </summary>
		/// <param name="text">The entry's text.</param>
		/// <param name="parent">The menu component parent.</param>
        public MenuEntryTreeNode(string text, MenuEntry parent)
            :base( text, new MenuEntry( text, parent ) )
        {
            this.Init();
        }

		protected MenuEntryTreeNode(RootMenuEntry rme)
			:base( rme.Name, rme )
		{
            this.Init();
		}

        private void Init()
        {
            this.ImageIndex = this.SelectedImageIndex = 4;
        }
    }
}
