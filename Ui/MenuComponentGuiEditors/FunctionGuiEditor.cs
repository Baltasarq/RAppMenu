using System;
using System.Drawing;
using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;

using RWABuilder.Ui.MenuComponentGuiEditors.FunctionGuiEditors;

namespace RWABuilder.Ui.MenuComponentGuiEditors {
	public class FunctionGuiEditor: NamedComponentGuiEditor {
		public FunctionGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
			: base( panel, mctn, mc )
		{
			this.fnCallsEditor = null;
			this.addFunctionArgumentAction = UserAction.LookUp( "addfunctionargument" );
			this.editFnCallArgumentsAction = UserAction.LookUp( "editfunctioncallarguments" );
			this.removeFunctionArgumentAction = UserAction.LookUp( "removefunctionargument" );

			this.Build();
			
 		     // Put in some data
			this.chkFunctionHasData.Checked = this.Function.HasData;
            this.chkFunctionDataHeader.Checked = this.Function.DataHeader;
			this.chkFunctionRemoveQuotes.Checked = this.Function.RemoveQuotationMarks;
			this.edFunctionPreCommand.Text = this.Function.PreProgramOnce.ToString();
            this.edFunctionData.Text = this.Function.ExampleData;
		}

		/// <summary>
		/// Gets the function being modified by this editor
		/// </summary>
		/// <value>The Function object</value>
		public Function Function {
			get {
				return (Function) this.MenuComponent;
			}
		}

		/// <summary>
		/// Shows and preedPackageNamethe editor
		/// </summary>
		public override void Show()
		{
			base.Show();
            this.tcPad.Show();

			this.removeFunctionArgumentAction.Enabled = ( this.grdArgsList.Rows.Count > 0 );
			this.addFunctionArgumentAction.CallBack = this.OnAddFunctionArgument;
			this.editFnCallArgumentsAction.CallBack = this.OnEditFunctionCallArguments;
			this.removeFunctionArgumentAction.CallBack = this.OnRemoveFunctionArgument;

            // Load PDF File names
			string[] pdfList = this.Function.Root.Owner.PDFNameList;
            this.edPDFFileName.Items.Clear();
            this.edPDFFileName.Items.Add( "" );
			this.edPDFFileName.Items.AddRange( pdfList );

			if ( pdfList.Length == 0 ) {
				this.pnlPDFReference.Enabled = false;
				this.Function.PDFName = "";
			}
			else
			if ( pdfList.Length > 0 ) {
				this.pnlPDFReference.Enabled = true;

				if ( pdfList.Length == 1 ) {
					this.edPDFFileName.SelectedItem = this.Function.PDFName = pdfList[ 0 ];
				} else {
					this.edPDFFileName.SelectedItem = this.Function.PDFName;
				}
			}
		}

		private void BuildArgumentsListTable()
		{
			var toolTips = new ToolTip();

			this.pnlArgsList = new GroupBox();
			this.pnlArgsList.SuspendLayout();
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
            comboBoxCellTemplate.Items.AddRange(
                Enum.GetNames( typeof( Function.Argument.ViewerType ) ) );

			var checkBoxCellTemplate = new DataGridViewCheckBoxCell();
			checkBoxCellTemplate.Style.BackColor = Color.AntiqueWhite;

			var imageCellTemplate = new DataGridViewImageCell();
			imageCellTemplate.Style.BackColor = Color.AntiqueWhite;

			var column0 = new DataGridViewTextBoxColumn();
			var column1 = new DataGridViewTextBoxColumn();
			var column2 = new DataGridViewTextBoxColumn();
			var column3 = new DataGridViewCheckBoxColumn();
			var column4 = new DataGridViewCheckBoxColumn();
			var column5 = new DataGridViewComboBoxColumn();
			var column6 = new DataGridViewImageColumn();

			column0.CellTemplate = textCellTemplate;
			column1.CellTemplate = textCellTemplate;
			column2.CellTemplate = textCellTemplate;
			column3.CellTemplate = checkBoxCellTemplate;
			column4.CellTemplate = checkBoxCellTemplate;
			column5.CellTemplate = comboBoxCellTemplate;
			column6.CellTemplate = imageCellTemplate;

			column0.HeaderText = "Name";
			column0.Width = 120;
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
			column1.HeaderText = "Value";
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
			column6.HeaderText = "";
			column6.ReadOnly = true;
			column6.Width = 10;
			column6.SortMode = DataGridViewColumnSortMode.NotSortable;

			this.grdArgsList.Columns.AddRange (new DataGridViewColumn[] {
				column0,
				column1,
				column2,
				column3,
				column4,
				column5,
				column6,
			}
			);


            this.grdArgsList.CellEnter += (object sender, DataGridViewCellEventArgs evt) => {
                if ( evt.RowIndex >= 0
                  && evt.ColumnIndex == 1 )
                {
                    this.OnArgsListCellEntered( evt.RowIndex, evt.ColumnIndex );
                }
            };

			this.grdArgsList.CellClick += (object sender, DataGridViewCellEventArgs e) => {
				if ( e.ColumnIndex == 6
				  && e.RowIndex >= 0 )
				{
					this.OnEditViewer( e.RowIndex );
				}
			};

			this.grdArgsList.CellEndEdit += (object sender, DataGridViewCellEventArgs evt) => {
                if ( evt.RowIndex >= 0
                  && evt.ColumnIndex >= 0 )
                {
                    this.OnArgsListCellEdited( evt.RowIndex, evt.ColumnIndex );
                }
			};

            this.grdArgsList.MinimumSize = new Size( 240, 100 );
			this.grdArgsList.Font = new Font( this.grdArgsList.Font, FontStyle.Regular );
			this.pnlArgsList.Font = new Font( this.pnlArgsList.Font, FontStyle.Bold );

			// Buttons panel
			this.pnlArgButtons = new FlowLayoutPanel();
			this.pnlArgButtons.AutoSize = true;
			this.pnlArgButtons.Dock = DockStyle.Bottom;
			this.btFunctionAddArgument = new Button();
			this.btEditFnCallArguments = new Button();
			this.btFunctionRemoveArgument = new Button();

			this.btFunctionAddArgument.Size = this.btFunctionAddArgument.MinimumSize = 
				this.btFunctionAddArgument.MaximumSize = new Size( 32, 32 );
			this.btFunctionAddArgument.ImageList = UserAction.ImageList;
			this.btFunctionAddArgument.ImageIndex = this.addFunctionArgumentAction.ImageIndex;
			this.btFunctionAddArgument.Click += (sender, e) => this.addFunctionArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionAddArgument, this.addFunctionArgumentAction.Text );

			this.btEditFnCallArguments.Size = this.btEditFnCallArguments.MinimumSize = 
				this.btEditFnCallArguments.MaximumSize = new Size( 32, 32 );
			this.btEditFnCallArguments.ImageList = UserAction.ImageList;
			this.btEditFnCallArguments.ImageIndex = this.editFnCallArgumentsAction.ImageIndex;
			this.btEditFnCallArguments.Click += (sender, e) => this.editFnCallArgumentsAction.CallBack();
			toolTips.SetToolTip( this.btEditFnCallArguments, this.editFnCallArgumentsAction.Text );

			this.btFunctionRemoveArgument.Size = this.btFunctionRemoveArgument.MinimumSize = 
				this.btFunctionRemoveArgument.MaximumSize = new Size( 32, 32 );
			this.btFunctionRemoveArgument.ImageList = UserAction.ImageList;
			this.btFunctionRemoveArgument.ImageIndex = this.removeFunctionArgumentAction.ImageIndex;
			this.btFunctionRemoveArgument.Click += (sender, e) => this.removeFunctionArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionRemoveArgument, this.removeFunctionArgumentAction.Text );

            // Prepare
			this.addFunctionArgumentAction.AddComponent( this.btFunctionAddArgument );
			this.removeFunctionArgumentAction.AddComponent( this.btFunctionRemoveArgument );
			this.pnlArgButtons.Controls.Add( this.btFunctionAddArgument );
			this.pnlArgButtons.Controls.Add( this.btEditFnCallArguments );
			this.pnlArgButtons.Controls.Add( this.btFunctionRemoveArgument );
			this.pnlArgsList.Controls.Add( this.grdArgsList );
			this.pnlArgsList.Controls.Add( this.pnlArgButtons );
            this.pnlContainer.Controls.Add( this.pnlArgsList );
			this.pnlArgsList.ResumeLayout( false );
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
			this.pnlChecks.Font = new Font( this.pnlChecks.Font, FontStyle.Regular );
			this.pnlChecks.Dock = DockStyle.Fill;
			this.pnlGroupChecks.Controls.Add( this.pnlChecks );
			this.pnlContainer.Controls.Add( this.pnlGroupChecks );

			this.chkFunctionHasData = new CheckBox();
			this.chkFunctionHasData.Text = "Data";
			this.chkFunctionHasData.Dock = DockStyle.Fill;
			this.chkFunctionHasData.MinimumSize =
				new Size( this.chkFunctionHasData.Width, this.chkFunctionHasData.Height );
			this.chkFunctionHasData.CheckedChanged += (object sender, EventArgs e) => {
				bool value = this.chkFunctionHasData.Checked;

				this.Function.HasData = value;
				this.Function.DataHeader = value;
				this.chkFunctionDataHeader.Checked = value;
				this.pnlGroupData.Enabled = value;
			};
			this.pnlChecks.Controls.Add( this.chkFunctionHasData );

			this.chkFunctionDataHeader = new CheckBox();
			this.chkFunctionDataHeader.Text = "Data header";
			this.chkFunctionDataHeader.Dock = DockStyle.Fill;
			this.chkFunctionDataHeader.MinimumSize =
				new Size( this.chkFunctionDataHeader.Width, this.chkFunctionDataHeader.Height );
			this.chkFunctionDataHeader.CheckedChanged += (object sender, EventArgs e) =>
				this.Function.DataHeader = this.chkFunctionDataHeader.Checked;
			this.pnlChecks.Controls.Add( chkFunctionDataHeader );

			this.chkFunctionRemoveQuotes = new CheckBox();
			this.chkFunctionRemoveQuotes.Text = "Remove quotes";
			this.chkFunctionRemoveQuotes.Dock = DockStyle.Fill;
			this.chkFunctionRemoveQuotes.MinimumSize =
				new Size( this.chkFunctionRemoveQuotes.Width, this.chkFunctionRemoveQuotes.Height );
			this.chkFunctionRemoveQuotes.CheckedChanged += (object sender, EventArgs e) => 
				this.Function.RemoveQuotationMarks = this.chkFunctionRemoveQuotes.Checked;
			this.pnlChecks.Controls.Add( chkFunctionRemoveQuotes );

			this.pnlChecks.ResumeLayout( false );
			this.pnlGroupChecks.ResumeLayout( false );
		}

		private void BuildDefaultData()
		{
            this.pnlGroupData = new GroupBox();
			this.pnlGroupData.SuspendLayout();
			this.pnlGroupData.AutoSize = true;
			this.pnlGroupData.Dock = DockStyle.Top;
			this.pnlGroupData.Text = "Data";
			this.pnlGroupData.Font = new Font( this.pnlGroupData.Font, FontStyle.Bold );

            var pnlInnerGroupData = new FlowLayoutPanel();
			pnlInnerGroupData.SuspendLayout();
			pnlInnerGroupData.Font = new Font( pnlInnerGroupData.Font, FontStyle.Regular );
			pnlInnerGroupData.Dock = DockStyle.Fill;
			pnlInnerGroupData.AutoSize = true;
			this.pnlGroupData.Controls.Add( pnlInnerGroupData );
			this.pnlContainer.Controls.Add( this.pnlGroupData );

			// Default data
            var lblData = new Label();
			lblData.Text = "Data:";
			lblData.AutoSize = false;
			lblData.TextAlign = ContentAlignment.MiddleLeft;
            this.edFunctionData = new TextBox();
			this.edFunctionData.Font = new Font( this.edFunctionData.Font, FontStyle.Bold );
			this.edFunctionData.KeyUp += (sender, e) => {
				string contents = this.edFunctionData.Text.Trim();

				if ( !string.IsNullOrEmpty( contents ) ) {
					this.Function.ExampleData = contents;
				}

				return;
			};
			pnlInnerGroupData.Controls.Add( lblData );
			pnlInnerGroupData.Controls.Add( this.edFunctionData );

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
            this.udFunctionStartColumn.ValueChanged += (sender, e) => {
                if ( !this.OnBuilding ) {
                    this.Function.StartColumn =
                        (int) this.udFunctionStartColumn.Value;
                }
            };
			pnlInnerGroupData.Controls.Add( lblStartColumn );
			pnlInnerGroupData.Controls.Add( this.udFunctionStartColumn );

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
            this.udFunctionEndColumn.ValueChanged += (sender, e) => {
                if ( !this.OnBuilding ) {
                    this.Function.EndColumn =
                        (int) this.udFunctionEndColumn.Value;
                }
            };

			pnlInnerGroupData.Controls.Add( lblEndColumn );
			pnlInnerGroupData.Controls.Add( this.udFunctionEndColumn );
			pnlInnerGroupData.ResumeLayout( false );
			this.pnlGroupData.ResumeLayout( false );
			this.pnlGroupData.Enabled = false;

            // Sizes for controls
            Graphics grf = new Form().CreateGraphics();
            SizeF fontSize = grf.MeasureString( "W", this.udFunctionEndColumn.Font );
            int charWidth = (int) fontSize.Width + 5;
            this.udFunctionEndColumn.MaximumSize = new Size( charWidth * 2, this.udFunctionEndColumn.Height );
            this.udFunctionStartColumn.MaximumSize = new Size( charWidth * 2, this.udFunctionStartColumn.Height );
            this.edFunctionData.MinimumSize = new Size( charWidth * 12, this.edFunctionData.Height );
		}

        private void BuildPDFReference()
        {
            this.pnlPDFReference = new GroupBox();
            this.pnlPDFReference.SuspendLayout();
            this.pnlPDFReference.AutoSize = true;
            this.pnlPDFReference.Dock = DockStyle.Top;
            this.pnlPDFReference.Text = "PDF Manual";
            this.pnlPDFReference.Font = new Font( this.pnlPDFReference.Font, FontStyle.Bold );

            var pnlInnerPDFReference = new FlowLayoutPanel();
            pnlInnerPDFReference.SuspendLayout();
            pnlInnerPDFReference.Font = new Font( pnlInnerPDFReference.Font, FontStyle.Regular );
            pnlInnerPDFReference.Dock = DockStyle.Fill;
            pnlInnerPDFReference.AutoSize = true;
            this.pnlPDFReference.Controls.Add( pnlInnerPDFReference );
            this.pnlContainer.Controls.Add( this.pnlPDFReference );

            // Default data
            var lblPDFFile = new Label();
            lblPDFFile.Text = "PDF file:";
            lblPDFFile.AutoSize = false;
            lblPDFFile.TextAlign = ContentAlignment.MiddleLeft;
            this.edPDFFileName = new ComboBox();
            this.edPDFFileName.DropDownStyle = ComboBoxStyle.DropDownList;
            this.edPDFFileName.Font = new Font( this.edPDFFileName.Font, FontStyle.Bold );
            this.edPDFFileName.SelectionChangeCommitted += (sender, e) => {
                string contents = this.edPDFFileName.SelectedItem.ToString();

				System.Diagnostics.Trace.WriteLine( "Changing PDF Ref to: " + contents );
                if ( !string.IsNullOrEmpty( contents ) ) {
                    this.Function.PDFName = contents;
					System.Diagnostics.Trace.WriteLine( "PDF ref changed" );
                }

                return;
            };
            pnlInnerPDFReference.Controls.Add( lblPDFFile );
            pnlInnerPDFReference.Controls.Add( this.edPDFFileName );

            // Start page
            var lblStartPage = new Label();
            lblStartPage.Text = "Start page:";
            lblStartPage.AutoSize = true;
            lblStartPage.TextAlign = ContentAlignment.MiddleLeft;
            this.udFunctionStartPage = new NumericUpDown();
            this.udFunctionStartPage.TextAlign = HorizontalAlignment.Right;
            this.udFunctionStartPage.Font = new Font( this.udFunctionStartPage.Font, FontStyle.Bold );
            this.udFunctionStartPage.Maximum = 999;
            this.udFunctionStartPage.Minimum = 1;
            this.udFunctionStartPage.ValueChanged += (sender, e) => {
                if ( !this.OnBuilding ) {
                    this.Function.PDFPageNumber =
                        (int) this.udFunctionStartPage.Value;
                }
            };
            pnlInnerPDFReference.Controls.Add( lblStartPage );
            pnlInnerPDFReference.Controls.Add( this.udFunctionStartPage );
            pnlInnerPDFReference.ResumeLayout( false );
            this.pnlPDFReference.ResumeLayout( false );

            // Sizes for controls
            Graphics grf = new Form().CreateGraphics();
            SizeF fontSize = grf.MeasureString( "W", this.udFunctionStartPage.Font );
            int charWidth = (int) fontSize.Width + 5;
            this.udFunctionStartPage.MaximumSize = new Size( charWidth * 2, this.udFunctionStartPage.Height );
            this.edPDFFileName.MinimumSize = new Size( charWidth * 12, this.edPDFFileName.Height );
            this.pnlPDFReference.MinimumSize = new Size(
                this.Panel.ClientSize.Width - 100,
                this.edPDFFileName.Height );
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

            this.tcPad.TabPages[ 1 ].Controls.Add( this.pnlGroupCommands );
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

				if ( !string.IsNullOrEmpty( contents ) ) {
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

				if ( !string.IsNullOrEmpty( contents ) ) {
                    this.Function.PreProgramOnce.Clear();
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

		private void BuildPackageEditor()
		{
			this.pnlEdPackage = new Panel();
			this.pnlEdPackage.SuspendLayout();
			this.pnlEdPackage.Dock = DockStyle.Top;

			var lblName = new Label();
			lblName.AutoSize = false;
			lblName.TextAlign = ContentAlignment.MiddleLeft;
			lblName.Dock = DockStyle.Left;
			lblName.Text = "Package name:";

			this.edPackageName = new TextBox();
			this.edPackageName.Font = new Font( this.edPackageName.Font, FontStyle.Bold );
			this.edPackageName.Dock = DockStyle.Fill;
			this.edPackageName.KeyUp += (sender, e) => {
				string name = this.edPackageName.Text;

				if ( !string.IsNullOrWhiteSpace( name ) ) {
					this.Function.Package = name;
				}
			};

			this.pnlEdPackage.Controls.Add( this.edPackageName );
			this.pnlEdPackage.Controls.Add( lblName );
			this.pnlEdPackage.MaximumSize = new Size( int.MaxValue, this.edPackageName.Height );
			this.pnlContainer.Controls.Add( this.pnlEdPackage );
			this.pnlEdPackage.ResumeLayout( false );
		}

		private void Build()
		{
            this.OnBuilding = true;

            // Tab control
            this.tcPad = new TabControl();
            this.tcPad.SuspendLayout();
            this.tcPad.Dock = DockStyle.Fill;
            this.tcPad.TabPages.Add( "Basic" );
            this.tcPad.TabPages.Add( "Extended" );

            // Main panel
            this.Panel.SuspendLayout();
            this.pnlContainer = new TableLayoutPanel();
			this.pnlContainer.SuspendLayout();
            this.pnlContainer.Dock = DockStyle.Fill;
            this.pnlContainer.AutoSize = true;
            this.tcPad.TabPages[ 0 ].Controls.Add( this.pnlContainer );
            this.Panel.Controls.Add( this.tcPad );

			// Sub panels
			this.BuildPackageEditor();
            this.BuildCheckBoxes();
			this.BuildDefaultData();
			this.BuildPDFReference();
            this.BuildArgumentsListTable();
            this.BuildCommands();

			this.pnlContainer.ResumeLayout( false );
            this.Panel.ResumeLayout( false );
            this.tcPad.ResumeLayout( false );
			this.pnlContainer.Resize += (sender, e) => this.OnResizeEditor();
            this.OnBuilding = false;

            this.tcPad.TabPages[ 0 ].Padding = new Padding( 5 );
            this.tcPad.TabPages[ 1 ].Padding = new Padding( 5 );
		}

		/// <summary>
		/// Adds a new argument to the UI and to the function.
		/// </summary>
		private void OnAddFunctionArgument()
		{
			int colCount = this.grdArgsList.Columns.Count;
			int rowCount = this.grdArgsList.Rows.Count;
            string name = "arg" + ( rowCount + 1 );

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
			var cmbCell = (DataGridViewComboBoxCell) cmbRow.Cells[ colCount - 2 ];
			cmbCell.Value = cmbCell.Items[ 0 ];

			// Put the edit image in the last column
			var imgCell = (DataGridViewImageCell) cmbRow.Cells[ colCount - 1 ];
			imgCell.Value = UserAction.ImageList.Images[ UserAction.LookUp( "properties" ).ImageIndex ];

			// Activate remove button
			this.btFunctionRemoveArgument.Enabled = true;

			// Add the new argument to the function
            this.Function.RegularArgumentList.Add( new Function.Argument( name, this.Function ) );
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
				this.Function.RegularArgumentList.RemoveAt( row );
			}

			return;
		}

        private void OnArgsListCellEntered(int rowIndex, int colIndex)
        {
            DataGridViewRow row = this.grdArgsList.Rows[ rowIndex ];

            if ( this.Function.RegularArgumentList.Count > rowIndex ) {
                var arg = (Function.Argument) this.Function.RegularArgumentList[ rowIndex ];

				if ( arg.Viewer == Function.Argument.ViewerType.SimpleColorPicker
				  || arg.Viewer == Function.Argument.ViewerType.MultiColorPicker )
				{
					var colorEditor = new ColorEditor( arg.Value, arg.Viewer );

					if ( colorEditor.ShowDialog() == DialogResult.OK ) {
						row.Cells[ colIndex ].Value = arg.Value = colorEditor.ToString();
					}
				}
				else
				if ( arg.Viewer == Function.Argument.ViewerType.SimpleValueSet
				  || arg.Viewer == Function.Argument.ViewerType.MultiValueSet )
				{
					var valed = new ValuesChooser( arg.ValueSet, arg.Viewer );

					if ( valed.ShowDialog() != DialogResult.Cancel ) {
						row.Cells[ 1 ].Value = arg.Value = valed.GetSelectedItemsAsList();
					}
				}
            }

            return;
        }

		private void OnEditViewer(int rowIndex)
		{
			DataGridViewRow row = this.grdArgsList.Rows[ rowIndex ];

			if ( this.Function.RegularArgumentList.Count > rowIndex ) {
				var arg = (Function.Argument) this.Function.RegularArgumentList[ rowIndex ];
				var valed = new CsvEditor( arg.ValueSet );

				if ( valed.ShowDialog() != DialogResult.Cancel ) {
					// Set simple value set
					arg.Viewer = Function.Argument.ViewerType.SimpleValueSet;
					var cmbCell = (DataGridViewComboBoxCell) row.Cells[ 5 ];
					cmbCell.Value = cmbCell.Items[ (int) Function.Argument.ViewerType.SimpleValueSet ];

					// Load data
					arg.ValueSet = valed.Data;
				}
			}

			return;
		}

		/// <summary>
		/// Updates the information of the regular argument being modified.
		/// </summary>
		/// <param name="rowIndex">The row index, as an int, which gives the argument number.</param>
		/// <param name="colIndex">The col index, as an int, which gives the attribute of the argument.</param>
		private void OnArgsListCellEdited(int rowIndex, int colIndex)
		{
			DataGridViewRow row = this.grdArgsList.Rows[ rowIndex ];
			var arg = (Function.Argument) this.Function.RegularArgumentList[ rowIndex ];

			// The name
			if ( colIndex == 0 ) {
				string contents = (string) row.Cells[ colIndex ].Value;

				if ( !string.IsNullOrWhiteSpace( contents ) ) {
					arg.Name = contents;
				}
			}
			else
			// The value
			if ( colIndex == 1 ) {
                arg.Value = (string) row.Cells[ colIndex ].Value;
			}
			else
			// The depends info
			if ( colIndex == 2 ) {
				arg.DependsFrom = (string) row.Cells[ colIndex ].Value;
			}
			else
			// The required info
			if ( colIndex == 3 ) {
				arg.IsRequired = (bool) row.Cells[ colIndex ].Value;
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
		/// Show a new dialog to edit the function call arguments
		/// </summary>
		private void OnEditFunctionCallArguments()
		{
            var fakeRoot = new RootMenu( new MenuDesign() );
            Function old = (Function) this.Function.Copy( fakeRoot );

			if ( this.fnCallsEditor == null ) {
				this.fnCallsEditor = new FunctionCallsGuiEditor( this.Function );
			}

            if ( this.fnCallsEditor.ShowDialog() != DialogResult.OK ) {
                this.Function.FunctionCallsArgumentList.Clear();

                foreach(Function.CallArgument fnCall in old.FunctionCallsArgumentList) {
                    this.Function.FunctionCallsArgumentList.Add(
                        (Function.CallArgument) fnCall.Copy( this.Function ) );
                }
            }

            return;
		}

		/// <summary>
		/// Makes the arguments list occupy the whole width of the container panel.
		/// </summary>
        private void OnResizeEditor()
        {
            int width = this.pnlContainer.ClientSize.Width;

            // Name
            this.grdArgsList.Columns[ 0 ].Width = (int) ( width * 0.20 );

            // Value
            this.grdArgsList.Columns[ 1 ].Width = (int) ( width * 0.15 );

            // Depends
            this.grdArgsList.Columns[ 2 ].Width = (int) ( width * 0.20 );

            // Required
            this.grdArgsList.Columns[ 3 ].Width = (int) ( width * 0.10 );

            // Multiselect
            this.grdArgsList.Columns[ 4 ].Width = (int) ( width * 0.10 );

            // Viewer
            this.grdArgsList.Columns[ 5 ].Width = (int) ( width * 0.20 );

			// Editor
			this.grdArgsList.Columns[ 6 ].Width = (int) ( width * 0.05 );
        }

        /// <summary>
        /// Reads the data from component, and reflects it on the editor.
        /// </summary>
        public override void ReadDataFromComponent()
        {
            base.ReadDataFromComponent();
            base.OnBuilding = true;

            // Checkboxes
            this.chkFunctionHasData.Checked = this.Function.HasData;
            this.chkFunctionRemoveQuotes.Checked = this.Function.RemoveQuotationMarks;
            this.chkFunctionDataHeader.Checked = this.Function.DataHeader;

            // Data
            this.edFunctionData.Text = this.Function.ExampleData;
            this.edFunctionPreCommand.Text = this.Function.PreCommand;
            this.edFunctionExecuteOnce.Text = this.Function.PreProgramOnce.ToString();
            this.udFunctionStartColumn.Value = Math.Max( 1, this.Function.StartColumn );
            this.udFunctionEndColumn.Value = Math.Max( 1, this.Function.EndColumn );
			this.edPDFFileName.Text = this.Function.PDFName;
			this.udFunctionStartPage.Value = this.Function.PDFPageNumber;

            // Arguments
            foreach(Function.Argument arg in this.Function.RegularArgumentList) {
                this.grdArgsList.Rows.Add();
                DataGridViewRow row = this.grdArgsList.Rows[ this.grdArgsList.Rows.Count - 1 ];

                row.Cells[ 0 ].Value = arg.Name;
                row.Cells[ 1 ].Value = arg.Value;
                row.Cells[ 2 ].Value = arg.DependsFrom;
                row.Cells[ 3 ].Value = arg.IsRequired;
                row.Cells[ 4 ].Value = arg.AllowMultiselect;
                row.Cells[ 5 ].Value = arg.Viewer.ToString();
				row.Cells[ 6 ].Value = UserAction.ImageList.Images[ UserAction.LookUp( "properties" ).ImageIndex ];
            }

            this.OnBuilding = false;
            return;
        }

		private FunctionCallsGuiEditor fnCallsEditor;
        private TableLayoutPanel pnlContainer;
		private FlowLayoutPanel pnlChecks;
		private GroupBox pnlGroupChecks;
		private GroupBox pnlGroupData;
        private GroupBox pnlPDFReference;
		private GroupBox pnlGroupCommands;
		private GroupBox pnlArgsList;
		private Panel pnlPreCommand;
		private Panel pnlExecuteOnce;
		private Panel pnlArgButtons;
		private Panel pnlEdPackage;
        private TabControl tcPad;

		private CheckBox chkFunctionHasData;
		private CheckBox chkFunctionRemoveQuotes;
		private CheckBox chkFunctionDataHeader;
		private DataGridView grdArgsList;
		private Button btFunctionAddArgument;
		private Button btEditFnCallArguments;
		private Button btFunctionRemoveArgument;

		private TextBox edFunctionData;
		private TextBox edFunctionPreCommand;
		private TextBox edFunctionExecuteOnce;
		private TextBox edPackageName;
        private ComboBox edPDFFileName;
		private NumericUpDown udFunctionStartColumn;
		private NumericUpDown udFunctionEndColumn;
        private NumericUpDown udFunctionStartPage;

		private UserAction addFunctionArgumentAction;
		private UserAction editFnCallArgumentsAction;
		private UserAction removeFunctionArgumentAction;
	}
}

