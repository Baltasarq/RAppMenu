using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// A separator in the menu.
	/// It will be stored as "<Separator/>"
	/// </summary>
	public class Separator: MenuComponent {
		public const string TagName = "Separator";

		public Separator(Menu parent)
            : base( TagName, parent )
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

