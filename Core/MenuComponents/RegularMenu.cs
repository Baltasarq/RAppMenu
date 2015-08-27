using System;

namespace RWABuilder.Core.MenuComponents {
    public partial class RegularMenu: Menu {
        public RegularMenu(string name, Menu parent)
            : base( name, parent )
        {
        }

		protected RegularMenu(string name)
			:base( name )
		{
		}

        public override void Add(MenuComponent mc)
        {
            var pdfFile = mc as PdfFile;
			var grfMenu = mc as GraphicMenu;

            base.Add( mc );

            // Store the new PDF File in the general PDF list
            if ( pdfFile != null ) {
                this.Root.Owner.GetPDFList().Add( pdfFile );
            }

			// Store the new graphic menu in the general graphic menus list
			if ( grfMenu != null ) {
				this.Root.Owner.GetGraphicMenuList().Add( grfMenu );
			}

            return;
        }

        public override void Remove(MenuComponent mc)
        {
            var pdfFile = mc as PdfFile;
			var grfMenu = mc as GraphicMenu;

            base.Remove( mc );

            //Keep the PDF general list in sync
            if ( mc is PdfFile ) {
                this.Root.Owner.GetPDFList().Remove( pdfFile );
            }

			// Keep the graphic menu general list in sync
			if ( grfMenu != null ) {
				this.Root.Owner.GetGraphicMenuList().Remove( grfMenu );
			}

            return;
        }

        public override void RemoveAt(int index)
        {
            var pdfmc = this.MenuComponents[ index ] as PdfFile;
			var gmmc = this.MenuComponents[ index ] as GraphicMenu;

            base.RemoveAt( index );

            // Keep the PDF general list in sync
            if ( pdfmc != null ) {
                this.Root.Owner.GetPDFList().Remove( pdfmc );
            }

			// Keep the graphic menu general list in sync
			if ( gmmc != null ) {
				this.Root.Owner.GetGraphicMenuList().Remove( gmmc );
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

