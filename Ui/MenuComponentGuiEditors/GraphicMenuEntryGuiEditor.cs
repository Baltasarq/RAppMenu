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
            this.Build();
			this.functionEditor =
                new FunctionGuiEditor( this.pnlFunction, mctn, this.GraphicMenuEntry.Function );

			this.edFileName.Text = this.GraphicMenuEntry.ImagePath;
		}

		public override void Show()
		{
			base.Show();
           
            this.functionEditor.Show();
            this.pnlContainer.Show();
		}

		private void BuildFileNamePanel()
		{
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
		}

		private void BuildTooltipPanel()
		{
			this.pnlImageTooltip = new Panel();
			this.pnlImageTooltip.SuspendLayout();
			this.pnlImageTooltip.Dock = DockStyle.Top;

			this.lblTooltip = new Label();
			this.lblTooltip.AutoSize = false;
			this.lblTooltip.TextAlign = ContentAlignment.MiddleLeft;
			this.lblTooltip.Dock = DockStyle.Left;
			this.lblTooltip.Text = "Tool tip:";

			this.edTooltip = new TextBox();
			this.edTooltip.Font = new Font( this.edFileName.Font, FontStyle.Bold );
			this.edTooltip.Dock = DockStyle.Fill;
			this.edTooltip.KeyUp += (sender, e) => {
				this.GraphicMenuEntry.ImageToolTip = this.edTooltip.Text.Trim();
			};

			this.pnlImageTooltip.Controls.Add( this.edTooltip );
			this.pnlImageTooltip.Controls.Add( this.lblTooltip );

			this.pnlImageTooltip.ResumeLayout( false );
			this.pnlImageTooltip.MaximumSize = new Size( int.MaxValue, this.btFileName.Height );
		}

        private void BuildFunctionEditorPanel()
        {
            this.pnlFunction = new Panel();
            this.pnlFunction.AutoSize = true;
            this.pnlFunction.Dock = DockStyle.Fill;
        }

		private void Build()
		{
			this.Panel.SuspendLayout();

            this.pnlContainer = new TableLayoutPanel();
            this.pnlContainer.Dock = DockStyle.Fill;
            this.pnlContainer.AutoSize = true;
            this.pnlContainer.SuspendLayout();

            this.pnlFile = new GroupBox();
            this.pnlFile.Text = "Graphic file";
            this.pnlFile.Font = new Font( this.pnlFile.Font, FontStyle.Bold );
            this.pnlFile.SuspendLayout();
            this.pnlFile.Dock = DockStyle.Top;

            var pnlInnerFile = new TableLayoutPanel();
            pnlInnerFile.SuspendLayout();
            pnlInnerFile.Font = new Font( pnlInnerFile.Font, FontStyle.Regular );
            pnlInnerFile.Dock = DockStyle.Fill;

			this.BuildFileNamePanel();
			this.BuildTooltipPanel();
            this.BuildFunctionEditorPanel();

            pnlInnerFile.Controls.Add( this.pnlEdFileName );
            pnlInnerFile.Controls.Add( this.pnlImageTooltip );
            this.pnlFile.Controls.Add( pnlInnerFile );
            this.pnlContainer.Controls.Add( this.pnlFile );
            this.pnlContainer.Controls.Add( this.pnlFunction );
            this.Panel.Controls.Add( this.pnlContainer );

            pnlInnerFile.ResumeLayout( false );
            this.pnlFile.ResumeLayout( false );
            this.pnlContainer.ResumeLayout( false );
			this.Panel.ResumeLayout( false );
		}

		private void OnFileNameButtonClicked()
		{
			var dlg = new OpenFileDialog();
			dlg.InitialDirectory = AppInfo.GraphsFolder;
			dlg.CheckFileExists = true;
			dlg.DefaultExt = "png";
            dlg.Filter = "PNG|*.png|JPG|*.jpg|All files|*";

			if ( dlg.ShowDialog() == DialogResult.OK ) {
				string fileName = Path.GetFileName( dlg.FileName );

				this.edFileName.Text = fileName;
				this.GraphicMenuEntry.ImagePath = fileName;
				AppInfo.GraphsFolder = Path.GetDirectoryName( dlg.FileName );
			}

			return;
		}

		public override void ReadDataFromComponent()
		{
			this.edFileName.Text = this.GraphicMenuEntry.ImagePath;
			this.edTooltip.Text = this.GraphicMenuEntry.ImageToolTip;

            this.functionEditor.ReadDataFromComponent();
		}

		public GraphicMenuEntry GraphicMenuEntry {
			get {
				return (GraphicMenuEntry) this.MenuComponent;
			}
		}

        private GroupBox pnlFile;
        private TableLayoutPanel pnlContainer;
		private Panel pnlEdFileName;
		private Panel pnlImageTooltip;
        private Panel pnlFunction;
		private Label lblFileName;
		private Label edFileName;
		private Button btFileName;
		private Label lblTooltip;
		private TextBox edTooltip;

		private FunctionGuiEditor functionEditor;
	}
}

