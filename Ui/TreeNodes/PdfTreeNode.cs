using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for PDF path terminals.
    /// </summary>
    public class PdfTreeNode: MenuComponentTreeNode {
		public PdfTreeNode(string text, Core.MenuComponents.Menu parent)
            :base( text, new PdfFile( text, parent ) )
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addpdffilepath" ).ImageIndex;
        }
    }
}

