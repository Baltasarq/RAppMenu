using System;
using System.Xml;

namespace RAppMenu.Core {
	/// <summary>
	/// Base class for all menu components:
	/// menu entries, functions, separators...
	/// </summary>
	public abstract class MenuComponent {
        public MenuComponent(string name)
		{
            this.name = name;
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
		/// Converts this menu component to XML.
		/// </summary>
		public abstract void ToXml(XmlTextWriter doc);

        private string name;
	}
}

