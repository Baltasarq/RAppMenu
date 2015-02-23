using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace RWABuilder.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
	public class ValuesChooser: Form {
		public ValuesChooser(string[] values)
		{
			Trace.WriteLine( "ValuesChooser: Booting dialog..." );

			if ( values == null
			  || values.Length < 1 )
			{
				Trace.Indent();
				Trace.WriteLine( "ValuesChooser: ERROR: null values" );
				Trace.Unindent();
				throw new ArgumentException( "null values" );
			}

			this.values = values;
			this.Build();
		}

		private void Populate()
		{
			this.lbValues.Items.Clear();
			this.lbValues.Items.AddRange( this.values );
			this.lbValues.SelectedIndex = 0;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown( e );

			this.Populate();
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

		private void BuildIcon()
		{
			Bitmap appIconBmp;
			System.Reflection.Assembly entryAssembly;

			try {
				entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

				appIconBmp = new Bitmap(
					entryAssembly.GetManifestResourceStream( "RWABuilder.Res.appIcon.png" )
					);

			} catch (Exception) {
				throw new ArgumentException( "Unable to load embedded app icon" );
			}

			this.Icon = Icon.FromHandle( appIconBmp.GetHicon() );
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

		private void BuildListBox()
		{
			this.pnlValues = new GroupBox();
			this.pnlValues.SuspendLayout();
			this.pnlValues.Dock = DockStyle.Fill;
			this.pnlValues.Padding = new Padding( 5 );
			this.pnlValues.Font = new Font( this.pnlValues.Font, FontStyle.Bold );
			this.pnlValues.Text = "Values";

			this.lbValues = new ListBox();
			this.lbValues.Dock = DockStyle.Fill;
			this.lbValues.Font = new Font( FontFamily.GenericMonospace, 12 );

			this.pnlValues.Controls.Add( this.lbValues );
			this.pnlValues.ResumeLayout( false );
		}

		private void Build()
		{
			this.BuildIcon();
			this.BuildToolbar();
			this.BuildListBox();

			// Add components
			this.Controls.Add( this.pnlValues );
			this.Controls.Add( this.tbToolbar );

			// Polish
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimizeBox = this.MaximizeBox = false;
			this.Text = "Values chooser";
			this.MinimumSize = new Size( 320, 240 );
		}

		/// <summary>
		/// Gets the index of the selected item.
		/// </summary>
		/// <returns>The selected index, as an int.</returns>
		public int GetSelectedIndex()
		{
			int toret = Math.Max( 0, this.lbValues.SelectedIndex );

			Trace.WriteLine( "ValuesChooser: selected index: " + toret );
			return toret;
		}

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <returns>The selected item, as string.</returns>
		public string GetSelectedItem()
		{
			string toret = this.values[ this.GetSelectedIndex() ];

			Trace.WriteLine( "ValuesChooser: selected index: " + toret );
			return toret;
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values, as string[].</value>
		public string[] Values {
			get {
				return this.values;
			}
		}

		private ToolStrip tbToolbar;
		private ToolStripButton tbbQuit;
		private ToolStripButton tbbSave;

		private GroupBox pnlValues;
		private ListBox lbValues;

		private string[] values;
	}
}

