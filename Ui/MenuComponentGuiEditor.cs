using System;
using System.Windows.Forms;

using RWABuilder.Ui.MenuComponentGuiEditors;
using RWABuilder.Core;

namespace RWABuilder.Ui {
    /// <summary>
    /// Component GUI editor.
    /// This is the base class for editors modifying the corresponding
    /// <see cref="RWABuilder.Core.MenuComponent"/> objects.
    /// </summary>
    public abstract class MenuComponentGuiEditor {
        /// <summary>
        /// Initializes a new instance of the <see cref="RWABuilder.Ui.ComponentGuiEditor"/> class.
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
			this.HideAllEditors();
		}

		/// <summary>
		/// Hides all editors from view.
		/// </summary>
		protected void HideAllEditors()
		{
			foreach(Control pnlEditor in this.Panel.Controls) {
				pnlEditor.Hide();
			}

			return;
		}

		/// <summary>
		/// Hides the editor.
		/// It is assured no other editor is shown.
		/// </summary>
		public void Hide()
		{
			this.HideAllEditors();
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

		/// <summary>
		/// Reads the data from the component.
		/// The editor reflects the info in the <see cref="MenuComponent"/>
		/// </summary>
		public void ReadDataFromComponent() {
		}

        /// <summary>
        /// Gets a value indicating whether this <see cref="RWABuilder.Ui.MenuComponentGuiEditor"/> is on building.
        /// </summary>
        /// <value><c>true</c> if on building; otherwise, <c>false</c>.</value>
        protected bool OnBuilding {
            get; set;
        }

        private Panel panel;
		private MenuComponentTreeNode mctn;

		private MenuComponent menuComponent;

    }
}

