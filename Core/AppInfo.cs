using System;

namespace RAppMenu.Core {
	public static class AppInfo {
		public const string Name = "RAppMenu";
		public const string Web = "http://www.ipez.es/rwizard/";
		public const string Version = "1.0 20141020";
        public const string FileExtension = "xml";

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
	}
}

