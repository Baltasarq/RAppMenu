using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a function entry in the menu.
	/// </summary>
	public class Function: MenuComponent {
        public const string TagName = "Function";
        public const string EtqName = "Name";
        public const string EtqDataHeader = "DataHeader";
        public const string EtqRemoveQuotationMarks = "RemoveQuotationMarks";
        public const string EtqHasData = "HasData";
        public const string EtqDefaultData = "DefaultData";
        public const string EtqStartColumn = "StartColumn";
        public const string EtqEndColumn = "EndColumn";

        public Function(string name, MenuEntry parent)
            :base( name, parent )
		{
            this.startColumn = 0;
            this.HasData = false;
            this.DataHeader = false;
            this.RemoveQuotationMarks = false;
            this.DefaultData = "";
		}

		public override void ToXml(XmlTextWriter doc)
		{
            doc.WriteStartElement( TagName );

            // Name = "f1"
            doc.WriteStartAttribute( EtqName );
            doc.WriteString( this.Name );
            doc.WriteEndAttribute();

            // HasData = "TRUE"
            if ( this.HasData ) {
                doc.WriteStartAttribute( EtqHasData );
                doc.WriteString( true.ToString().ToUpper() );
                doc.WriteEndAttribute();
            }

            // RemoveQuotationMarks = "TRUE"
            if ( this.RemoveQuotationMarks ) {
                doc.WriteStartAttribute( EtqRemoveQuotationMarks );
                doc.WriteString( true.ToString().ToUpper() );
                doc.WriteEndAttribute();
            }

            if ( this.DataHeader ) {
                doc.WriteStartAttribute( EtqDataHeader );
                doc.WriteString( true.ToString().ToUpper() );
                doc.WriteEndAttribute();
            }

            // DefaultData = "Carnivores"
            if ( !string.IsNullOrWhiteSpace( this.DefaultData ) ) {
                doc.WriteStartAttribute( EtqDefaultData );
                doc.WriteString( this.DefaultData );
                doc.WriteEndAttribute();
            }

            // StartColumn = "1"
            if ( this.StartColumn > 0 ) {
                doc.WriteStartAttribute( EtqStartColumn );
                doc.WriteString( this.StartColumn.ToString() );
                doc.WriteEndAttribute();
            }

            // EndColumn = "1"
            if ( this.EndColumn > 0 ) {
                doc.WriteStartAttribute( EtqEndColumn );
                doc.WriteString( this.EndColumn.ToString() );
                doc.WriteEndAttribute();
            }

            doc.WriteEndElement();
		}

        /// <summary>
        /// Gets or sets a value indicating whether this function has data.
        /// </summary>
        /// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
        public bool HasData {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="RAppMenu.Core.MenuComponents.Function"/> has a data header.
        /// </summary>
        /// <value><c>true</c> if data header; otherwise, <c>false</c>.</value>
        public bool DataHeader {
            get; set;
        }

        /// <summary>
        /// Gets or sets the default data.
        /// </summary>
        /// <value>The default data, as a string.</value>
        public string DefaultData {
            get {
                return this.defaultData;
            }
            set {
                this.defaultData = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the start column.
        /// </summary>
        /// <value>The start column, as an int.</value>
        public int StartColumn {
            get {
                return this.startColumn;
            }
            set {
                if ( value < 0 ) {
                    throw new ArgumentOutOfRangeException( "Function.StartColumn should be >= 0" );
                }

                this.startColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets the ending column.
        /// </summary>
        /// <value>The ending column, as an int.</value>
        public int EndColumn {
            get {
                return this.endColumn;
            }
            set {
                if ( value < 0 ) {
                    throw new ArgumentOutOfRangeException( "Function.EndColumn should be >= 0" );
                }

                this.endColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="RAppMenu.Core.MenuComponents.Function"/> should remove
        /// the quotation marks.
        /// </summary>
        /// <value><c>true</c> if remove quotation marks; otherwise, <c>false</c>.</value>
        public bool RemoveQuotationMarks {
            get; set;
        }

        private int startColumn;
        private int endColumn;
        private string defaultData;
	}
}

