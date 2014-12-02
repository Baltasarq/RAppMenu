using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	public class GraphicMenuEntry: Menu {
		public const string EtqImagePath = "Image";
        public const string EtqImageToolTip = "ImageTooltip";

		public GraphicMenuEntry(string id, GraphicMenu parent)
			:base( id, parent )
		{
			this.ImagePath = "image.png";
            this.ImageToolTip = "";

			// Create a function that will be the only subcomponent here.
			new Function( id, this );
		}

		/// <summary>
		/// Gets the name of the image menu entry.
		/// Note that the function inside will also be renamed.
		/// </summary>
		/// <value>
		/// The name, as a string.
		/// </value>
		public override string Name {
			get {
				return base.Name;
			}
			set {
				base.Name = "ime_" + value;

				if ( this.MenuComponents.Count > 0 ) {
					this.Function.Name = value;
				}
			}
		}

        /// <summary>
        /// Discards the function being held internally, and stores this one
        /// instead.
        /// </summary>
        /// <param name="mc">A <see cref="MenuComponent"/>, should be a <see cref="Function"/> object.</param>
		public override void Add(MenuComponent mc)
		{
            if ( !( mc is Function ) ) {
                throw new ArgumentException( "this graphic menu entry can only hold functions" );
            }

            if ( this.MenuComponents.Count > 0 ) {
                base.RemoveAt( 0 );
            }
                
            base.Add( mc );
		}

		public override void Remove(MenuComponent mc)
		{
			throw new ArgumentException( "cannot remove from an image menu entry" );
		}

		public override void RemoveAt(int index)
		{
			throw new ArgumentException( "cannot remove from an image menu entry" );
		}

		/// <summary>
        /// Gets or sets the path to the image of the menu.
        /// </summary>
        /// <value>The image path, as a string.</value>
        public string ImagePath {
            get {
                return this.imagePath;
            }
            set {
                this.imagePath = value.Trim();
                this.SetNeedsSave();
            }
        }

        /// <summary>
        /// Gets or sets the image tool tip.
        /// </summary>
        /// <value>The image tool tip.</value>
        public string ImageToolTip {
            get {
                return this.imageToolTip;
            }
            set {
                this.imageToolTip = value.Trim();
                this.SetNeedsSave();
            }
        }

		/// <summary>
		/// Gets or sets the function associated with this entry.
		/// </summary>
		/// <value>The <see cref="Function"/>.</value>
		public Function Function {
			get {
				return (Function) this.MenuComponents[ 0 ];
			}
		}

		public override string ToString()
		{
			return string.Format( "[GraphicMenuEntry: Name={0}, ImagePath={1}, "
				+ "ImageToolTip={2}, Function={3}]",
			    Name, ImagePath, ImageToolTip, Function.ToString() );
		}

		public override void ToXml(XmlTextWriter doc)
		{
			doc.WriteStartElement( Menu.TagName );

            // Name = "m1"
			doc.WriteStartAttribute( Menu.EtqName );
			doc.WriteString( this.Name );
			doc.WriteEndAttribute();

            if ( !string.IsNullOrWhiteSpace( this.ImagePath ) ) {
                // Image = "/path/to/image1.png"
                doc.WriteStartAttribute( EtqImagePath );
                doc.WriteString( this.ImagePath );
                doc.WriteEndAttribute();

                if ( !string.IsNullOrWhiteSpace( this.ImageToolTip ) ) {
                    // ImageToolTip = "help"
                    doc.WriteStartAttribute( EtqImageToolTip );
                    doc.WriteString( this.ImageToolTip );
                    doc.WriteEndAttribute();
                }
            }

			this.Function.ToXml( doc );
			doc.WriteEndElement();
		}

		public static GraphicMenuEntry FromXml(XmlNode node, GraphicMenu menu)
		{
            var toret = new GraphicMenuEntry(
                                       node.GetAttribute( EtqName ).InnerText,
                                       menu );

            // Retrieve attribute data
            // Remember that the data here can invalidate the data in the parent
            foreach(XmlAttribute attr in node.Attributes) {
                if ( attr.Name.Equals( GraphicMenu.EtqImageWidth, StringComparison.OrdinalIgnoreCase ) )
                {
                    menu.ImageWidth = attr.GetValueAsInt();
                }
                else
                if ( attr.Name.Equals( GraphicMenu.EtqImageHeight, StringComparison.OrdinalIgnoreCase ) )
                {
                    menu.ImageHeight = attr.GetValueAsInt();
                }
                else
                if ( attr.Name.Equals( GraphicMenu.EtqMinimumNumberOfColumns, StringComparison.OrdinalIgnoreCase ) )
                {
                    menu.MinimumNumberOfColumns = attr.GetValueAsInt();
                }
                else
                if ( attr.Name.Equals( EtqImagePath, StringComparison.OrdinalIgnoreCase ) )
                {
                    toret.ImagePath = attr.InnerText.Trim();
                }
                else
                if ( attr.Name.Equals( EtqImageToolTip, StringComparison.OrdinalIgnoreCase ) )
                {
                    toret.ImageToolTip = attr.InnerText.Trim();
                }
            }

            // Retrieve enclosed functions
            if ( node.ChildNodes.Count != 1 ) { 
                throw new XmlException( "each graphic entry should have exactly one function" );
            } else {
                var subNode = node.ChildNodes[ 0 ];

                if ( subNode.Name.Equals( Function.TagName, StringComparison.OrdinalIgnoreCase ) )
                {
                    Function.FromXml( subNode, toret );
                } else {
                    throw new XmlException( "sub-component of graphic meny entry is not a function" );
                }
            }

            return toret;
		}

		private string imagePath;
        private string imageToolTip;
	}
}

