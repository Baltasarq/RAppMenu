using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a function entry in the menu.
	/// </summary>
	public class Function: MenuComponent {
		public const string TagName = "Function";
		public const string TagSentence = "ExecuteOnce";
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
		public class FunctionArgument: MenuComponent {
			public const string TagName = "Argument";
			public enum ViewerType { DataColumnsViewer, DataValuesViewer, Map, TaxTree };

			/// <summary>
			/// Initializes a new instance of the <see cref="RAppMenu.Core.MenuComponents.Function+FunctionArgument"/> class.
			/// </summary>
			/// <param name="name">Name.</param>
			public FunctionArgument(string name)
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

			public ViewerType Viewer {
				get; set;
			}

			public string DependsFrom {
				get; set;
			}

			public bool AllowMultiselect {
				get; set;
			}

			/// <summary>
			/// Converts this menu component to XML.
			/// </summary>
			/// <param name="doc">The document, as a XmlTextWriter.</param>
			public override void ToXml(XmlTextWriter doc)
			{
				doc.WriteStartElement( TagName );
				doc.WriteStartAttribute( EtqName );
				doc.WriteString( this.Name );
				doc.WriteEndAttribute();
				doc.WriteEndElement();
			}
		}

		/// <summary>
		/// Represents the collection of arguments in this function.
		/// </summary>
		public class ArgumentList: Collection<FunctionArgument> {
			/// <summary>
			/// Insert a new function argument in the collection.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="item">The <see cref="FunctionArgument"/> to insert.</param>
			protected override void InsertItem(int index, FunctionArgument item)
			{
				this.Chk( item );
				base.InsertItem( index, item );
			}

			/// <summary>
			/// Modifies a given <see cref="FunctionArgument"/> at the given index.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="item">The <see cref="FunctionArgument"/> to modify.</param>
			protected override void SetItem(int index, FunctionArgument item)
			{
				this.Chk( item );
				base.SetItem( index, item );
			}

			/// <summary>
			/// Cheks that the specified value is not null.
			/// </summary>
			/// <param name="value">The <see cref="FunctionArgument"/> to check.</param>
			private void Chk(FunctionArgument value)
			{
				if ( value == null ) {
					throw new ArgumentException( "function argument cannot be null" ); 
				}

				if ( this.Contains( value ) ) {
					throw new ArgumentException( "function argument duplicated" ); 
				}

				return;
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
			this.preProgram = new ExecuteOnceProgram();
            this.startColumn = 0;
            this.HasData = false;
            this.DataHeader = false;
            this.RemoveQuotationMarks = false;
            this.DefaultData = "";
		}

		/// <summary>
		/// Converts this menu component to XML.
		/// </summary>
		/// <param name="doc">The document, as a XmlTextWriter.</param>
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

			// ExecuteOnce sentences
			foreach(string sentence in this.PreProgram) {
				doc.WriteStartElement( TagSentence );
				doc.WriteStartAttribute( EtqName );
				doc.WriteString( sentence );
				doc.WriteEndAttribute();
				doc.WriteEndElement();
			}

			// Arguments
			foreach(FunctionArgument arg in this.ArgList) {
				arg.ToXml( doc );
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

		/// <summary>
		/// Gets the execute once program.
		/// <seealso cref="ExecuteOnce"/>
		/// </summary>
		/// <value>The execute once program.</value>
		public ExecuteOnceProgram PreProgram {
			get {
				return this.preProgram;
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

        private int startColumn;
        private int endColumn;
        private string defaultData;
		private ExecuteOnceProgram preProgram;
		private ArgumentList argumentList;
	}
}

