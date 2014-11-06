using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
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
    }
}

