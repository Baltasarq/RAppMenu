using System;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a function entry in the menu.
	/// </summary>
	public class Function: MenuComponent {
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
		/// Represents arguments in functions.
		/// </summary>
		public class Argument: MenuComponent {
			public const string ArgumentTagName = "Argument";
			public const string RequiredArgumentTagName = "RequiredArgument";
			public const string EtqTag = "Tag";
			public const string EtqViewer = "Viewer";
			public const string EtqDepends = "DependsFrom";
			public const string EtqAllowMultiSelect = "AllowMultiSelect";

			public enum ViewerType { DataColumnsViewer, DataValuesViewer, Map, TaxTree };

			/// <summary>
			/// Initializes a new instance of the <see cref="RAppMenu.Core.MenuComponents.Function+FunctionArgument"/> class.
			/// </summary>
			/// <param name="name">Name.</param>
			public Argument(string name)
				: base( name )
			{
			}

			/// <summary>
			/// Gets or sets a value indicating whether this argument is required.
			/// </summary>
			/// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
			public bool IsRequired {
				get; set;
			}

			/// <summary>
			/// Gets or sets the viewer.
			/// </summary>
			/// <value>The viewer, as a <see cref="ViewerType"/>.</value>
			public ViewerType Viewer {
				get; set;
			}

			public string DependsFrom {
				get; set;
			}

			public string Tag {
				get; set;
			}

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="RAppMenu.Core.MenuComponents.Function+Argument"/> allow multiselect.
			/// </summary>
			/// <value><c>true</c> if allow multiselect; otherwise, <c>false</c>.</value>
			public bool AllowMultiselect {
				get; set;
			}

			public override string ToString()
			{
				return string.Format( "[Argument: IsRequired={0}, Viewer={1}, "
							+ "DependsFrom={2}, Tag={3}, AllowMultiselect={4}]",
				            IsRequired, Viewer, DependsFrom, Tag, AllowMultiselect );
			}

			/// <summary>
			/// Converts this menu component to XML.
			/// </summary>
			/// <param name="doc">The document, as a XmlTextWriter.</param>
			public override void ToXml(XmlTextWriter doc)
			{
				string tagName = ArgumentTagName;

				// Choose the tag to use
				if ( this.IsRequired ) {
					tagName = RequiredArgumentTagName;
				}

				doc.WriteStartElement( tagName );

				// Name = "arg1"
				doc.WriteStartAttribute( EtqName );
				doc.WriteString( this.Name );
				doc.WriteEndAttribute();

				if ( !this.IsRequired ) {
					// DependsFrom = "?"
					if ( !string.IsNullOrWhiteSpace( this.DependsFrom ) ) {
						doc.WriteStartAttribute( EtqDepends );
						doc.WriteString( this.DependsFrom );
						doc.WriteEndAttribute();
					}

					// Tag = "?"
					if ( !string.IsNullOrWhiteSpace( this.Tag ) ) {
						doc.WriteStartAttribute( EtqTag );
						doc.WriteString( this.Tag );
						doc.WriteEndAttribute();
					}

					// AllowMultiSelect = "TRUE"
					if ( this.AllowMultiselect ) {
						doc.WriteStartAttribute( EtqAllowMultiSelect );
						doc.WriteString( true.ToString().ToUpper() );
						doc.WriteEndAttribute();
					}

					// Viewer = "Map"
					doc.WriteStartAttribute( EtqViewer );
					doc.WriteString( this.Viewer.ToString() );
					doc.WriteEndAttribute();
				}

				doc.WriteEndElement();
				return;
			}

			public static Argument FromXml(XmlNode node, Function fn)
			{
                bool isNewArgument = false;
                string name = node.GetAttribute( EtqName ).InnerText;
                Argument toret = fn.ArgList.LookUp( name );

                // Is it a new argument?
                if ( toret == null ) {
                    toret = new Argument( "tempArg" );
                    isNewArgument = true;
                }

				// RequiredArgument or Argument ?
                if ( !toret.IsRequired ) {
                    toret.IsRequired =
                        ( node.Name.Equals( RequiredArgumentTagName,
                                            StringComparison.OrdinalIgnoreCase ) );
                }

				foreach(XmlAttribute attr in node.Attributes) {
					// Name = "arg"
					if ( attr.Name.Equals( EtqName, StringComparison.OrdinalIgnoreCase ) ) {
						toret.Name = attr.InnerText.Trim();
					}
					else
					// DependsFrom = "?"
					if ( attr.Name.Equals( EtqDepends, StringComparison.OrdinalIgnoreCase ) ) {
						toret.DependsFrom = attr.InnerText.Trim();
					}
					else
					// Tag = "?"
					if ( attr.Name.Equals( EtqTag, StringComparison.OrdinalIgnoreCase ) ) {
						toret.Tag = attr.InnerText.Trim();
					}
					else
					// AllowMultiSelect = "TRUE"
					if ( attr.Name.Equals( EtqAllowMultiSelect, StringComparison.OrdinalIgnoreCase ) ) {
						toret.AllowMultiselect = bool.Parse( attr.InnerText.Trim() );
					}
					else
					// Viewer = "Map"
					if ( attr.Name.Equals( EtqViewer, StringComparison.OrdinalIgnoreCase ) ) {
                        string viewerId = attr.InnerText.Trim();
                        ViewerType viewer;

                        if ( Enum.TryParse<ViewerType>( viewerId, true, out viewer ) ) {
                            toret.Viewer = viewer;
                        } else {
                            throw new XmlException( "unknown viewer type: " + viewerId
                                                  + " at argument " + toret.Name );
                        }
					}
				}

                if ( isNewArgument ) {
                    fn.ArgList.Add( toret );
                }

				return toret;
			}
		}

		/// <summary>
		/// Represents the collection of arguments in this function.
		/// </summary>
		public class ArgumentList: Collection<Argument> {
			/// <summary>
			/// Insert a new function argument in the collection.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="item">The <see cref="FunctionArgument"/> to insert.</param>
			protected override void InsertItem(int index, Argument item)
			{
				this.Chk( item );
				base.InsertItem( index, item );
			}

			/// <summary>
			/// Modifies a given <see cref="FunctionArgument"/> at the given index.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="item">The <see cref="FunctionArgument"/> to modify.</param>
			protected override void SetItem(int index, Argument item)
			{
				this.Chk( item );
				base.SetItem( index, item );
			}

			/// <summary>
			/// Cheks that the specified value is not null.
			/// </summary>
			/// <param name="value">The <see cref="FunctionArgument"/> to check.</param>
			private void Chk(Argument value)
			{
				if ( value == null ) {
					throw new ArgumentException( "function argument cannot be null" ); 
				}

                if ( this.LookUp( value.Name ) != null ) {
					throw new ArgumentException( "function argument duplicated" ); 
				}

				return;
			}

            /// <summary>
            /// Looks up a given argument by its id.
            /// </summary>
            /// <returns>The <see cref="Argument"/>.</returns>
            /// <param name="id">An identifier, as string.</param>
            public Argument LookUp(string id)
            {
                Argument toret = null;

                foreach(Argument arg in this) {
                    if ( arg.Name == id ) {
                        toret = arg;
                        break;
                    }
                }

                return toret;
            }

			public override string ToString()
			{
				var toret = new StringBuilder();

				toret.Append( "[ArgumentList arguments=[" );

				foreach(Argument arg in this) {
					toret.Append( arg.ToString() );
				}

				return toret.Append( "]]" ).ToString();
			}
		}

		/// <summary>
		/// Represents the execute once list of string values, a program.
		/// </summary>
		public class ExecuteOnceProgram: Collection<string> {
			/// <summary>
			/// Inserts a new sentence in the program.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="newValue">The new sentence, as a string.</param>
			protected override void InsertItem(int index, string newValue)
			{
				this.Chk( newValue );
				base.InsertItem( index, newValue );
			}

			/// <summary>
			/// Modifies a sentence of the program.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="newValue">The new sentence, as a string.</param>
			protected override void SetItem(int index, string newValue)
			{
				this.Chk( newValue );
				base.SetItem( index, newValue );
			}

			/// <summary>
			/// Cheks that the specified value is not a null or empty string.
			/// </summary>
			/// <param name="value">The string to check.</param>
			private void Chk(string value)
			{
				if ( string.IsNullOrWhiteSpace( value ) ) {
					throw new ArgumentException( "sentence cannot be null or empty" ); 
				}

				return;
			}

			/// <summary>
			/// Adds a vector of sentences.
			/// </summary>
			/// <param name="sentences">The sentences to add, as a vector of string.</param>
			public void AddRange(string[] sentences)
			{
				foreach(string sentence in sentences) {
					this.Add( sentence );
				}

				return;
			}

            public override string ToString()
            {
                var txt = new StringBuilder();

                foreach(string sentence in this) {
                    txt.AppendLine( sentence );
                }

                return txt.ToString();
            }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Core.MenuComponents.Function"/> class.
		/// </summary>
		/// <param name="name">The name of the function, as a string.</param>
		/// <param name="parent">The parent <see cref="MenuComponent"/> of this function.</param>
        public Function(string name, Menu parent)
            :base( name, parent )
		{
			this.argumentList = new ArgumentList();
			this.preOnceProgram = new ExecuteOnceProgram();
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
		/// Gets or sets the command to execute prior this function.
		/// </summary>
		/// <value>The command, as a string.</value>
		public string PreCommand {
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
		public ArgumentList ArgList {
			get {
				return this.argumentList;
			}
		}

		public override string ToString()
		{
			return string.Format( "[Function: Name={0} HasData={1}, DataHeader={2}, "
					+ "PreCommand={3}, DefaultData={4}, StartColumn={5}, EndColumn={6}, "
					+ "RemoveQuotationMarks={7}, PreProgramOnce={8}, ArgList={9}]",
			        Name,
				    HasData, DataHeader, PreCommand, DefaultData, StartColumn,
			        EndColumn, RemoveQuotationMarks, PreProgramOnce, ArgList.ToString() );
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
			foreach(Argument arg in this.ArgList) {
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
					toret.HasData = bool.Parse( attr.InnerText );
				}
				else
				// RemoveQuotationMarks = "TRUE"
				if ( attr.Name.Equals( EtqRemoveQuotationMarks, StringComparison.OrdinalIgnoreCase ) ) {
					toret.HasData = bool.Parse( attr.InnerText );
				}
				else
				// DataHeader = "TRUE"
				if ( attr.Name.Equals( EtqDataHeader, StringComparison.OrdinalIgnoreCase ) ) {
					toret.DataHeader = bool.Parse( attr.InnerText );
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

        private int startColumn;
        private int endColumn;
        private string defaultData;
		private ExecuteOnceProgram preOnceProgram;
		private ArgumentList argumentList;
	}
}

