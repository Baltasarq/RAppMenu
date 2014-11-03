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
            set {
                if ( string.IsNullOrWhiteSpace( value ) ) {
                    throw new ArgumentNullException( "invalid name for menu component" );
                }

                this.name = value.Trim().Replace( " ", "" ).ToLower();
            }
        }

		/// <summary>
		/// Gets the parent of this menu component.
		/// </summary>
		/// <value>The parent.</value>
        public MenuEntry Parent {
			get {
				return this.parent;
			}
		}

        /// <summary>
        /// Removes this instance, by calling itself to its parent.Remove().
        /// Its parent is a <see cref="MenuEntry"/> 
        /// </summary>
        public void Remove()
        {
            this.Parent.Remove( this );
        }

        /// <summary>
        /// Swaps this instance, by calling itself to its parent.SwapPrevious().
        /// Its parent is a <see cref="MenuEntry"/> 
        /// </summary>
        public void SwapPrevious()
        {
            this.Parent.SwapPrevious( this );
        }

        /// <summary>
        /// Swaps this instance, by calling itself to its parent.SwapNext().
        /// Its parent is a <see cref="MenuEntry"/> 
        /// </summary>
        public void SwapNext()
        {
            this.Parent.SwapNext( this );
        }

		/// <summary>
		/// Converts this menu component to XML.
		/// </summary>
		public abstract void ToXml(XmlTextWriter doc);

        private string name;
		private MenuEntry parent;
	}
}

