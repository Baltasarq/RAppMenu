using System.Windows.Forms;

using RAppMenu.Core;

namespace RAppMenu.Ui
{
	public class MenuComponentTreeNode: TreeNode
	{
		public MenuComponentTreeNode(string text, MenuComponent mc)
			:base( text )
		{
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

		private MenuComponent menuComponent;
	}
}

