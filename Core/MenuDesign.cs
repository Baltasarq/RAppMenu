using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Core {
	public class MenuDesign {
        /// <summary>
        /// Initializes a new instance of the <see cref="RAppMenu.Core.MenuDesign"/> class.
        /// Creates an empty menu, with a Root node.
        /// </summary>
		public MenuDesign()
		{
            this.root = new RootMenu( this );
            this.NeedsSave = true;
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

			xmlDocWriter.WriteStartDocument();

			this.Root.ToXml( xmlDocWriter );

			// Produce the file
			xmlDocWriter.WriteEndDocument();
			xmlDocWriter.Close();

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

            this.Root.Name = Path.GetFileNameWithoutExtension( fileNameDest );
            this.NeedsSave = false;

            Trace.Unindent();
			return;
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
        /// Loads the document from a file.
        /// </summary>
        /// <returns>The <see cref="MenuDesign"/> loaded from file.</returns>
        /// <param name="fileName">File name.</param>
        public static MenuDesign LoadFromFile(string fileName)
        {
            Trace.WriteLine( DateTime.Now + ": read XML from " + fileName );
            Trace.Indent();

            var reader = new XmlReader( fileName );

            reader.Read();
            Trace.Unindent();
            return reader.MenuDesign;
        }

		public override string ToString()
		{
            return string.Format( "[MenuDesign: Root={0}]", Root.ToString() );
		}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RAppMenu.Core.MenuDesign"/> needs save.
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

		private RootMenu root;
	}
}
