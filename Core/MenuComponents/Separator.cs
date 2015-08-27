using System.Diagnostics;
using System.Xml;

namespace RWABuilder.Core.MenuComponents {
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
		public override void ToXml(XmlWriter doc)
		{
            Trace.WriteLine( "Separator.ToXml()" );

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
            Trace.WriteLine( "Separator.FromXml()" );

            return new Separator( parent );
        }

		/// <summary>
		/// Copies this separator.
		/// </summary>
		/// <param name="newParent">
		/// The <see cref="Menu"/> which will be the parent of the copy.
		/// </param>
		/// <returns>
		/// A new <see cref="Separator"/>, which is an exact copy of this one.
		/// </returns>
		public override MenuComponent Copy(MenuComponent newParent)
		{
			return new Separator( (Menu) newParent );
		}

		public override string ToString()
		{
			return "[Separator]";
		}
	}
}

