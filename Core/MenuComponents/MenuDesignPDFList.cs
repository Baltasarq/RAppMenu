using System;
using System.Text;
using System.Collections.ObjectModel;

using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Core {
    public partial class MenuDesign {
        /// <summary>
        /// Represents the collection of PDF's in the whole menu design.
        /// </summary>
        public class PDFList: Collection<PdfFile> {
            public PDFList(MenuDesign owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Gets the owner of this list of PDF's
            /// </summary>
            /// <value>The owner, a <see cref="MenuDesign"/>.</value>
            public MenuDesign Owner {
                get {
                    return this.owner;
                }
            }

            public override string ToString()
            {
                var toret = new StringBuilder();

                toret.Append( "[PDFList PDFs=[" );

                foreach(PdfFile pdf in this) {
                    toret.Append( pdf.Name + ' ' );
                }

                return toret.Append( "]]" ).ToString();
            }

            private MenuDesign owner;
        }
}
}

