using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for separators.
    /// </summary>
    public class SeparatorTreeNode: MenuComponentTreeNode {
        public SeparatorTreeNode(string text, MenuEntry parent)
            :base( text, new Separator( parent ) )
        {
            this.ImageIndex = 4;
            this.SelectedImageIndex = 4;
        }
    }
}

