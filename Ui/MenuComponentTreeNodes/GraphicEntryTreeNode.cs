using System;
using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;
using CoreComponents = RWABuilder.Core.MenuComponents;
using UiEditors = RWABuilder.Ui.MenuComponentGuiEditors;

namespace RWABuilder.Ui.MenuComponentTreeNodes {
	/// <summary>
	/// Image menu entry in the UI side.
	/// </summary>
	public class GraphicEntryTreeNode: MenuComponentTreeNode {
        public GraphicEntryTreeNode(GraphicEntry gme)
			:base( gme )
		{
			this.ImageIndex = this.SelectedImageIndex =
				UserAction.LookUp( "addfunction" ).ImageIndex;

			this.Text = UiEditors.FunctionGuiEditor.BuildCaptionCombination(
				( (CoreComponents.GraphicEntry) this.MenuComponent ).Function, "", "" );
		}

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new Ui.MenuComponentGuiEditors.GraphicEntryGuiEditor( pnl, this, this.MenuComponent );
		}
	}
}

