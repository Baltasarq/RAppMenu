using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RAppMenu.Core {
	public class Document {
		public const string TagName = "Menue";

		public Document()
		{
			this.menuComponents = new List<MenuComponent>();
		}

		/// <summary>
		/// Saves the info in the document to a given file.
		/// </summary>
		/// <param name='fileName'>
		/// The file name, as a string.
		/// </param>
		public void SaveToFile(string fileNameDest)
		{
			string fileNameOrg = System.IO.Path.GetTempFileName();
			var xmlDocWriter = new XmlTextWriter( fileNameOrg, Encoding.UTF8 );


			// Create main node
			xmlDocWriter.WriteStartDocument();
			xmlDocWriter.WriteStartElement( TagName );

			// Run all over menu components
			foreach (MenuComponent mc in this.menuComponents) {
				mc.ToXml( xmlDocWriter );
			}

			// Produce the file
			xmlDocWriter.WriteEndElement();
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

			return;
		}

		/// <summary>
		/// Gets the menu components.
		/// </summary>
		/// <value>
		/// The menu entries, as a <see cref="MenuEntry"/> collection.
		/// </value>
		public ReadOnlyCollection<MenuComponent> MenuEntries {
			get {
				return new ReadOnlyCollection<MenuComponent>(
							this.menuComponents );
			}
		}

		private List<MenuComponent> menuComponents;
	}
}
