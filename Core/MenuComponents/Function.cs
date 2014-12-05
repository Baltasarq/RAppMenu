using System;
using System.Text;
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
		/// Converts this menu component to XML.
		/// </summary>
		/// <param name="doc">The document, as a XmlTextWriter.</param>
		public override void ToXml(XmlTextWriter doc)
		{
			// Functions must be surrounded by dedicated menus,
			// unless they are at top level
			bool needsEnclosingMenu = !( this.Parent is RootMenu );

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

			// Close the function
            doc.WriteEndElement();

			// Close the enclosing menu, if needed
			if ( needsEnclosingMenu ) {
				doc.WriteEndElement();
			}

			return;
		}

		public static Function FromXml(XmlNode node, Menu menu)
		{
			Menu trueParent = menu;

			// Determine the parent for the deletion of the unneeded upper menu
            // Note that this must not be done if we are at top level,
            // Or if we are working with a graphic menu.
			if ( !( menu is RootMenu )
              && !( menu is GraphicMenuEntry ) )
            {
				trueParent = menu.Parent;

				// Eliminate the unneeded enclosing menu
				if ( trueParent == null ) {
					throw new XmlException( "functions should be enclosed in dedicated menu entries" );
				}

				menu.Remove();
			}

            var toret = new Function( "tempFn", trueParent );

			// Attribute info
			foreach (XmlAttribute attr in node.Attributes) {
				// Name = "m1"
				if ( attr.Name.Equals( EtqName, StringComparison.OrdinalIgnoreCase ) ) {
					toret.Name = attr.InnerText;
				}
				else
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
			}

			return toret;
		}

        private bool hasData;
        private bool dataHeader;
        private bool removeQuotes;
        private string preCommand;
        private int startColumn;
        private int endColumn;
        private string defaultData;
		private ExecuteOnceProgram preOnceProgram;
		private ArgumentList regularArgumentList;
		private ArgumentList fnCallArgumentList;
	}
}

