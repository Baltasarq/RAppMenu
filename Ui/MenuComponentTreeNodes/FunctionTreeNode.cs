using System;
using System.Windows.Forms;

using RAppMenu.Core;
using CoreComponents = RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for a function.
    /// </summary>
    public class FunctionTreeNode: MenuComponentTreeNode {		
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNodes.FunctionTreeNode"/> class.
		/// </summary>
        /// <param name="f">The <see cref="Function"/></param>
		public FunctionTreeNode(CoreComponents.Function f)
			:base( f )
		{
            this.ImageIndex =
                this.SelectedImageIndex =
                    UserAction.LookUp( "addfunction" ).ImageIndex;
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.FunctionGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}

