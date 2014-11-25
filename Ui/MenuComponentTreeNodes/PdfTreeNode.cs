using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for PDF path terminals.
    /// </summary>
    public class PdfTreeNode: MenuComponentTreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNodes.PdfTreeNode"/> class.
		/// The corresponding <see cref="MenuComponent" is created under parent./>
		/// </summary>
		/// <param name="text">The caption to be included in the tree node, as string.</param>
		/// <param name="parent">The <see cref="MenuComponent"/> which is parent of the one to be created.</param>
		public PdfTreeNode(string text, Core.MenuComponents.Menu parent)
            :base( text, new PdfFile( text, parent ) )
        {
            this.Init();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNodes.PdfTreeNode"/> class.
		/// Nothing is created. This is useful when building a view while loading.
		/// </summary>
		/// <param name="pdf">The <see cref="PdfFile"/>.</param>
		public PdfTreeNode(PdfFile pdf)
			:base( pdf )
		{
			this.Init();
		}

		private void Init()
		{
			this.ImageIndex =
				this.SelectedImageIndex = UserAction.LookUp( "addpdffilepath" ).ImageIndex;
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.PdfFileGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}

