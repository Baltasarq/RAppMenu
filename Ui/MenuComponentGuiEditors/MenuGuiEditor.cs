using System;
using System.Windows.Forms;

using RAppMenu.Core;

namespace RAppMenu.Ui.MenuComponentGuiEditors {
	public class MenuGuiEditor: NamedComponentGuiEditor {
		public MenuGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
			: base( panel, mctn, mc )
		{
		}
	}
}

