using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Core {
    /// <summary>
    /// Represents applications, specifically the menu's design.
    /// </summary>
	public partial class MenuDesign {
        public const string DefaultEmail = "";
		public const string TagName = "RWApp";
		public const string EtqEmail = "AuthorEmail";
		public const string EtqDate = "Date";
		public const string EtqSourceCodePath = "SourceCodePath";
		public const string EtqDocsPath = "WindowsBinariesPath";
		public const string EtqReqPacksPath = "RequiredPackagesPath";

        /// <summary>
        /// Initializes a new instance of the <see cref="RWABuilder.Core.MenuDesign"/> class.
        /// Creates an empty menu, with a Root node.
        /// </summary>
		public MenuDesign()
		{
            this.root = new RootMenu( this );
			this.pdfList = new List<PdfFile>();
			this.sourceCodePath = this.windowsBinariesPath = this.requiredPackagesPath = "";
			this.grfMenuList = new List<GraphicMenu>();
            this.AuthorEmail = DefaultEmail;
            this.Date = DateTime.Now;
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
				value = ( value ?? "" ).Trim();

				if ( value != this.Root.Name ) {
					this.Root.Name = value;
				}

				return;
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
				this.authorEmail = ( value ?? "" ).Trim();
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
			get {
				return this.date;
			}
			set {
				if ( value != this.date ) {
					this.date = value;
				}

				return;
			}
		}

		public void ToXml(XmlWriter doc)
		{
			doc.WriteStartElement( TagName );

			// Email = "jbgarcia@uvigo.es"
			if ( this.AuthorEmail.Length > 0 ) {
				doc.WriteStartAttribute( EtqEmail );
				doc.WriteString( this.AuthorEmail );
				doc.WriteEndAttribute();
			}

			// Date = "2015-01-06"
			doc.WriteStartAttribute( EtqDate );
			doc.WriteString( this.Date.ToString( "yyyy-MM-dd" ) );
			doc.WriteEndAttribute();

			// SourceCode = "/path/to/source.zip"
			if ( this.SourceCodePath.Length > 0 ) {
				doc.WriteStartAttribute( EtqSourceCodePath );
				doc.WriteString( Path.GetFileName( this.SourceCodePath ) );
				doc.WriteEndAttribute();
			}

			// Documentation = "/path/to/source.tgz"
			if ( this.WindowsBinariesPath.Length > 0 ) {
				doc.WriteStartAttribute( EtqDocsPath );
				doc.WriteString( Path.GetFileName( this.WindowsBinariesPath ) );
				doc.WriteEndAttribute();
			}

			// RequiredPackages = "/path/to/source.txt"
			if ( this.RequiredPackagesPath.Length > 0 ) {
				doc.WriteStartAttribute( EtqReqPacksPath );
				doc.WriteString( Path.GetFileName( this.RequiredPackagesPath ) );
				doc.WriteEndAttribute();
			}

			this.Root.ToXml( doc );
			doc.WriteEndElement();
		}

		public void FromXml(XmlNode node)
		{
			DateTime date;
			XmlNode attrDate = node.Attributes.GetNamedItemIgnoreCase( EtqDate );
			XmlNode attrAuthorEmail = node.Attributes.GetNamedItemIgnoreCase( EtqEmail );
			XmlNode attrSourceCodePath = node.Attributes.GetNamedItemIgnoreCase( EtqSourceCodePath );
			XmlNode attrDocsCodePath = node.Attributes.GetNamedItemIgnoreCase( EtqDocsPath );
			XmlNode attrReqPacksPath = node.Attributes.GetNamedItemIgnoreCase( EtqReqPacksPath );

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

			if ( attrSourceCodePath != null ) {
				this.SourceCodePath = attrSourceCodePath.InnerText;
			}

			if ( attrDocsCodePath != null ) {
				this.WindowsBinariesPath = attrDocsCodePath.InnerText;
			}

			if ( attrReqPacksPath != null ) {
				this.RequiredPackagesPath = attrReqPacksPath.InnerText;
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
			xmlDocWriter.Formatting = Formatting.Indented;

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
        /// Gets the PDF list of file names.
        /// </summary>
        /// <returns>A string array with the file names.</returns>
		public string[] GetPDFNameList()
        {
			var fileNames = new HashSet<string>();

			foreach(PdfFile pdfFile in this.pdfList) {
				fileNames.Add( pdfFile.GetFileFullPath() );
			}

			var toret = new string[ fileNames.Count ];
			fileNames.CopyTo( toret );
			return toret;
        }

		/// <summary>
		/// Gets the graphics file name list.
		/// </summary>
		/// <returns>The graphics file name list, as a string array.</returns>
		public string[] GetGRFNameList()
		{
			var fileNames = new HashSet<string>();

			foreach (GraphicMenu grfm in this.grfMenuList) {
				foreach (GraphicEntry grfe in grfm.MenuComponents) {
					fileNames.Add( grfe.GetFileFullPath() );
				}
			}

			var toret = new string[ fileNames.Count ];
			fileNames.CopyTo( toret );
			return toret;
		}

		/// <summary>
		/// Gets or sets the windows binaries path.
		/// This is a path to a compressed file.
		/// </summary>
		/// <value>The binaries path, as a string.</value>
		public string WindowsBinariesPath {
			get {
				return this.windowsBinariesPath;
			}
			set {
				this.windowsBinariesPath = ( value ?? "" ).Trim();
			}
		}

		/// <summary>
		/// Gets or sets the source code path.
		/// This is a path to a compressed file.
		/// </summary>
		/// <value>The documents path, as a string.</value>
		public string SourceCodePath {
			get {
				return this.sourceCodePath;
			}
			set {
				this.sourceCodePath = ( value ?? "" ).Trim();
			}
		}

		/// <summary>
		/// Gets or sets the required packages path.
		/// </summary>
		/// <value>The required packages path.</value>
		public string RequiredPackagesPath {
			get {
				return this.requiredPackagesPath;
			}
			set {
				this.requiredPackagesPath = ( value ?? "" ).Trim();
			}
		}

		/// Tries to find all resource files. Reverts their path to the default RWizard's dir
		/// <param name="packagePath">The path the package was unpacked, as a string</param>
		public void FindResourceFiles(string packagePath)
		{
			var mcs = new List<MenuComponent>();

			mcs.Add( this.Root );

			while( mcs.Count > 0 ) {
				MenuComponent mc = mcs[ 0 ];
				var menu = mc as Menu;
				var pdf = mc as PdfFile;
				var grf = mc as GraphicEntry;

				// Add all subnodes to the list
				if ( menu != null ) {
					mcs.AddRange( menu.MenuComponents );
				}

				// Evaluate
				if ( pdf != null ) {
					// This is a PDF file
					if ( !File.Exists( pdf.GetFileFullPath() ) ) {
						// Try in the package dir
						pdf.Name = Path.Combine(
							Path.Combine( packagePath, Package.ZipPdfDir ),
							pdf.GetFileName() );

						// Try in the main app dir
						if ( !File.Exists( pdf.GetFileFullPath() ) ) {
							pdf.Name = Path.Combine(
								LocalStorageManager.DefaultPdfFolder,
								pdf.GetFileName() );
						}
					}
				}
				else
				if ( grf != null ) {
					// This is a graphic file
					if ( !File.Exists( grf.GetFileFullPath() ) ) {
						// Try in the package dir
						grf.ImagePath = Path.Combine(
							Path.Combine( packagePath, Package.ZipGrfDir ),
							grf.GetFileName() );

						// Try in the main app dir
						if ( !File.Exists( grf.GetFileFullPath() ) ) {
							grf.ImagePath = Path.Combine(
								LocalStorageManager.DefaultGraphsFolder,
								grf.GetFileName() );
						}
					}
				}

				mcs.RemoveAt( 0 );
			}

			return;
		}

		internal IList<PdfFile> GetPDFList()
		{
			return this.pdfList;
		}

		internal IList<GraphicMenu> GetGraphicMenuList()
		{
			return this.grfMenuList;
		}

		private RootMenu root;
		private string authorEmail;
		private string sourceCodePath;
		private string windowsBinariesPath;
		private string requiredPackagesPath;
		private DateTime date;
		private List<PdfFile> pdfList;
		private List<GraphicMenu> grfMenuList;
	}
}
