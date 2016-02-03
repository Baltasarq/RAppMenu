using System;
using System.Diagnostics;
using System.Xml;

namespace RWABuilder.Core.MenuComponents {
	public partial class Function {
		/// <summary>
		/// Arguments for function calls.
		/// </summary>
		public class CallArgument: BaseArgument {
            public const string TagName = "FunctionArgument";
            public const string EtqName = "Name";
            public const string EtqFunctionName = "Function";
            public const string EtqVariant = "Variant";

			public class Arg: BaseArgument {
                public const string TagName = "Argument";
                public const string EtqReadOnly = "ReadOnly";
                public const string EtqName = "Name";
                public const string EtqValue = "Value";

				public Arg(string name, CallArgument callArgument)
					:base( name, callArgument.Owner )
				{
					this.callArgument = callArgument;
                    this.value = "";
                    this.IsReadOnly = false;
				}

                /// <summary>
                /// Gets the call argument of a function that this sub argument pertaints to.
                /// </summary>
                /// <value>The <see cref="CallArgument"/>.</value>
				public CallArgument CallArgument {
					get {
						return this.callArgument;
					}
				}

                /// <summary>
                /// Gets or sets the value for this argument.
                /// </summary>
                /// <value>The value, as a string.</value>
				public string Value {
					get {
						return this.value;
					}
					set {
						this.value = value.Trim();
					}
				}

                /// <summary>
                /// Copies this Arg into a new one.
                /// </summary>
                /// <param name="newOwner">New <see cref="CallArgument"/> owner.</param>
                /// <returns>A new <see cref="Arg"</see>./></returns>
                public override MenuComponent Copy(MenuComponent newOwner)
                {
                    var callArgumentOwner = newOwner as CallArgument;

                    if ( callArgumentOwner == null ) {
                        throw new ArgumentException( "new owner should be call argument copying arg" );
                    }

                    var toret = new Arg( this.Name, callArgumentOwner) {
                        Value = this.Value,
                        IsReadOnly = this.IsReadOnly
                    };

                    callArgumentOwner.ArgumentList.Add( toret );
                    return toret;
                }

				public override void ToXml(XmlWriter doc)
				{
                    Trace.WriteLine( "Arg.ToXml: " + this.ToString() );
                    Trace.Indent();

                    doc.WriteStartElement( TagName ); 

                    // Name = "x"
                    doc.WriteStartAttribute( EtqName );
                    doc.WriteString( this.Name );
                    doc.WriteEndAttribute();

					// ReadOnly = "TRUE"
					if ( this.IsReadOnly ) {
						doc.WriteStartAttribute( EtqReadOnly );
						doc.WriteString( true.ToString().ToUpper() );
						doc.WriteEndAttribute();
					}

                    // Value = "0"
					if ( !string.IsNullOrWhiteSpace( this.Value ) ) {
						doc.WriteStartAttribute( EtqValue );
						doc.WriteString( this.Value );
						doc.WriteEndAttribute();
					}

                    doc.WriteEndElement();
                    Trace.Unindent();
				}

                public static Arg FromXml(XmlNode node, CallArgument fnCall)
                {
                    string name = node.GetAttribute( EtqName ).InnerText;
                    XmlNode attrValue = node.Attributes.GetNamedItemIgnoreCase( EtqValue );
					XmlNode attrReadonly = node.Attributes.GetNamedItemIgnoreCase( EtqReadOnly );

					Trace.WriteLine( "Arg.FromXml: " + name );

                    // Create
					Arg toret = new Arg( name, fnCall );
					fnCall.ArgumentList.Add( toret );

                    // Is read only?
                    if ( attrReadonly != null ) {
                        toret.IsReadOnly = attrReadonly.GetValueAsBool();
                    }
                        
                    // Value = "v1"
                    if ( attrValue != null ) {
                        toret.Value = attrValue.InnerText.Trim();
                    }

                    return toret;
                }

				private CallArgument callArgument;
				private string value;
			}

			public CallArgument(string name, Function owner)
				:base( name, owner )
			{
                this.args = new ArgumentList( owner );
                this.functionName = "";
                this.variant = "";
			}

            /// <summary>
            /// Returns the arguments in this function call.
            /// </summary>
            /// <value>The argument list, as an <see cref="ArgumentList"/>.</value>
            public ArgumentList ArgumentList {
                get {
                    return this.args;
                }
            }

            /// <summary>
            /// Gets or sets the variant for this function call.
            /// </summary>
            /// <value>The variant, as a string.</value>
            public string Variant {
                get {
                    return this.variant;
                }
                set {
                    this.variant = value.Trim();
                }
            }

            /// <summary>
            /// Gets or sets the name of the function to be called.
            /// </summary>
            /// <value>The name of the function, as a string.</value>
            public string FunctionName {
                get {
                    return this.functionName;
                }
                set {
                    this.functionName = value.Trim();
                }
            }

            /// <summary>
            /// Copies this CallArgument
            /// </summary>
            /// <param name="newOwner">New <see cref="Function"/> owner.</param>
            /// <returns>A new, identical <see cref="CallArgument"/>.
            public override MenuComponent Copy(MenuComponent newOwner)
            {
                var functionOwner = newOwner as Function;

                if ( functionOwner == null ) {
                    throw new ArgumentException( "new owner should be a Function when copying CallArgument's" );
                }

                var toret = new CallArgument( this.Name, functionOwner ) {
                    Variant = this.Variant,
                    FunctionName = this.FunctionName,
					IsReadOnly = this.IsReadOnly,
                };

                foreach(Arg arg in this.ArgumentList) {
                    arg.Copy( toret );
                }

                return toret;
            }

			public override void ToXml(XmlWriter doc)
			{
                Trace.WriteLine( "CallArgument.ToXml node: " + this.ToString() );

                doc.WriteStartElement( TagName );

                // Name = "x"
                doc.WriteStartAttribute( EtqName );
                doc.WriteString( this.Name );
                doc.WriteEndAttribute();

                // Function = "pow"
                doc.WriteStartAttribute( EtqFunctionName );
                doc.WriteString( this.FunctionName );
                doc.WriteEndAttribute();

                // Variant = "variant(1)"
				if ( !string.IsNullOrWhiteSpace( this.Variant ) ) {
					doc.WriteStartAttribute( EtqVariant );
					doc.WriteString( this.Variant );
					doc.WriteEndAttribute();
				}

                // Arguments for the function call
                foreach(Arg arg in this.ArgumentList) {
                    arg.ToXml( doc );
                }

                doc.WriteEndElement();
			}

            public static CallArgument FromXml(XmlNode node, Function f)
            {
                XmlNode variantAttr = node.Attributes.GetNamedItemIgnoreCase( EtqVariant );
                string name = node.GetAttribute( EtqName ).InnerText;
                var toret = new CallArgument( name, f );

				Trace.WriteLine( "CallArgument.FromXml node: " + name );

                toret.FunctionName = node.GetAttribute( EtqFunctionName ).InnerText;

                if ( variantAttr != null ) {
                    toret.Variant = variantAttr.InnerText;
                }

                foreach(XmlNode subNode in node.ChildNodes) {
                    Arg.FromXml( subNode, toret );
                }

                f.FunctionCallsArgumentList.Add( toret );
                return toret;
            }

            private string variant;
            private string functionName;
            private ArgumentList args;
		}
	}
}

