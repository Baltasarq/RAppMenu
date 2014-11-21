using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Ui;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentGuiEditors {
	public class GraphicMenuEntryGuiEditor: MenuComponentGuiEditor {
		public GraphicMenuEntryGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
			: base( panel, mctn, mc )
		{
			var ime = (GraphicMenuEntry) mc;

			this.Build();
			this.functionEditor =
				new FunctionGuiEditor( this.Panel, mctn, ( (GraphicMenuEntry) mc ).Function );

			this.edFileName.Text = ime.ImagePath;
		}

		public override void Show()
		{
			base.Show();
			this.functionEditor.Show();
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

			this.pnlEdFileName.MaximumSize = new Size( int.MaxValue, this.btFileName.Height );
			this.Panel.Controls.Add( this.pnlEdFileName );
		}

		private void OnFileNameButtonClicked()
		{
			var imc = (GraphicMenuEntry) this.MenuComponent;
			var dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppInfo.GraphsFolder;
			dlg.CheckFileExists = true;
			dlg.DefaultExt = "png";
			dlg.Filter = "JPG|*.jpg|PNG|*.png|All files|*";

			if ( dlg.ShowDialog() == DialogResult.OK ) {
				string fileName = Path.GetFileName( dlg.FileName );

				this.edFileName.Text = fileName;
				imc.ImagePath = fileName;
				AppInfo.GraphsFolder = Path.GetDirectoryName( dlg.FileName );
			}

			return;
		}

		private Panel pnlEdFileName;
		private Label lblFileName;
		private Label edFileName;
		private Button btFileName;

		private FunctionGuiEditor functionEditor;
	}
}

