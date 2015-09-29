using System;

namespace RWABuilder.Core {
	class MenuComponentClipboard {
		public MenuComponentClipboard() {
			this.fakeMenuDesignForCopying = new MenuDesign();
			this.fakeGraphMenuForCopying = new Core.MenuComponents.GraphicMenu(
				"graphicMenu",
				this.fakeMenuDesignForCopying.Root );

			this.fakeMenuDesignForCopying.Root.Add( this.fakeGraphMenuForCopying );
			this.menuComponent = null;
		}

		public MenuComponent MenuComponent {
			get {
				MenuComponent toret = this.menuComponent;

				if ( toret != null ) {
					this.menuComponent = this.Copy( toret );
				}

				return toret;
			}
			set {
				if ( value != null ) {
					this.menuComponent = this.Copy( value );
				}

				return;
			}
		}

		/// <summary>
		/// Has the clipbard any component to copy?
		/// </summary>
		/// <value><c>true</c> if the clipboard has contents; otherwise, <c>false</c>.</value>
		public bool HasContents {
			get {
				return ( this.menuComponent != null );
			}
		}

		/// <summary>
		/// Has the clipboard *valid* contents?
		/// You can't paste a graphicentry inside a regular menu, nor paste anything inside
		/// a graphic menu (except graphic entries).
		/// </summary>
		/// <returns><c>true</c> if this instance has contents for the specified mc; otherwise, <c>false</c>.</returns>
		/// <param name="mc">A <see cref="MenuComponent"/>.</param>
		public bool HasContentsFor(MenuComponent targetMc) {
			bool toret = false;

			if ( this.HasContents
			  && targetMc is Core.MenuComponents.Menu )
			{
				bool isMenuComponentGraphicEntry = this.menuComponent is Core.MenuComponents.GraphicEntry;

				bool isGraphicEntryForGraphicMenu =
					( ( targetMc is Core.MenuComponents.GraphicMenu ) && isMenuComponentGraphicEntry );
				bool isNotGraphicEntryForRegularMenu =
					( ( targetMc is Core.MenuComponents.RegularMenu ) && !isMenuComponentGraphicEntry );

				toret = isGraphicEntryForGraphicMenu || isNotGraphicEntryForRegularMenu;
			}

			return toret;
		}

		private MenuComponent Copy(MenuComponent value) {
			MenuComponent toret = null;

			if ( value != null ) {
				MenuComponent parent = this.fakeMenuDesignForCopying.Root;

				if ( value is Core.MenuComponents.GraphicEntry ) {
					parent = this.fakeGraphMenuForCopying;
				}

				toret = value.Copy( parent );
			}

			return toret;
		}

		private MenuComponent menuComponent;
		private MenuDesign fakeMenuDesignForCopying;
		private Core.MenuComponents.GraphicMenu fakeGraphMenuForCopying;
	}
}
