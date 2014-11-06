using System;
using System.Xml;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a menu composed by images, which launch functions.
	/// </summary>
	public class ImagesMenu: Menu {
		public ImagesMenu(string name, Menu parent)
			:base( name, parent )
		{
		}

		/// <summary>
		/// Adds a given function to the images menu, which will become a subentry
		/// Note that this superseeds the Add in base class (MenuEntry).
		/// </summary>
		/// <param name="f">The function to add, as a <see cref="Function"/> object.</param>
		public override void Add(MenuComponent ime)
		{
			if ( !( ime is ImageMenuEntry) ) {
				throw new ArgumentException(
					"argument should be a ImageMenuEntry for ImagesMenu.Add()" );
			}

			base.Add( ime );
		}
	}
}

