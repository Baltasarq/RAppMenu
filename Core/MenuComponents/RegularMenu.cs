using System;

namespace RAppMenu.Core.MenuComponents {
    public partial class RegularMenu: Menu {
        public RegularMenu(string name, Menu parent)
            :base( name, parent )
        {
            this.pdfList = new PDFList( this );
        }

        public override void Add(MenuComponent mc)
        {
            base.Add( mc );

            // We need to store the new PDF File in the list
            if ( mc is PdfFile ) {
                this.PDFNameList.Add( mc.Name );
            }
        }

        /// <summary>
        /// Gets the PDF list.
        /// </summary>
        /// <value>A <see cref="PDFList"/>.</value>
        public PDFList PDFNameList {
            get {
                return this.pdfList;
            }
        }

        public override string ToString()
        {
            string toret = base.ToString();

            toret = "[RegularMenu " + toret;
            toret = " " + this.PDFNameList.ToString();

            return toret;
        }
                

        private PDFList pdfList;
    }
}

