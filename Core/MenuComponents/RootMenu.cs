using System;
using System.Xml;
using System.Diagnostics;

namespace RWABuilder.Core.MenuComponents {
	/// <summary>
	/// This is the root menu emtry.
	/// In the XML, it is represented as "Menue"
	/// </summary>
	public class RootMenu: RegularMenu {
        public RootMenu(MenuDesign owner)
			:base( "Root" )
		{
            this.menuDesign = owner;
		}

		public override void ToXml(XmlWriter doc)
        {
            Trace.WriteLine( "RootMenu.ToXml: " + this.ToString() );
            Trace.Indent();
			Trace.WriteLine( "Menu Name=" + this.Name );
			base.ToXml( doc );
			Trace.Unindent();
		}

		public void FromXml(XmlNode node)
		{
			Trace.WriteLine( "RootMenu.FromXml: " + node.AsString() );
			this.ClearComponents();

			// Name = "m1"
			var attrName = (XmlAttribute) node.GetAttribute( EtqName );
			this.Name = attrName.InnerText;

			// Subnodes of node
			this.LoadComponentsFromXml( node );
			return;
		}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RWABuilder.Core.MenuDesign"/> needs save.
        /// </summary>
        /// <value><c>true</c> if needs save; otherwise, <c>false</c>.</value>
        public bool NeedsSave {
            get; set;
        }

        /// <summary>
        /// Gets the owner, a <see cref="MenuDesign"/>.
        /// </summary>
        /// <value>The owner, a a <see cref="MenuDesign"/>.</value>
        public MenuDesign Owner {
            get {
                return this.menuDesign;
            }
        }

        private MenuDesign menuDesign;
	}
}

