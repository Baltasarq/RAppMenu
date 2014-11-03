using System;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// This is the root menu emtry.
	/// In the XML, it is represented as "Menue"
	/// </summary>
	public class RootMenuEntry: MenuEntry {
		public new const string TagName = "Menue";

		public RootMenuEntry()
			:base( "Root" )
		{
		}

        public override void ToXml(System.Xml.XmlTextWriter doc)
        {
			doc.WriteStartElement( TagName );

            foreach (MenuComponent mc in this.MenuComponents) {
                mc.ToXml( doc );
            }

			doc.WriteEndElement();
        }
	}
}

