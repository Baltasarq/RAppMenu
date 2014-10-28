using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents menu entries.
	/// </summary>
	public class MenuEntry: MenuComponent {
		public const string TagName = "MenueEntry";
        public const string EtqName = "Name";
        public const string EtqImagePath = "Image";
        public const string EtqImageWidth = "ImageWidth";
        public const string EtqImageHeight = "ImageHeight";
        public const string EtqImageToolTip = "ImageTooltip";
        public const string EtqMinimumNumberOfColumns = "MinNumberColumns";

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.MenuEntry"/> class.
		/// </summary>
		/// <param name='name'>
		/// The name of the menu entry.
		/// </param>
		public MenuEntry(string name)
            :base( name )
		{
            this.ImagePath = "";
            this.ImageToolTip = "";
            this.ImageWidth = this.ImageHeight = 0;
            this.MinimumNumberOfColumns = 0;
			this.menuComponents = new List<MenuComponent>();
		}

		/// <summary>
		/// Gets the menu entries.
		/// </summary>
		/// <value>
		/// The menu entries, as a <see cref="MenuEntry"/> collection.
		/// </value>
		public ReadOnlyCollection<MenuComponent> MenuEntries {
			get {
				return new ReadOnlyCollection<MenuComponent>(
							this.menuComponents );
			}
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
                    throw new ArgumentOutOfRangeException( "MenuEntry.ImageHight should be >= 0" );
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

		public override void ToXml(XmlTextWriter doc)
		{
			doc.WriteStartElement( TagName );

            // Name = "m1"
			doc.WriteStartAttribute( EtqName );
			doc.WriteString( this.Name );
			doc.WriteEndAttribute();

            if ( this.ImagePath.Length > 0 ) {
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

                if ( this.ImageToolTip.Length > 0 ) {
                    // ImageToolTip = "help"
                    doc.WriteStartAttribute( EtqImageToolTip );
                    doc.WriteString( this.ImageToolTip );
                    doc.WriteEndAttribute();
                }
            }

			foreach (MenuComponent mc in this.menuComponents) {
				mc.ToXml( doc );
			}

			doc.WriteEndElement();
		}

		private List<MenuComponent> menuComponents;
        private string imagePath;
        private string imageToolTip;
        private int imageWidth;
        private int imageHeight;
        private int minimumNumberOfColumns;
	}
}

