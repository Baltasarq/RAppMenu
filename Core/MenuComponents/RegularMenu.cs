using System;

namespace RAppMenu.Core.MenuComponents {
    public partial class RegularMenu: Menu {
        public RegularMenu(string name, Menu parent)
            :base( name, parent )
        {
        }

        public override void Add(MenuComponent mc)
        {
            var pdfFile = mc as PdfFile;
            base.Add( mc );

            // We need to store the new PDF File in the list
            if (pdfFile != null ) {
                this.Root.Owner.GetPDFList().Add( pdfFile );
            }

            return;
        }

        public override void Remove(MenuComponent mc)
        {
            var pdfFile = mc as PdfFile;
            base.Remove( mc );

            // We need to keep the PDF list in sync
            if ( mc is PdfFile ) {
                this.Root.Owner.GetPDFList().Remove( pdfFile );
            }

            return;
        }

        public override void RemoveAt(int index)
        {
            var pdfmc = this.MenuComponents[ index ] as PdfFile;
            base.RemoveAt( index );

            // We need to keep the PDF list in sync
            if ( pdfmc != null ) {
                this.Root.Owner.GetPDFList().Remove( pdfmc );
            }

            return;
        }

        public override string ToString()
        {
            string toret = base.ToString();

            toret = "[RegularMenu " + toret;
            return toret + "]";
        }
    }
}

