using System;
using System.Xml;
using System.Diagnostics;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a menu composed by images, which launch functions.
	/// </summary>
	public class GraphicMenu: Menu {
        public const string EtqMinimumNumberOfColumns = "MinNumberColumns";
        public const string EtqImageWidth = "ImageWidth";
        public const string EtqImageHeight = "ImageHeight";

		public const int MinimumGraphicSize = 16;
		public const int MaximumGraphicSize = 250;
		public const int MinimumColumns = 1;
		public const int MaximumColumns = 10;

		public GraphicMenu(string name, Menu parent)
			:base( name, parent )
		{
			this.ImageWidth = this.ImageHeight = MinimumGraphicSize;
            this.MinimumNumberOfColumns = MinimumColumns;
		}

		/// <summary>
		/// Adds a given entry to the graphic menu, which will become a subentry
		/// Note that this superseeds the Add in base class (MenuEntry).
		/// </summary>
        /// <param name="ime">The <see cref="GraphicMenuEntry"/> object.</param>
		public override void Add(MenuComponent ime)
		{
			if ( !( ime is GraphicMenuEntry) ) {
				throw new ArgumentException(
					"argument should be a GraphicMenuEntry for GraphicMenu.Add()" );
			}

			base.Add( ime );
		}

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>The width of the image, as a positive int.</value>
        public int ImageWidth {
            get {
                return this.imageWidth;
            }
            set {
                this.imageWidth = Math.Max( MinimumGraphicSize, value );
				this.imageWidth = Math.Min( MaximumGraphicSize, value );
                this.SetNeedsSave();
            }
        }

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>The height of the image, as a positive int.</value>
        public int ImageHeight {
            get {
                return this.imageHeight;
            }
            set {
				this.imageHeight = Math.Max( MinimumGraphicSize, value );
				this.imageHeight = Math.Min( MaximumGraphicSize, value );
                this.SetNeedsSave();
            }
        }

        /// <summary>
        /// Gets or sets the minimum number of columns for this menu.
        /// </summary>
        /// <value>The minimum number of columns, as a positive int.</value>
        public int MinimumNumberOfColumns {
            get {
                return this.minimumNumberOfColumns;
            }
            set {
				this.minimumNumberOfColumns = Math.Max( MinimumColumns, value );
				this.minimumNumberOfColumns = Math.Min( MaximumColumns, value );
                this.SetNeedsSave();
            }
        }

		public override string ToString()
		{
            string toret = "[GraphicMenu " + base.ToString();

			return toret
				+ string.Format(
                    " ImageWidth={0}, ImageHeight={1}, MinimumNumberOfColumns={2}]",
                    ImageWidth, ImageHeight, MinimumNumberOfColumns );
		}

        public override void ToXml(XmlTextWriter doc)
        {
            Trace.WriteLine( "GraphicMenu.ToXml: " + this.ToString() );
            Trace.Indent();

            doc.WriteStartElement( Menu.TagName );

            // Name = "m1"
            doc.WriteStartAttribute( Menu.EtqName );
            doc.WriteString( this.Name );
            doc.WriteEndAttribute();

            if ( this.ImageWidth > 0 ) {
                // ImageWidth = "16"
                doc.WriteStartAttribute( EtqImageWidth );
                doc.WriteString( this.ImageWidth.ToString() );
                doc.WriteEndAttribute();
            }

            if ( this.ImageHeight > 0 ) {
                // ImageHeight = "16"
                doc.WriteStartAttribute( EtqImageHeight );
                doc.WriteString( this.ImageWidth.ToString() );
                doc.WriteEndAttribute();
            }

            if ( this.MinimumNumberOfColumns > 0 ) {
                // MinNumberColumns = "1"
                doc.WriteStartAttribute( EtqMinimumNumberOfColumns );
                doc.WriteString( this.MinimumNumberOfColumns.ToString() );
                doc.WriteEndAttribute();
            }

			foreach(GraphicMenuEntry imgmc in this.MenuComponents) {
				imgmc.ToXml( doc );
			}

            doc.WriteEndElement();
            Trace.Unindent();
        }

		public new static GraphicMenu FromXml(XmlNode node, Menu menu)
		{
            Trace.WriteLine( "RootMenu.FromXml: " + node.AsString() );
            Trace.Indent();

            var toret = new GraphicMenu( node.GetAttribute( EtqName ).InnerText,
                                         menu );

            // Retrieve attribute data
            foreach(XmlAttribute attr in node.Attributes) {
                if ( attr.Name.Equals( EtqImageWidth, StringComparison.OrdinalIgnoreCase ) )
                {
                    toret.ImageWidth = attr.GetValueAsInt();
                }
                else
                if ( attr.Name.Equals( EtqImageHeight, StringComparison.OrdinalIgnoreCase ) )
                {
                    toret.ImageHeight = attr.GetValueAsInt();
                }
                else
                if ( attr.Name.Equals( EtqMinimumNumberOfColumns, StringComparison.OrdinalIgnoreCase ) )
                {
                    toret.MinimumNumberOfColumns = attr.GetValueAsInt();
                }
            }

            // Retrieve ImageMenuEntries (and, eventually, their enclosed functions)
            foreach(XmlNode subNode in node.ChildNodes) {
                if ( subNode.Name.Equals( TagName, StringComparison.OrdinalIgnoreCase ) )
                {
                    GraphicMenuEntry.FromXml( subNode, toret );
                }
            }

            Trace.Unindent();
			return toret;
		}

        private int imageWidth;
        private int imageHeight;
        private int minimumNumberOfColumns;
	}
}

