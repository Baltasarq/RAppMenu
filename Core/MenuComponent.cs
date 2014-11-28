using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Core {
	/// <summary>
	/// Base class for all menu components:
	/// menu entries, functions, separators...
	/// </summary>
	public abstract class MenuComponent {
		public const string PathDelimiter = ": ";

        public MenuComponent(string name, Menu parent)
		{
			this.SetName( name );
			this.parent = parent;
			this.parent.Add( this );
		}

		protected MenuComponent(string name)
		{
			this.SetName( name );
			this.parent = null;
		}

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        /// <value>The name, as a string.</value>
        public virtual string Name {
            get {
                return this.name;
            }
            set {
				this.SetName( value );
            }
        }

		private void SetName(string value)
		{
			if ( string.IsNullOrWhiteSpace( value ) ) {
                throw new ArgumentNullException( "invalid name for menu component" );
            }

            this.name = value.Trim().Replace( " ", "" );
		}

		/// <summary>
		/// Gets the parent of this menu component.
		/// </summary>
		/// <value>The parent.</value>
        public Menu Parent {
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

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <returns>Returns a MenuComponent[] vector, with the ancestors of this node.</returns>
		public MenuComponent[] GetPath()
		{
			MenuComponent mc = this.Parent;
			var path = new List<MenuComponent>();

			while ( mc != null ) {
				path.Insert( 0, mc );
				mc = mc.Parent;
			}

			return path.ToArray();
		}

		/// <summary>
		/// Gets the path to this <see cref="MenuComponent"/> as string.
		/// </summary>
		/// <returns>The path, as a string.</returns>
		/// <seealso cref="GetPath"/>
		public string GetPathAsString()
		{
			int delimiterLength = PathDelimiter.Length;
			var toret = new StringBuilder();
			MenuComponent[] path = this.GetPath();

			// Build the string representing this path
			foreach(MenuComponent mc in path) {
				toret.Append( mc.name );
				toret.Append( ": " );
			}
			// Remove last delimiter
			toret.Remove( toret.Length - delimiterLength, delimiterLength );

			return toret.ToString();
		}

		public override string ToString()
		{
			return string.Format( "[MenuComponent: Name={0}]", Name );
		}

        private string name;
		private Menu parent;
	}
}

