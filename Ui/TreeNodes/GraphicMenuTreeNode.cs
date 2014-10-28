using System.Windows.Forms;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for graphical menuterminals.
    /// </summary>
    public class GraphicMenuTreeNode: TreeNode {
        public GraphicMenuTreeNode(string text)
            :base( text )
        {
            this.ImageIndex = 3;
            this.SelectedImageIndex = 3;
        }
    }
}

