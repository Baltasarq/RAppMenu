using System;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// This is the root menu emtry.
	/// In the XML, it is represented as "Menue"
	/// </summary>
	public class RootMenuEntry: MenuEntry {
		public RootMenuEntry()
			:base( "Root" )
		{
		}

        public override void ToXml(System.Xml.XmlTextWriter doc)
        {
            foreach (MenuComponent mc in this.MenuComponents) {
                mc.ToXml( doc );
            }
        }
	}
}

