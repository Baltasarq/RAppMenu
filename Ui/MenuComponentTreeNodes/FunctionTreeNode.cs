using System;
using System.Windows.Forms;

using RAppMenu.Core;
using CoreComponents = RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for a function.
    /// </summary>
    public class FunctionTreeNode: MenuComponentTreeNode {
        public FunctionTreeNode(string text, CoreComponents.Menu parent)
            :base( text, new CoreComponents.Function( text, parent) )
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addfunction" ).ImageIndex;
        }

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl, MenuComponent mc)
		{
			throw new System.NotImplementedException();
			// return new FunctionGuiEditor( pnl, this, mc );
		}
    }
}

