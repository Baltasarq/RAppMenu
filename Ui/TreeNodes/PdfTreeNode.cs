using System.Windows.Forms;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for PDF path terminals.
    /// </summary>
    public class PdfTreeNode: TreeNode {
        public PdfTreeNode(string text)
            :base( text )
        {
            this.ImageIndex = 2;
            this.SelectedImageIndex = 2;
        }
    }
}

