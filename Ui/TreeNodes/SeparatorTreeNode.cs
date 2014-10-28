using System.Windows.Forms;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for separators.
    /// </summary>
    public class SeparatorTreeNode: TreeNode {
        public SeparatorTreeNode(string text)
            :base( text )
        {
            this.ImageIndex = 4;
            this.SelectedImageIndex = 4;
        }
    }
}

