using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
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
	}
}

