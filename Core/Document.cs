using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Core {
	public class Document {
		public Document()
		{
			this.root = new RootMenuEntry();
		}

		/// <summary>
		/// Saves the info in the document to a given file.
		/// </summary>
		/// <param name='fileName'>
		/// The file name, as a string.
		/// </param>
		public void SaveToFile(string fileNameDest)
		{
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
			return;
		}

		/// <summary>
		/// Gets the menu components.
		/// </summary>
		/// <value>
		/// The menu entries, as a <see cref="MenuEntry"/> collection.
		/// </value>
		public RootMenuEntry Root {
			get {
				return this.root;
			}
		}

		private RootMenuEntry root;
	}
}
