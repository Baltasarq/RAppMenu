using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for a function.
    /// </summary>
    public class FunctionTreeNode: MenuComponentTreeNode {
        public FunctionTreeNode(string text, MenuEntry parent)
            :base( text, new Function( text, parent) )
        {
            this.ImageIndex = this.SelectedImageIndex = 6;
        }
    }
}

