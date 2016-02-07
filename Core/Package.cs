using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Collections.Generic; 


namespace RWABuilder.Core {
	/// <summary>
	/// This is a package, the class responsible of managing
	/// Zip files which include the menu design and the resources,
	/// i.e., PDF's and images.
	/// </summary>
	public class Package {
		// Directories inside the zip
		public const string ZipPdfDir = "Pdf/";
		public const string ZipGrfDir = "Graph/";
		public const string ZipAppsDir = "Applications/";
		public const string ZipSrcDir = "Src/";
		public const string ZipWinBinDir = "WinBin/";

		// Labels for manifest file
		public const string ManifestFileName = "manifest";
		public const string EtqName = "Name";
		public const string EtqApp = "App";
		public const string EtqTime = "Time";
		public const string EtqUUID = "UUID";
		public const string EtqPDF = "Pdf";
		public const string EtqGRF = "Grf";
		public const string EtqWinBin = "WinBin";
		public const string EtqSrc = "Src";

		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Core.Packager"/> class.
		/// </summary>
		/// <param name="m">A MenuDesign object.</param>
		public Package(MenuDesign m)
			: this()
		{
			this.Menu = m;
		}

		public Package()
		{
			this.Menu = new MenuDesign();
			this.grfFiles = new List<string>();
			this.pdfFiles = new List<string>();
			this.otherFiles = new List<string>();
			this.UUID = Guid.NewGuid();
			this.PathToDir = LocalStorageManager.CreateTempFolder();
		}

		/// <summary>
		/// Package this rwapp.
		/// </summary>
		public void Pack(string nf)
		{
			Trace.WriteLine( DateTime.Now + ": Packaging: " + nf );
			Trace.Indent();

			// Create resource lists
			this.pdfFiles.Clear();
			this.grfFiles.Clear();
			this.pdfFiles.AddRange( this.Menu.GetPDFNameList() );
			this.grfFiles.AddRange( this.Menu.GetGRFNameList() );

			try {
				using ( FileStream f = new FileStream( nf, FileMode.Create ) ) {
					using( var zip = new ZipArchive( f, ZipArchiveMode.Create, true, Encoding.UTF8 ) )
					{
						if ( this.Menu.SourceCodePath.Length > 0 ) {
							zip.CreateEntryFromFile( this.Menu.SourceCodePath,
													  ZipSrcDir + Path.GetFileName( this.Menu.SourceCodePath ) );
						}

						if ( this.Menu.WindowsBinariesPath.Length > 0 ) {
							zip.CreateEntryFromFile( this.Menu.WindowsBinariesPath,
								ZipWinBinDir + Path.GetFileName( this.Menu.WindowsBinariesPath ) );
						}

						// Insert each pdf file in the zip
						DumpResFilesToZip( zip, ZipPdfDir, this.pdfFiles );

						// Insert each graphic file in the zip
						DumpResFilesToZip( zip, ZipGrfDir, this.grfFiles );

						// Insert fixed assets
						DumpMenuFileToZip( this.Menu, zip );
						DumpManifestToZip( this, zip );
					}
				}
			} catch(ArgumentException exc) {
				Trace.WriteLine( DateTime.Now + ": ERROR: locating file: " + exc.Message );
				throw new IOException( "locating file: " + exc.Message );
			} catch(FileNotFoundException exc) {
				Trace.WriteLine( DateTime.Now + ": ERROR: file not found: " + exc.FileName );
				throw new IOException( "file not found: " + exc.FileName );
			} catch(IOException exc) {
				Trace.WriteLine( DateTime.Now + ": ERROR input/output: " + exc.Message );
				throw;
			}

			Trace.WriteLine( DateTime.Now + ": Packaging finished for: " + Menu.Name );
			Trace.Unindent();
			return;
		}

		/// <summary>
		/// Creates and inserts the manifest into the zip file.
		/// </summary>
		/// <param name="zip">The ZipArchive object.</param>
		private static void DumpManifestToZip(Package package, ZipArchive zip)
		{
			var manifestFile = zip.CreateEntry( ManifestFileName );

			using (var entryStream = manifestFile.Open()) {
				using (var streamWriter = new StreamWriter( entryStream )) {
					streamWriter.WriteLine( "# " + AppInfo.Name );
					streamWriter.WriteLine( "# RWizard packaged app" );
					streamWriter.WriteLine( EtqName + ": " + package.Menu.Name );
					streamWriter.WriteLine( EtqApp + ": " + ZipAppsDir + package.Menu.Name + "." + AppInfo.FileExtension );
					streamWriter.WriteLine( EtqUUID + ": " + package.UUID );
					streamWriter.WriteLine( EtqTime + ": " + DateTime.Now.ToString( @"yyyy-MM-dd\THH:mm:sszzz" ) );

					if ( package.Menu.SourceCodePath.Length > 0 ) {
						streamWriter.WriteLine( EtqSrc + ": " + ZipSrcDir + Path.GetFileName( package.Menu.SourceCodePath ) );
					}

					if ( package.Menu.WindowsBinariesPath.Length > 0 ) {
						streamWriter.WriteLine( EtqWinBin + ": " + ZipWinBinDir + Path.GetFileName( package.Menu.WindowsBinariesPath ) );
					}

					foreach ( string file in package.pdfFiles ) {
						streamWriter.WriteLine( EtqPDF + ": " + ZipPdfDir + Path.GetFileName( file ) );	
					}

					foreach ( string file in package.grfFiles ) {
						streamWriter.WriteLine( EtqGRF + ": " + ZipGrfDir + Path.GetFileName( file ) );	
					}

					streamWriter.Flush();
				}
			}

			return;
		}

		/// <summary>
		/// Inserts the menu file.
		/// This copies the XML of the design menu into the zip.
		/// </summary>
		/// <param name="zip">The ZipArchive object.</param>
		private static void DumpMenuFileToZip(MenuDesign menu, ZipArchive zip)
		{
			var menuFile = zip.CreateEntry( ZipAppsDir + menu.Name + "." + AppInfo.FileExtension );

			using (var entryStream = menuFile.Open()) {
				using (var streamWriter = new StreamWriter( entryStream )) {
					var settings = new XmlWriterSettings() {
						Encoding = Encoding.UTF8,
						Indent = true
					};
					var xmlDocWriter = XmlWriter.Create( streamWriter, settings );

					// Write the document's XML
					xmlDocWriter.WriteStartDocument();
					menu.ToXml( xmlDocWriter );
					xmlDocWriter.WriteEndDocument();
					xmlDocWriter.Close();
					xmlDocWriter.Dispose();
				}
			}

			return;
		}

		/// <summary>
		/// Inserts various files.
		/// </summary>
		/// <param name="zip">The ZipArchive object</param>
		/// <param name="targetDir">The target dir.</param>
		/// <param name="fileNames">The file names, as a IList<string> collection.</param>
		private static void DumpResFilesToZip(ZipArchive zip, string targetDir, IList<string> fileNames)
		{
			for (int i = 0; i < fileNames.Count; ++i) {
				string fileName = fileNames[ i ];

				if ( File.Exists( fileName ) ) {
					Trace.WriteLine(
						String.Format(
							"{0}: Inserting file '{1}' in zip at {2}",
							DateTime.Now, fileName, targetDir
						) );
					
					zip.CreateEntryFromFile( fileName, targetDir + Path.GetFileName( fileName ) );
				} else {
					fileNames.RemoveAt( i );
					--i;
					Trace.WriteLine(
						String.Format(
							"{0}: Warning, missing file '{1}'",
							DateTime.Now, fileName
						) );
				}
			}
		}

		/// <summary>
		/// Unpackages the specified rwa file.
		/// </summary>
		/// <param name="pkgFile">The path to the rwa file, as a string.</param>
		/// <returns>A packager with all the info, or null if unable to unpackage</returns>
		public static Package Unpack(string pkgFile)
		{
			Package toret = null;
			string destDir = "";
			Trace.WriteLine( DateTime.Now + ": Start unpacking of: " + pkgFile );
			Trace.Indent();

			try {
				destDir = LocalStorageManager.CreateTempFolder();
				ZipFile.ExtractToDirectory( pkgFile, destDir );

				toret = BuildFromDir( destDir );
			} catch(IOException exc) {
				string msg = "I/O error unpackaging: " + pkgFile + ":\n" + exc.Message;
				Trace.WriteLine( DateTime.Now + ": " + msg );
				throw new IOException( msg );
			} catch(UnauthorizedAccessException exc) {
				string msg = "no access possible to: " + pkgFile + ": " + exc.Message;
				Trace.WriteLine( DateTime.Now + ": " + msg );
				throw new IOException( msg );
			} catch(NotSupportedException exc) {
				string msg = "invalid format: " + exc.Message;
				Trace.WriteLine( DateTime.Now + ": " + msg );
				throw new IOException( msg );
			} catch(InvalidDataException exc) {
				string msg = "invalid data in: " + pkgFile + ": " + exc.Message;
				Trace.WriteLine( DateTime.Now + ": " + msg );
				throw new IOException( msg );
			}

			Trace.Unindent();
			Trace.WriteLine( 
				string.Format( "{0}: Finished unpacking of '{1}' to '{2}'",
					DateTime.Now, pkgFile, destDir ) );
			return toret;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="RWABuilder.Core.Package"/> class.
		/// Specifically, it creates a package from a directory which sould have all the files
		/// and subdirectories.
		/// </summary>
		/// <param name="destDir">The dir the package was unpacked.</param>
		public static Package BuildFromDir(string dir) {
			var toret = new Package();

			toret.PathToDir = dir;
			ParseManifestFor( dir, toret );
			toret.Menu.FindResourceFiles( dir );
			toret.CheckSanity();

			return toret;
		}

		/// <summary>
		/// Checks the sanity of the package.
		/// The should be a manifest, all files in the manifest should exist...
		/// </summary>
		private void CheckSanity()
		{
			// Check there is an application menu
			if ( this.Menu == null
			  || this.Menu.Root == null )
			{
				throw new ArgumentException( "Empty menu design" );
			}

			// Check there is a name
			if ( this.Menu.Name.Length == 0 ) {
				this.Menu.Name = this.Menu.Root.Name;
			}

			// Check there is a valid UUID
			if ( this.UUID == Guid.Empty ) {
				throw new ArgumentException( "Package without valid UUID" );
			}

			// Check that all files exist
			foreach(string path in this.ResourceFiles) {
				if ( !File.Exists( path ) ) {
					throw new IOException( "Path not found: " + path );
				}
			}

			return;
		}

		/// <summary>
		/// Parses the manifest for the given package.
		/// </summary>
		/// <param name="package">The package, as a Package object.</param>
		private static void ParseManifestFor(string dir, Package package)
		{
			// Read file
			using( var manifestFile = new StreamReader( Path.Combine( dir, ManifestFileName ) ) ) {
				string line = manifestFile.ReadLine();
				string[] parts;
				string path;

				while( line != null ) {
					line = line.Trim();

					if ( line[ 0 ] != '#' ) {
						// The application itself (menu design in xml)
						if ( line.ToUpper().StartsWith( EtqApp.ToUpper() ) ) {
							parts = line.Split( new char[]{ ':' } );
							path = Path.Combine( dir, parts[ 1 ].Trim() );

							if ( parts.Length == 2 ) {
								package.Menu = MenuDesign.LoadFromFile( path );
							} else {
								throw new ArgumentException( "parsing manifest: application entry erroneous" );
							}
						}
						else
						// The name of the package
						if ( line.ToUpper().StartsWith( EtqName.ToUpper() ) ) {
							parts = line.Split( new char[]{ ':' } );
							
							if ( parts.Length == 2 ) {
								package.Menu.Name = parts[ 1 ].Trim();
							} else {
								throw new ArgumentException( "parsing manifest: name entry erroneous" );
							}
						}
						else
						// The UUID of the package
						if ( line.ToUpper().StartsWith( EtqUUID.ToUpper() ) ) {
							parts = line.Split( new char[]{ ':' } );

							if ( parts.Length == 2 ) {
								package.UUID = Guid.Parse( parts[ 1 ].Trim() );
							} else {
								throw new ArgumentException( "parsing manifest: uuid entry erroneous" );
							}
						}
						else
						// A PDF file
						if ( line.ToUpper().StartsWith( EtqPDF.ToUpper() ) ) {
							parts = line.Split( new char[]{ ':' } );
							path = Path.Combine( dir, parts[ 1 ].Trim() );

							if ( parts.Length == 2 ) {
								package.AddPdfFile( path );
							} else {
								throw new ArgumentException( "parsing manifest: pdf entry erroneous" );
							}
						}
						else
						// A GRF file
						if ( line.ToUpper().StartsWith( EtqGRF.ToUpper() ) ) {
							parts = line.Split( new char[]{ ':' } );
							path = Path.Combine( dir, parts[ 1 ].Trim() );

							if ( parts.Length == 2 ) {
								package.AddGrfFile( path );
							} else {
								throw new ArgumentException( "parsing manifest: grf entry erroneous" );
							}
						}
						else
						// A source targ.gz file
						if ( line.ToUpper().StartsWith( EtqSrc.ToUpper() ) ) {
							parts = line.Split( new char[]{ ':' } );

							if ( parts.Length == 2 ) {
								package.Menu.SourceCodePath = Path.Combine( dir, parts[ 1 ].Trim() );
								package.AddSrcFile( package.Menu.SourceCodePath );
							} else {
								throw new ArgumentException( "parsing manifest: src entry erroneous" );
							}
						}
						else
						// A windows binary file
						if ( line.ToUpper().StartsWith( EtqWinBin.ToUpper() ) ) {
							parts = line.Split( new char[]{ ':' } );

							if ( parts.Length == 2 ) {
								package.Menu.WindowsBinariesPath = Path.Combine( dir, parts[ 1 ].Trim() );
								package.AddWinBinFile( package.Menu.WindowsBinariesPath );
							} else {
								throw new ArgumentException( "parsing manifest: win-bin entry erroneous" );
							}
						}
					}

					line = manifestFile.ReadLine();
				}
			}

			return;
		}

		/// <summary>
		/// Adds a given pdf file.
		/// </summary>
		/// <param name="path">The path to the pdf file, as a string.</param>
		public void AddPdfFile(string path) {
			this.pdfFiles.Add( path );
		}

		/// <summary>
		/// Adds a given graphic file.
		/// </summary>
		/// <param name="path">The path to the graphic file, as a string.</param>
		public void AddGrfFile(string path) {
			this.grfFiles.Add( path );
		}

		public void AddSrcFile(string path) {
			this.otherFiles.Add( path );
		}

		public void AddWinBinFile(string path) {
			this.otherFiles.Add( path );
		}

		/// <summary>
		/// Gets the UUI associated with this package.
		/// </summary>
		/// <value>The UUID, as a string.</value>
		public Guid UUID {
			get; private set;
		}

		/// <summary>
		/// Gets the resource files that were used last time.
		/// Does only have items after a call to Package() or Unpackage()
		/// </summary>
		/// <value>The resource files, as an array.</value>
        public string[] ResourceFiles {
			get {
				List<string> toret = new List<string>();

				toret.AddRange( this.pdfFiles );
				toret.AddRange( this.grfFiles );
				toret.AddRange( this.otherFiles );
				return toret.ToArray();
			}
		}

		/// <summary>
		/// Gets the pdf files that were inserted in the zip.
		/// Does only have items after a call to Package() or Unpackage()
		/// </summary>
		/// <value>The pdf files, as an array of string.</value>
		public string[] PdfFiles {
			get {
				return this.pdfFiles.ToArray();
			}
		}

		/// <summary>
		/// Gets the grf files that were inserted in the zip.
		/// Does only have items after a call to Package()
		/// </summary>
		/// <value>The grf files, as an array of string.</value>
		public string[] GrfFiles {
			get {
				return this.grfFiles.ToArray();
			}
		}

		/// <summary>
		/// Gets the design menu to be saved, or recently unpackaged.
		/// </summary>
		/// <value>The MenuDesign object.</value>
		public MenuDesign Menu {
			get; private set;
		}

		/// <summary>
		/// It holds the directory
		/// the files of this package are stored.
		/// A directory is assigned from the beginning, but it just have files
		/// stored after Unpack().
		/// </summary>
		/// <value>The path to dir, as a string.</value>
		public string PathToDir {
			get; private set;
		}

		private List<string> pdfFiles;
		private List<string> grfFiles;
		private List<string> otherFiles;
	}
}
