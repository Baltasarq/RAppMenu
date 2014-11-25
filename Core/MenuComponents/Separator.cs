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

        /// <summary>
        /// Loads a separator from XML.
        /// </summary>
        /// <returns>A new <see cref="Separator"/>.</returns>
        /// <param name="node">The <see cref="System.Xml.XmlNode"/> describing the separator.</param>
        /// <param name="parent">The parent <see cref="Menu"/>.</param>
        public static Separator FromXml(XmlNode node, Menu parent)
        {
            return new Separator( parent );
        }

		public override string ToString()
		{
			return "[Separator]";
		}
	}
}

