using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui
{
	public abstract class MenuComponentTreeNode: TreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Ui.MenuComponentTreeNode"/> class.
		/// </summary>
		/// <param name="mc">The <see cref="MenuComponent"/> being represented.</param>
		public MenuComponentTreeNode(MenuComponent mc)
			: base( mc.Name )
		{
			this.editor = null;
			this.menuComponent = mc;
		}

		/// <summary>
		/// Creates the correct menu component tree node for the given menu component.
		/// </summary>
		/// <param name="mc">A <see cref="Core.MenuComponent"/> object.</param>
		public static MenuComponentTreeNode Create(MenuComponent mc) {
			MenuComponentTreeNode toret = null;
			var function = mc as Function;
			var separator = mc as Separator;
			var pdfFile = mc as PdfFile;
			var subMenu = mc as RegularMenu;
			var grphMenu = mc as GraphicMenu;
			var grphEntry = mc as GraphicEntry;

			if ( separator != null ) {
				toret = new MenuComponentTreeNodes.SeparatorTreeNode( separator );
			}
			else
			if ( pdfFile != null ) {
				toret = new MenuComponentTreeNodes.PdfFileTreeNode( pdfFile );
			}
			else
			if ( function != null ) {
				toret = new MenuComponentTreeNodes.FunctionTreeNode( function );
			}
			else
			if ( grphMenu != null ) {
				toret = new MenuComponentTreeNodes.GraphicMenuTreeNode( grphMenu );
			}
			else
			if ( grphEntry != null ) {
				toret = new MenuComponentTreeNodes.GraphicEntryTreeNode( grphEntry );
			}
			else
			if ( subMenu != null ) {
				toret = new MenuComponentTreeNodes.MenuTreeNode( subMenu );
			}

			return toret;
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

