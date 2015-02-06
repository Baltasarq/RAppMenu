using System;
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
			this.pnlMeta.Dock = DockStyle.Fill;
			this.pnlMeta.Font = new Font( this.pnlMeta.Font, FontStyle.Bold );
			this.pnlMeta.SuspendLayout();

			var pnlSubPanel = new TableLayoutPanel();
			pnlSubPanel.Dock = DockStyle.Fill;
			pnlSubPanel.SuspendLayout();

			// Create e.mail sub-panel
			var pnlEmail = new Panel();
			var lblEmail = new Label();
			var edEmail = new TextBox();
			lblEmail.Text = "Author e.mail";
			lblEmail.Font = new Font( lblEmail.Font, FontStyle.Regular );
			pnlEmail.Dock = DockStyle.Top;
			edEmail.Dock = DockStyle.Fill;
			lblEmail.Dock = DockStyle.Left;
			pnlEmail.Controls.Add( edEmail );
			pnlEmail.Controls.Add( lblEmail );

			// Create date sub-panel
			var pnlDate = new Panel();
			var lblDate = new Label();
			var edDate = new DateTimePicker();
			edDate.Format = DateTimePickerFormat.Short;
			lblDate.Text = "Date of modification";
			lblDate.Font = new Font( lblDate.Font, FontStyle.Regular );
			edDate.Dock = DockStyle.Fill;
			lblDate.Dock = DockStyle.Left;
			pnlDate.Controls.Add( edDate );
			pnlDate.Controls.Add( lblDate );

			// Compose
			pnlSubPanel.Controls.Add( pnlEmail );
			pnlSubPanel.Controls.Add( pnlDate );
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
			this.MinimumSize = this.pnlMeta.Size;
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

