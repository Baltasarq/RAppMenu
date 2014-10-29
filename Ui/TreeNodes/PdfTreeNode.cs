using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for PDF path terminals.
    /// </summary>
    public class PdfTreeNode: MenuComponentTreeNode {
        public PdfTreeNode(string text, PdfFile pdf)
            :base( text, pdf )
        {
            this.ImageIndex = 2;
            this.SelectedImageIndex = 2;
        }
    }
}

