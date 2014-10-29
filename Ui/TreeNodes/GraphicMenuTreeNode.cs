using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for graphical menuterminals.
    /// </summary>
    public class GraphicMenuTreeNode: MenuComponentTreeNode {
        public GraphicMenuTreeNode(string text, ImagesMenu im)
            :base( text, im )
        {
            this.ImageIndex = 3;
            this.SelectedImageIndex = 3;
        }
    }
}

