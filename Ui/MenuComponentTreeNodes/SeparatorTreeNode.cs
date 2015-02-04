using System.Windows.Forms;

using RWABuilder.Ui.MenuComponentGuiEditors;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for separators.
    /// </summary>
    public class SeparatorTreeNode: MenuComponentTreeNode {
        /// <summary>
        /// Initializes a new instance of the <see cref="RWABuilder.Ui.MenuComponentTreeNodes.SeparatorTreeNode"/> class.
        /// </summary>
        /// <param name="separator">The <see cref="Separator"/> to represent.</param>
        public SeparatorTreeNode(Separator separator)
			:base( separator )
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
		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new SeparatorGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}

