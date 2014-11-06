using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	public class ImageMenuEntry: Menu {
		public const string EtqImagePath = "Image";
        public const string EtqImageWidth = "ImageWidth";
        public const string EtqImageHeight = "ImageHeight";
        public const string EtqImageToolTip = "ImageTooltip";
        public const string EtqMinimumNumberOfColumns = "MinNumberColumns";

		public ImageMenuEntry(string id, ImagesMenu parent)
			:base( id, parent )
		{
			this.Function = new Function( "fn", this );
			this.ImagePath = "";
            this.ImageToolTip = "";
            this.ImageWidth = this.ImageHeight = 0;
            this.MinimumNumberOfColumns = 0;
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
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>The width of the image, as a positive int.</value>
        public int ImageWidth {
            get {
                return this.imageWidth;
            }
            set {
                if ( value < 0 ) {
                    throw new ArgumentOutOfRangeException( "MenuEntry.ImageWidth should be >= 0" );
                }

                this.imageWidth = value;
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
                if ( value < 0 ) {
                    throw new ArgumentOutOfRangeException( "MenuEntry.ImageHeight should be >= 0" );
                }

                this.imageHeight = value;
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
                if ( value < 0 ) {
                    throw new ArgumentOutOfRangeException( "MenuEntry.MinimumNumberOfColumns should be >= 0" );
                }

                this.minimumNumberOfColumns = value;
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

                if ( this.ImageWidth > 0 ) {
                    // ImageWidth = "10"
                    doc.WriteStartAttribute( EtqImageWidth );
                    doc.WriteString( this.ImageWidth.ToString() );
                    doc.WriteEndAttribute();
                }

                if ( this.ImageHeight > 0 ) {
                    // ImageHeight = "10"
                    doc.WriteStartAttribute( EtqImageHeight );
                    doc.WriteString( this.ImageWidth.ToString() );
                    doc.WriteEndAttribute();
                }

                if ( this.MinimumNumberOfColumns > 0 ) {
                    // MinNumberColumns = "2"
                    doc.WriteStartAttribute( EtqMinimumNumberOfColumns );
                    doc.WriteString( this.MinimumNumberOfColumns.ToString() );
                    doc.WriteEndAttribute();
                }

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
        private int imageWidth;
        private int imageHeight;
        private int minimumNumberOfColumns;
	}
}

