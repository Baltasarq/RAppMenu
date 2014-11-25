using System;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Ui;
using RAppMenu.Core.MenuComponents;
using RAppMenu.Ui.MenuComponentGuiEditors;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for a menu entry.
    /// </summary>
    public class MenuTreeNode: MenuComponentTreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.TreeNodes.MenuEntryTreeNode"/> class.
		/// The corresponding <see cref="MenuComponent"/> is created.
		/// </summary>
		/// <param name="text">The entry's text.</param>
		/// <param name="parent">The menu component parent.</param>
        public MenuTreeNode(string text, Core.MenuComponents.Menu parent)
			:base( text, new Core.MenuComponents.Menu( text, parent ) )
        {
            this.Init();
        }

		public MenuTreeNode(Core.MenuComponents.Menu menu)
			:base( menu )
		{
			this.Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNodes.MenuTreeNode"/> class.
		/// This is used in order to created the top root menu.
		/// </summary>
		/// <param name="rme">Rme.</param>
		protected MenuTreeNode(RootMenu rme)
			:base( rme.Name, rme )
		{
            this.Init();
		}

        private void Init()
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addmenu" ).ImageIndex;
        }

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new MenuGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}
