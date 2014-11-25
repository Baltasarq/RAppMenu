using System;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for graphical menuterminals.
    /// </summary>
    public class GraphicMenuTreeNode: MenuComponentTreeNode {
        public GraphicMenuTreeNode(GraphicMenu gm)
            :base( gm )
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addgraphicmenu" ).ImageIndex;
        }

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.GraphicMenuGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}

