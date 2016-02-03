using System;

namespace RWABuilder.Core.MenuComponents {
	public partial class Function {
		public abstract class BaseArgument: MenuComponent {
			public BaseArgument(string name, Function owner)
				:base( name )
			{
				this.Owner = owner;
				this.IsReadOnly = false;
			}

			/// <summary>
			/// Gets the function that owns this argument.
			/// </summary>
			/// <value>The owner, as a <see cref="Function"/>.</value>
			public Function Owner {
				get; private set;
			}

			/// <summary>
			/// Gets or sets a value indicating whether this argument is read only.
			/// </summary>
			/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
			public bool IsReadOnly {
				get; set;
			}
		}
	}
}

