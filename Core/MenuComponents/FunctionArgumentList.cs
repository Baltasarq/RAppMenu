using System;
using System.Text;
using System.Collections.ObjectModel;

namespace RAppMenu.Core.MenuComponents {
	public partial class Function {
		/// <summary>
		/// Represents the collection of arguments in this function.
		/// </summary>
		public class ArgumentList: Collection<BaseArgument> {
			/// <summary>
			/// Initializes a new instance of the <see cref="RAppMenu.Core.MenuComponents.Function+ArgumentList"/> class.
			/// </summary>
			/// <param name="owner">The function owner.</param>
			public ArgumentList(Function owner)
			{
				this.owner = owner;
			}

			/// <summary>
			/// Gets the owner of this argument list.
			/// </summary>
			/// <value>The owner, as a <see cref="Function"/>.</value>
			public Function Owner {
				get {
					return this.owner;
				}
			}

			/// <summary>
			/// Insert a new function argument in the collection.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="item">The <see cref="FunctionArgument"/> to insert.</param>
			protected override void InsertItem(int index, BaseArgument item)
			{
				this.Chk( item );
				base.InsertItem( index, item );
				this.Owner.SetNeedsSave();
			}

			/// <summary>
			/// Modifies a given <see cref="FunctionArgument"/> at the given index.
			/// </summary>
			/// <param name="index">The index, as an int.</param>
			/// <param name="item">The <see cref="FunctionArgument"/> to modify.</param>
			protected override void SetItem(int index, BaseArgument item)
			{
				this.Chk( item );
				base.SetItem( index, item );
				this.Owner.SetNeedsSave();
			}

			/// <summary>
			/// Cheks that the specified value is not null.
			/// </summary>
			/// <param name="value">The <see cref="FunctionArgument"/> to check.</param>
			private void Chk(BaseArgument value)
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

			private Function owner;
		}
	}
}

