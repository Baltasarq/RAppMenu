using System;
using System.Text;
using System.Collections.ObjectModel;

namespace RAppMenu.Core.MenuComponents {
    public partial class Function {
        /// <summary>
        /// Represents the execute once list of string values, a program.
        /// </summary>
        public class ExecuteOnceProgram: Collection<string> {
            public ExecuteOnceProgram(Function owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Gets the owner of this program.
            /// </summary>
            /// <value>The owner, as a <see cref="Function"/>.</value>
            public Function Owner {
                get {
                    return this.owner;
                }
            }

            /// <summary>
            /// Inserts a new sentence in the program.
            /// </summary>
            /// <param name="index">The index, as an int.</param>
            /// <param name="newValue">The new sentence, as a string.</param>
            protected override void InsertItem(int index, string newValue)
            {
                this.Chk( newValue );
                base.InsertItem( index, newValue );
                this.Owner.SetNeedsSave();
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
                this.Owner.SetNeedsSave();
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

                this.Owner.SetNeedsSave();
            }

            public override string ToString()
            {
                var txt = new StringBuilder();

                foreach(string sentence in this) {
                    txt.AppendLine( sentence );
                }

                return txt.ToString();
            }

            private Function owner;
        }
    }
}

