using System;
using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	public partial class Function {
		/// <summary>
		/// Arguments for function calls.
		/// </summary>
		public class CallArgument: BaseArgument {
			public class Arg: BaseArgument {
				public Arg(string name, CallArgument callArgument)
					:base( name, callArgument.Owner )
				{
					this.callArgument = callArgument;
				}

				public CallArgument CallArgument {
					get {
						return this.callArgument;
					}
				}

				public string Value {
					get {
						return this.value;
					}
					set {
						this.value = value;
						this.SetNeedsSave();
					}
				}

				public override void ToXml(XmlTextWriter doc)
				{
				}

				private CallArgument callArgument;
				private string value;
			}

			public CallArgument(string name, Function owner)
				:base( name, owner )
			{
			}

			public override void ToXml(XmlTextWriter doc)
			{
			}
		}
	}
}

