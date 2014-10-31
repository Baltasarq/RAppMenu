using System;
using System.Xml;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Core {
	/// <summary>
	/// Base class for all menu components:
	/// menu entries, functions, separators...
	/// </summary>
	public abstract class MenuComponent {
        public MenuComponent(string name, MenuEntry parent)
		{
            this.name = name;
			this.parent = parent;
			this.parent.Add( this );
		}

		protected MenuComponent(string name)
		{
			this.name = name;
			this.parent = null;
		}

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        /// <value>The name, as a string.</value>
        public string Name {
            get {
                return this.name;
            }
        }

		/// <summary>
		/// Gets the parent of this menu component.
		/// </summary>
		/// <value>The parent.</value>
		public MenuComponent Parent {
			get {
				return this.Parent;
			}
		}

		/// <summary>
		/// Converts this menu component to XML.
		/// </summary>
		public abstract void ToXml(XmlTextWriter doc);

        private string name;
		private MenuEntry parent;
	}
}

