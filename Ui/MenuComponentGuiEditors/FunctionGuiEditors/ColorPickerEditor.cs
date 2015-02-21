using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RWABuilder.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
	public class ColorPickerEditor: Form {
		public const char Separator = ',';

		public ColorPickerEditor(string strColors)
		{
			this.addColorAction = UserAction.LookUp( "addcolor" );
			this.removeColorAction = UserAction.LookUp( "removecolor" );
			this.colors = new List<Color>();
			this.DecodeColors( strColors );

			this.Build();
			this.OnResize( null );
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown( e );

			this.addColorAction.CallBack = this.OnAddColor;
			this.removeColorAction.CallBack = this.OnRemoveColor;

			this.addColorAction.Enabled = true;
			this.removeColorAction.Enabled = ( this.colors.Count > 0 );

			this.Populate();
		}

		private void DecodeColors(string strColor)
		{
			string[] strColors = strColor.Trim().Split( Separator );

			foreach (string sc in strColors) {
				Color c = new Color();

				this.colors.Add( c );
			}

			return;
		}

		private void Populate()
		{
			foreach (Color c in this.colors) {
				this.grdColors.Rows.Add( c.ToString() );
			}

			return;
		}

		private void BuildIcon()
		{
			Bitmap appIconBmp;
			System.Reflection.Assembly entryAssembly;

			try {
				entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

				appIconBmp = new Bitmap(
					entryAssembly.GetManifestResourceStream( "RWABuilder.Res.appIcon.png" )
				);

				this.paletteIconBmp = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly().
					GetManifestResourceStream( "RWABuilder.Res.palette.png" )
				);

			} catch (Exception) {
				throw new ArgumentException( "Unable to load embedded app icon" );
			}

			this.Icon = Icon.FromHandle( appIconBmp.GetHicon() );
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing( e );

			if ( this.DialogResult != DialogResult.OK ) {
				DialogResult result = MessageBox.Show(
					"Changes will be lost. Are you sure?",
					"Discard changes",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Information,
					MessageBoxDefaultButton.Button2 );

				if ( result == DialogResult.No ) {
					e.Cancel = true;
				}
			}

			return;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize( e );

			// FnCall: Name
			this.grdColors.Columns[ 0 ].Width = (int) ( this.pnlColorList.Size.Width * 0.85 );

			// FnCall: Function name
            this.grdColors.Columns[ 1 ].Width = (int) ( this.pnlColorList.Size.Height * 0.15 );
		}

		private void BuildColorDialog()
		{
			this.colorDialog = new ColorDialog();
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

		private void BuildColorTable()
		{
			this.pnlColorList = new Panel();
            this.pnlColorList.AutoSize = true;
			this.pnlColorList.SuspendLayout();
			this.pnlColorList.Dock = DockStyle.Fill;

			this.grdColors = new DataGridView();
			this.grdColors.BackgroundColor = Color.White;
			this.grdColors.AllowUserToResizeRows = false;
			this.grdColors.RowHeadersVisible = false;
			this.grdColors.AutoGenerateColumns = false;
			this.grdColors.AllowUserToAddRows = false;
			this.grdColors.MultiSelect = false;
			this.grdColors.Dock = DockStyle.Fill;
			this.grdColors.AllowUserToOrderColumns = false;

			var imageCellTemplate = new DataGridViewImageCell();
			imageCellTemplate.ToolTipText = "Select color";
			imageCellTemplate.Value = this.paletteIconBmp;

			var textCellTemplateMonoSpaced = new DataGridViewTextBoxCell();
			textCellTemplateMonoSpaced.Style.BackColor = Color.Wheat;
			textCellTemplateMonoSpaced.Style.Font = new Font( FontFamily.GenericMonospace, 8 );

			var column0 = new DataGridViewTextBoxColumn();
			var column1 = new DataGridViewImageColumn();

			column0.HeaderText = "Color hex code";
			column0.CellTemplate = textCellTemplateMonoSpaced;
			column0.Width = 120;
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
			column0.ReadOnly = true;
			column1.HeaderText = "";
			column1.CellTemplate = imageCellTemplate;
			column1.Width = 120;
			column1.SortMode = DataGridViewColumnSortMode.NotSortable;

			this.grdColors.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
			} );

			this.grdColors.CellClick +=
				(object sender, DataGridViewCellEventArgs evt) => {
					if ( evt.ColumnIndex == 1 ) {
						this.OnColorEdited( evt.RowIndex );
					}
			};

			// Buttons panel
			var toolTips = new ToolTip();
			this.pnlButtons = new FlowLayoutPanel();
			this.pnlButtons.Dock = DockStyle.Bottom;
            this.pnlButtons.AutoSize = true;
			this.btAddColor = new Button();
			this.btRemoveColor = new Button();

			this.btAddColor.Size = this.btAddColor.MinimumSize = 
				this.btAddColor.MaximumSize = new Size( 32, 32 );
			this.btAddColor.ImageList = UserAction.ImageList;
			this.btAddColor.ImageIndex = this.addColorAction.ImageIndex;
			this.btAddColor.Click += (sender, e) => this.addColorAction.CallBack();
			toolTips.SetToolTip( this.btAddColor, this.addColorAction.Text );

			this.btRemoveColor.Size = this.btRemoveColor.MinimumSize = 
				this.btRemoveColor.MaximumSize = new Size( 32, 32 );
			this.btRemoveColor.ImageList = UserAction.ImageList;
			this.btRemoveColor.ImageIndex = this.removeColorAction.ImageIndex;
			this.btRemoveColor.Click += (sender, e) => this.removeColorAction.CallBack();
			toolTips.SetToolTip( this.btRemoveColor, this.removeColorAction.Text );

            this.pnlButtons.Controls.Add( this.btAddColor );
            this.pnlButtons.Controls.Add( this.btRemoveColor );

            // Finish
			this.pnlColorList.Controls.Add( this.grdColors );
			this.pnlColorList.Controls.Add( this.pnlButtons );
            this.pnlColorList.ResumeLayout( false );
		}

		private void BuildEditorPanel()
		{
			this.pnlColors = new GroupBox();
            this.pnlColors.AutoSize = true;
			this.pnlColors.Dock = DockStyle.Fill;
			this.pnlEditor = new TableLayoutPanel();
			this.pnlEditor.Dock = DockStyle.Fill;
            this.pnlEditor.AutoSize = true;
			this.pnlColors.SuspendLayout();
			this.pnlEditor.SuspendLayout();

			this.pnlColors.Text = "Colors";
			this.pnlColors.Padding = new Padding( 5 );
			this.pnlColors.Font = new Font( this.pnlColors.Font, FontStyle.Bold );
			this.pnlEditor.Font = new Font( this.pnlEditor.Font, FontStyle.Regular );

			this.pnlColors.ResumeLayout( false );
			this.pnlEditor.ResumeLayout( false );
			this.pnlColors.Controls.Add( this.pnlEditor );
		}

		private void Build()
		{
			this.BuildIcon();
			this.BuildColorDialog();
			this.BuildToolbar();
			this.BuildColorTable();
			this.BuildEditorPanel();

			// Add
            this.pnlEditor.Controls.Add( this.pnlColorList );
			this.Controls.Add( this.pnlColors );
			this.Controls.Add( this.tbToolbar );

			// Polish
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimizeBox = this.MaximizeBox = false;
			this.Text = "Colors editor";
            this.MinimumSize = new Size( 450, 400 );
		}

		private bool GetColorFromUser(ref Color c)
		{
			this.colorDialog.Color = c;
			bool toret = ( this.colorDialog.ShowDialog() == DialogResult.OK );

			if ( toret ) {
				c = this.colorDialog.Color;
			}

			return toret;
		}

		/// <summary>
		/// Gets the current row.
		/// </summary>
		/// <returns>The current row, as an int.</returns>
		private int GetCurrentRow()
		{
			int toret = 0;
			DataGridViewCell cell = this.grdColors.CurrentCell;

			if ( cell != null ) {
				toret = cell.RowIndex;
			}

			return toret;
		}

		private void OnColorEdited(int row)
		{
			Color c = new Color();

			this.GetColorFromUser( ref c );
		}

		private void OnAddColor()
		{
		}

		private void OnRemoveColor()
		{
		}

		public override string ToString()
		{
			StringBuilder toret = new StringBuilder();

			foreach (Color c in this.colors) {
				toret.Append( c.ToString() );
				toret.Append( ',' );
			}

			toret.Remove( toret.Length - 1, 1 );
			return toret.ToString();
		}

		/// <summary>
		/// Gets the colors.
		/// </summary>
		/// <value>The colors.</value>
		public Color[] Colors {
			get {
				return this.colors.ToArray();
			}
		}

		private DataGridView grdColors;
		private ColorDialog colorDialog;
		private Panel pnlEditor;
		private Panel pnlColorList;
		private GroupBox pnlColors;
		private FlowLayoutPanel pnlButtons;
		private ToolStrip tbToolbar;
		private ToolStripButton tbbQuit;
		private ToolStripButton tbbSave;
		private Button btAddColor;
		private Button btRemoveColor;

		private Bitmap paletteIconBmp;

		private UserAction addColorAction;
		private UserAction removeColorAction;

		private List<Color> colors;
	}
}
