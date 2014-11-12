using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents menu entries.
	/// </summary>
	public class Menu: MenuComponent {
		public const string TagName = "MenueEntry";
        public const string EtqName = "Name";

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.MenuEntry"/> class.
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
		/// Gets the menu entries.
		/// </summary>
		/// <value>
		/// The menu entries, as a <see cref="MenuEntry"/> collection.
		/// </value>
		public ReadOnlyCollection<MenuComponent> MenuComponents {
			get {
				return this.menuComponents.AsReadOnly();
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

		public override void ToXml(XmlTextWriter doc)
		{
			doc.WriteStartElement( TagName );

            // Name = "m1"
			doc.WriteStartAttribute( EtqName );
			doc.WriteString( this.Name );
			doc.WriteEndAttribute();

			// Sub entries
			foreach (MenuComponent mc in this.menuComponents) {
				mc.ToXml( doc );
			}

			doc.WriteEndElement();
		}

		private List<MenuComponent> menuComponents;
	}
}

