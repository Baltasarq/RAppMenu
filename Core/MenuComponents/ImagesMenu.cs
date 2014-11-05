using System;
using System.Xml;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a menu composed by images, which launch functions.
	/// </summary>
	public class ImagesMenu: MenuEntry {
		public ImagesMenu(string name, MenuEntry parent)
			:base( name, parent )
		{
			this.imageList = new Dictionary<string, Bitmap>();
		}

		/// <summary>
		/// Adds a given function to the images menu, which will become a subentry
		/// Note that this superseeds the Add in base class (MenuEntry).
		/// </summary>
		/// <param name="f">The function to add, as a <see cref="Function"/> object.</param>
		public override void Add(MenuComponent f)
		{
			if ( !( f is Function) ) {
				throw new ArgumentException(
					"argument should be a function for images menu" );
			}

			base.Add( f );
		}

		/// <summary>
		/// Adds the specified bitmap, related to the function
		/// of name id. Note that the function must exist.
		/// </summary>
		/// <param name='id'>
		/// The identifier, as a string.
		/// </param>
		/// <param name='bitmap'>
		/// The <see cref="Bitmap"/>.
		/// </param>
		public void AddBitmap(string id, Bitmap bitmap)
		{
			// Throws if the sub entry is not found.
			this.LookUp( id );

			// Store it.
			this.imageList.Add( id, bitmap );
		}

		/// <summary>
		/// Gets the image list for this menu.
		/// </summary>
		/// <value>
		/// The image list, as a read-only collection of <see cref="Bitmap"/>.
		/// </value>
		public ReadOnlyCollection<Bitmap> ImageList {
			get {
				ReadOnlyCollection<MenuComponent> mcs = this.MenuComponents;
				var toret = new List<Bitmap>();

				for(int i = 0; i < mcs.Count; ++i) {
					toret.Add( this.imageList[ mcs[ i ].Name ] );
				}


				return toret.AsReadOnly();
			}
		}

		public override void ToXml(XmlTextWriter doc)
		{
		}

		private Dictionary<string, Bitmap> imageList;
	}
}

