using System;
using System.Windows.Forms;

using RAppMenu.Ui.MenuComponentGuiEditors;
using RAppMenu.Core;

namespace RAppMenu.Ui {
    /// <summary>
    /// Component GUI editor.
    /// This is the base class for editors modifying the corresponding
    /// <see cref="RAppMenu.Core.MenuComponent"/> objects.
    /// </summary>
    public abstract class MenuComponentGuiEditor {
        /// <summary>
        /// Initializes a new instance of the <see cref="RAppMenu.Ui.ComponentGuiEditor"/> class.
        /// </summary>
        /// <param name="panel">A <see cref="Panel"/> object in which to place the controls.</param>
		/// <param name="mctn">A <see cref="MenuComponentTreeNode"/> which represents the component in the tree.</param>
		/// <param name="mc">A <see cref="MenuComponent"/> object to be edited.</param>
		protected MenuComponentGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
        {
			this.menuComponent = mc;
			this.mctn = mctn;
            this.panel = panel;
        }

		/// <summary>
		/// Show the editor.
		/// </summary>
		public virtual void Show()
		{
			this.Hide();
		}

		protected void Hide()
		{
			foreach(Control ctrl in this.Panel.Controls) {
				ctrl.Hide();
			}

			return;
		}

        /// <summary>
        /// Gets the panel in which the editor is going to be placed.
        /// </summary>
        /// <value>The panel.</value>
        public Panel Panel {
            get {
                return this.panel;
            }
        }

		/// <summary>
		/// Gets the treenode corresponding to this MenuComponent being edited.
		/// </summary>
		/// <value>The menu component tree node.</value>
		public MenuComponentTreeNode MenuComponentTreeNode {
			get {
				return this.mctn;
			}
		}

		/// <summary>
		/// Gets the menu component that is being edited.
		/// </summary>
		/// <value>The menu component.</value>
		public MenuComponent MenuComponent {
			get {
				return this.menuComponent;
			}
		}

        private Panel panel;
		private MenuComponentTreeNode mctn;

		private MenuComponent menuComponent;

    }
}

