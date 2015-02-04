using System;
using System.Windows.Forms;

using RWABuilder.Core;
using CoreComponents = RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for a function.
    /// </summary>
    public class FunctionTreeNode: MenuComponentTreeNode {		
		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Ui.MenuComponentTreeNodes.FunctionTreeNode"/> class.
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

