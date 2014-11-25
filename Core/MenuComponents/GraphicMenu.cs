using System;
using System.Xml;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a menu composed by images, which launch functions.
	/// </summary>
	public class GraphicMenu: Menu {
        public const string EtqMinimumNumberOfColumns = "MinNumberColumns";
        public const string EtqImageWidth = "ImageWidth";
        public const string EtqImageHeight = "ImageHeight";

		public GraphicMenu(string name, Menu parent)
			:base( name, parent )
		{
            this.ImageWidth = this.ImageHeight = 0;
            this.MinimumNumberOfColumns = 0;
		}

		/// <summary>
		/// Adds a given function to the images menu, which will become a subentry
		/// Note that this superseeds the Add in base class (MenuEntry).
		/// </summary>
		/// <param name="f">The function to add, as a <see cref="Function"/> object.</param>
		public override void Add(MenuComponent ime)
		{
			if ( !( ime is GraphicMenuEntry) ) {
				throw new ArgumentException(
					"argument should be a ImageMenuEntry for ImagesMenu.Add()" );
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

		public override string ToString()
		{
			string toret = base.ToString();

			toret = "[GraphicMenu" + toret.Substring( 5, toret.Length -1 );

			return toret
				+ string.Format( "ImageWidth={0}, ImageHeight={1}, MinimumNumberOfColumns={2}]", ImageWidth, ImageHeight, MinimumNumberOfColumns );
		}

        public override void ToXml(XmlTextWriter doc)
        {
            doc.WriteStartElement( Menu.TagName );

            // Name = "m1"
            doc.WriteStartAttribute( Menu.EtqName );
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

			foreach(GraphicMenuEntry imgmc in this.MenuComponents) {
				imgmc.ToXml( doc );
			}

            doc.WriteEndElement();
        }

		public new static GraphicMenu FromXml(XmlNode node, Menu menu)
		{
			throw new NotImplementedException();
		}

        private int imageWidth;
        private int imageHeight;
        private int minimumNumberOfColumns;
	}
}

