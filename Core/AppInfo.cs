using System;
using System.IO;
using System.Diagnostics;

namespace RAppMenu.Core {
	public static class AppInfo {
		public const string Name = "RAppMenu";
		public const string Web = "http://www.ipez.es/rwizard/";
		public const string Version = "1.0.4 20141209";
        public const string FileExtension = "xml";
        public const string LogFile = "rappmenu.errors.log";

		/// <summary>
		/// Gets or sets the applications folder.
		/// By default, it is "applications".
		/// </summary>
		/// <value>The applications folder, as a string.</value>
		public static string ApplicationsFolder {
			get; set;
		}

		/// <summary>
		/// Gets or sets the pdf folder.
		/// </summary>
		/// <value>The pdf folder path, as a string.</value>
		public static string PdfFolder {
			get; set;
		}

		/// <summary>
		/// Gets or sets the graphs folder.
		/// </summary>
		/// <value>The graphs folder, as a string.</value>
		public static string GraphsFolder {
			get; set;
		}

        /// <summary>
        /// Prepares the log for all events in the app.
        /// </summary>
        public static void BuildLog()
        {
            Trace.Listeners.Add( new TextWriterTraceListener( 
                new FileStream( AppInfo.LogFile, FileMode.OpenOrCreate )
            ) );

            Trace.AutoFlush = true;

            Trace.Write( Name );
            Trace.Write( ' ' );
            Trace.Write( Version );
            Trace.Write( ' ' );
            Trace.WriteLine( DateTime.Now );
            Trace.WriteLine( "============================================" );
        }

        /// <summary>
        /// Closes the log, avoiding any data loss.
        /// </summary>
        public static void CloseLog()
        {
            Trace.Flush();
            Trace.Close();
        }
	}
}

