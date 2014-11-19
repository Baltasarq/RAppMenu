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
			this.UpdateFunctionProperties();
		}

		public override void Show()
		{
			base.Show();
			this.pnlChecks.Show();
			this.pnlPreCommand.Show();
			this.pnlDefaultData.Show();
			this.pnlExecuteOnce.Show();
			this.pnlStartColumn.Show();
			this.pnlEndColumn.Show();
			this.pnlArgsList.Show();

			this.addFunctionArgumentAction.CallBack = this.OnAddFunctionArgument;
			this.removeFunctionArgumentAction.CallBack = this.OnRemoveFunctionArgument;
		}

		private void BuildArgumentsListTable()
		{
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
			//comboBoxCellTemplate.ComboBoxStyle = ComboBoxStyle.DropDown;
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

			this.grdArgsList.CellContentClick += (sender, e) => this.OnCellClicked();
		}

		private void Build()
		{
			var toolTips = new ToolTip();

			// Check boxes
			this.pnlChecks = new TableLayoutPanel();
			this.pnlChecks.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
			this.pnlChecks.AutoSize = true;
			this.pnlChecks.Dock = DockStyle.Top;

			this.chkFunctionHasData = new CheckBox();
			this.chkFunctionHasData.Text = "Has data";
			this.chkFunctionHasData.Dock = DockStyle.Fill;
			this.chkFunctionHasData.MinimumSize =
				new Size( this.chkFunctionHasData.Width, this.chkFunctionHasData.Height * 2 );
			pnlChecks.Controls.Add( this.chkFunctionHasData );

			this.chkFunctionDataHeader = new CheckBox();
			this.chkFunctionDataHeader.Text = "Has data header";
			this.chkFunctionDataHeader.Dock = DockStyle.Fill;
			this.chkFunctionDataHeader.MinimumSize =
				new Size( this.chkFunctionDataHeader.Width, this.chkFunctionDataHeader.Height * 2 );
			pnlChecks.Controls.Add( chkFunctionDataHeader );

			this.chkFunctionRemoveQuotes = new CheckBox();
			this.chkFunctionRemoveQuotes.Text = "Remove quotation marks";
			this.chkFunctionRemoveQuotes.Dock = DockStyle.Fill;
			this.chkFunctionRemoveQuotes.MinimumSize =
				new Size( this.chkFunctionRemoveQuotes.Width, this.chkFunctionRemoveQuotes.Height * 2 );
			pnlChecks.Controls.Add( chkFunctionRemoveQuotes );
			this.Panel.Controls.Add( this.pnlChecks );

			// Default data
			this.pnlDefaultData = new Panel();
			this.pnlDefaultData.Dock = DockStyle.Top;
			var lblDefaultData = new Label();
			lblDefaultData.Text = "Default data:";
			lblDefaultData.AutoSize = false;
			lblDefaultData.TextAlign = ContentAlignment.MiddleLeft;
			lblDefaultData.Dock = DockStyle.Left;
			this.edFunctionDefaultData = new TextBox();
			this.edFunctionDefaultData.Font = new Font( this.edFunctionDefaultData.Font, FontStyle.Bold );
			this.edFunctionDefaultData.Dock = DockStyle.Fill;
			pnlDefaultData.Controls.Add( this.edFunctionDefaultData );
			pnlDefaultData.Controls.Add( lblDefaultData );
			pnlDefaultData.MaximumSize = new Size( int.MaxValue, this.edFunctionDefaultData.Height );
			this.Panel.Controls.Add( this.pnlDefaultData );

			// Pre-command
			this.pnlPreCommand = new Panel();
			this.pnlPreCommand.Dock = DockStyle.Top;
			var lblPreCommand = new Label();
			lblPreCommand.Text = "Pre-Command:";
			lblPreCommand.AutoSize = false;
			lblPreCommand.TextAlign = ContentAlignment.MiddleLeft;
			lblPreCommand.Dock = DockStyle.Left;
			this.edFunctionPreCommand = new TextBox();
			this.edFunctionPreCommand.Dock = DockStyle.Fill;
			this.edFunctionPreCommand.Font = new Font( this.edFunctionPreCommand.Font, FontStyle.Bold );
			this.pnlPreCommand.Controls.Add( this.edFunctionPreCommand );
			this.pnlPreCommand.Controls.Add( lblPreCommand );
			this.pnlPreCommand.MaximumSize = new Size( int.MaxValue, this.edFunctionPreCommand.Height );
			this.Panel.Controls.Add( this.pnlPreCommand );

			// Execute once
			this.pnlExecuteOnce = new Panel();
			this.pnlExecuteOnce.Dock = DockStyle.Top;
			var lblExecuteOnce = new Label();
			lblExecuteOnce.Text = "Execute once:";
			lblExecuteOnce.AutoSize = false;
			lblExecuteOnce.TextAlign = ContentAlignment.MiddleLeft;
			lblExecuteOnce.Dock = DockStyle.Left;
			this.edFunctionExecuteOnce = new TextBox();
			this.edFunctionExecuteOnce.Dock = DockStyle.Fill;
			this.edFunctionExecuteOnce.Font = new Font( this.edFunctionExecuteOnce.Font, FontStyle.Bold );
			this.edFunctionExecuteOnce.Multiline = true;
			this.pnlExecuteOnce.Controls.Add( this.edFunctionExecuteOnce );
			this.pnlExecuteOnce.Controls.Add( lblExecuteOnce );
			this.pnlExecuteOnce.MaximumSize = new Size( int.MaxValue, this.edFunctionExecuteOnce.Height );
			this.Panel.Controls.Add( this.pnlExecuteOnce );

			// Start column
			this.pnlStartColumn = new FlowLayoutPanel();
			this.pnlStartColumn.Dock = DockStyle.Top;
			var lblStartColumn = new Label();
			lblStartColumn.Text = "Start column:";
			lblStartColumn.AutoSize = false;
			lblStartColumn.TextAlign = ContentAlignment.MiddleLeft;
			lblStartColumn.Dock = DockStyle.Left;
			this.udFunctionStartColumn = new NumericUpDown();
			this.udFunctionStartColumn.Dock = DockStyle.Fill;
			this.udFunctionStartColumn.TextAlign = HorizontalAlignment.Right;
			this.udFunctionStartColumn.Font = new Font( this.udFunctionStartColumn.Font, FontStyle.Bold );
			this.pnlStartColumn.Controls.Add( lblStartColumn );
			this.pnlStartColumn.Controls.Add( this.udFunctionStartColumn );
			this.pnlStartColumn.MaximumSize = new Size( int.MaxValue, this.udFunctionStartColumn.Height );
			this.Panel.Controls.Add( this.pnlStartColumn );

			// End column
			this.pnlEndColumn = new FlowLayoutPanel();
			this.pnlEndColumn.Dock = DockStyle.Top;
			var lblEndColumn = new Label();
			lblEndColumn.Text = "End column:";
			lblEndColumn.AutoSize = false;
			lblEndColumn.TextAlign = ContentAlignment.MiddleLeft;
			this.udFunctionEndColumn = new NumericUpDown();

			this.udFunctionEndColumn.Font = new Font( this.udFunctionEndColumn.Font, FontStyle.Bold );
			this.udFunctionEndColumn.TextAlign = HorizontalAlignment.Right;
			this.pnlEndColumn.Controls.Add( lblEndColumn );
			this.pnlEndColumn.Controls.Add( this.udFunctionEndColumn );
			this.pnlEndColumn.MaximumSize = new Size( int.MaxValue, this.udFunctionStartColumn.Height );
			this.Panel.Controls.Add( this.pnlEndColumn );

			//Font f = this.udFunctionEndColumn.Font;
			// CALCULATE HERE THE SIZE OF EACH DIGIT and SET WIDTH x 10

			// Arguments gridview
			this.pnlArgsList = new GroupBox();
			this.pnlArgsList.Dock = DockStyle.Fill;
			this.pnlArgsList.Text = "Arguments";
			this.BuildArgumentsListTable();
			this.pnlArgsList.Controls.Add( this.grdArgsList );
			this.Panel.Controls.Add( this.pnlArgsList );

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

			this.addFunctionArgumentAction.AddComponent( this.btFunctionAddArgument );
			this.removeFunctionArgumentAction.AddComponent( this.btFunctionRemoveArgument );
			this.pnlButtons.Controls.Add( this.btFunctionAddArgument );
			this.pnlButtons.Controls.Add( this.btFunctionRemoveArgument );
			this.pnlArgsList.Controls.Add( this.pnlButtons );

			return;
		}

		private void UpdateFunctionProperties()
		{
			var f = (Function) this.MenuComponent;

			this.chkFunctionHasData.Checked = f.HasData;
			this.chkFunctionDataHeader.Checked = f.DataHeader;
			this.chkFunctionRemoveQuotes.Checked = f.RemoveQuotationMarks;

			this.edFunctionPreCommand.Text = f.PreProgram.ToString();
			this.edFunctionDefaultData.Text = f.DefaultData;
		}

		private void OnAddFunctionArgument()
		{
			int colCount = this.grdArgsList.Rows.Count;
			string rowCount = this.grdArgsList.Rows.Count.ToString();

			this.grdArgsList.Rows.Add( new object[] {
				"arg" + rowCount,
				"argTag" + rowCount,
				"argDepends" + rowCount,
				false,
				false,
				null
			});

			var cmbCol = (DataGridViewComboBoxCell) this.grdArgsList.Columns[ colCount -1 ];
			cmbCol.item
		}

		private void OnRemoveFunctionArgument()
		{
		}

		private void OnCellClicked()
		{
		}

		private TableLayoutPanel pnlChecks;
		private Panel pnlDefaultData;
		private Panel pnlPreCommand;
		private Panel pnlExecuteOnce;
		private Panel pnlStartColumn;
		private Panel pnlEndColumn;
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

