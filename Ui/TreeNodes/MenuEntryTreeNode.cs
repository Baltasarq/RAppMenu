using System.Windows.Forms;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for a menu entry.
    /// </summary>
    public class MenuEntryTreeNode: TreeNode {
        public MenuEntryTreeNode(string text)
            :base( text )
        {
            this.ImageIndex = 0;
            this.SelectedImageIndex = 0;
        }
    }
}

