using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// A separator in the menu.
	/// It will be stored as "<Separator/>"
	/// </summary>
	public class Separator: MenuComponent {
		const string TagName = "Separator";

		public Separator(MenuEntry parent)
            : base( "<Separator>", parent )
		{
		}

		/// <summary>
		/// Converts this node to XML.
		/// </summary>
		public override void ToXml(XmlTextWriter doc)
		{
			doc.WriteStartElement( TagName );
			doc.WriteEndElement();
		}
	}
}

