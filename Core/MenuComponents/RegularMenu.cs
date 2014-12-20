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
                this.pdfList.Add( mc.Name );
            }

            return;
        }

        public override void Remove(MenuComponent mc)
        {
            base.Remove( mc );

            // We need to keep the PDF list in sync
            if ( mc is PdfFile ) {
                this.pdfList.Remove( mc.Name );
            }

            return;
        }

        public override void RemoveAt(int index)
        {
            var pdfmc = this.MenuComponents[ index ] as PdfFile;
            base.RemoveAt( index );

            // We need to keep the PDF list in sync
            if ( pdfmc != null ) {
                this.pdfList.Remove( pdfmc.Name );
            }

            return;
        }

        /// <summary>
        /// Gets the PDF list.
        /// </summary>
        /// <value>A vector of string.</value>
        public string[] PDFNameList {
            get {
                var toret = new string[ this.pdfList.Count ];

                this.pdfList.CopyTo( toret, 0 );
                return toret;
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

