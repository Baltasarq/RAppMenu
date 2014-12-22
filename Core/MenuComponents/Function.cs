using System;
using System.Diagnostics;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a function entry in the menu.
	/// </summary>
	public partial class Function: MenuComponent {
		public const string TagName = "Function";
		public const string TagSentences = "ExecuteCommandOnce";
		public const string EtqPreCommand = "PreCommand";
		public const string EtqName = "Name";
		public const string EtqDataHeader = "DataHeader";
		public const string EtqRemoveQuotationMarks = "RemoveQuotationMarks";
		public const string EtqHasData = "HasData";
		public const string EtqDefaultData = "DefaultData";
		public const string EtqStartColumn = "StartColumn";
        public const string EtqEndColumn = "EndColumn";
        public const string EtqPDFRef = "PDF";

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Core.MenuComponents.Function"/> class.
		/// </summary>
		/// <param name="name">The name of the function, as a string.</param>
		/// <param name="parent">The parent <see cref="MenuComponent"/> of this function.</param>
        public Function(string name, Menu parent)
            :base( name, parent )
		{
            this.regularArgumentList = new ArgumentList( this );
			this.fnCallArgumentList = new ArgumentList( this );
            this.preOnceProgram = new ExecuteOnceProgram( this );
            this.startColumn = 0;
            this.HasData = false;
            this.DataHeader = false;
            this.RemoveQuotationMarks = false;
            this.DefaultData = "";
            this.PDFPageNumber = 1;
            this.PDFName = "";
		}

		/// <summary>
		/// Gets or sets a value indicating whether this function has data.
		/// </summary>
		/// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
		public bool HasData {
            get {
                return this.hasData;
            }
            set {
                this.hasData = value;
                this.SetNeedsSave();
            }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="RAppMenu.Core.MenuComponents.Function"/> has a data header.
		/// </summary>
		/// <value><c>true</c> if data header; otherwise, <c>false</c>.</value>
		public bool DataHeader {
            get {
                return this.dataHeader;
            }
            set {
                this.dataHeader = value;
                this.SetNeedsSave();
            }
		}

		/// <summary>
		/// Gets or sets the command to execute prior this function.
		/// </summary>
		/// <value>The command, as a string.</value>
		public string PreCommand {
            get {
                return this.preCommand;
            }
            set {
                this.preCommand = value.Trim();
                this.SetNeedsSave();
            }
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
                this.SetNeedsSave();
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
                this.SetNeedsSave();
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
                this.SetNeedsSave();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="RAppMenu.Core.MenuComponents.Function"/> should remove
		/// the quotation marks.
		/// </summary>
		/// <value><c>true</c> if remove quotation marks; otherwise, <c>false</c>.</value>
		public bool RemoveQuotationMarks {
            get {
                return this.removeQuotes;
            }
            set {
                this.removeQuotes = value;
                this.SetNeedsSave();
            }
		}

		/// <summary>
		/// Gets the execute once program.
		/// <seealso cref="ExecuteOnce"/>
		/// </summary>
		/// <value>The execute once program.</value>
		public ExecuteOnceProgram PreProgramOnce {
			get {
				return this.preOnceProgram;
			}
		}

		/// <summary>
		/// Gets the argument list.
		/// </summary>
		/// <value>The argument list, as a <see cref="ArgumentList"/>.</value>
		public ArgumentList RegularArgumentList {
			get {
				return this.regularArgumentList;
			}
		}

		/// <summary>
		/// Gets the function calls argument list.
		/// </summary>
		/// <value>The argument list, as a <see cref="ArgumentList"/>.</value>
		public ArgumentList FunctionCallsArgumentList {
			get {
				return this.fnCallArgumentList;
			}
		}

        /// <summary>
        /// Gets or sets the name of the PDF for the manual of this function
        /// </summary>
        /// <value>The name of the PDF.</value>
        public string PDFName {
            get {
                return this.pdfName;
            }
            set {
                this.pdfName = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number, as a positive, greater than 0, int.
        /// </value>
        public int PDFPageNumber {
            get {
                return this.pdfPageNumber;
            }
            set {
                if ( value < 1 ) {
                    throw new ArgumentException( "page should be > 0" );
                }

                this.pdfPageNumber = value;
            }
        }

		public override string ToString()
		{
			return string.Format( "[Function: Name={0} HasData={1}, DataHeader={2}, "
					+ "PreCommand={3}, DefaultData={4}, StartColumn={5}, EndColumn={6}, "
					+ "RemoveQuotationMarks={7}, PreProgramOnce={8}, ArgList={9}]",
			        Name,
				    HasData, DataHeader, PreCommand, DefaultData, StartColumn,
			        EndColumn, RemoveQuotationMarks, PreProgramOnce, RegularArgumentList.ToString() );
		}

        /// <summary>
        /// Copies this Function.
        /// </summary>
        /// <param name="newParent">The new parent of the <see cref="Function"/>.</param>
        /// <returns>A new <see cref="Function"/>.</returns>
        public override MenuComponent Copy(MenuComponent newParent)
        {
            var menuParent = newParent as Menu;

            if ( menuParent == null ) {
                throw new ArgumentException( "parent of a copied function should be a menu" );
            }

            var toret = new Function( this.Name, menuParent ) {
                HasData = this.HasData,
                DataHeader = this.DataHeader,
                PreCommand = this.PreCommand,
                RemoveQuotationMarks = this.RemoveQuotationMarks,
                StartColumn = this.StartColumn,
                EndColumn = this.EndColumn,
                DefaultData = this.DefaultData,
                PDFName = this.PDFName,
                PDFPageNumber = this.PDFPageNumber
            };

            foreach(string sentence in this.PreProgramOnce) {
                toret.PreProgramOnce.Add( sentence );
            }

            foreach(Argument arg in this.RegularArgumentList) {
                arg.Copy( toret );
            }

            foreach(Argument arg in this.FunctionCallsArgumentList) {
                arg.Copy( toret );
            }

            return toret;
        }

		/// <summary>
		/// Converts this menu component to XML.
		/// </summary>
		/// <param name="doc">The document, as a XmlTextWriter.</param>
		public override void ToXml(XmlTextWriter doc)
		{
            Trace.WriteLine( "Function.ToXml: " + this.ToString() );
            Trace.Indent();

			// Functions must be surrounded by dedicated menus,
			// unless they are at top level
			bool needsEnclosingMenu = !( this.Parent is RootMenu )
                                   && !( this.Parent is GraphicMenuEntry );

			if (  needsEnclosingMenu ) {
				doc.WriteStartElement( Menu.TagName );
				doc.WriteStartAttribute( EtqName );
				doc.WriteString( this.Name );
				doc.WriteEndAttribute();
			}

			// The function itself
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

			// DataHeader = "TRUE"
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

			// PreCommand = "quit()"
			if ( !string.IsNullOrWhiteSpace( this.PreCommand ) ) {
				doc.WriteStartAttribute( EtqPreCommand );
				doc.WriteString( this.PreCommand );
				doc.WriteEndAttribute();
			}

            // PDF = "manual.pdf$6"
            if ( !string.IsNullOrWhiteSpace( this.PDFName ) ) {
                string pdfReference = this.PDFName;

                if ( this.PDFPageNumber > 1 ) {
                    pdfReference += "$" + this.PDFPageNumber;
                }

                doc.WriteStartAttribute( EtqPDFRef );
                doc.WriteString( pdfReference );
                doc.WriteEndAttribute();
            }

			// ExecuteOnce sentences
			foreach(string sentence in this.PreProgramOnce) {
				doc.WriteStartElement( TagSentences );
				doc.WriteStartAttribute( EtqName );
				doc.WriteString( sentence );
				doc.WriteEndAttribute();
				doc.WriteEndElement();
			}

			// Arguments
			foreach(Argument arg in this.RegularArgumentList) {
				arg.ToXml( doc );
			}

            // Function call arguments
            foreach(CallArgument arg in this.FunctionCallsArgumentList) {
                arg.ToXml( doc );
            }

			// Close the function
            doc.WriteEndElement();

			// Close the enclosing menu, if needed
			if ( needsEnclosingMenu ) {
				doc.WriteEndElement();
			}

            Trace.Unindent();
			return;
		}

		public static Function FromXml(XmlNode node, Menu menu)
		{
			string functionName = node.GetAttribute( EtqName ).InnerText.Trim();
			Menu trueParent = menu;

            Trace.WriteLine( "Function.FromXml: " + node.AsString() );
            Trace.Indent();

			// Determine the parent for the deletion of the unneeded upper menu
            // Note that this must not be done if we are at top level,
            // Or if we are working with a graphic menu.
			if ( !( menu is RootMenu )
              && !( menu is GraphicMenuEntry )
			  && menu.Name == functionName )
            {
				trueParent = menu.Parent;

				// Eliminate the unneeded enclosing menu
				if ( trueParent == null ) {
					throw new XmlException( "functions should be enclosed in dedicated menu entries" );
				}

				menu.Remove();
			}

            var toret = new Function( functionName, trueParent );

			// Attribute info
			foreach (XmlAttribute attr in node.Attributes) {
				// HasData = "true"
				if ( attr.Name.Equals( EtqHasData, StringComparison.OrdinalIgnoreCase ) ) {
                    toret.HasData = attr.GetValueAsBool();
				}
				else
				// RemoveQuotationMarks = "TRUE"
				if ( attr.Name.Equals( EtqRemoveQuotationMarks, StringComparison.OrdinalIgnoreCase ) ) {
                    toret.HasData = attr.GetValueAsBool();
				}
				else
				// DataHeader = "TRUE"
				if ( attr.Name.Equals( EtqDataHeader, StringComparison.OrdinalIgnoreCase ) ) {
                    toret.DataHeader = attr.GetValueAsBool();
				}
				else
				// DefaultData = "Carnivores"
				if ( attr.Name.Equals( EtqDefaultData, StringComparison.OrdinalIgnoreCase ) ) {
					toret.DefaultData = attr.InnerText.Trim();
				}
				else
				// StartColumn = "1"
				if ( attr.Name.Equals( EtqStartColumn, StringComparison.OrdinalIgnoreCase ) ) {
                    toret.StartColumn = attr.GetValueAsInt();
				}
				else
				// EndColumn = "1"
				if ( attr.Name.Equals( EtqEndColumn, StringComparison.OrdinalIgnoreCase ) ) {
                    toret.EndColumn = attr.GetValueAsInt();
				}
				else
				// PreCommand = "quit()"
				if ( attr.Name.Equals( EtqPreCommand, StringComparison.OrdinalIgnoreCase ) ) {
					toret.PreCommand = attr.InnerText.Trim();
				}
                else
                // PDF = "manual.pdf$5"
                if ( attr.Name.Equals( EtqPDFRef, StringComparison.OrdinalIgnoreCase ) ) {
                    string pdfReference = attr.InnerText.Trim();

                    if ( pdfReference.IndexOf( '$' ) >= 0 ) {
                        string[] parts = pdfReference.Split( '$' );

                        toret.PDFName = parts[ 0 ].Trim();
                        toret.PDFPageNumber = Convert.ToInt32( parts[ 1 ] );
                    } else {
                        toret.PDFName = pdfReference;
                    }
                }
			}

			// Function arguments & execute once sentences
			foreach (XmlNode subNode in node.ChildNodes) {
				if ( subNode.Name.Equals( TagSentences, StringComparison.OrdinalIgnoreCase ) ) {
					toret.PreProgramOnce.Add(
						subNode.Attributes.GetNamedItemIgnoreCase( EtqName ).InnerText );
				}
				else
				if ( subNode.Name.Equals( Argument.ArgumentTagName, StringComparison.OrdinalIgnoreCase )
				  || subNode.Name.Equals( Argument.RequiredArgumentTagName, StringComparison.OrdinalIgnoreCase ) )
				{
					Argument.FromXml( subNode, toret );
				}
                else
                if ( subNode.Name.Equals( CallArgument.TagName, StringComparison.OrdinalIgnoreCase ) )
                {
                    CallArgument.FromXml( subNode, toret );
                }
			}

            Trace.Unindent();
			return toret;
		}

        private bool hasData;
        private bool dataHeader;
        private bool removeQuotes;
        private string preCommand;
        private int startColumn;
        private int endColumn;
        private string defaultData;
        private int pdfPageNumber;
        private string pdfName;
		private ExecuteOnceProgram preOnceProgram;
		private ArgumentList regularArgumentList;
		private ArgumentList fnCallArgumentList;
	}
}

