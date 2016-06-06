using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace RWABuilder.Core.MenuComponents {
	/// <summary>
	/// Represents a function entry in the menu.
	/// </summary>
	public partial class Function: MenuComponent {
		public const string TagName = "Function";
		public const string TagSentences = "ExecuteCommandOnce";
		public const string TagPDFRef = "PDF";
		public const string TagExampleData = "ExampleData";

		public const string EtqPreCommand = "PreCommand";
		public const string EtqName = "Name";
		public const string EtqCaption = "Caption";
		public const string EtqPackage = "Package";
		public const string EtqPage = "Page";
		public const string EtqDataHeader = "DataHeader";
		public const string EtqHasData = "HasData";
		public const string EtqStartColumn = "StartColumn";
        public const string EtqEndColumn = "EndColumn";
		public const string EtqDefault = "Default";

		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Core.MenuComponents.Function"/> class.
		/// </summary>
		/// <param name="name">The name of the function, as a string.</param>
		/// <param name="parent">The parent <see cref="MenuComponent"/> of this function.</param>
        public Function(string name, Menu parent)
            :base( name, parent )
		{
            this.regularArgumentList = new ArgumentList( this );
			this.fnCallArgumentList = new ArgumentList( this );
            this.preOnceProgram = new ExecuteOnceProgram( this );
			this.endColumn = 0;
            this.startColumn = 0;
            this.HasData = false;
            this.DataHeader = false;
			this.IsDefault = false;
            this.ExampleData = "";
			this.PDFPageNumber = 1;
            this.PDFName = "";
			this.PreCommand = "";
			this.Package = "";
			this.Caption = "";
		}

		/// <summary>
		/// Gets or sets a value indicating whether this function is the default.
		/// </summary>
		/// <value><c>true</c> if this instance is the default; otherwise, <c>false</c>.</value>
		public bool IsDefault {
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this function has data.
		/// This is independent of whether there is example data available or not.
		/// </summary>
		/// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
		public bool HasData {
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="RWABuilder.Core.MenuComponents.Function"/> has a data header.
		/// </summary>
		/// <value><c>true</c> if data header; otherwise, <c>false</c>.</value>
		public bool DataHeader {
			get; set;
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
				this.preCommand = ( value ?? "" ).Trim();
            }
		}

		/// <summary>
		/// Gets or sets the default data.
		/// </summary>
		/// <value>The default data, as a string.</value>
		public string ExampleData {
			get {
				return this.exampleData;
			}
			set {
				this.exampleData = ( value ?? "" ).Trim();
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
				this.pdfName = Path.GetFileName( ( value ?? "" ).Trim() );
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

		/// <summary>
		/// Gets or sets the name of the package of this function
		/// </summary>
		/// <value>The name of the package.</value>
		public string Package {
			get {
				return this.package;
			}
			set {
				this.package = ( value ?? "" ).Trim();
			}
		}

		/// <summary>
		/// Gets or sets the caption in the menu entry for this function.
		/// </summary>
		/// <value>The caption, as a string.</value>
		public string Caption {
			get {
				string toret = this.caption;

				if ( string.IsNullOrEmpty( toret ) ) {
					toret = this.Name;
				}

				return toret;
			}
			set {
				this.caption = ( value ?? "" ).Trim();
			}
		}

		public override string ToString()
		{
			return string.Format( "[Function: Name='{0}' HasData={1},"
			        + "DataHeader={2}, "
					+ "PreCommand='{3}', ExampleData='{4}', StartColumn={5}, EndColumn={6}, "
					+ "PreProgramOnce='{7}', ArgList={8}, Package='{9}' PDF='{10}'"
					+ "PDFStartPage='{11}']",
			        Name, HasData, DataHeader, PreCommand, ExampleData, StartColumn,
			        EndColumn, PreProgramOnce, RegularArgumentList.ToString(),
					this.Package, this.PDFName, this.PDFPageNumber );
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
				Package = this.Package,
				Caption = this.Caption,
                HasData = this.HasData,
                DataHeader = this.DataHeader,
                PreCommand = this.PreCommand,
                StartColumn = this.StartColumn,
                EndColumn = this.EndColumn,
                ExampleData = this.ExampleData,
                PDFName = this.PDFName,
                PDFPageNumber = this.PDFPageNumber
            };

            foreach(string sentence in this.PreProgramOnce) {
                toret.PreProgramOnce.Add( sentence );
            }

            this.RegularArgumentList.Copy( toret.RegularArgumentList );
            this.FunctionCallsArgumentList.Copy( toret.FunctionCallsArgumentList );
            return toret;
        }

		/// <summary>
		/// Converts this menu component to XML.
		/// </summary>
		/// <param name="doc">The document, as a XmlTextWriter.</param>
		public override void ToXml(XmlWriter doc)
		{
            Trace.WriteLine( "Function.ToXml: " + this.ToString() );
            Trace.Indent();

			// The function itself
            doc.WriteStartElement( TagName );

            // Name = "f1"
            doc.WriteStartAttribute( EtqName );
            doc.WriteString( this.Name );
            doc.WriteEndAttribute();

			// Package = "package"
			if ( !string.IsNullOrWhiteSpace( this.Package ) ) {
				doc.WriteStartAttribute( EtqPackage );
				doc.WriteString( this.Package );
				doc.WriteEndAttribute();
			}

			// Caption = "caption"
			if ( !string.IsNullOrWhiteSpace( this.Caption )
			  && this.Caption != this.Name )
			{
				doc.WriteStartAttribute( EtqCaption );
				doc.WriteString( this.Caption );
				doc.WriteEndAttribute();
			}

			// Default = "TRUE"
			if ( this.IsDefault ) {
				doc.WriteStartAttribute( EtqDefault );
				doc.WriteString( true.ToString().ToUpper() );
				doc.WriteEndAttribute();
			}

            // HasData = "TRUE"
            if ( this.HasData ) {
                doc.WriteStartAttribute( EtqHasData );
                doc.WriteString( true.ToString().ToUpper() );
                doc.WriteEndAttribute();
            }

			// DataHeader = "TRUE"
			if ( this.DataHeader ) {
				doc.WriteStartAttribute( EtqDataHeader );
				doc.WriteString( true.ToString().ToUpper() );
				doc.WriteEndAttribute();
			}

			// PreCommand = "quit()"
			if ( !string.IsNullOrWhiteSpace( this.PreCommand ) ) {
				doc.WriteStartAttribute( EtqPreCommand );
				doc.WriteString( this.PreCommand );
				doc.WriteEndAttribute();
			}

			// <ExampleData Name= "Carnivores" StartColumn=2 EndColumn=5/>
			if ( !string.IsNullOrWhiteSpace( this.ExampleData ) ) {
				doc.WriteStartElement( TagExampleData );
				doc.WriteStartAttribute( EtqName );
				doc.WriteString( this.ExampleData );
				doc.WriteEndAttribute();

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

            // <PDF Name="manual.pdf" Page="2" />
            if ( !string.IsNullOrWhiteSpace( this.PDFName ) ) {
                doc.WriteStartElement( TagPDFRef );

				doc.WriteStartAttribute( EtqName );
                doc.WriteString( this.PDFName );
                doc.WriteEndAttribute();

				doc.WriteStartAttribute( EtqPage );
				doc.WriteString( this.PDFPageNumber.ToString() );
				doc.WriteEndAttribute();

				doc.WriteEndElement();
            }

			// ExecuteOnce sentences
			if ( this.PreProgramOnce.Count > 0 ) {
				doc.WriteStartElement( TagSentences );
				doc.WriteString( this.PreProgramOnce.ToString() );
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

            Trace.Unindent();
			return;
		}

		public static Function FromXml(XmlNode node, Menu menu)
		{
			Trace.WriteLine( "Function.FromXml: " + node.AsString() );
			Trace.Indent();

			string functionName = node.GetAttribute( EtqName ).InnerText.Trim();
			Trace.WriteLine( "Function Name: " + functionName );
            var toret = new Function( functionName, menu );

			// Attribute info
			foreach (XmlAttribute attr in node.Attributes) {
				// Package = "base"
				if ( attr.Name.Equals( EtqPackage, StringComparison.OrdinalIgnoreCase ) ) {
					toret.Package = attr.InnerText.Trim();
				}
				else
				// Caption = "awesome function"
				if ( attr.Name.Equals( EtqCaption, StringComparison.OrdinalIgnoreCase ) ) {
					toret.Caption = attr.InnerText.Trim();
				}
				else
				// Default = "true"
				if ( attr.Name.Equals( EtqDefault, StringComparison.OrdinalIgnoreCase ) ) {
					toret.IsDefault = attr.GetValueAsBool();
				}
				else
				// HasData = "true"
				if ( attr.Name.Equals( EtqHasData, StringComparison.OrdinalIgnoreCase ) ) {
                    toret.HasData = attr.GetValueAsBool();
				}
				else
				// DataHeader = "TRUE"
				if ( attr.Name.Equals( EtqDataHeader, StringComparison.OrdinalIgnoreCase ) ) {
                    toret.DataHeader = attr.GetValueAsBool();
				}
				else
				// PreCommand = "quit()"
				if ( attr.Name.Equals( EtqPreCommand, StringComparison.OrdinalIgnoreCase ) ) {
					toret.PreCommand = attr.InnerText.Trim();
				}
				else
				// Data = "data(ZII1)"
				if ( attr.Name.Equals( TagExampleData, StringComparison.OrdinalIgnoreCase ) ) {
					toret.HasData = true;
					toret.ExampleData = attr.InnerText.Trim();
				}
			}

			// Sub nodes
			foreach (XmlNode subNode in node.ChildNodes) {
				// <ExampleData Name= "Carnivores" StartColumn=2 EndColumn=5/>
				if ( subNode.Name.Equals( TagExampleData, StringComparison.OrdinalIgnoreCase ) ) {
					XmlAttribute attrName = subNode.GetAttribute( EtqName );
					XmlNode attrStart = subNode.Attributes.GetNamedItemIgnoreCase( EtqStartColumn );
					XmlNode attrEnd = subNode.Attributes.GetNamedItemIgnoreCase( EtqEndColumn );

					if ( attrName != null ) {
						toret.ExampleData = attrName.InnerText;
						toret.HasData = true;

						if ( attrStart != null ) {
							toret.StartColumn = attrStart.GetValueAsInt();
						}

						if ( attrEnd != null ) {
							toret.EndColumn = attrEnd.GetValueAsInt();
						}
					} else {
						throw new XmlException( TagExampleData + " without " + TagName );
					}
				}
				else
				// <PDF Name="manual.pdf" Page="1"/>
				if ( subNode.Name.Equals( TagPDFRef, StringComparison.OrdinalIgnoreCase ) ) {
					XmlNode attrName = subNode.Attributes.GetNamedItemIgnoreCase( EtqName );
					XmlNode attrPage = subNode.Attributes.GetNamedItemIgnoreCase( EtqPage );

					if ( attrName != null ) {
						toret.PDFName = attrName.InnerText;

						if ( attrPage != null ) {
							toret.PDFPageNumber = attrPage.GetValueAsInt();
						}
					} else {
						throw new XmlException( TagPDFRef + " without " + TagName );
					}
				}
				else
				// Execute once
				if ( subNode.Name.Equals( TagSentences, StringComparison.OrdinalIgnoreCase ) ) {
					var sentences = new List<string>( subNode.InnerText.Split( '\n' ) );
					var sentencesToDelete = new SortedSet<int>();

					// Check the sentences for blank lines
					for (int i = 0; i < sentences.Count; ++i) {
						if ( string.IsNullOrWhiteSpace( sentences[ i ] ) ) {
							sentencesToDelete.Add( i );
						}
					}

					// Remove blank lines
					int pos = 0;
					foreach (int x in sentencesToDelete) {
						sentences.RemoveAt( x - pos );
						++pos;
					}

					toret.PreProgramOnce.AddRange( sentences.ToArray() );
				}
				else
				if ( subNode.Name.Equals( Argument.ArgumentTagName, StringComparison.OrdinalIgnoreCase ) )
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

		/// <summary>
		/// Looks in contents for the given string. Ignores case.
		/// </summary>
		/// <returns><c>true</c>, if in contents there is txt, <c>false</c> otherwise.</returns>
		/// <param name="txt">Text.</param>
		public override bool LookInContentsFor(string txt)
		{
			bool toret = base.LookInContentsFor( txt );

			if ( !toret ) {
				toret = ( this.Caption.ToLower().IndexOf( txt.Trim().ToLower() ) >= 0 );
			}

			return toret;
		}

		private string package;
		private string caption;
        private string preCommand;
        private int startColumn;
        private int endColumn;
        private string exampleData;
        private int pdfPageNumber;
        private string pdfName;
		private ExecuteOnceProgram preOnceProgram;
		private ArgumentList regularArgumentList;
		private ArgumentList fnCallArgumentList;
	}
}

