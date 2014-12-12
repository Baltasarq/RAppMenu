using System;
using System.Xml;
using System.Diagnostics;

namespace RAppMenu.Core.MenuComponents {
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
        /// <seealso cref="RAppMenu.Core.MenuComponent.Name"/>
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
			if ( !( newParent is Menu ) ) {
				throw new ArgumentException( "parent of new PdfFile should be Menu" );
			}

			return new PdfFile( this.FileName, (Menu) newParent );
		}

		public override string ToString()
		{
			return string.Format( "[PdfFile: FileName={0}]", FileName );
		}

        public override void ToXml(XmlTextWriter doc)
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
    }
}

