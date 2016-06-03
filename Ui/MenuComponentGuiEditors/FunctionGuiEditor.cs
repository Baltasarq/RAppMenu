using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
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
			this.editDescriptionsAction = UserAction.LookUp( "editdescriptions" );
			this.removeFunctionArgumentAction = UserAction.LookUp( "removefunctionargument" );

			this.Panel.Hide();
			this.Build();
			this.ReadDataFromComponent();
			this.Panel.Show();
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
		/// Modifies the name accordingly to what the user typed.
		/// Format: caption (name)
		/// The first impression of this name is done in the treenode's <see cref="FunctionTreeNode"/> code.
		/// <seealso cref="FunctionTreeNode"/>
		/// </summary>
		protected override void OnNameModified()
		{
			string name = this.Name;
			string caption = this.edCaption.Text.Trim();

			// Set name & caption
			this.Function.Caption = caption;
			if ( !string.IsNullOrWhiteSpace( name ) ) {
				this.Function.Name = name;
			}

			// Set caption in tree node
			this.MenuComponentTreeNode.Text = BuildCaptionCombination( this.Function, name, caption );
		}

		public static string BuildCaptionCombination(Function f, string name, string caption) {
			string toret;

			// Prepare name
			if ( string.IsNullOrWhiteSpace( name ) ) {
				name = f.Name;
			} else {
				name = name.Trim();
			}

			// Prepare caption
			if ( string.IsNullOrWhiteSpace( caption ) ) {
				caption = f.Caption;
			} else {
				caption = caption.Trim();
			}

			// Build
			toret = name;

			if ( name != caption ) {
				toret = string.Format( "{0} ({1})", caption, toret );
			}

			return toret;
		}

		/// <summary>
		/// Shows and prepares the editor
		/// </summary>
		public override void Show()
		{
			bool existingArgs = ( this.grdArgsList.Rows.Count > 0 );

			base.Show();
			this.OnNameModified();
			this.removeFunctionArgumentAction.Enabled = existingArgs;
            this.editDescriptionsAction.Enabled = existingArgs;

			this.addFunctionArgumentAction.CallBack = this.OnAddFunctionArgument;
			this.editFnCallArgumentsAction.CallBack = this.OnEditFunctionCallArguments;
			this.editDescriptionsAction.CallBack = this.OnEditDescriptions;
			this.removeFunctionArgumentAction.CallBack = this.OnRemoveFunctionArgument;

            // Load PDF File names
			string[] pdfList = this.Function.Root.Owner.GetPDFNameList();
            this.edPDFFileName.Items.Clear();
            this.edPDFFileName.Items.Add( "" );
			this.edPDFFileName.Items.AddRange( pdfList );

			// Modify PDF file name list so only the file name is shown, not the whole path
			for(int i = 0; i < this.edPDFFileName.Items.Count; ++i) {
				this.edPDFFileName.Items[ i ] = Path.GetFileName( (string) this.edPDFFileName.Items[ i ] );
			}

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
					this.edPDFFileName.SelectedItem = Path.GetFileName( this.Function.PDFName );
				}
			}

			this.tcPad.Show();
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
			var column5 = new DataGridViewCheckBoxColumn();
			var column6 = new DataGridViewCheckBoxColumn();
			var column7 = new DataGridViewComboBoxColumn();
			var column8 = new DataGridViewImageColumn();

			column0.CellTemplate = textCellTemplate;
			column1.CellTemplate = textCellTemplate;
			column2.CellTemplate = textCellTemplate;
			column3.CellTemplate = checkBoxCellTemplate;
			column4.CellTemplate = checkBoxCellTemplate;
			column5.CellTemplate = checkBoxCellTemplate;
			column6.CellTemplate = checkBoxCellTemplate;
			column7.CellTemplate = comboBoxCellTemplate;
			column8.CellTemplate = imageCellTemplate;

			column0.HeaderText = "Name";
			column0.Width = 120;
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
			column1.HeaderText = "Value";
			column1.Width = 120;
			column1.SortMode = DataGridViewColumnSortMode.NotSortable;
			column2.HeaderText = "Depends";
			column2.Width = 80;
			column2.SortMode = DataGridViewColumnSortMode.NotSortable;
			column3.HeaderText = "Read only";
			column3.Width = 80;
			column3.SortMode = DataGridViewColumnSortMode.NotSortable;
			column4.HeaderText = "Required";
			column4.Width = 80;
			column4.SortMode = DataGridViewColumnSortMode.NotSortable;
			column5.HeaderText = "Is data";
			column5.Width = 80;
			column5.SortMode = DataGridViewColumnSortMode.NotSortable;
			column6.HeaderText = "Multiselect";
			column6.Width = 80;
			column6.SortMode = DataGridViewColumnSortMode.NotSortable;
			column7.HeaderText = "Viewer";
			column7.Width = 80;
			column7.SortMode = DataGridViewColumnSortMode.NotSortable;
			column8.HeaderText = "";
			column8.ReadOnly = true;
			column8.Width = 10;
			column8.SortMode = DataGridViewColumnSortMode.NotSortable;

			this.grdArgsList.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
				column2,
				column3,
				column4,
				column5,
				column6,
				column7,
				column8,
			}
			);

			// Edit argument cell
            this.grdArgsList.CellEnter += (object sender, DataGridViewCellEventArgs evt) => {
                if ( evt.RowIndex >= 0
                  && evt.ColumnIndex == 1 )
                {
                    this.OnArgsListCellEntered( evt.RowIndex, evt.ColumnIndex );
                }
            };

			this.grdArgsList.CellEndEdit += (object sender, DataGridViewCellEventArgs evt) => {
				if ( evt.RowIndex >= 0
					&& evt.ColumnIndex >= 0 )
				{
					this.OnArgsListCellEdited( evt.RowIndex, evt.ColumnIndex );
				}
			};

			// Enter the editor
			this.grdArgsList.CellClick += (object sender, DataGridViewCellEventArgs e) => {
				if ( e.ColumnIndex == 8
				  && e.RowIndex >= 0 )
				{
					this.OnEditViewer( e.RowIndex );
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
			this.btEditDescriptions = new Button();

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

			this.btEditDescriptions.Size = this.btEditDescriptions.MinimumSize = 
				this.btEditDescriptions.MaximumSize = new Size( 32, 32 );
			this.btEditDescriptions.ImageList = UserAction.ImageList;
			this.btEditDescriptions.ImageIndex = this.editDescriptionsAction.ImageIndex;
			this.btEditDescriptions.Click += (sender, e) => this.editDescriptionsAction.CallBack();
			toolTips.SetToolTip( this.btEditDescriptions, this.editDescriptionsAction.Text );

            // Prepare
			this.addFunctionArgumentAction.AddComponent( this.btFunctionAddArgument );
			this.removeFunctionArgumentAction.AddComponent( this.btFunctionRemoveArgument );
			this.editDescriptionsAction.AddComponent( this.btEditDescriptions );
			this.editFnCallArgumentsAction.AddComponent( this.btEditFnCallArguments );
			this.pnlArgButtons.Controls.Add( this.btFunctionAddArgument );
			this.pnlArgButtons.Controls.Add( this.btFunctionRemoveArgument );
			this.pnlArgButtons.Controls.Add( this.btEditFnCallArguments );
			this.pnlArgButtons.Controls.Add( this.btEditDescriptions );
			this.pnlArgsList.Controls.Add( this.grdArgsList );
			this.pnlArgsList.Controls.Add( this.pnlArgButtons );
            this.pnlContainer.Controls.Add( this.pnlArgsList );
			this.pnlArgsList.ResumeLayout( false );
		}

		private void BuildCheckBoxes()
		{
			this.pnlGroupChecks = new GroupBox() {
				AutoSize = true,
				Text = "Options",
				Dock = DockStyle.Top
			};
			this.pnlGroupChecks.Font = new Font( this.pnlGroupChecks.Font, FontStyle.Bold );
			this.pnlGroupChecks.SuspendLayout();

			this.pnlChecks = new FlowLayoutPanel() {
				AutoSize = true,				
				Dock = DockStyle.Fill
			};
			this.pnlChecks.Font = new Font( this.pnlChecks.Font, FontStyle.Regular );
			this.pnlChecks.SuspendLayout();

			this.pnlGroupChecks.Controls.Add( this.pnlChecks );
			this.pnlContainer.Controls.Add( this.pnlGroupChecks );

			this.chkFunctionHasData = new CheckBox() {
				Text = "Data",
				Dock = DockStyle.Fill
			};
			this.chkFunctionHasData.MinimumSize = new Size( this.chkFunctionHasData.Width, this.chkFunctionHasData.Height );

			this.chkFunctionHasData.CheckedChanged += (object sender, EventArgs e) => {
				if ( !this.OnBuilding ) {
					bool value = this.chkFunctionHasData.Checked;

					this.Function.HasData = value;
					this.Function.DataHeader = value;
					this.chkFunctionDataHeader.Checked = value;
				}

				return;
			};
			this.pnlChecks.Controls.Add( this.chkFunctionHasData );

			this.chkFunctionDataHeader = new CheckBox() {
				Text = "Data header",
				Dock = DockStyle.Fill
			};
			this.chkFunctionDataHeader.MinimumSize = new Size( this.chkFunctionDataHeader.Width, this.chkFunctionDataHeader.Height );

			this.chkFunctionDataHeader.CheckedChanged += (object sender, EventArgs e) =>
				this.Function.DataHeader = this.chkFunctionDataHeader.Checked;
			this.pnlChecks.Controls.Add( chkFunctionDataHeader );

			this.chkFunctionRemoveQuotes = new CheckBox() {
				Text = "Remove quotes",
				Dock = DockStyle.Fill
			};
			this.chkFunctionRemoveQuotes.MinimumSize = new Size( this.chkFunctionRemoveQuotes.Width, this.chkFunctionRemoveQuotes.Height );

			this.chkFunctionRemoveQuotes.CheckedChanged += (object sender, EventArgs e) => 
				this.Function.RemoveQuotationMarks = this.chkFunctionRemoveQuotes.Checked;
			this.pnlChecks.Controls.Add( chkFunctionRemoveQuotes );

			this.chkIsDefault = new CheckBox() {
				Text = "Default",
				Dock = DockStyle.Fill
			};
			this.chkIsDefault.MinimumSize = new Size( this.chkIsDefault.Width, this.chkIsDefault.Height );

			this.chkIsDefault.CheckedChanged += (object sender, EventArgs e) => 
				this.Function.IsDefault = this.chkIsDefault.Checked;
			this.pnlChecks.Controls.Add( chkIsDefault );

			this.pnlChecks.ResumeLayout( false );
			this.pnlGroupChecks.ResumeLayout( false );
		}

		private void BuildExampleData()
		{
            this.pnlGroupData = new GroupBox();
			this.pnlGroupData.SuspendLayout();
			this.pnlGroupData.AutoSize = true;
			this.pnlGroupData.Dock = DockStyle.Top;
			this.pnlGroupData.Text = "Example data";
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
			this.udFunctionStartColumn.Maximum = 999;
			this.udFunctionStartColumn.Minimum = 0;
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
            this.udFunctionEndColumn.Maximum = 999;
            this.udFunctionEndColumn.Minimum = 0;
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
            this.udFunctionStartPage.MaximumSize = new Size( charWidth * 3, this.udFunctionStartPage.Height );
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
            this.tcPad.TabPages[ 1 ].Controls.Add( this.pnlEdPackage );
			this.pnlEdPackage.ResumeLayout( false );
		}

		private void BuildCaptionEditor()
		{
			this.pnlEdCaption = new Panel();
			this.pnlEdCaption.SuspendLayout();
			this.pnlEdCaption.Dock = DockStyle.Top;

			var lblCaption = new Label();
			lblCaption.AutoSize = false;
			lblCaption.TextAlign = ContentAlignment.MiddleLeft;
			lblCaption.Dock = DockStyle.Left;
			lblCaption.Text = "Caption:";

			this.edCaption = new TextBox();
			this.edCaption.Font = new Font( this.edCaption.Font, FontStyle.Bold );
			this.edCaption.Dock = DockStyle.Fill;
			this.edCaption.KeyUp += (sender, e) => this.OnNameModified();

			this.pnlEdCaption.Controls.Add( this.edCaption );
			this.pnlEdCaption.Controls.Add( lblCaption );
            this.pnlEdCaption.MaximumSize = new Size( int.MaxValue, this.edCaption.Height );
			this.pnlContainer.Controls.Add( this.pnlEdCaption );
			this.pnlEdCaption.ResumeLayout( false );
		}

		private void Build()
		{
            this.OnBuilding = true;

            // Tab control
            this.tcPad = new TabControl();
			this.tcPad.Hide();
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
			this.BuildCaptionEditor();
            this.BuildCheckBoxes();
			this.BuildExampleData();
			this.BuildPDFReference();
            this.BuildArgumentsListTable();
            this.BuildCommands();
            this.BuildPackageEditor();

			this.pnlContainer.ResumeLayout( false );
            this.Panel.ResumeLayout( false );
            this.tcPad.ResumeLayout( false );
			this.pnlContainer.Resize += (sender, e) => this.OnResizeEditor();
            this.OnBuilding = false;

            this.tcPad.TabPages[ 0 ].Padding = new Padding( 5 );
            this.tcPad.TabPages[ 1 ].Padding = new Padding( 5 );
			this.tcPad.Show();
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
				false,
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

			// Activate actions
            this.removeFunctionArgumentAction.Enabled =
                this.editDescriptionsAction.Enabled = true;


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

				// Disable related actions
				if ( this.grdArgsList.Rows.Count == 0 ) {
                    this.removeFunctionArgumentAction.Disable();
                    this.editDescriptionsAction.Disable();
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

				if ( arg.Viewer == Function.Argument.ViewerType.ColorPicker ) {
					var colorEditor = new ColorEditor( arg.Value, arg.Viewer, arg.AllowMultiselect );

					if ( colorEditor.ShowDialog() == DialogResult.OK ) {
						row.Cells[ colIndex ].Value = arg.Value = colorEditor.ToString();
					}
				}
				else
				if ( arg.Viewer == Function.Argument.ViewerType.ValueSet ) {
					var valed = new ValuesChooser( arg.ValueSet, arg.AllowMultiselect );

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
					// Set viewer to value set
					arg.Viewer = Function.Argument.ViewerType.ValueSet;
					var cmbCell = (DataGridViewComboBoxCell) row.Cells[ 6 ];
					cmbCell.Value = cmbCell.Items[ (int) Function.Argument.ViewerType.ValueSet ];

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
				arg.Name = (string) row.Cells[ colIndex ].Value;
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
			// The read-only info
			if ( colIndex == 3 ) {
				arg.IsReadOnly = (bool) row.Cells[ colIndex ].Value;
			}
			else
			// The required info
			if ( colIndex == 4 ) {
				arg.IsRequired = (bool) row.Cells[ colIndex ].Value;
			}
		    else
			// The "is data" info
			if ( colIndex == 5 ) {
				arg.IsDataArgument = (bool) row.Cells[ colIndex ].Value;
			}
			else
			// The multiselect info
			if ( colIndex == 6 ) {
				arg.AllowMultiselect = (bool) row.Cells[ colIndex ].Value;
			}
			else
			// The viewer info
			if ( colIndex == 7 ) {
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
		/// Shows the dialog of the descriptions editor
		/// </summary>
		private void OnEditDescriptions()
        {
            var descsEd = new DescriptionsEditor( this.Function );
            var fakeRoot = new RootMenu( new MenuDesign() );
            Function old = (Function) this.Function.Copy( fakeRoot );

            if ( descsEd.ShowDialog() != DialogResult.OK ) {
				this.Function.RegularArgumentList.Clear();

				foreach(Function.Argument arg in old.RegularArgumentList) {
					this.Function.RegularArgumentList.Add(
                        (Function.Argument) arg.Copy( this.Function ) );
                }
            }

            return;
		}

		/// <summary>
		/// Show a new dialog to edit the function call arguments
		/// </summary>
		private void OnEditFunctionCallArguments()
		{
			try {
	            var fakeRoot = new RootMenu( new MenuDesign() );
	            Function old = (Function) this.Function.Copy( fakeRoot );

				if ( this.fnCallsEditor == null ) {
					this.fnCallsEditor = new FunctionCallsGuiEditor( this.Function );
				}

				Trace.WriteLine( DateTime.Now + ": Opening function calls editor" );
	            if ( this.fnCallsEditor.ShowDialog() != DialogResult.OK ) {
	                this.Function.FunctionCallsArgumentList.Clear();

	                foreach(Function.CallArgument fnCall in old.FunctionCallsArgumentList) {
	                    this.Function.FunctionCallsArgumentList.Add(
	                        (Function.CallArgument) fnCall.Copy( this.Function ) );
	                }
	            }
			} catch(Exception exc)
			{
				Trace.WriteLine( DateTime.Now + ": Unexpected exception." );
				Trace.Indent();
				Trace.WriteLine( exc.Message );
				Trace.WriteLine( exc.StackTrace );
				Trace.Unindent();
				MessageBox.Show( "Unexpected exception. \nPlease send an error report with the log (Help >> Show Log).",
									AppInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			Trace.WriteLine( DateTime.Now + ": Finished function calls editor" );
            return;
		}

		/// <summary>
		/// Makes the arguments list occupy the whole width of the container panel.
		/// </summary>
        private void OnResizeEditor()
        {
            int width = this.pnlContainer.ClientSize.Width;

            // Name
            this.grdArgsList.Columns[ 0 ].Width = (int) ( width * 0.10 );

            // Value
            this.grdArgsList.Columns[ 1 ].Width = (int) ( width * 0.15 );

            // Depends
            this.grdArgsList.Columns[ 2 ].Width = (int) ( width * 0.10 );

			// Read only
			this.grdArgsList.Columns[ 3 ].Width = (int) ( width * 0.10 );

            // Is data
            this.grdArgsList.Columns[ 4 ].Width = (int) ( width * 0.10 );

			// Multiselect
			this.grdArgsList.Columns[ 5 ].Width = (int) ( width * 0.10 );

			// Required
            this.grdArgsList.Columns[ 6 ].Width = (int) ( width * 0.10 );

            // Viewer
            this.grdArgsList.Columns[ 7 ].Width = (int) ( width * 0.15 );

			// Editor
			this.grdArgsList.Columns[ 8 ].Width = (int) ( width * 0.07 );
        }

        /// <summary>
        /// Reads the data from component, and reflects it on the editor.
        /// </summary>
        public new void ReadDataFromComponent()
        {
            base.ReadDataFromComponent();
            base.OnBuilding = true;

            // Checkboxes
            this.chkFunctionHasData.Checked = this.Function.HasData;
            this.chkFunctionRemoveQuotes.Checked = this.Function.RemoveQuotationMarks;
            this.chkFunctionDataHeader.Checked = this.Function.DataHeader;
			this.chkIsDefault.Checked = this.Function.IsDefault;

            // Data
            this.edFunctionData.Text = this.Function.ExampleData;
            this.edFunctionPreCommand.Text = this.Function.PreCommand;
            this.edFunctionExecuteOnce.Text = this.Function.PreProgramOnce.ToString();
            this.udFunctionStartColumn.Value = Math.Max( 0, this.Function.StartColumn );
            this.udFunctionEndColumn.Value = Math.Max( 0, this.Function.EndColumn );
			this.edPDFFileName.Text = Path.GetFileName( this.Function.PDFName );
			this.udFunctionStartPage.Value = this.Function.PDFPageNumber;
			this.edPackageName.Text = this.Function.Package;
			this.edCaption.Text = this.Function.Caption;

            // Arguments
			this.grdArgsList.Rows.Clear();
            foreach(Function.Argument arg in this.Function.RegularArgumentList) {
                this.grdArgsList.Rows.Add();
                DataGridViewRow row = this.grdArgsList.Rows[ this.grdArgsList.Rows.Count - 1 ];

                row.Cells[ 0 ].Value = arg.Name;
                row.Cells[ 1 ].Value = arg.Value;
                row.Cells[ 2 ].Value = arg.DependsFrom;
				row.Cells[ 3 ].Value = arg.IsReadOnly;
                row.Cells[ 4 ].Value = arg.IsRequired;
				row.Cells[ 5 ].Value = arg.IsDataArgument;
                row.Cells[ 6 ].Value = arg.AllowMultiselect;
                row.Cells[ 7 ].Value = arg.Viewer.ToString();
				row.Cells[ 8 ].Value = UserAction.ImageList.Images[ UserAction.LookUp( "properties" ).ImageIndex ];
            }

            this.OnBuilding = false;
            return;
        }

		/// <summary>
		/// Ensures all edits are finished.
		/// </summary>
		public override void FinishEditing()
		{
			this.grdArgsList.EndEdit();
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
		private Panel pnlEdCaption;
        private TabControl tcPad;

		private CheckBox chkFunctionHasData;
		private CheckBox chkFunctionRemoveQuotes;
		private CheckBox chkFunctionDataHeader;
		private CheckBox chkIsDefault;
		private DataGridView grdArgsList;
		private Button btFunctionAddArgument;
		private Button btEditFnCallArguments;
		private Button btEditDescriptions;
		private Button btFunctionRemoveArgument;

		private TextBox edFunctionData;
		private TextBox edFunctionPreCommand;
		private TextBox edFunctionExecuteOnce;
		private TextBox edPackageName;
		private TextBox edCaption;
        private ComboBox edPDFFileName;
		private NumericUpDown udFunctionStartColumn;
		private NumericUpDown udFunctionEndColumn;
        private NumericUpDown udFunctionStartPage;

		private UserAction addFunctionArgumentAction;
		private UserAction editFnCallArgumentsAction;
		private UserAction editDescriptionsAction;
		private UserAction removeFunctionArgumentAction;
	}
}

