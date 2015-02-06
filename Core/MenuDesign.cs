using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text;

using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Core {
    /// <summary>
    /// Represents applications, specifically the menu's design.
    /// </summary>
	public partial class MenuDesign {
        public const string DefaultEmail = "joh@doe.com";
		public const string TagName = "RWApp";
		public const string EtqEmail = "AuthorEmail";
		public const string EtqDate = "Date";

        /// <summary>
        /// Initializes a new instance of the <see cref="RWABuilder.Core.MenuDesign"/> class.
        /// Creates an empty menu, with a Root node.
        /// </summary>
		public MenuDesign()
		{
            this.root = new RootMenu( this );
            this.pdfList = new PDFList( this );
            this.AuthorEmail = DefaultEmail;
            this.Date = DateTime.Now;
            this.NeedsSave = true;
		}

		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		/// <value>The name of the application</value>
		public string Name {
			get {
				return this.Root.Name;
			}
			set {
				this.Root.Name = value.Trim();
			}
		}

		/// <summary>
		/// Gets or sets the author email.
		/// </summary>
		/// <value>The author email, as a string.</value>
		public string AuthorEmail {
			get {
				return this.authorEmail;
			}
			set {
                if ( string.IsNullOrWhiteSpace( value )
                  || value.IndexOf( '@' ) < 0 )
                {
                    throw new ArgumentException( "invalid email: " + value );
                }

				this.authorEmail = value.Trim();
			}
		}

		/// <summary>
		/// Gets the root menu component.
		/// </summary>
		/// <value>
		/// The menu root component, as a <see cref="RootMenu"/> object.
		/// </value>
		public RootMenu Root {
			get {
				return this.root;
			}
		}

		/// <summary>
		/// Gets or sets the creation date.
		/// </summary>
		/// <value>The creation date, as a DateTime.</value>
		public DateTime Date {
			get; set;
		}

		public void ToXml(XmlTextWriter doc)
		{
			doc.WriteStartElement( TagName );

			// Email = "jbgarcia@uvigo.es"
			doc.WriteStartAttribute( EtqEmail );
			doc.WriteString( this.AuthorEmail );
			doc.WriteEndAttribute();

			// Date = "2015-01-06"
			doc.WriteStartAttribute( EtqDate );
			doc.WriteString( this.Date.ToString( "yyyy-MM-dd" ) );
			doc.WriteEndAttribute();

			this.Root.ToXml( doc );
			doc.WriteEndElement();
		}

		public void FromXml(XmlNode node)
		{
			DateTime date;
			XmlNode attrDate = node.Attributes.GetNamedItemIgnoreCase( EtqDate );
			XmlNode attrAuthorEmail = node.Attributes.GetNamedItemIgnoreCase( EtqEmail );

			// Attributes
			if ( attrDate != null ) {
				if ( !DateTime.TryParseExact(
									attrDate.InnerText,
									"yyyy-MM-dd",
									System.Globalization.DateTimeFormatInfo.InvariantInfo,
									System.Globalization.DateTimeStyles.None,
									out date ) )
				{
					throw new XmlException( "Date format" );
				}

				this.Date = date;
			}

			if ( attrAuthorEmail != null ) {
				this.AuthorEmail = attrAuthorEmail.InnerText;
			}

			// Root menu
			if ( node.HasChildNodes
			  && node.ChildNodes.Count == 1 )
			{
				XmlNode menuNode = node.FirstChild;

				if ( !menuNode.Name.Equals( RootMenu.TagName, StringComparison.OrdinalIgnoreCase ) )
				{
					throw new XmlException( "root element should be: " + TagName );
				}

				this.Root.FromXml( menuNode );
			} else {
				throw new XmlException( TagName + " should have exactly one child." );
			}

			return;
		}

		/// <summary>
		/// Saves the info in the document to a given file.
		/// </summary>
        /// <param name='fileNameDest'>
		/// The file name, as a string.
		/// </param>
		public void SaveToFile(string fileNameDest)
		{
            Trace.WriteLine( DateTime.Now + ": MenuDesign.SaveToFile: " + fileNameDest );
            Trace.Indent();

			string fileNameOrg = Path.GetTempFileName();
			var xmlDocWriter = new XmlTextWriter( fileNameOrg, Encoding.UTF8 );

			// Get default values
			if ( this.Date == default(DateTime) ) {
				this.Date = DateTime.Now.Date;
			}

			if ( string.IsNullOrWhiteSpace( this.AuthorEmail ) ) {
                this.AuthorEmail = DefaultEmail;
			}

			// Write the document's XML
			xmlDocWriter.WriteStartDocument();
			this.ToXml( xmlDocWriter );
			xmlDocWriter.WriteEndDocument();
			xmlDocWriter.Close();

			// Save the file
			try {
				if ( File.Exists( fileNameDest ) ) {
					File.Delete( fileNameDest );
				}

				File.Move( fileNameOrg, fileNameDest );
			}
			catch(IOException)
			{
				File.Copy( fileNameOrg, fileNameDest, true );
			}

            this.NeedsSave = false;
            Trace.Unindent();
			return;
		}

        /// <summary>
        /// Loads the document from a file.
        /// </summary>
        /// <returns>The <see cref="MenuDesign"/> loaded from file.</returns>
        /// <param name="fileName">File name.</param>
        public static MenuDesign LoadFromFile(string fileName)
        {
            Trace.WriteLine( DateTime.Now + ": read XML from " + fileName );
            Trace.Indent();

			var toret = new MenuDesign();

			// Open the file
			var docXml = new XmlDocument();
			docXml.Load( fileName );

			if ( !docXml.DocumentElement.Name.Equals( TagName, StringComparison.OrdinalIgnoreCase ) )
			{
				throw new XmlException( "root element should be: " + TagName );
			}

			// Read the data
			toret.FromXml( docXml.DocumentElement );
			toret.NeedsSave = false;

			if ( string.IsNullOrWhiteSpace( toret.Name ) ) {
				throw new XmlException( "missing name of application" );
			}

            Trace.Unindent();
            return toret;
        }

        /// <summary>
        /// Gets the version of the menu design.
        /// </summary>
        /// <returns>
        /// The version, as a string, in the format <name>-<year><month><day>
        /// For example: Test-20150206
        /// </returns>
		public string GetVersion()
		{
			return string.Format( "{0}-{1}", this.Name, this.Date.ToString( "yyyyyMMdd" ) );
		}

		public override string ToString()
		{
			return string.Format( "[MenuDesign: Name: {0} Date: {1} AuthorEmail: {2} Root: {3}]",
			                     Name, Date, AuthorEmail, Root.ToString() );
		}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RWABuilder.Core.MenuDesign"/> needs save.
        /// Honors the NeedsSave property in Root, a <see cref="RootMenu"/>
        /// </summary>
        /// <value><c>true</c> if needs save; otherwise, <c>false</c>.</value>
        public bool NeedsSave {
            get {
                return this.Root.NeedsSave;
            }
            set {
                this.Root.NeedsSave = value;
            }
        }

        /// <summary>
        /// Gets the PDF list.
        /// </summary>
        /// <value>A vector of string.</value>
        public string[] PDFNameList {
            get {
                var toret = new string[ this.pdfList.Count ];

				for(int i = 0; i < this.pdfList.Count; ++i) {
                    toret[ i ] = this.pdfList[ i ].Name;
                }

                return toret;
            }
        }

        /// <summary>
        /// Gets the PDF list, for internal uses.
        /// Menus must update the pdf list, after all.
        /// </summary>
        /// <returns>The <see cref="PDFList"/>.</returns>
        internal PDFList GetPDFList()
        {
            return this.pdfList;
        }

		private RootMenu root;
		private string authorEmail;
        private PDFList pdfList;
	}
}
