using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.TreeNodes {
    /// <summary>
    /// Tree node for separators.
    /// </summary>
    public class SeparatorTreeNode: MenuComponentTreeNode {
		public SeparatorTreeNode(Core.MenuComponents.Menu parent)
            :base( Separator.TagName, new Separator( parent ) )
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addseparator" ).ImageIndex;
        }
    }
}

