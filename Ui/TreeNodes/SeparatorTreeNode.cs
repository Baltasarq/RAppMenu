using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for separators.
    /// </summary>
    public class SeparatorTreeNode: MenuComponentTreeNode {
        public SeparatorTreeNode(string text, Separator s)
            :base( text, s )
        {
            this.ImageIndex = 4;
            this.SelectedImageIndex = 4;
        }
    }
}

