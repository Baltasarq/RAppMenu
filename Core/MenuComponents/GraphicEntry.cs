using System;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace RWABuilder.Core.MenuComponents {
	public class GraphicEntry: Menu {
		public new const string TagName = "GraphicEntry";
		public const string EtqImagePath = "Image";
        public const string EtqImageToolTip = "ImageTooltip";

		public GraphicEntry(string id, GraphicMenu parent)
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
				return this.Function.Name;
			}
			set {
				value = ( value ?? "" ).Trim();

				if ( base.Name != value ) {
					base.Name = value;
					this.Function.Name = value;
				}

				return;
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
				this.imagePath = ( value ?? "" ).Trim();
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
				this.imageToolTip = ( value ?? "" ).Trim();
            }
        }

		/// <summary>
		/// Gets or sets the function associated with this entry.
		/// </summary>
		/// <value>The <see cref="Function"/>.</value>
		public Function Function {
			get {
				Function toret = null;

				if ( this.MenuComponents.Count > 0 ) {
					toret = (Function) this.MenuComponents[ 0 ];
				}

				return toret;
			}
		}

		/// <summary>
		/// Copies this graphic menu entry.
		/// </summary>
		/// <param name="newParentOrOwner">
		/// The <see cref="GraphicMenu"/> which will be the parent or owner of the copy.
		/// </param>
		/// <returns>
		/// A new <see cref="GraphicMenu"/>, which is an exact copy of this one.
		/// </returns>
		public override MenuComponent Copy(MenuComponent newParent)
		{
			if ( !( newParent is GraphicMenu ) ) {
				throw new ArgumentException( "parent of copies graphic menu entry should be a graphic menu" );
			}

            var toret = new GraphicEntry( this.Name, (GraphicMenu) newParent ) {
				ImagePath = this.ImagePath,
				ImageToolTip = this.ImageToolTip
			};

			this.Function.Copy( toret );
			return toret;
		}

		/// <summary>
		/// Gets the file's full path.
		/// Warning: The file is not checked to verify it is an image file.
		/// </summary>
		/// <returns>The file full path, as a string.</returns>
		/// <seealso cref="ImagePath"/>
		public string GetFileFullPath()
		{
			return GetFileFullPathOf( this.ImagePath );
		}

		/// <summary>
		/// Gets the name of the file, without any path part.
		/// </summary>
		/// <returns>Just the file name and the extension, as a string.</returns>
		public string GetFileName() {
			return Path.GetFileName( this.imagePath );
		}

		public override string ToString()
		{
			return string.Format( "[GraphicEntry: Name={0}, ImagePath={1}, "
				+ "ImageToolTip={2}, Function={3}]",
			    Name, ImagePath, ImageToolTip, Function.ToString() );
		}

		public override void ToXml(XmlWriter doc)
		{
            Trace.WriteLine( "GraphicEntry.ToXml: " + this.ToString() );
            Trace.Indent();

			doc.WriteStartElement( TagName );

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
            Trace.Unindent();
		}

		public static GraphicEntry FromXml(XmlNode node, GraphicMenu menu)
		{
            Trace.WriteLine( "GraphicMenuEntry.ToXml: " + node.AsString() );
            Trace.Indent();

            var toret = new GraphicEntry( TagName, menu );

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

            Trace.Unindent();
            return toret;
		}

		/// <summary>
		/// Looks in contents for the given string. Ignores case.
		/// </summary>
		/// <returns><c>true</c>, if in contents there is txt, <c>false</c> otherwise.</returns>
		/// <param name="txt">Text.</param>
		public override bool LookInContentsFor(string txt)
		{
			bool toret = base.LookInContentsFor( txt );

			if ( !toret ) {
				toret = this.Function.LookInContentsFor( txt );
			}

			return toret;
		}

		/// <summary>
		/// Gets the full path of the given file.
		/// Warning: The file is not checked to verify it is an image file.
		/// </summary>
		/// <param name="path">The path to the image file.</param>
		/// <returns>The file full path, as a string.</returns>
		public static string GetFileFullPathOf(string path)
		{
			if ( Path.GetDirectoryName( path ) == string.Empty ) {
				path = Path.Combine( LocalStorageManager.GraphsFolder, path );
			}

			return path;
		}

		private string imagePath;
        private string imageToolTip;
	}
}

