using System;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace RWABuilder.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
    public class CsvEditor: Form {
		public const char DataSeparator = ',';
		public const char DecimalSeparator = '.';

		public CsvEditor()
			:this(new string[0])
		{
		}

		public CsvEditor(string[] data)
        {
            this.Build();

			// Load input data in control
			for(int i = 0; i < data.Length; ++i) {
				this.edData.Text += data[ i ];

				if ( i < data.Length - 1 ) {
					this.edData.Text += DataSeparator;
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
				throw new ArgumentException( "Unable to load embedded icons" );
			}

			this.Icon = Icon.FromHandle( appIconBmp.GetHicon() );
		}


        private void BuildToolbar()
        {
            var quitAction = UserAction.LookUp( "quit" );
            var saveAction = UserAction.LookUp( "save" );
            this.checkAction = UserAction.LookUp( "verify" );

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

            this.tbbCheck = new ToolStripButton();
            this.tbbCheck.ImageIndex = this.checkAction.ImageIndex;
            this.tbbCheck.ToolTipText = this.checkAction.Text;
            this.tbbCheck.Click += (sender, e) =>  this.OnCheckData();

            this.checkAction.ClearComponents();
            this.checkAction.AddComponent( this.tbbCheck );

            this.tbToolbar.Items.Add( tbbQuit );
            this.tbToolbar.Items.Add( tbbSave );
            this.tbToolbar.Items.Add( tbbCheck );

            this.checkAction.Disable();
        }

        private void BuildEditor()
        {
            this.edData = new TextBox();
            this.edData.Dock = DockStyle.Fill;
            this.edData.Multiline = true;
            this.edData.Font = new Font( FontFamily.GenericMonospace, 10 );
            this.edData.KeyUp += (sender, e) => {
				this.data = null;
                this.checkAction.Enabled = ( this.edData.Text.Length > 0 );
            };

			return;
        }

		private void BuildOptionsPanel()
		{
			this.pnlOptions = new FlowLayoutPanel();
			this.pnlOptions.SuspendLayout();
			this.pnlOptions.Dock = DockStyle.Bottom;

			// Components
			var lblSeparator = new Label();
			lblSeparator.TextAlign = ContentAlignment.MiddleCenter;
			lblSeparator.Text = "Separator";

			var lblDecimal = new Label();
			lblDecimal.TextAlign = ContentAlignment.MiddleCenter;
			lblDecimal.Text = "Decimal";

			this.cbSeparator = new ComboBox();
			this.cbSeparator.Font = new Font( FontFamily.GenericMonospace, 12 );
			this.cbSeparator.Items.AddRange( new string[] { ",", ";" } );
			this.cbSeparator.SelectedIndex = 0;
			this.cbSeparator.SelectedValueChanged += (sender, e) => this.data = null;

			this.cbDecimal = new ComboBox();
			this.cbDecimal.Font = new Font( FontFamily.GenericMonospace, 12 );
			this.cbDecimal.Items.AddRange( new string[] { ".", "," } );
			this.cbDecimal.SelectedIndex = 0;
			this.cbDecimal.SelectedValueChanged += (sender, e) => this.data = null;

			// Add them to the panel
			this.pnlOptions.Controls.Add( lblSeparator );
			this.pnlOptions.Controls.Add( cbSeparator );
			this.pnlOptions.Controls.Add( lblDecimal );
			this.pnlOptions.Controls.Add( cbDecimal );
			this.pnlOptions.ResumeLayout( false );

			// Sizes for controls
			Graphics grf = new Form().CreateGraphics();
			SizeF fontSize = grf.MeasureString( "W", this.cbSeparator.Font );
			int charWidth = (int) fontSize.Width + 5;
			this.cbSeparator.MaximumSize = new Size( charWidth * 2, this.cbSeparator.Height );
			this.cbDecimal.MaximumSize = new Size( charWidth * 2, this.cbDecimal.Height );
			this.pnlOptions.MaximumSize = new Size( int.MaxValue, this.cbDecimal.Height + 5 );
		}

        private void Build()
        {
            var mainPanel = new Panel();
            mainPanel.SuspendLayout();
			mainPanel.Dock = DockStyle.Fill;

			this.BuildIcon();
			this.BuildToolbar();
            this.BuildEditor();
			this.BuildOptionsPanel();

			mainPanel.Controls.Add( this.edData );
            mainPanel.Controls.Add( this.tbToolbar );
			mainPanel.Controls.Add( this.pnlOptions );
            mainPanel.ResumeLayout( false );

			this.Controls.Add( mainPanel );
			this.MinimumSize = new Size( 320, 240 );
			this.Text = "CSV Editor";
			this.StartPosition = FormStartPosition.CenterParent;
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

		private void ShowData()
		{
			var dlgData = new Form();
			dlgData.Text = "Data";
			dlgData.StartPosition = FormStartPosition.CenterParent;
			dlgData.MinimumSize = new Size( 200, 150 );
			dlgData.Size = dlgData.MinimumSize;
			dlgData.Icon = this.Icon;

			var lbData = new ListBox();
			lbData.Dock = DockStyle.Fill;
			lbData.Items.AddRange( this.Data );

			dlgData.Controls.Add( lbData );
			dlgData.ShowDialog();
		}

        /// <summary>
        /// Checks the data entered in the editor.
        /// </summary>
        public void OnCheckData()
        {
			if ( this.Decimal == this.Separator ) {
				MessageBox.Show( "Decimal and data separator should be different.",
				                 "Error",
				                 MessageBoxButtons.OK,
				                 MessageBoxIcon.Error );
			} else {
				foreach (string value in this.Data) {
					double d;
					bool isNumber = double.TryParse(
										value,
										NumberStyles.Float,
										CultureInfo.InvariantCulture,
										out d );

					if ( !isNumber ) {
						MessageBox.Show( "Is not a valid number: " + value,
						                 "Error",
						                 MessageBoxButtons.OK,
						                 MessageBoxIcon.Error );
						break;
					}
				}

				this.ShowData();
			}

			return;
        }

		private void CreateData()
		{
			char dataSeparator = this.Separator;
			char decimalSeparator = this.Decimal;
			string data = this.edData.Text.Trim();

			// Normalize data
			this.data = data.Split( dataSeparator );
			for (int i = 0; i < this.data.Length; ++i ) {
				string value = this.data[ i ].Trim().ToLower();

				if ( value.Length == 0 ) {
					value = "0";
				}
				else
				if ( DecimalSeparator != decimalSeparator ) {
					value = value.Replace( decimalSeparator, DecimalSeparator );
				}

				this.data[ i ] = value;
			}
		}

		/// <summary>
		/// Gets the decimal separator chosen (default: '.').
		/// </summary>
		/// <value>The decimal, as a char.</value>
		public char Decimal {
			get {
				char toret = DecimalSeparator;
				string strDecimalSeparator = this.cbDecimal.Text;

				if ( !string.IsNullOrWhiteSpace( strDecimalSeparator ) ) {
					strDecimalSeparator = strDecimalSeparator.Trim();

					if ( strDecimalSeparator.Length > 0 ) {
						toret = strDecimalSeparator[ 0 ];
					}
				}

				return toret;
			}
		}

		/// <summary>
		/// Gets the data separator chosen (default: ',').
		/// </summary>
		/// <value>The separator, as a char.</value>
		public char Separator {
			get {
				char toret = DataSeparator;
				string strDataSeparator = this.cbSeparator.Text;

				if ( !string.IsNullOrWhiteSpace( strDataSeparator ) ) {
					strDataSeparator = strDataSeparator.Trim();

					if ( strDataSeparator.Length > 0 ) {
						toret = strDataSeparator[ 0 ];
					}
				}

				return toret;
			}
		}

		/// <summary>
		/// Gets the input data.
		/// </summary>
		/// <value>The data, as a string[].</value>
		public string[] Data {
			get {
				if ( this.data == null ) {
					this.CreateData();
				}

				return this.data;
			}
		}

		private FlowLayoutPanel pnlOptions;
        private ToolStrip tbToolbar;
        private ToolStripButton tbbQuit;
        private ToolStripButton tbbSave;
        private ToolStripButton tbbCheck;
        private TextBox edData;
		private ComboBox cbSeparator;
		private ComboBox cbDecimal;

        private UserAction checkAction;
		private string[] data;
    }


}

