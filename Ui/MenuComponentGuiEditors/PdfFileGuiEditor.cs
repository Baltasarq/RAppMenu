using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentGuiEditors {
	public class PdfFileGuiEditor: MenuComponentGuiEditor {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.ComponentGuiEditors.PdfFileGuiEditor"/> class.
		/// </summary>
		/// <param name="panel">A <see cref="Panel"/> object.</param>
		/// <param name="mctn">A <see cref="MenuComponentTreeNode"/> object.</param>
		/// <param name="mc">A <see cref="MenuComponent"/> object.</param>
		/// <param name="pdfFolder">The folder for storing pdf's, as a string.</param>
		/// <param name="graphsFolder">The folder for storing graphs, as a string.</param>
		public PdfFileGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
			: base( panel, mctn, mc )
		{
			this.Build();
		}

		public override void Show()
		{
			base.Show();
			this.pnlEdFileName.Show();
		}

		private void Build()
		{
			var tooltipManager = new ToolTip();
			this.pnlEdFileName = new Panel();
			this.pnlEdFileName.Dock = DockStyle.Top;

			this.lblFileName = new Label();
			this.lblFileName.AutoSize = false;
			this.lblFileName.TextAlign = ContentAlignment.MiddleLeft;
			this.lblFileName.Dock = DockStyle.Left;
			this.lblFileName.Text = "File name:";

			this.edFileName = new Label();
			this.edFileName.Font = new Font( this.edFileName.Font, FontStyle.Bold );
			this.edFileName.Dock = DockStyle.Fill;
			this.edFileName.TextAlign = ContentAlignment.MiddleLeft;
			this.edFileName.AutoSize = false;

			this.btFileName = new Button();
			this.btFileName.ImageList = UserAction.ImageList;
			tooltipManager.SetToolTip( this.btFileName, UserAction.LookUp( "open" ).Text );
			this.btFileName.ImageIndex = UserAction.LookUp( "open" ).ImageIndex;
			this.btFileName.Dock = DockStyle.Right;
			this.btFileName.MaximumSize = this.btFileName.Size = new Size( 32, 32 );
			this.btFileName.Click += (sender, e) => this.OnFileNameButtonClicked();

			this.pnlEdFileName.Controls.Add( this.edFileName );
			this.pnlEdFileName.Controls.Add( this.lblFileName );
			this.pnlEdFileName.Controls.Add( this.btFileName );

			// Polish
			this.pnlEdFileName.MaximumSize = new Size( int.MaxValue, this.btFileName.Height );
		}

		private void OnFileNameButtonClicked()
		{
			var pmc = this.MenuComponent as Core.MenuComponents.PdfFile;
			var imc = this.MenuComponent as Core.MenuComponents.ImageMenuEntry;

			if ( pmc != null ) {
				this.OnPdfFileNameButtonClicked( pmc );
			}
			else
				if ( imc != null ) {
				this.OnImageFileNameButtonClicked( imc );
			}

			return;
		}

		private void OnImageFileNameButtonClicked(Core.MenuComponents.ImageMenuEntry imc)
		{
			var dlg = new OpenFileDialog();
			dlg.InitialDirectory = this.GraphsFolder;
			dlg.CheckFileExists = true;
			dlg.DefaultExt = "png";
			dlg.Filter = "PDF|*.pdf|All files|*";

			if ( dlg.ShowDialog() == DialogResult.OK ) {
				string fileName = Path.GetFileName( dlg.FileName );

				this.edFileName.Text = fileName;
				imc.ImagePath = fileName;
				this.GraphsFolder = Path.GetDirectoryName( dlg.FileName );
			}

			return;
		}

		private void OnPdfFileNameButtonClicked(Core.MenuComponents.PdfFile pmc)
		{
			var dlg = new OpenFileDialog();
			dlg.InitialDirectory = this.PdfFolder;
			dlg.CheckFileExists = true;
			dlg.DefaultExt = "pdf";
			dlg.Filter = "PDF|*.pdf|All files|*";

			if ( dlg.ShowDialog() == DialogResult.OK ) {
				string fileName = Path.GetFileName( dlg.FileName );

				this.edFileName.Text = fileName;
				pmc.Name = fileName;
				this.PdfFolder = Path.GetDirectoryName( dlg.FileName );
			}

			return;
		}

		/// <summary>
		/// Gets or sets the pdf folder.
		/// </summary>
		/// <value>The pdf folder path, as a string.</value>
		public string PdfFolder {
			get; set;
		}

		/// <summary>
		/// Gets or sets the graphs folder.
		/// </summary>
		/// <value>The graphs folder, as a string.</value>
		public string GraphsFolder {
			get; set;
		}

		private Panel pnlEdFileName;
		private Label lblFileName;
		private Label edFileName;
		private Button btFileName;
	}
}

