using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for a menu entry.
    /// </summary>
    public class MenuEntryTreeNode: MenuComponentTreeNode {
        public MenuEntryTreeNode(string text, MenuEntry me)
            :base( text, me )
        {
            this.ImageIndex = 0;
            this.SelectedImageIndex = 0;
        }
    }
}
