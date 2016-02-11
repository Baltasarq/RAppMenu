using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using RWABuilder.Core;

namespace RWABuilder.Ui {
	/// <summary>
	/// A windows for editing properties.
	/// </summary>
	public class PropertiesWindow: Form {
		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.PropertiesWindow"/> class.
		/// </summary>
		/// <param name="doc">A MenuDesign object, to be edited.</param>
		/// <param name="icon">The icon of the main application.</param>
		public PropertiesWindow(MenuDesign doc, Icon icon)
		{
			this.document = doc;
			this.Build( icon );
		}

		private void BuildToolbar()
		{
			var quitAction = UserAction.LookUp( "quit" );
			var saveAction = UserAction.LookUp( "save" );

			this.tbToolbar = new ToolStrip();
			this.tbToolbar.BackColor = Color.DarkGray;
			this.tbToolbar.Dock = DockStyle.Top;
			this.tbToolbar.ImageList = UserAction.ImageList;

			// Buttons
			this.tbbQuit = new ToolStripButton();
			this.tbbQuit.ImageIndex = quitAction.ImageIndex;
			this.tbbQuit.ToolTipText = quitAction.Text;
			this.tbbQuit.Click += (sender, e) => {
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			};

			this.tbbSave = new ToolStripButton();
			this.tbbSave.ImageIndex = saveAction.ImageIndex;
			this.tbbSave.ToolTipText = saveAction.Text;
			this.tbbSave.Click += (sender, e) =>  {
				this.DialogResult = DialogResult.OK;
				this.Close();
			};

			this.tbToolbar.Items.Add( tbbQuit );
			this.tbToolbar.Items.Add( tbbSave );
		}

		private void BuildMetaPanel()
		{
			this.pnlMeta = new GroupBox();
            this.pnlMeta.AutoSize = true;
            this.pnlMeta.Margin = new Padding( 5 );
			this.pnlMeta.Dock = DockStyle.Fill;
			this.pnlMeta.Font = new Font( this.pnlMeta.Font, FontStyle.Bold );
			this.pnlMeta.SuspendLayout();

			var pnlSubPanel = new TableLayoutPanel();
            pnlSubPanel.AutoSize = true;
			pnlSubPanel.Dock = DockStyle.Fill;
			pnlSubPanel.SuspendLayout();

			// Create e.mail sub-panel
			var pnlEmail = new Panel(){ Dock = DockStyle.Top };
            pnlEmail.Margin = new Padding( 5 );
			var lblEmail = new Label(){
				Text = "Author e.mail",
				Dock = DockStyle.Left,
				Font = new Font( Font, FontStyle.Regular ) 
			};
			var edEmail = new TextBox() { Dock = DockStyle.Fill };
			pnlEmail.Controls.Add( edEmail );
			pnlEmail.Controls.Add( lblEmail );
            pnlEmail.MaximumSize = new Size( int.MaxValue, edEmail.Height );

            // Load email's data & event
            edEmail.Text = this.Document.AuthorEmail;
            edEmail.TextChanged += (sender, e) => this.Document.AuthorEmail = edEmail.Text.Trim();

			// Create date sub-panel
			var pnlDate = new Panel(){ Dock = DockStyle.Top };
            pnlDate.Margin = new Padding( 5 );
			var lblDate = new Label(){
				Text = "Modified",
				Dock = DockStyle.Left,
				Font = new Font( Font, FontStyle.Regular )
			};
			var edDate = new DateTimePicker(){
				Dock = DockStyle.Fill,
				Format = DateTimePickerFormat.Short
			};
			pnlDate.Controls.Add( edDate );
			pnlDate.Controls.Add( lblDate );
            pnlDate.MaximumSize = new Size( int.MaxValue, edDate.Height );

			// Load date's data & event
			if ( this.Document.Date > default(DateTime) ) {
				edDate.Value = this.Document.Date;
			} else {
				edDate.Value = DateTime.Now;
			}
			edDate.ValueChanged += (sender, e) => this.Document.Date = edDate.Value;

			// Create the source code sub panel
			var pnlSource = new Panel(){ Dock = DockStyle.Top };
			pnlSource.Margin = new Padding( 5 );
			var lblSource = new Label() {
				Text = "Package source",
				Font = new Font( lblDate.Font, FontStyle.Regular ),
				Dock = DockStyle.Left
			};
			var btSource = new Button() {
				ImageList = UserAction.ImageList,
				ImageIndex = UserAction.LookUp( "open" ).ImageIndex,
				Dock = DockStyle.Right,
				MaximumSize = new Size( 24, 24 )
			};
			var edSource = new TextBox() { ReadOnly = true, Dock = DockStyle.Fill };
			pnlSource.Controls.Add( edSource );
			pnlSource.Controls.Add( lblSource );
			pnlSource.Controls.Add( btSource );
			pnlSource.MaximumSize = new Size( int.MaxValue, btSource.Height );

			// Load source code's data & event
			edSource.Text = Path.GetFileName( this.Document.SourceCodePath );
			btSource.Click += (sender, e) => {
				var fileDlg = new OpenFileDialog() {
					Title = "Path to package source",
					Filter = "Tar.gz files|*.tar.gz|Tgz files|*.tgz|Zip files|*.zip|All files|*"
				};

				if ( fileDlg.ShowDialog() == DialogResult.OK ) {
					this.document.SourceCodePath = fileDlg.FileName;
					edSource.Text = Path.GetFileName( fileDlg.FileName );
				}
			};

			// Create the windows binaries sub panel
			var pnlDocs = new Panel(){ Dock = DockStyle.Top };
			pnlDocs.Margin = new Padding( 5 );
			var lblDocs = new Label() {
				Text = "Windows binaries",
				Font = new Font( lblDate.Font, FontStyle.Regular ),
				Dock = DockStyle.Left
			};
			var btDocs = new Button() {
				ImageList = UserAction.ImageList,
				ImageIndex = UserAction.LookUp( "open" ).ImageIndex,
				Dock = DockStyle.Right,
				MaximumSize = new Size( 24, 24 )
			};
			var edDocs = new TextBox() { ReadOnly = true, Dock = DockStyle.Fill };
			pnlDocs.Controls.Add( edDocs );
			pnlDocs.Controls.Add( lblDocs );
			pnlDocs.Controls.Add( btDocs );
			pnlDocs.MaximumSize = new Size( int.MaxValue, btDocs.Height );

			// Load windows binaries's data & event
			edDocs.Text = Path.GetFileName( this.Document.WindowsBinariesPath );
			btDocs.Click += (sender, e) => {
				var fileDlg = new OpenFileDialog() {
					Title = "Path to windows binaries",
					Filter = "Zip files|*.zip|Targ.gz files|*.tar.gz|Tgz files|*.tgz|All files|*"
				};

				if ( fileDlg.ShowDialog() == DialogResult.OK ) {
					this.document.WindowsBinariesPath = fileDlg.FileName;
					edDocs.Text = Path.GetFileName( fileDlg.FileName );
				}
			};

			// Create the required packages sub panel
			var pnlReqPacks = new Panel(){ Dock = DockStyle.Top, Margin = new Padding( 5 ) };
			var lblReqPacks = new Label() {
				Text = "Required packages",
				Font = new Font( lblDate.Font, FontStyle.Regular ),
				Dock = DockStyle.Left
			};
			var btReqPacks = new Button() {
				ImageList = UserAction.ImageList,
				ImageIndex = UserAction.LookUp( "open" ).ImageIndex,
				Dock = DockStyle.Right,
				MaximumSize = new Size( 24, 24 )
			};
			var edReqPacks = new TextBox() { ReadOnly = true, Dock = DockStyle.Fill };
			pnlReqPacks.Controls.Add( edReqPacks );
			pnlReqPacks.Controls.Add( lblReqPacks );
			pnlReqPacks.Controls.Add( btReqPacks );
			pnlReqPacks.MaximumSize = new Size( int.MaxValue, btReqPacks.Height );

			// Load windows binaries's data & event
			edReqPacks.Text = Path.GetFileName( this.Document.RequiredPackagesPath );
			btReqPacks.Click += (sender, e) => {
				var fileDlg = new OpenFileDialog() {
					Title = "Path to required packages",
					Filter = "Txt files|*.txt|All files|*"
				};

				if ( fileDlg.ShowDialog() == DialogResult.OK ) {
					this.document.RequiredPackagesPath = fileDlg.FileName;
					edReqPacks.Text = Path.GetFileName( fileDlg.FileName );
				}
			};

			// Compose
			pnlSubPanel.Controls.Add( pnlEmail );
			pnlSubPanel.Controls.Add( pnlDate );
			pnlSubPanel.Controls.Add( pnlSource );
			pnlSubPanel.Controls.Add( pnlDocs );
			pnlSubPanel.Controls.Add( pnlReqPacks );
			this.pnlMeta.Controls.Add( pnlSubPanel );
			pnlSubPanel.ResumeLayout( false );
			this.pnlMeta.ResumeLayout( false );
			this.pnlMeta.Text = "App meta info";
		}

		/// <summary>
		/// Builds the necessary widgets.
		/// </summary>
		/// <param name="icon">Icon.</param>
		private void Build(Icon icon)
		{
			this.BuildToolbar();
			this.BuildMetaPanel();

			// Compose
			this.Controls.Add( this.pnlMeta );
			this.Controls.Add( this.tbToolbar );

			// Polish
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimizeBox = this.MaximizeBox = false;
			this.Icon = icon;
			this.Text = AppInfo.Name + " app properties";
			this.MinimumSize = new Size( this.pnlMeta.Size.Width + 20, this.pnlMeta.Size.Height + 20 );
			this.Size = this.MinimumSize;
		}

		/// <summary>
		/// Gets the document this preview window is showing.
		/// </summary>
		/// <value>The <see cref="Document"/> object.</value>
		public MenuDesign Document {
			get {
				return this.document;
			}
		}

		private MenuDesign document;

		private ToolStrip tbToolbar;
		private GroupBox pnlMeta;
		private ToolStripButton tbbQuit;
		private ToolStripButton tbbSave;
	}
}

