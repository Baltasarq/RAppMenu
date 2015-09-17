using System;
using System.Windows.Forms;

using RWABuilder.Core;
using CoreComponents = RWABuilder.Core.MenuComponents;
using UiEditors = RWABuilder.Ui.MenuComponentGuiEditors;

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

			// The caption in the treenode is not directly the name
			this.Text = UiEditors.FunctionGuiEditor.BuildCaptionCombination(
				(CoreComponents.Function) this.MenuComponent, "", "" );
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new UiEditors.FunctionGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}

