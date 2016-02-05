using System;
using System.IO;
using System.Diagnostics;

namespace RWABuilder.Core {
	/// <summary>
	/// Local storage manager.
	/// This class provides the management of configuration files,
	/// temporary files, and other configuration needs in general.
	/// </summary>
	public static class LocalStorageManager {
		public const string LogFile = AppInfo.Name + ".errors.log";

		public const string RegistryPath = "Software\\RWizard";
		public const string RegistryVersionKey = "Version";
		public const string RegistryInstallPathKey = "InstallPath";

		/// <summary>
		/// Gets the application configuration folder ready to work.
		/// </summary>
		/// <returns>The app config folder, as a string.</returns>
		public static string PrepareAppConfigFolder()
		{
			string toret = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
			toret = Path.Combine( toret, AppInfo.Name );

			Directory.CreateDirectory( toret );
			return toret;
		}

		/// <summary>
		/// Returns the full path to an unexisting temporary directory.
		/// </summary>
		/// <returns>The full path to the temp folder, as a string.</returns>
		public static string CreateTempFolder()
		{
			string toret;

			Trace.WriteLine( DateTime.Now + ": Creating temporary folder"  );

			do {
				toret = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() );
			} while( File.Exists( toret )
				  || Directory.Exists( toret ) );

			Trace.WriteLine( DateTime.Now + ": Created temporary folder: " + toret  );
			return toret;
		}

		/// <summary>
		/// Gets the default applications folder.
		/// </summary>
		/// <value>The default applications folder, as a string.</value>
		public static string DefaultApplicationsFolder {
			get {
				GetMainAppInfo();
				return pathToApplications = Path.Combine( PathToMainApp, "ApplicationsV2" );
			}
		}

		/// <summary>
		/// Gets or sets the applications folder.
		/// By default, it is "applications".
		/// </summary>
		/// <value>The applications folder, as a string.</value>
		public static string ApplicationsFolder {
			get {
				if ( string.IsNullOrWhiteSpace( pathToApplications ) ) {
					pathToApplications = DefaultApplicationsFolder;
				}

				return pathToApplications;
			}
			set {
				pathToApplications = ( value ?? "" ).Trim();
			}
		}

		/// <summary>
		/// Gets the default applications folder.
		/// </summary>
		/// <value>The default applications folder, as a string.</value>
		public static string DefaultPdfFolder {
			get {
				GetMainAppInfo();
				return Path.Combine( PathToMainApp, "Pdf" );
			}
		}

		/// <summary>
		/// Gets or sets the pdf folder.
		/// </summary>
		/// <value>The pdf folder path, as a string.</value>
		public static string PdfFolder {
			get {
				if ( string.IsNullOrWhiteSpace( pathToPDFs ) ) {
					pathToPDFs = DefaultPdfFolder;
				}

				return pathToPDFs;
			}
			set {
				pathToPDFs = ( value ?? "" ).Trim();
			}
		}

		public static string DefaultGraphsFolder {
			get {
				GetMainAppInfo();
				return Path.Combine( PathToMainApp, "Graphs" );
			}
		}

		/// <summary>
		/// Gets or sets the graphs folder.
		/// </summary>
		/// <value>The graphs folder, as a string.</value>
		public static string GraphsFolder {
			get {
				if ( string.IsNullOrWhiteSpace( pathToGraphics ) ) {
					pathToGraphics = DefaultGraphsFolder;
				}

				return pathToGraphics;
			}
			set {
				pathToGraphics = ( value ?? "" ).Trim();
			}
		}

		/// <summary>
		/// Gets the path to main app.
		/// </summary>
		/// <value>The path to main app, as a string.</value>
		public static string PathToMainApp {
			get {
				return pathToMainApp;
			}
		}

		/// <summary>
		/// Gets the main app version info.
		/// </summary>
		/// <value>The main app version, as a string.</value>
		public static string MainAppVersionInfo {
			get {
				return mainAppVersion;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the main app was located.
		/// </summary>
		/// <value><c>true</c> if main app located; otherwise, <c>false</c>.</value>
		public static Boolean MainAppLocated {
			get {
				return mainAppLocated;
			}
		}

		/// <summary>
		/// Prepares the log for all events in the app.
		/// </summary>
		public static void BuildLog()
		{
			string logPath = Path.Combine(
				PrepareAppConfigFolder(),
				LogFile );

			Trace.Listeners.Add( new TextWriterTraceListener(
				new FileStream( logPath, FileMode.Create )
			) );

			Trace.AutoFlush = true;

			Trace.Write( AppInfo.Name );
			Trace.Write( ' ' );
			Trace.Write( AppInfo.Version );
			Trace.Write( ' ' );
			Trace.WriteLine( DateTime.Now );
			Trace.WriteLine( "================================================" );
		}

		/// <summary>
		/// Closes the log, avoiding any data loss.
		/// </summary>
		public static void CloseLog()
		{
			Trace.WriteLine( "================================================" );
			Trace.WriteLine( DateTime.Now + ": finishing...." );

			Trace.Flush();
			Trace.Close();
		}

		/// <summary>
		/// Gets the main app info, suing the registry.
		/// </summary>
		/// <returns>The main app info.</returns>
		public static void GetMainAppInfo()
		// Root: "HKLM"; Subkey: "Software\RWizard"; ValueType: string; ValueName: "InstallPath";
		// Root: "HKLM"; Subkey: "Software\RWizard"; ValueType: string; ValueName: "Version";
		{
			Trace.WriteLine( DateTime.Now + ": " + "Starting registry search..." );

			try {
				var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey( RegistryPath );

				if ( regKey != null ) {
					pathToMainApp = (string) regKey.GetValue( RegistryInstallPathKey );
					mainAppVersion = (string) regKey.GetValue( RegistryVersionKey );
					mainAppLocated = true;

					Trace.WriteLine( DateTime.Now + ": " + "main app "
						+ mainAppVersion + " located at: " + pathToMainApp );
				} else {
					Trace.WriteLine( DateTime.Now + ": " + "main app not installed." );
				}
			} catch(Exception exc)
			{
				Trace.WriteLine( DateTime.Now + ": " + "Error trying to locate main app:" );
				Trace.WriteLine( exc.Message + "\n" + exc.StackTrace );
			}

			Trace.WriteLine( DateTime.Now + ": " + "Finished registry search." );
		}

		private static string mainAppVersion = "";
		private static string pathToMainApp = ".";
		private static bool mainAppLocated = false;
		private static string pathToApplications = "";
		private static string pathToPDFs = "";
		private static string pathToGraphics = "";
	}
}

