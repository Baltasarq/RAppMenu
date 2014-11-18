using System;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
	/// <summary>
	/// Image menu entry in the UI side.
	/// </summary>
	public class ImageMenuEntryTreeNode: MenuComponentTreeNode {
		public ImageMenuEntryTreeNode(string text, ImagesMenu parent)
			:base( text, new ImageMenuEntry( text, parent) )
		{
			this.ImageIndex = this.SelectedImageIndex =
				UserAction.LookUp( "addfunction" ).ImageIndex;
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.ImageMenuEntryGuiEditor( pnl, this, this.MenuComponent );
		}
	}
}

