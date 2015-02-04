using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for PDF path terminals.
    /// </summary>
    public class PdfFileTreeNode: MenuComponentTreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Ui.MenuComponentTreeNodes.PdfTreeNode"/> class.
		/// </summary>
		/// <param name="pdf">The <see cref="PdfFile"/>.</param>
		public PdfFileTreeNode(PdfFile pdf)
			:base( pdf )
		{
            this.ImageIndex =
                this.SelectedImageIndex =
                    UserAction.LookUp( "addpdffilepath" ).ImageIndex;
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.PdfFileGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}

