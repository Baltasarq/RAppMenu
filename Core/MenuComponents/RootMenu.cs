using System;
using System.Diagnostics;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// This is the root menu emtry.
	/// In the XML, it is represented as "Menue"
	/// </summary>
	public class RootMenu: Menu {
		public new const string TagName = "Menue";

		public RootMenu()
			:base( "Root" )
		{
		}

        public override void ToXml(System.Xml.XmlTextWriter doc)
        {
            Trace.WriteLine( "RootMenu.ToXml: " + this.ToString() );
            Trace.Indent();

			doc.WriteStartElement( TagName );

            foreach (MenuComponent mc in this.MenuComponents) {
                mc.ToXml( doc );
            }

			doc.WriteEndElement();
            Trace.Unindent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RAppMenu.Core.MenuDesign"/> needs save.
        /// </summary>
        /// <value><c>true</c> if needs save; otherwise, <c>false</c>.</value>
        public bool NeedsSave {
            get; set;
        }
	}
}

