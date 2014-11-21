using System;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
	/// <summary>
	/// Image menu entry in the UI side.
	/// </summary>
	public class ImageMenuEntryTreeNode: MenuComponentTreeNode {
		public ImageMenuEntryTreeNode(string text, GraphicMenu parent)
			:base( text, new GraphicMenuEntry( text, parent) )
		{
			this.ImageIndex = this.SelectedImageIndex =
				UserAction.LookUp( "addfunction" ).ImageIndex;
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.GraphicMenuEntryGuiEditor( pnl, this, this.MenuComponent );
		}
	}
}

