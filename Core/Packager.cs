﻿using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Collections.Generic;

namespace RWABuilder.Core {
	/// <summary>
	/// This is the packager, the class responsible of managing
	/// Zip files which include the menu design and the resources,
	/// i.e., PDF's and images.
	/// </summary>
	public class Packager {
		public const string ManifestFileName = "manifest";
		public const string ZipPdfDir = "Pdf/";
		public const string ZipGrfDir = "Graph/";
		public const string ZipAppsDir = "Applications/";
		public const string ZipSrcDir = "Src/";
		public const string ZipWinBinDir = "WinBin/";

		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Core.Packager"/> class.
		/// </summary>
		/// <param name="m">A MenuDesign object.</param>
		public Packager(MenuDesign m)
		{
			menu = m;
			this.grfFiles = new List<string>();
			this.pdfFiles = new List<string>();
		}

		/// <summary>
		/// Package this rwapp.
		/// </summary>
		public void Package(string nf)
		{
			Trace.WriteLine( DateTime.Now + ": Packaging: " + nf );
			Trace.Indent();

			// Create resource lists
			this.pdfFiles.Clear();
			this.grfFiles.Clear();
			this.pdfFiles.AddRange( menu.GetPDFNameList() );
			this.grfFiles.AddRange( menu.GetGRFNameList() );

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
						this.InsertFiles( zip, ZipPdfDir, this.pdfFiles );

						// Insert each graphic file in the zip
						this.InsertFiles( zip, ZipGrfDir, this.grfFiles );

						// Insert fixed assets
						this.InsertMenuFile( zip );
						this.InsertManifest( zip );
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
		private void InsertManifest(ZipArchive zip)
		{
			var manifestFile = zip.CreateEntry( ManifestFileName );

			using (var entryStream = manifestFile.Open()) {
				using (var streamWriter = new StreamWriter( entryStream )) {
					streamWriter.WriteLine( "# " + AppInfo.Name );
					streamWriter.WriteLine( "# RWizard packaged app" );
					streamWriter.WriteLine( "Name: " + Menu.Name );
					streamWriter.WriteLine( "App: " + ZipAppsDir + Menu.Name + "." + AppInfo.FileExtension );
					streamWriter.WriteLine( "UUID: " + Guid.NewGuid().ToString() );
					streamWriter.WriteLine( "Time: " + DateTime.Now.ToString( @"yyyy-MM-dd\THH:mm:sszzz" ) );

					if ( this.Menu.SourceCodePath.Length > 0 ) {
						streamWriter.WriteLine( "Src: " + ZipSrcDir + Path.GetFileName( this.Menu.SourceCodePath ) );
					}

					if ( this.Menu.WindowsBinariesPath.Length > 0 ) {
						streamWriter.WriteLine( "WinBin: " + ZipWinBinDir + Path.GetFileName( this.Menu.WindowsBinariesPath ) );
					}

					foreach ( string file in this.pdfFiles ) {
						streamWriter.WriteLine( "Pdf: " + ZipPdfDir + Path.GetFileName( file ) );	
					}

					foreach ( string file in this.grfFiles ) {
						streamWriter.WriteLine( "Grf: " + ZipGrfDir + Path.GetFileName( file ) );	
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
		private void InsertMenuFile(ZipArchive zip)
		{
			var menuFile = zip.CreateEntry( ZipAppsDir + Menu.Name + "." + AppInfo.FileExtension );

			using (var entryStream = menuFile.Open()) {
				using (var streamWriter = new StreamWriter( entryStream )) {
					var settings = new XmlWriterSettings() {
						Encoding = Encoding.UTF8,
						Indent = true
					};
					var xmlDocWriter = XmlWriter.Create( streamWriter, settings );

					// Write the document's XML
					xmlDocWriter.WriteStartDocument();
					Menu.ToXml( xmlDocWriter );
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
		private void InsertFiles(ZipArchive zip, string targetDir, IList<string> fileNames)
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
		/// Gets the resource files that were used last time.
		/// Does only have items after a call to Package()
		/// </summary>
		/// <value>The resource files, as an array.</value>
        public string[] ResourceFiles {
			get {
				List<string> toret = new List<string>();

				toret.AddRange( this.pdfFiles );
				toret.AddRange( this.grfFiles );
				return toret.ToArray();
			}
		}

		/// <summary>
		/// Gets the pdf files that were inserted in the zip.
		/// Does only have items after a call to Package()
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
		/// Gets the design menu to be saved.
		/// </summary>
		/// <value>The MenuDesign object.</value>
		public MenuDesign Menu {
			get {
				return this.menu;
			}
		}

		private MenuDesign menu;
		private List<string> pdfFiles;
		private List<string> grfFiles;
	}
}
