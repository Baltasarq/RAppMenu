using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	public class ImageMenuEntry: Menu {
		public const string EtqImagePath = "Image";
        public const string EtqImageToolTip = "ImageTooltip";

		public ImageMenuEntry(string id, ImagesMenu parent)
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

		public override void Add(MenuComponent mc)
		{
			if ( this.MenuComponents.Count > 0 ) {
				throw new ArgumentException( "an image menu entry can only hold one function" );
			} else {
				base.Add( mc );
			}

			return;
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
			get {
				return (Function) this.MenuComponents[ 0 ];
			}
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

		private string imagePath;
        private string imageToolTip;
	}
}

