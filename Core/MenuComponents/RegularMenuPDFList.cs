using System;
using System.Text;
using System.Collections.ObjectModel;

namespace RAppMenu.Core.MenuComponents {
    public partial class RegularMenu {
        /// <summary>
        /// Represents the collection of PDF's in this regular menu.
        /// </summary>
        public class PDFList: Collection<string> {
            public PDFList(RegularMenu owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Gets the owner of this list of PDF's
            /// </summary>
            /// <value>The owner, a <see cref="RegularMenu"/>.</value>
            public RegularMenu Owner {
                get {
                    return this.owner;
                }
            }

            public override string ToString()
            {
                var toret = new StringBuilder();

                toret.Append( "[PDFList PDFs=[" );

                foreach(string pdfName in this) {
                    toret.Append( pdfName + ' ' );
                }

                return toret.Append( "]]" ).ToString();
            }

            private RegularMenu owner;
        }
}
}

