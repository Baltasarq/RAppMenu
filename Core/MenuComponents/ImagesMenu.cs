using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a menu composed by images, which launch functions.
	/// </summary>
	public class ImagesMenu: MenuEntry {
		public ImagesMenu(string name, MenuEntry parent)
			:base( name, parent )
		{
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

		public override void ToXml(XmlTextWriter doc)
		{
		}
	}
}

