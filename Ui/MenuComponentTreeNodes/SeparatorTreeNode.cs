using System.Windows.Forms;

using RAppMenu.Ui.MenuComponentGuiEditors;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for separators.
    /// </summary>
    public class SeparatorTreeNode: MenuComponentTreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNodes.SeparatorTreeNode"/> class.
		/// Creates the associated MenuComponent.
		/// </summary>
		/// <param name="text">This string parameter is ignored.</param>
		/// <param name="parent">The <see cref="Menu"/> which is parent of this component.</param>
		public SeparatorTreeNode(string text, Core.MenuComponents.Menu parent)
            :base( Separator.TagName, new Separator( parent ) )
        {
			this.Init();
        }

		public SeparatorTreeNode(Separator separator)
			:base( separator )
		{
			this.Init();
		}

		private void Init()
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

