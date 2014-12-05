using System;
using System.Drawing;
using System.Windows.Forms;

namespace RAppMenu.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
	public class FunctionCallsGuiEditor: Form {
		public FunctionCallsGuiEditor()
		{
			this.addFunctionCallAction = UserAction.LookUp( "addfunctioncallargument" );
			this.removeFunctionCallAction = UserAction.LookUp( "removefunctioncallargument" );
			this.addFunctionCallArgumentAction = UserAction.LookUp( "addargumenttofunctioncall" );
			this.removeFunctionCallArgumentAction = UserAction.LookUp( "removeargumentfromfunctioncall" );

			this.Build();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown( e );

			this.addFunctionCallAction.CallBack = this.OnAddFunctionCall;
			this.removeFunctionCallAction.CallBack = this.OnRemoveFunctionCall;
			this.addFunctionCallArgumentAction.CallBack = this.OnAddFunctionCallArgument;
			this.removeFunctionCallArgumentAction.CallBack = this.OnRemoveFunctionCallArgument;

			this.removeFunctionCallAction.Enabled = ( this.grdFnCallList.Rows.Count > 0 );
			this.removeFunctionCallArgumentAction.Enabled = ( this.grdFnCallArgsList.Rows.Count > 0 );
		}

		private void Build()
		{
			this.pnlFnCallsLists = new GroupBox();
			this.pnlFnCallsLists.SuspendLayout();
			this.pnlFnCallsLists.Dock = DockStyle.Fill;
			this.pnlFnCallsLists.Text = "Function calls";

			this.spFnCallLists = new SplitContainer();
			this.spFnCallLists.Dock = DockStyle.Fill;

			this.BuildToolbar();
			this.BuildFunctionCallListTable();
			this.BuildFunctionCallArgumentsListTable();

			this.spFnCallLists.Panel1.Controls.Add( this.pnlFnCalls );
			this.spFnCallLists.Panel2.Controls.Add( this.pnlFnCallArgs );
			this.spFnCallLists.IsSplitterFixed = true;

			this.pnlFnCallsLists.Controls.Add( this.spFnCallLists );
			this.Controls.Add( this.pnlFnCallsLists );
			this.Controls.Add( this.tbToolbar );

			this.pnlFnCallsLists.ResumeLayout( false );
			this.MinimumSize = new Size( 640, 480 );
		}

		private void BuildToolbar()
		{
			var quitAction = UserAction.LookUp( "quit" );

			this.tbToolbar = new ToolStrip();
			this.tbToolbar.BackColor = Color.DarkGray;
			this.tbToolbar.Dock = DockStyle.Top;
			this.tbToolbar.ImageList = UserAction.ImageList;

			// Buttons
			this.tbbQuit = new ToolStripButton();
			this.tbbQuit.ImageIndex = quitAction.ImageIndex;
			this.tbbQuit.ToolTipText = quitAction.Text;
			this.tbbQuit.Click += (sender, e) => this.Close();

			this.tbToolbar.Items.Add( tbbQuit );
		}

		/// <summary>
		/// Builds the grid view for the arguments of function call arguments.
		/// </summary>
		private void BuildFunctionCallArgumentsListTable()
		{
			var toolTips = new ToolTip();

			this.pnlFnCallArgs = new Panel();
			this.pnlFnCallArgs.Dock = DockStyle.Fill;
			this.pnlFnCallArgs.SuspendLayout();

			this.grdFnCallArgsList = new DataGridView();
			this.grdFnCallArgsList.AllowUserToResizeRows = false;
			this.grdFnCallArgsList.RowHeadersVisible = false;
			this.grdFnCallArgsList.AutoGenerateColumns = false;
			this.grdFnCallArgsList.AllowUserToAddRows = false;
			this.grdFnCallArgsList.MultiSelect = false;
			this.grdFnCallArgsList.Dock = DockStyle.Fill;
			this.grdFnCallArgsList.AllowUserToOrderColumns = false;

			var textCellTemplate = new DataGridViewTextBoxCell();
			textCellTemplate.Style.BackColor = Color.Wheat;

			var column0 = new DataGridViewTextBoxColumn();
			var column1 = new DataGridViewTextBoxColumn();

			column0.CellTemplate = textCellTemplate;
			column1.CellTemplate = textCellTemplate;

			column0.HeaderText = "Name";
			column0.Width = 120;
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
			column1.HeaderText = "Value";
			column1.Width = 120;

			this.grdFnCallArgsList.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
			} );

			this.grdFnCallArgsList.MinimumSize = new Size( 240, 100 );
			this.grdFnCallArgsList.Font = new Font( FontFamily.GenericMonospace, 10 );
			this.grdFnCallArgsList.CellEndEdit +=
				(object sender, DataGridViewCellEventArgs evt) => {
				this.OnFnCallArgCellEdited( evt.RowIndex, evt.ColumnIndex );
			};

			// Buttons panel
			this.pnlFnCallArgButtons = new FlowLayoutPanel();
			this.pnlFnCallArgButtons.AutoSize = true;
			this.pnlFnCallArgButtons.Dock = DockStyle.Bottom;
			this.btFunctionAddFnCallArg = new Button();

			this.btFunctionAddFnCallArg.Size = this.btFunctionAddFnCallArg.MinimumSize = 
				this.btFunctionAddFnCallArg.MaximumSize = new Size( 32, 32 );
			this.btFunctionAddFnCallArg.ImageList = UserAction.ImageList;
			this.btFunctionAddFnCallArg.ImageIndex = this.addFunctionCallArgumentAction.ImageIndex;
			this.btFunctionAddFnCallArg.Click += (sender, e) => this.addFunctionCallArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionAddFnCallArg, this.addFunctionCallArgumentAction.Text );

			this.btFunctionRemoveFnCallArg = new Button();
			this.btFunctionRemoveFnCallArg.Size = this.btFunctionRemoveFnCallArg.MinimumSize = 
				this.btFunctionRemoveFnCallArg.MaximumSize = new Size( 32, 32 );
			this.btFunctionRemoveFnCallArg.ImageList = UserAction.ImageList;
			this.btFunctionRemoveFnCallArg.ImageIndex = this.removeFunctionCallArgumentAction.ImageIndex;
			this.btFunctionRemoveFnCallArg.Click += (sender, e) => this.removeFunctionCallArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionRemoveFnCallArg, this.removeFunctionCallArgumentAction.Text );

			this.pnlFnCallArgButtons.Controls.Add( btFunctionAddFnCallArg );
			this.pnlFnCallArgButtons.Controls.Add( btFunctionRemoveFnCallArg );

			this.pnlFnCallArgs.Controls.Add( this.pnlFnCallArgButtons );
			this.pnlFnCallArgs.Controls.Add( this.grdFnCallArgsList );
			this.pnlFnCallArgs.ResumeLayout( false );
		}

		private void BuildFunctionCallListTable()
		{
			var toolTips = new ToolTip();

			this.pnlFnCalls = new Panel();
			this.pnlFnCalls.SuspendLayout();
			this.pnlFnCalls.Dock = DockStyle.Fill;

			this.grdFnCallList = new DataGridView();
			this.grdFnCallList.AllowUserToResizeRows = false;
			this.grdFnCallList.RowHeadersVisible = false;
			this.grdFnCallList.AutoGenerateColumns = false;
			this.grdFnCallList.AllowUserToAddRows = false;
			this.grdFnCallList.MultiSelect = false;
			this.grdFnCallList.Dock = DockStyle.Fill;
			this.grdFnCallList.AllowUserToOrderColumns = false;

			var textCellTemplate = new DataGridViewTextBoxCell();
			textCellTemplate.Style.BackColor = Color.Wheat;

			var column0 = new DataGridViewTextBoxColumn();
			var column1 = new DataGridViewTextBoxColumn();
			var column2 = new DataGridViewTextBoxColumn();

			column0.CellTemplate = textCellTemplate;
			column1.CellTemplate = textCellTemplate;
			column2.CellTemplate = textCellTemplate;

			column0.HeaderText = "Name";
			column0.Width = 120;
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
			column1.HeaderText = "Function";
			column1.Width = 120;
			column1.SortMode = DataGridViewColumnSortMode.NotSortable;
			column2.HeaderText = "Variant";
			column2.Width = 120;
			column2.SortMode = DataGridViewColumnSortMode.NotSortable;

			this.grdFnCallList.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
				column2,
			} );

			this.grdFnCallList.CellEndEdit +=
				(object sender, DataGridViewCellEventArgs evt) => {
				this.OnFnCallCellEdited( evt.RowIndex, evt.ColumnIndex );
			};

			this.grdFnCallList.MinimumSize = new Size( 360, 100 );
			this.grdFnCallList.Font = new Font( this.grdFnCallList.Font, FontStyle.Regular );
			this.pnlFnCallsLists.Font = new Font( this.pnlFnCallsLists.Font, FontStyle.Bold );

			// Buttons panel
			this.pnlFnCallButtons = new FlowLayoutPanel();
			this.pnlFnCallButtons.AutoSize = true;
			this.pnlFnCallButtons.Dock = DockStyle.Bottom;
			this.btFunctionAddFnCall = new Button();

			this.btFunctionAddFnCall.Size = this.btFunctionAddFnCall.MinimumSize = 
				this.btFunctionAddFnCall.MaximumSize = new Size( 32, 32 );
			this.btFunctionAddFnCall.ImageList = UserAction.ImageList;
			this.btFunctionAddFnCall.ImageIndex = this.addFunctionCallArgumentAction.ImageIndex;
			this.btFunctionAddFnCall.Click += (sender, e) => this.addFunctionCallArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionAddFnCall, this.addFunctionCallArgumentAction.Text );

			this.btFunctionRemoveFnCall = new Button();
			this.btFunctionRemoveFnCall.Size = this.btFunctionRemoveFnCall.MinimumSize = 
				this.btFunctionRemoveFnCall.MaximumSize = new Size( 32, 32 );
			this.btFunctionRemoveFnCall.ImageList = UserAction.ImageList;
			this.btFunctionRemoveFnCall.ImageIndex = this.removeFunctionCallArgumentAction.ImageIndex;
			this.btFunctionRemoveFnCall.Click += (sender, e) => this.removeFunctionCallArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionRemoveFnCall, this.removeFunctionCallArgumentAction.Text );

			// Prepare
			this.addFunctionCallArgumentAction.AddComponent( this.btFunctionAddFnCall );
			this.removeFunctionCallArgumentAction.AddComponent( this.btFunctionRemoveFnCall );
			this.pnlFnCallButtons.Controls.Add( this.btFunctionAddFnCall );
			this.pnlFnCallButtons.Controls.Add( this.btFunctionRemoveFnCall );
			this.pnlFnCalls.Controls.Add( this.pnlFnCallButtons );
			this.pnlFnCalls.Controls.Add( this.grdFnCallList );
			this.pnlFnCalls.ResumeLayout( false );
		}

		
		/// <summary>
		/// Updates the information of the function call being modified.
		/// </summary>
		/// <param name="rowIndex">The row index, as an int, which gives the argument number.</param>
		/// <param name="colIndex">The col index, as an int, which gives the attribute of the argument.</param>
		private void OnFnCallCellEdited(int rowIndex, int colIndex)
		{
		}

		/// <summary>
		/// Updates the information of the function call argument being modified.
		/// </summary>
		/// <param name="rowIndex">The row index, as an int, which gives the argument number.</param>
		/// <param name="colIndex">The col index, as an int, which gives the attribute of the argument.</param>
		private void OnFnCallArgCellEdited(int rowIndex, int colIndex)
		{
		}

		private void OnAddFunctionCall()
		{
		}

		private void OnRemoveFunctionCall()
		{
		}

		private void OnAddFunctionCallArgument()
		{
		}

		private void OnRemoveFunctionCallArgument()
		{
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize( e );

			this.spFnCallLists.SplitterDistance = (int) ( this.pnlFnCallsLists.Width * 0.50 );
		}


		private ToolStrip tbToolbar;
		private GroupBox pnlFnCallsLists;
		private SplitContainer spFnCallLists;
		private Panel pnlFnCallButtons;
		private Panel pnlFnCallArgButtons;
		private Panel pnlFnCallArgs;
		private Panel pnlFnCalls;

		private DataGridView grdFnCallList;
		private DataGridView grdFnCallArgsList;

		private ToolStripButton tbbQuit;
		private Button btFunctionAddFnCall;
		private Button btFunctionRemoveFnCall;
		private Button btFunctionAddFnCallArg;
		private Button btFunctionRemoveFnCallArg;

		private UserAction addFunctionCallAction;
		private UserAction removeFunctionCallAction;
		private UserAction addFunctionCallArgumentAction;
		private UserAction removeFunctionCallArgumentAction;
	}
}

