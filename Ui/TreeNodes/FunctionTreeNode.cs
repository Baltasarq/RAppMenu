using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for a function.
    /// </summary>
    public class FunctionTreeNode: MenuComponentTreeNode {
        public FunctionTreeNode(string text, Function f)
            :base( text, f )
        {
            this.ImageIndex = 1;
            this.SelectedImageIndex = 1;
        }
    }
}

