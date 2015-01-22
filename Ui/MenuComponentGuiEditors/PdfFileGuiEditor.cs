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
			this.edFileName.Text = mc.Name;
		}

		public override void Show()
		{
			base.Show();
			this.pnlEdFileName.Show();
		}

		private void Build()
		{
            this.OnBuilding = true;
            this.Panel.SuspendLayout();
			var tooltipManager = new ToolTip();
			this.pnlEdFileName = new Panel();
			this.pnlEdFileName.SuspendLayout();
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

			this.pnlEdFileName.ResumeLayout( false );
			this.pnlEdFileName.MaximumSize = new Size( int.MaxValue, this.btFileName.Height );
			this.Panel.Controls.Add( this.pnlEdFileName );
            this.Panel.ResumeLayout( false );
            this.OnBuilding = false;
		}

		private void OnFileNameButtonClicked()
		{
			var pmc = (PdfFile) this.MenuComponent;
			var dlg = new OpenFileDialog();

			dlg.InitialDirectory = AppInfo.PdfFolder;
			dlg.CheckFileExists = true;
			dlg.DefaultExt = "pdf";
			dlg.Filter = "PDF|*.pdf|All files|*";

			if ( dlg.ShowDialog() == DialogResult.OK ) {
				string fileName = Path.GetFileName( dlg.FileName );

				this.edFileName.Text = fileName;
				pmc.Name = fileName;
				this.MenuComponentTreeNode.Text = fileName;
				AppInfo.PdfFolder = Path.GetDirectoryName( dlg.FileName );
			}

			return;
		}

		/// <summary>
		/// Reads the data from component.
		/// Stores the filename info in the edFileName control.
		/// </summary>
		public override void ReadDataFromComponent()
		{
            if ( !this.OnBuilding ) {
    			string fileName = Path.GetFileName( this.MenuComponent.Name );

    			this.edFileName.Text = fileName;
    			this.MenuComponentTreeNode.Text = fileName;
            }

            return;
		}

		private Panel pnlEdFileName;
		private Label lblFileName;
		private Label edFileName;
		private Button btFileName;
	}
}

