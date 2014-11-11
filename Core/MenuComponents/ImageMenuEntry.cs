using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	public class ImageMenuEntry: Menu {
		public const string EtqImagePath = "Image";
        public const string EtqImageToolTip = "ImageTooltip";

		public ImageMenuEntry(string id, ImagesMenu parent)
			:base( id, parent )
		{
			this.Function = new Function( "fn", this );
			this.ImagePath = "";
            this.ImageToolTip = "";
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
            }
        }

		/// <summary>
		/// Gets or sets the function associated with this entry.
		/// </summary>
		/// <value>The <see cref="Function"/>.</value>
		public Function Function {
			get; set;
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
                doc.WriteString( this.Name );
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

		private string imagePath;
        private string imageToolTip;
	}
}

