using System;
using System.Windows.Forms;

using RAppMenu.Core;

namespace RAppMenu.Ui.MenuComponentGuiEditors {
    /// <summary>
    /// Separator GUI editor.
    /// This GUI editor does nothing, since separators do not have
    /// anything to modify.
    /// </summary>
    public class SeparatorGuiEditor: MenuComponentGuiEditor {
        public SeparatorGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
            : base( panel, mctn, mc )
        {
        }

		public override void Show()
		{
			base.Show();
		}
    }
}
