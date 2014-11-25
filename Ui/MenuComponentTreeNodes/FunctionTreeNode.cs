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
		/// Creates the corresponding MenuComponent.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="parent">Parent.</param>
        public FunctionTreeNode(string text, CoreComponents.Menu parent)
            :base( text, new CoreComponents.Function( text, parent) )
        {
            this.Init();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNodes.FunctionTreeNode"/> class.
		/// Does not created anything. Usefult when loading a menu.
		/// </summary>
		/// <param name="f">F.</param>
		public FunctionTreeNode(CoreComponents.Function f)
			:base( f )
		{
			this.Init();
		}

		private void Init()
		{
			this.ImageIndex = this.SelectedImageIndex = UserAction.LookUp( "addfunction" ).ImageIndex;
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.FunctionGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}

