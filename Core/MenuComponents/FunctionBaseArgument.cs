using System;

namespace RWABuilder.Core.MenuComponents {
	public partial class Function {
		public abstract class BaseArgument: MenuComponent {
			public BaseArgument(string name, Function owner)
				:base( name )
			{
				this.owner = owner;
				this.readOnly = false;
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
			/// Gets or sets a value indicating whether this argument is read only.
			/// </summary>
			/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
			public bool IsReadOnly {
				get {
					return this.readOnly;
				}
				set {
					if ( value != this.readOnly ) {
						this.readOnly = value;
						this.SetNeedsSave();
					}

					return;
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
			private bool readOnly;
		}
	}
}

