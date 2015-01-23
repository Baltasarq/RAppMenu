using System;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
	/// <summary>
	/// Image menu entry in the UI side.
	/// </summary>
	public class GraphicEntryTreeNode: MenuComponentTreeNode {
        public GraphicEntryTreeNode(GraphicEntry gme)
			:base( gme )
		{
			this.ImageIndex = this.SelectedImageIndex =
				UserAction.LookUp( "addfunction" ).ImageIndex;
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.GraphicEntryGuiEditor( pnl, this, this.MenuComponent );
		}
	}
}

