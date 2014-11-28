using System;
using System.Drawing;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentGuiEditors {
	public class FunctionGuiEditor: NamedComponentGuiEditor {
		public FunctionGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
			: base( panel, mctn, mc )
		{
			this.addFunctionArgumentAction = UserAction.LookUp( "addfunctionargument" );
			this.removeFunctionArgumentAction = UserAction.LookUp( "removefunctionargument" );

			this.Build();
			
            // Put in some data
            this.chkFunctionHasData.Checked = this.Function.HasData;
            this.chkFunctionDataHeader.Checked = this.Function.DataHeader;
            this.chkFunctionRemoveQuotes.Checked = this.Function.RemoveQuotationMarks;

            this.edFunctionPreCommand.Text = this.Function.PreProgramOnce.ToString();
            this.edFunctionDefaultData.Text = this.Function.DefaultData;
		}

		/// <summary>
		/// Gets the function being modified by this editor
		/// </summary>
		/// <value>The <see cref="Function"/> object.</value>
		public Function Function {
			get {
				return (Function) this.MenuComponent;
			}
		}

		/// <summary>
		/// Shows and prepares the editor
		/// </summary>
		public override void Show()
		{
			base.Show();
            this.pnlContainer.Show();

			this.btFunctionRemoveArgument.Enabled = ( this.grdArgsList.Rows.Count > 0 );
			this.addFunctionArgumentAction.CallBack = this.OnAddFunctionArgument;
			this.removeFunctionArgumentAction.CallBack = this.OnRemoveFunctionArgument;
		}

		private void BuildArgumentsListTable()
		{
			var toolTips = new ToolTip();

			this.pnlArgsList = new GroupBox();
			this.pnlArgsList.SuspendLayout();
			this.pnlArgsList.Resize += (sender, e) => this.OnResizeArgsList();
			this.pnlArgsList.Dock = DockStyle.Fill;
			this.pnlArgsList.Text = "Arguments";

			this.grdArgsList = new DataGridView();
			this.grdArgsList.AllowUserToResizeRows = false;
			this.grdArgsList.RowHeadersVisible = false;
			this.grdArgsList.AutoGenerateColumns = false;
			this.grdArgsList.AllowUserToAddRows = false;
			this.grdArgsList.MultiSelect = false;
			this.grdArgsList.Dock = DockStyle.Fill;
			this.grdArgsList.AllowUserToOrderColumns = false;

			var textCellTemplate = new DataGridViewTextBoxCell();
			textCellTemplate.Style.BackColor = Color.Wheat;

			var comboBoxCellTemplate = new DataGridViewComboBoxCell();
			comboBoxCellTemplate.Style.BackColor = Color.AntiqueWhite;
			comboBoxCellTemplate.Items.AddRange( new string[] {
				"DataColumnsViewer",
				"DataValuesViewer",
				"Map",
				"TaxTree"
			});

			var checkBoxCellTemplate = new DataGridViewCheckBoxCell();
			checkBoxCellTemplate.Style.BackColor = Color.AntiqueWhite;

			var column0 = new DataGridViewTextBoxColumn();
			var column1 = new DataGridViewTextBoxColumn();
			var column2 = new DataGridViewTextBoxColumn();
			var column3 = new DataGridViewCheckBoxColumn();
			var column4 = new DataGridViewCheckBoxColumn();
			var column5 = new DataGridViewComboBoxColumn();

			column0.CellTemplate = textCellTemplate;
			column1.CellTemplate = textCellTemplate;
			column2.CellTemplate = textCellTemplate;
			column3.CellTemplate = checkBoxCellTemplate;
			column4.CellTemplate = checkBoxCellTemplate;
			column5.CellTemplate = comboBoxCellTemplate;

			column0.HeaderText = "Name";
			column0.Width = 120;
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
			column1.HeaderText = "Tag";
			column1.Width = 120;
			column1.SortMode = DataGridViewColumnSortMode.NotSortable;
			column2.HeaderText = "Depends";
			column2.Width = 80;
			column2.SortMode = DataGridViewColumnSortMode.NotSortable;
			column3.HeaderText = "Required";
			column3.Width = 80;
			column3.SortMode = DataGridViewColumnSortMode.NotSortable;
			column4.HeaderText = "Multiselect";
			column4.Width = 80;
			column4.SortMode = DataGridViewColumnSortMode.NotSortable;
			column5.HeaderText = "Viewer";
			column5.Width = 80;
			column5.SortMode = DataGridViewColumnSortMode.NotSortable;

			this.grdArgsList.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
				column2,
				column3,
				column4,
				column5,
			} );

			this.grdArgsList.CellEndEdit += (object sender, DataGridViewCellEventArgs evt) =>
				this.OnCellEdited( evt.RowIndex, evt.ColumnIndex );

			this.grdArgsList.Font = new Font( this.grdArgsList.Font, FontStyle.Regular );
			this.pnlArgsList.Font = new Font( this.pnlArgsList.Font, FontStyle.Bold );

			// Buttons panel
			this.pnlButtons = new FlowLayoutPanel();
			this.pnlButtons.AutoSize = true;
			this.pnlButtons.Dock = DockStyle.Bottom;
			this.btFunctionAddArgument = new Button();

			this.btFunctionAddArgument.Size = this.btFunctionAddArgument.MinimumSize = 
				this.btFunctionAddArgument.MaximumSize = new Size( 32, 32 );
			this.btFunctionAddArgument.ImageList = UserAction.ImageList;
			this.btFunctionAddArgument.ImageIndex = this.addFunctionArgumentAction.ImageIndex;
			this.btFunctionAddArgument.Click += (sender, e) => this.addFunctionArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionAddArgument, this.addFunctionArgumentAction.Text );

			this.btFunctionRemoveArgument = new Button();
			this.btFunctionRemoveArgument.Size = this.btFunctionRemoveArgument.MinimumSize = 
				this.btFunctionRemoveArgument.MaximumSize = new Size( 32, 32 );
			this.btFunctionRemoveArgument.ImageList = UserAction.ImageList;
			this.btFunctionRemoveArgument.ImageIndex = this.removeFunctionArgumentAction.ImageIndex;
			this.btFunctionRemoveArgument.Click += (sender, e) => this.removeFunctionArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionRemoveArgument, this.removeFunctionArgumentAction.Text );

            // Prepare
			this.addFunctionArgumentAction.AddComponent( this.btFunctionAddArgument );
			this.removeFunctionArgumentAction.AddComponent( this.btFunctionRemoveArgument );
			this.pnlButtons.Controls.Add( this.btFunctionAddArgument );
			this.pnlButtons.Controls.Add( this.btFunctionRemoveArgument );
			this.pnlArgsList.Controls.Add( this.grdArgsList );
			this.pnlArgsList.Controls.Add( this.pnlButtons );
            this.pnlContainer.Controls.Add( this.pnlArgsList );
			this.pnlArgsList.ResumeLayout( false );
			this.OnResizeArgsList();
		}

		private void BuildCheckBoxes()
		{
			this.pnlGroupChecks = new GroupBox();
			this.pnlGroupChecks.SuspendLayout();
            this.pnlGroupChecks.AutoSize = true;
			this.pnlGroupChecks.Text = "Options";
			this.pnlGroupChecks.Font = new Font( this.pnlGroupChecks.Font, FontStyle.Bold );
			this.pnlGroupChecks.Dock = DockStyle.Top;

			this.pnlChecks = new FlowLayoutPanel();
			this.pnlChecks.SuspendLayout();
            this.pnlChecks.AutoSize = true;
			this.pnlChecks.AutoSize = true;
			this.pnlChecks.Font = new Font( this.pnlChecks.Font, FontStyle.Regular );
			this.pnlChecks.Dock = DockStyle.Fill;
			this.pnlGroupChecks.Controls.Add( this.pnlChecks );
			this.pnlContainer.Controls.Add( this.pnlGroupChecks );

			this.chkFunctionHasData = new CheckBox();
			this.chkFunctionHasData.Text = "Has data";
			this.chkFunctionHasData.Dock = DockStyle.Fill;
			this.chkFunctionHasData.MinimumSize =
				new Size( this.chkFunctionHasData.Width, this.chkFunctionHasData.Height * 2 );
			this.chkFunctionHasData.CheckedChanged += (object sender, EventArgs e) =>
				this.Function.HasData = this.chkFunctionHasData.Checked;
			this.pnlChecks.Controls.Add( this.chkFunctionHasData );

			this.chkFunctionDataHeader = new CheckBox();
			this.chkFunctionDataHeader.Text = "Has data header";
			this.chkFunctionDataHeader.Dock = DockStyle.Fill;
			this.chkFunctionDataHeader.MinimumSize =
				new Size( this.chkFunctionDataHeader.Width, this.chkFunctionDataHeader.Height * 2 );
			this.chkFunctionDataHeader.CheckedChanged += (object sender, EventArgs e) =>
				this.Function.DataHeader = this.chkFunctionDataHeader.Checked;
			this.pnlChecks.Controls.Add( chkFunctionDataHeader );

			this.chkFunctionRemoveQuotes = new CheckBox();
			this.chkFunctionRemoveQuotes.Text = "Remove quotation marks";
			this.chkFunctionRemoveQuotes.Dock = DockStyle.Fill;
			this.chkFunctionRemoveQuotes.MinimumSize =
				new Size( this.chkFunctionRemoveQuotes.Width, this.chkFunctionRemoveQuotes.Height * 2 );
			this.chkFunctionRemoveQuotes.CheckedChanged += (object sender, EventArgs e) => 
				this.Function.RemoveQuotationMarks = this.chkFunctionRemoveQuotes.Checked;
			this.pnlChecks.Controls.Add( chkFunctionRemoveQuotes );

			this.pnlChecks.ResumeLayout( false );
			this.pnlGroupChecks.ResumeLayout( false );
		}

		private void BuildDefaultData()
		{
			this.pnlGroupDefaultData = new GroupBox();
			this.pnlGroupDefaultData.SuspendLayout();
			this.pnlGroupDefaultData.AutoSize = true;
			this.pnlGroupDefaultData.Dock = DockStyle.Top;
			this.pnlGroupDefaultData.Text = "Default data";
			this.pnlGroupDefaultData.Font = new Font( this.pnlGroupDefaultData.Font, FontStyle.Bold );

			var pnlInnerGroupDefaultData = new FlowLayoutPanel();
			pnlInnerGroupDefaultData.SuspendLayout();
			pnlInnerGroupDefaultData.Font = new Font( pnlInnerGroupDefaultData.Font, FontStyle.Regular );
			pnlInnerGroupDefaultData.Dock = DockStyle.Fill;
			pnlInnerGroupDefaultData.AutoSize = true;
			this.pnlGroupDefaultData.Controls.Add( pnlInnerGroupDefaultData );
			this.pnlContainer.Controls.Add( this.pnlGroupDefaultData );

			// Default data
			var lblDefaultData = new Label();
			lblDefaultData.Text = "Default data:";
			lblDefaultData.AutoSize = false;
			lblDefaultData.TextAlign = ContentAlignment.MiddleLeft;
			this.edFunctionDefaultData = new TextBox();
			this.edFunctionDefaultData.Font = new Font( this.edFunctionDefaultData.Font, FontStyle.Bold );
			this.edFunctionDefaultData.KeyUp += (sender, e) => {
				string contents = this.edFunctionDefaultData.Text.Trim();

				if ( string.IsNullOrEmpty( contents ) ) {
					this.Function.DefaultData = contents;
				}

				return;
			};
			pnlInnerGroupDefaultData.Controls.Add( lblDefaultData );
			pnlInnerGroupDefaultData.Controls.Add( this.edFunctionDefaultData );

			// Start column
			var lblStartColumn = new Label();
			lblStartColumn.Text = "Start column:";
			lblStartColumn.AutoSize = true;
			lblStartColumn.TextAlign = ContentAlignment.MiddleLeft;
			this.udFunctionStartColumn = new NumericUpDown();
			this.udFunctionStartColumn.TextAlign = HorizontalAlignment.Right;
			this.udFunctionStartColumn.Font = new Font( this.udFunctionStartColumn.Font, FontStyle.Bold );
			this.udFunctionStartColumn.Maximum = 99;
			this.udFunctionStartColumn.Minimum = 1;
			this.udFunctionStartColumn.ValueChanged += (sender, e) =>
				this.Function.StartColumn = (int) this.udFunctionStartColumn.Value;
			pnlInnerGroupDefaultData.Controls.Add( lblStartColumn );
			pnlInnerGroupDefaultData.Controls.Add( this.udFunctionStartColumn );

			// End column
			var lblEndColumn = new Label();
			lblEndColumn.Text = "End column:";
			lblEndColumn.AutoSize = true;
			lblEndColumn.TextAlign = ContentAlignment.MiddleLeft;
			this.udFunctionEndColumn = new NumericUpDown();
			this.udFunctionEndColumn.Font = new Font( this.udFunctionEndColumn.Font, FontStyle.Bold );
			this.udFunctionEndColumn.TextAlign = HorizontalAlignment.Right;
            this.udFunctionEndColumn.Maximum = 99;
            this.udFunctionEndColumn.Minimum = 1;
			this.udFunctionEndColumn.ValueChanged += (sender, e) =>
				this.Function.EndColumn = (int) this.udFunctionEndColumn.Value;
			pnlInnerGroupDefaultData.Controls.Add( lblEndColumn );
			pnlInnerGroupDefaultData.Controls.Add( this.udFunctionEndColumn );
			pnlInnerGroupDefaultData.ResumeLayout( false );
			this.pnlGroupDefaultData.ResumeLayout( false );

            // Sizes for controls
            Graphics grf = new Form().CreateGraphics();
            SizeF fontSize = grf.MeasureString( "W", this.udFunctionEndColumn.Font );
            int charWidth = (int) fontSize.Width + 5;
            this.udFunctionEndColumn.MaximumSize = new Size( charWidth * 2, this.udFunctionEndColumn.Height );
            this.udFunctionStartColumn.MaximumSize = new Size( charWidth * 2, this.udFunctionStartColumn.Height );
            this.edFunctionDefaultData.MinimumSize = new Size( charWidth * 12, this.edFunctionDefaultData.Height );
		}

		private void BuildCommands()
		{
			this.pnlGroupCommands = new GroupBox();
			this.pnlGroupCommands.SuspendLayout();
            this.pnlGroupCommands.AutoSize = true;
			this.pnlGroupCommands.Text = "Commands";
			this.pnlGroupCommands.Font = new Font( this.pnlGroupCommands.Font, FontStyle.Bold );
			this.pnlGroupCommands.Dock = DockStyle.Top;

			var pnlInnerGroupCommands = new TableLayoutPanel();
			pnlInnerGroupCommands.SuspendLayout();
            pnlInnerGroupCommands.AutoSize = true;
			pnlInnerGroupCommands.Dock = DockStyle.Fill;
			pnlInnerGroupCommands.AutoSize = true;
			pnlInnerGroupCommands.Font = new Font( pnlInnerGroupCommands.Font, FontStyle.Regular );

			this.pnlContainer.Controls.Add( this.pnlGroupCommands );
			this.pnlGroupCommands.Controls.Add( pnlInnerGroupCommands );

			this.pnlPreCommand = new Panel();
			this.pnlPreCommand.SuspendLayout();
			this.pnlPreCommand.Dock = DockStyle.Top;
			var lblPreCommand = new Label();
			lblPreCommand.Text = "Pre-Command:";
			lblPreCommand.AutoSize = false;
			lblPreCommand.TextAlign = ContentAlignment.MiddleLeft;
			lblPreCommand.Dock = DockStyle.Left;
			this.edFunctionPreCommand = new TextBox();
			this.edFunctionPreCommand.Dock = DockStyle.Fill;
            this.edFunctionPreCommand.Font = new Font( FontFamily.GenericMonospace, 10, FontStyle.Bold );
			this.edFunctionPreCommand.KeyUp += (sender, e) => {
				string contents = this.edFunctionPreCommand.Text.Trim();

				if ( string.IsNullOrEmpty( contents ) ) {
					this.Function.PreCommand = contents;
				}

				return;
			};
			this.pnlPreCommand.Controls.Add( this.edFunctionPreCommand );
			this.pnlPreCommand.Controls.Add( lblPreCommand );
			this.pnlPreCommand.MaximumSize = new Size( int.MaxValue, this.edFunctionPreCommand.Height );
			pnlInnerGroupCommands.Controls.Add( this.pnlPreCommand );

			this.pnlExecuteOnce = new Panel();
			this.pnlExecuteOnce.SuspendLayout();
			this.pnlExecuteOnce.Dock = DockStyle.Top;
			var lblExecuteOnce = new Label();
			lblExecuteOnce.Text = "Execute once:";
			lblExecuteOnce.AutoSize = false;
			lblExecuteOnce.TextAlign = ContentAlignment.MiddleLeft;
			lblExecuteOnce.Dock = DockStyle.Left;
			this.edFunctionExecuteOnce = new TextBox();
			this.edFunctionExecuteOnce.Dock = DockStyle.Fill;
            this.edFunctionExecuteOnce.Font = new Font( FontFamily.GenericMonospace, 10, FontStyle.Bold );
			this.edFunctionExecuteOnce.Multiline = true;
            this.edFunctionExecuteOnce.WordWrap = false;
            this.edFunctionExecuteOnce.ScrollBars = ScrollBars.Both;
			this.edFunctionExecuteOnce.KeyUp += (sender, e) => {
				string contents = this.edFunctionExecuteOnce.Text.Trim();

				if ( string.IsNullOrEmpty( contents ) ) {
					this.Function.PreProgramOnce.AddRange( contents.Split( '\n' ) );
				}

				return;
			};
			this.pnlExecuteOnce.Controls.Add( this.edFunctionExecuteOnce );
			this.pnlExecuteOnce.Controls.Add( lblExecuteOnce );
			pnlInnerGroupCommands.Controls.Add( this.pnlExecuteOnce );

			this.pnlPreCommand.ResumeLayout( false );
			this.pnlExecuteOnce.ResumeLayout( false );
			pnlInnerGroupCommands.ResumeLayout( false );
			this.pnlGroupCommands.ResumeLayout( false );
		}

		private void Build()
		{
            // Main panel
            this.pnlContainer = new TableLayoutPanel();
			this.pnlContainer.SuspendLayout();
            this.pnlContainer.Dock = DockStyle.Fill;
            this.pnlContainer.AutoSize = true;
            this.Panel.Controls.Add( this.pnlContainer );

			// Sub panels
			this.BuildCheckBoxes();
			this.BuildDefaultData();
			this.BuildCommands();
			this.BuildArgumentsListTable();

			this.pnlContainer.ResumeLayout( false );
		}

		/// <summary>
		/// Adds a new argument to the UI and to the function.
		/// </summary>
		private void OnAddFunctionArgument()
		{
			int colCount = this.grdArgsList.Columns.Count;
			int rowCount = this.grdArgsList.Rows.Count;
			string name = "arg" + rowCount.ToString();

			// Add a new row
			this.grdArgsList.Rows.Add( new object[] {
				name,
				"",
				"",
				false,
				false
			});

			// Select the first value of the drop-down list
			DataGridViewRow cmbRow = this.grdArgsList.Rows[ rowCount ];
			var cmbCell = (DataGridViewComboBoxCell) cmbRow.Cells[ colCount -1 ];
			cmbCell.Value = cmbCell.Items[ 0 ];

			// Activate remove button
			this.btFunctionRemoveArgument.Enabled = true;

			// Add the new argument to the function
			this.Function.ArgList.Add( new Function.Argument( name ) );
		}

		/// <summary>
		/// Removes the function from the UI and the function itself.
		/// It uses the row of the arguments list with a cell selected.
		/// </summary>
		private void OnRemoveFunctionArgument()
		{
			DataGridViewCell cell = this.grdArgsList.CurrentCell;

			if ( this.grdArgsList.Rows.Count > 0 ) {
				int row = 0;

				// Remove selected argument in the UI
				if ( cell != null ) {
					row = cell.RowIndex;
				}

				this.grdArgsList.Rows.RemoveAt( row );

				// Dsiable remove button
				if ( this.grdArgsList.Rows.Count == 0 ) {
					this.btFunctionRemoveArgument.Enabled = false;
				}

				// Remove the same argument in the function
				this.Function.ArgList.RemoveAt( row );
			}

			return;
		}

		/// <summary>
		/// Updates the information of the argument being modified.
		/// </summary>
		/// <param name="rowIndex">The row index, as an int, which gives the argument number.</param>
		/// <param name="colIndex">The col index, as an int, which gives the attribute of the argument..</param>
		private void OnCellEdited(int rowIndex, int colIndex)
		{
			DataGridViewRow row = this.grdArgsList.Rows[ rowIndex ];
			Function.Argument arg = this.Function.ArgList[ rowIndex ];

			// The name
			if ( colIndex == 0 ) {
				string contents = (string) row.Cells[ colIndex ].Value;

				if ( !string.IsNullOrWhiteSpace( contents ) ) {
					arg.Name = contents;
				}
			}
			else
			// The tag
			if ( colIndex == 1 ) {
				arg.Tag = (string) row.Cells[ colIndex ].Value;
			}
			else
			// The depends info
			if ( colIndex == 2 ) {
				arg.DependsFrom = (string) row.Cells[ colIndex ].Value;
			}
			else
			// The requires info
			if ( colIndex == 3 ) {
				arg.IsRequired = (bool) row.Cells[ colIndex ].Value;

				// Disable next cells
                this.EnableCellsHonoringRequired( row.Index );
			}
			else
			// The multiselect info
			if ( colIndex == 4 ) {
				arg.AllowMultiselect = (bool) row.Cells[ colIndex ].Value;
			}
			else
			// The viewer info
			if ( colIndex == 5 ) {
				Function.Argument.ViewerType viewer;

				bool parsedOk = Enum.TryParse( 
					(string) row.Cells[ colIndex ].Value,
					out viewer );

				if ( parsedOk ) {
					arg.Viewer = viewer;
				}
			}

			return;
		}

        /// <summary>
        /// Enables or disables the cells following required,
        /// depending on its value.
        /// </summary>
        /// <param name="rowIndex">The row index, as int.</param>
        private void EnableCellsHonoringRequired(int rowIndex)
        {
            DataGridViewRow row = this.grdArgsList.Rows[ rowIndex ];
            bool isRequired = this.Function.ArgList[ rowIndex ].IsRequired;

            for (int i = 4; i < row.Cells.Count; ++i)
            {
                row.Cells[ i ].ReadOnly = isRequired;
            }

            return;
        }

		/// <summary>
		/// Makes the arguments list occupy the whole width of the container panel.
		/// </summary>
        private void OnResizeArgsList()
        {
            int width = this.pnlContainer.Width;

            // Name
            this.grdArgsList.Columns[ 0 ].Width = (int) ( width * 0.20 );

            // Tag
            this.grdArgsList.Columns[ 1 ].Width = (int) ( width * 0.18 );

            // Depends
            this.grdArgsList.Columns[ 2 ].Width = (int) ( width * 0.20 );

            // Required
            this.grdArgsList.Columns[ 3 ].Width = (int) ( width * 0.10 );

            // Multiselect
            this.grdArgsList.Columns[ 4 ].Width = (int) ( width * 0.10 );

            // Viewer
            this.grdArgsList.Columns[ 5 ].Width = (int) ( width * 0.20 );
        }

        /// <summary>
        /// Reads the data from component, and reflects it on the editor.
        /// </summary>
        public override void ReadDataFromComponent()
        {
            base.ReadDataFromComponent();

            // Checkboxes
            this.chkFunctionHasData.Checked = this.Function.HasData;
            this.chkFunctionRemoveQuotes.Checked = this.Function.RemoveQuotationMarks;
            this.chkFunctionDataHeader.Checked = this.Function.DataHeader;

            // Data
            this.edFunctionDefaultData.Text = this.Function.DefaultData;
            this.edFunctionPreCommand.Text = this.Function.PreCommand;
            this.edFunctionExecuteOnce.Text = this.Function.PreProgramOnce.ToString();
            this.udFunctionStartColumn.Value = Math.Max( 1, this.Function.StartColumn );
            this.udFunctionEndColumn.Value = Math.Max( 1, this.Function.EndColumn );

            // Arguments
            foreach(Function.Argument arg in this.Function.ArgList) {
                this.grdArgsList.Rows.Add();
                DataGridViewRow row = this.grdArgsList.Rows[ this.grdArgsList.Rows.Count - 1 ];

                row.Cells[ 0 ].Value = arg.Name;
                row.Cells[ 1 ].Value = arg.Tag;
                row.Cells[ 2 ].Value = arg.DependsFrom;
                row.Cells[ 3 ].Value = arg.IsRequired;
                row.Cells[ 4 ].Value = arg.AllowMultiselect;
                row.Cells[ 5 ].Value = arg.Viewer.ToString();

                this.EnableCellsHonoringRequired( row.Index );
            }

            return;
        }

        private TableLayoutPanel pnlContainer;
		private FlowLayoutPanel pnlChecks;
		private GroupBox pnlGroupChecks;
		private GroupBox pnlGroupDefaultData;
		private GroupBox pnlGroupCommands;
		private Panel pnlPreCommand;
		private Panel pnlExecuteOnce;
		private Panel pnlButtons;
		private GroupBox pnlArgsList;

		private CheckBox chkFunctionHasData;
		private CheckBox chkFunctionRemoveQuotes;
		private CheckBox chkFunctionDataHeader;
		private DataGridView grdArgsList;
		private Button btFunctionAddArgument;
		private Button btFunctionRemoveArgument;

		private TextBox edFunctionDefaultData;
		private TextBox edFunctionPreCommand;
		private TextBox edFunctionExecuteOnce;
		private NumericUpDown udFunctionStartColumn;
		private NumericUpDown udFunctionEndColumn;

		private UserAction addFunctionArgumentAction;
		private UserAction removeFunctionArgumentAction;
	}
}

