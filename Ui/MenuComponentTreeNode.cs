using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui
{
	public abstract class MenuComponentTreeNode: TreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNode"/> class.
		/// </summary>
		/// <param name="mc">The <see cref="MenuComponent"/> being represented.</param>
		public MenuComponentTreeNode(MenuComponent mc)
			:base( mc.Name )
		{
			this.editor = null;
			this.menuComponent = mc;
		}

		/// <summary>
		/// Gets the menu component associated to this tree node.
		/// </summary>
		/// <value>
		/// The menu component, as a <see cref="MenuComponent"/> object.
		/// </value>
		public MenuComponent MenuComponent {
			get {
				return this.menuComponent;
			}
		}

		/// <summary>
		/// Gets the editor related to this menu component.
		/// </summary>
		/// <value>The editor.</value>
		public MenuComponentGuiEditor GetEditor(Panel panel)
		{
			if ( this.editor == null ) {
				this.editor = this.CreateEditor( panel );
			}

			return this.editor;
		}

		/// <summary>
		/// Creates the corresponding editor for this tree node.
		/// </summary>
		/// <returns>The editor, as a <see cref="MenuComponentGuiEditor"/>.</returns>
		/// <param name="pnl">The <see cref="Panel"/> in which the editor will be created.</param>
		/// <param name="mc">The <see cref="MenuComponent"/> object to edit.</param>
		protected abstract MenuComponentGuiEditor CreateEditor(Panel pnl);

		private MenuComponentGuiEditor editor;
		private MenuComponent menuComponent;
	}
}

