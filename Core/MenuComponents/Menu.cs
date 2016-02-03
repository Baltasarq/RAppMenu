using System;
using System.Xml;
using System.Diagnostics;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RWABuilder.Core.MenuComponents {
	/// <summary>
	/// Represents menu entries.
	/// </summary>
	public abstract class Menu: MenuComponent {
		public const string TagName = "Menu";
        public const string EtqName = "Name";

		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.MenuEntry"/> class.
		/// </summary>
		/// <param name='name'>
		/// The name of the menu entry.
		/// </param>
		public Menu(string name, Menu parent)
            :base( name, parent )
		{
			this.Init();
		}

		protected Menu(string name)
			:base( name )
		{
			this.Init();
		}

		private void Init()
		{
			this.menuComponents = new List<MenuComponent>();
		}

		/// <summary>
		/// Adds a given mc, which will become a subentry
		/// </summary>
		/// <param name="mc">Mc.</param>
		public virtual void Add(MenuComponent mc)
		{
			this.menuComponents.Add( mc );
			mc.Parent = this;
		}

		/// <summary>
		/// Looks up a sub entry by its id.
		/// </summary>
		/// <returns>
		/// The sub entry, as a <see cref="MenuComponent"/>.
		/// </returns>
		/// <param name='id'>
		/// The identifier, as a string.
		/// </param>
		public MenuComponent LookUp(string id)
		{
			MenuComponent toret = null;

			foreach (MenuComponent mc in this.menuComponents) {
				if ( mc.Name == id ) {
					toret = mc;
					break;
				}
			}

			if ( toret == null ) {
				throw new ArgumentException( "id was not found as menu component" );
			}

			return toret;
		}

        /// <summary>
        /// Removes the component at index.
        /// </summary>
        /// <param name="index">The index, as an int.</param>
        public virtual void RemoveAt(int index)
        {
            this.menuComponents.RemoveAt( index );
        }

        /// <summary>
        /// Removes the specified <see cref="MenuComponent"/> from this menu.
        /// </summary>
        /// <param name="mc">A menu component, as a <see cref="MenuComponent"/> object</param>
        public virtual void Remove(MenuComponent mc)
        {
            this.menuComponents.Remove( mc );
        }

		/// <summary>
		/// Clears all the components.
		/// </summary>
		public virtual void ClearComponents()
		{
			this.menuComponents.Clear();
		}

		/// <summary>
		/// Gets the menu entries.
		/// </summary>
		/// <value>
		/// The menu entries, as a <see cref="MenuEntry"/> collection.
		/// </value>
		public ReadOnlyCollection<MenuComponent> MenuComponents {
			get {
				ReadOnlyCollection<MenuComponent> toret = null;

				if ( this.menuComponents != null ) {
					toret = this.menuComponents.AsReadOnly();
				} else {
					toret = new ReadOnlyCollection<MenuComponent>( new MenuComponent[]{} );
				}

				return toret;
			}
		}

        /// <summary>
        /// Swap the specified <see cref="MenuComponent"/> objects,
        /// with indexes org and dest.
        /// </summary>
        /// <param name="org">Index denoting a sub menu component.</param>
        /// <param name="dest">Index denoting a sub menu component.</param>
        public void Swap(int org, int dest)
        {
            int biggest = Math.Max( org, dest );

            if ( this.menuComponents.Count > biggest ) {
                var mcDest = this.menuComponents[ dest ];

                this.menuComponents[ dest ] = this.menuComponents[ org ];
                this.menuComponents[ org ] = mcDest;
            }

            return;
        }

        /// <summary>
        /// Swaps the given <see cref="MenuComponent"/> with the next one.
        /// </summary>
        /// <param name="mc">The menu component, as an object.</param>
        public void SwapNext(MenuComponent mc)
        {
            int index = this.menuComponents.IndexOf( mc );

            if ( index >= 0 ) {
                this.SwapNext( index );
            }

            return;
        }

        /// <summary>
        /// Swaps the <see cref="MenuComponent"/> at index with the next one.
        /// </summary>
        /// <param name="index">The index of the menu component to swap.</param>
        public void SwapNext(int index)
        {
            this.Swap( index, index + 1 );
        }

        /// <summary>
        /// Swaps the <see cref="MenuComponent"/> at index with the previous one.
        /// </summary>
        /// <param name="index">The index of the menu component to swap.</param>
        public void SwapPrevious(int index)
        {
            this.Swap( index, index - 1 );
        }

        /// <summary>
        /// Swaps the given <see cref="MenuComponent"/> with the previous one.
        /// </summary>
        /// <param name="mc">The menu component, as an object.</param>
        public void SwapPrevious(MenuComponent mc)
        {
            int index = this.menuComponents.IndexOf( mc );

            if ( index >= 0 ) {
                this.SwapPrevious( index );
            }

            return;
        }

		/// <summary>
		/// Copies this instance.
		/// </summary>
		/// <param name="newParent">
		/// The <see cref="Menu"/> which will be the parent.
		/// </param>
		/// <returns>
		/// A new <see cref="Menu"/>, which is an exact copy of this one.
		/// </returns>
		public override MenuComponent Copy(MenuComponent newParent)
		{
			if ( !( newParent is RegularMenu ) ) {
				throw new ArgumentException( "parent of copy of menu should be a menu" );
			}

            var toret = new RegularMenu( this.Name, (RegularMenu) newParent );

			foreach ( MenuComponent mc in this.MenuComponents ) {
				mc.Copy( toret );
			}

			return toret;
		}

		public override string ToString()
		{
			var toret = new StringBuilder();

			toret.AppendFormat( "[Menu: name={0} sub-components=[", this.Name );

			foreach(MenuComponent mc in this.MenuComponents) {
				toret.Append( mc.ToString() );
			}

			return toret.Append( "]]" ).ToString();
		}

		public override void ToXml(XmlWriter doc)
		{
            Trace.WriteLine( "Menu.ToXml: " + this.ToString() );
            Trace.Indent();

			doc.WriteStartElement( TagName );

            // Name = "m1"
			doc.WriteStartAttribute( EtqName );
			doc.WriteString( this.Name );
			doc.WriteEndAttribute();

			// Sub entries
			foreach (MenuComponent mc in this.menuComponents) {
				mc.ToXml( doc );
			}

            Trace.Unindent();
			doc.WriteEndElement();
		}

		protected void LoadComponentsFromXml(XmlNode node)
		{
			foreach(XmlNode subNode in node.ChildNodes) {
				if ( subNode.Name.Equals( RegularMenu.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					RegularMenu.FromXml( subNode, this );
				}
				else
				if ( subNode.Name.Equals( GraphicMenu.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					GraphicMenu.FromXml( subNode, this );
				}
				else
				if ( subNode.Name.Equals( PdfFile.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					PdfFile.FromXml( subNode, this );
				}
				else
				if ( subNode.Name.Equals( Separator.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					Separator.FromXml( subNode, this );
				}
				else
				if ( subNode.Name.Equals( Function.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					Function.FromXml( subNode, this );
				}
			}

			return;
		}

        /// <summary>
        /// Loads a menu from XML.
        /// </summary>
        /// <returns>A new <see cref="Menu"/>.</returns>
        /// <param name="node">The <see cref="System.Xml.XmlNode"/> describing the menu.</param>
        /// <param name="parent">The parent <see cref="Menu"/>.</param>
        public static Menu FromXml(XmlNode node, Menu parent)
        {
            Trace.WriteLine( "Menu.FromXml: " + node.AsString() );

            var toret = new RegularMenu( "tempMenu", parent );

            // Name = "m1"
			var attrName = (XmlAttribute) node.GetAttribute( EtqName );
			if ( attrName != null ) {
				toret.Name = attrName.InnerText;
			} else {
				throw new XmlException( TagName + ": expected attribute " + EtqName );
			}

			// Subnodes of node
			toret.LoadComponentsFromXml( node );

            return toret;
        }

		private List<MenuComponent> menuComponents;
	}
}

