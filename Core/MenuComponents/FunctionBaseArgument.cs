using System;

namespace RWABuilder.Core.MenuComponents {
	public partial class Function {
		public abstract class BaseArgument: MenuComponent {
			public BaseArgument(string name, Function owner)
				:base( name )
			{
				this.owner = owner;
			}

			/// <summary>
			/// Gets the function that owns this argument.
			/// </summary>
			/// <value>The owner, as a <see cref="Function"/>.</value>
			public Function Owner {
				get {
					return this.owner;
				}
			}

			/// <summary>
			/// Sets this menu as needing save.
			/// </summary>
			public override void SetNeedsSave()
			{
                if ( this.Owner != null ) {
                    this.Owner.SetNeedsSave();
                }

                return;
			}

			private Function owner;
		}
	}
}

