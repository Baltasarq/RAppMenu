using System.Windows.Forms;

using RAppMenu.Ui.MenuComponentGuiEditors;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
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

		/// <summary>
		/// Creates the corresponding (empty) editor for this separator.
		/// </summary>
		/// <returns>The editor, as a <see cref="MenuComponentGuiEditor"/>.</returns>
		/// <param name="pnl">The <see cref="Panel"/> in which the editor will be created.</param>
		/// <param name="mc">The <see cref="MenuComponent"/> object to edit.</param>
		protected override MenuComponentGuiEditor CreateEditor(Panel pnl, MenuComponent mc)
		{
			return new SeparatorGuiEditor( pnl, this, mc );
		}
    }
}

