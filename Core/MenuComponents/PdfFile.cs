using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace RWABuilder.Core.MenuComponents {
    public class PdfFile: MenuComponent {
        public const string TagName = "PDF";
        public const string EtqName = "Name";

        public PdfFile(string fileName, Menu parent)
            : base( fileName, parent )
        {
        }

        /// <summary>
        /// Gets the file path to the PDF file.
        /// </summary>
        /// <value>The file path, as a string.</value>
        /// <seealso cref="RWABuilder.Core.MenuComponent.Name"/>
        public string FileName {
            get {
                return this.Name;
            }
        }

		/// <summary>
		/// Copies this Argument.
		/// </summary>
		/// <param name="newParent">
		/// The <see cref="Function"/> which will be the parent of the copy.
		/// </param>
		/// <returns>
		/// A new <see cref="FunctionArgument"/>, which is an exact copy of this one.
		/// </returns>
		public override MenuComponent Copy(MenuComponent newParent)
		{
			if ( !( newParent is RegularMenu ) ) {
				throw new ArgumentException( "parent of new PdfFile should be Menu" );
			}

			return new PdfFile( this.FileName, (RegularMenu) newParent );
		}

		public override string ToString()
		{
			return string.Format( "[PdfFile: FileName={0}]", FileName );
		}

		/// <summary>
		/// Gets just the name and the extension of the file.
		/// </summary>
		/// <returns>The file name and extension, as a string.</returns>
		public string GetFileName() {
			return Path.GetFileName( this.FileName );
		}

		/// <summary>
		/// Gets the file's full path.
		/// Warning: The file is not checked to verify it is a PDF file.
		/// </summary>
		/// <returns>The file full path, as a string.</returns>
		/// <seealso cref="FileName"/>
		public string GetFileFullPath()
		{
			return GetFileFullPathOf( this.FileName );
		}

        public override void ToXml(XmlWriter doc)
        {
            Trace.WriteLine( "PdfFile.ToXml: " + this.ToString() );
            doc.WriteStartElement( TagName );

            doc.WriteStartAttribute( EtqName );
            doc.WriteString( this.FileName );
            doc.WriteEndAttribute();

            doc.WriteEndElement();
        }

		public static PdfFile FromXml(XmlNode node, Menu parent)
		{
            Trace.WriteLine( "PdfFile.FromXml: " + node.AsString() );

			var toret = new PdfFile( "tempFileName.test", parent );

			// Name = "m1"
			var nameAttr = (XmlAttribute) node.Attributes.GetNamedItemIgnoreCase( EtqName );
			toret.Name = nameAttr.InnerText;

			return toret;
		}

		/// <summary>
		/// Gets the full path of the given file.
		/// Warning: The file is not checked to verify it is a PDF file.
		/// </summary>
		/// <param name="path">The path to the PDF file.</param>
		/// <returns>The file full path, as a string.</returns>
		public static string GetFileFullPathOf(string path)
		{
			if ( Path.GetDirectoryName( path ) == string.Empty ) {
				path = Path.Combine( LocalStorageManager.PdfFolder, path );
			}

			return path;
		}
    }
}

