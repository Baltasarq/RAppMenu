using System;
using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui.MenuComponentTreeNodes {
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

