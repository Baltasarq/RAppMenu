using System;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for graphical menuterminals.
    /// </summary>
    public class GraphicMenuTreeNode: MenuComponentTreeNode {
		public GraphicMenuTreeNode(string text, Core.MenuComponents.Menu parent)
            :base( text, new ImagesMenu( text, parent ) )
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addgraphicmenu" ).ImageIndex;
        }

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl, MenuComponent mc)
		{
			throw new NotImplementedException();
		}
    }
}

