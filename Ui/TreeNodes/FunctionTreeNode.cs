using System.Windows.Forms;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for a function.
    /// </summary>
    public class FunctionTreeNode: TreeNode {
        public FunctionTreeNode(string text)
            :base( text )
        {
            this.ImageIndex = 1;
            this.SelectedImageIndex = 1;
        }
    }
}

