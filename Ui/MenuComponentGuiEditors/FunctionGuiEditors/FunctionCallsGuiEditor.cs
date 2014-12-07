using System;
using System.Drawing;
using System.Windows.Forms;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
	public class FunctionCallsGuiEditor: Form {
		public FunctionCallsGuiEditor(Function f)
		{
			this.function = f;
			this.StartPosition = FormStartPosition.CenterParent;

			Bitmap icon = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.r-editor.png" )
			);

			this.Icon = Icon.FromHandle( icon.GetHicon() );
			this.Text = "Function call arguments editor";

			this.addFunctionCallAction = UserAction.LookUp( "addfunctioncallargument" );
			this.removeFunctionCallAction = UserAction.LookUp( "removefunctioncallargument" );
			this.addFunctionCallArgumentAction = UserAction.LookUp( "addargumenttofunctioncall" );
			this.removeFunctionCallArgumentAction = UserAction.LookUp( "removeargumentfromfunctioncall" );

			this.Build();
		}

		/// <summary>
		/// Gets the function this editor modifies its function call arguments.
		/// </summary>
		/// <value>The function.</value>
		public Function Function {
			get {
				return this.function;
			}
		}

        /// <summary>
        /// Populates the datagridview of function calls.
        /// </summary>
        private void Populate()
        {
            this.grdFnCallList.Hide();
            this.grdFnCallArgsList.Hide();

            this.grdFnCallList.Rows.Clear();
            this.grdFnCallArgsList.Rows.Clear();

            foreach(Function.CallArgument call in this.Function.FunctionCallsArgumentList)
            {
                this.grdFnCallList.Rows.Add(
                    call.Name,
                    call.FunctionName,
                    call.Variant
                );
            }

            this.grdFnCallList.Show();
            this.grdFnCallArgsList.Show();
            return;
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
            this.addFunctionCallArgumentAction.Enabled = this.removeFunctionCallAction.Enabled;

            this.Populate();
		}

		private void Build()
		{
			this.pnlFnCallsLists = new GroupBox();
            this.pnlFnCallsLists.Font = new Font( this.pnlFnCallsLists.Font, FontStyle.Bold );
			this.pnlFnCallsLists.SuspendLayout();
			this.pnlFnCallsLists.Dock = DockStyle.Fill;
			this.pnlFnCallsLists.Text = "Function call arguments";

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
			this.pnlFnCallArgs.AutoScroll = true;
			this.pnlFnCallArgs.Dock = DockStyle.Fill;
			this.pnlFnCallArgs.SuspendLayout();

			this.grdFnCallArgsList = new DataGridView();
            this.grdFnCallArgsList.Font = new Font( this.grdFnCallArgsList.Font, FontStyle.Regular );
            this.grdFnCallArgsList.BackgroundColor = Color.White;
			this.grdFnCallArgsList.AllowUserToResizeRows = false;
			this.grdFnCallArgsList.RowHeadersVisible = false;
			this.grdFnCallArgsList.AutoGenerateColumns = false;
			this.grdFnCallArgsList.AllowUserToAddRows = false;
			this.grdFnCallArgsList.MultiSelect = false;
			this.grdFnCallArgsList.Dock = DockStyle.Fill;
			this.grdFnCallArgsList.AllowUserToOrderColumns = false;

			var textCellTemplate = new DataGridViewTextBoxCell();
			textCellTemplate.Style.BackColor = Color.Wheat;

            var checkBoxCellTemplate = new DataGridViewCheckBoxCell();
            checkBoxCellTemplate.Style.BackColor = Color.White;

            var textCellTemplateMonoSpaced = new DataGridViewTextBoxCell();
            textCellTemplateMonoSpaced.Style.BackColor = Color.Wheat;
            textCellTemplateMonoSpaced.Style.Font = new Font( FontFamily.GenericMonospace, 8 );

			var column0 = new DataGridViewTextBoxColumn();
            var column1 = new DataGridViewCheckBoxColumn();
            var column2 = new DataGridViewTextBoxColumn();

			column0.CellTemplate = textCellTemplate;
            column1.CellTemplate = checkBoxCellTemplate;
            column2.CellTemplate = textCellTemplateMonoSpaced;

			column0.HeaderText = "Name";
			column0.Width = 120;
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
            column1.HeaderText = "Read only";
            column1.Width = 120;
            column1.SortMode = DataGridViewColumnSortMode.NotSortable;
			column2.HeaderText = "Value";
			column2.Width = 120;
            column2.SortMode = DataGridViewColumnSortMode.NotSortable;

			this.grdFnCallArgsList.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
                column2
			} );

			this.grdFnCallArgsList.MinimumSize = new Size( 240, 100 );
			this.grdFnCallArgsList.CellEndEdit +=
				(object sender, DataGridViewCellEventArgs evt) => {
				this.OnFnCallArgCellEdited( evt.RowIndex, evt.ColumnIndex );
			};

			// Buttons panel
			this.pnlFnCallArgButtons = new FlowLayoutPanel();
			this.pnlFnCallArgButtons.AutoSize = true;
			this.pnlFnCallArgButtons.Dock = DockStyle.Bottom;
            this.btFunctionAddFnCallArg = new Button();
            this.btFunctionRemoveFnCallArg = new Button();

			this.btFunctionAddFnCallArg.Size = this.btFunctionAddFnCallArg.MinimumSize = 
				this.btFunctionAddFnCallArg.MaximumSize = new Size( 32, 32 );
			this.btFunctionAddFnCallArg.ImageList = UserAction.ImageList;
			this.btFunctionAddFnCallArg.ImageIndex = this.addFunctionCallArgumentAction.ImageIndex;
			this.btFunctionAddFnCallArg.Click += (sender, e) => this.addFunctionCallArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionAddFnCallArg, this.addFunctionCallArgumentAction.Text );

			this.btFunctionRemoveFnCallArg.Size = this.btFunctionRemoveFnCallArg.MinimumSize = 
				this.btFunctionRemoveFnCallArg.MaximumSize = new Size( 32, 32 );
			this.btFunctionRemoveFnCallArg.ImageList = UserAction.ImageList;
			this.btFunctionRemoveFnCallArg.ImageIndex = this.removeFunctionCallArgumentAction.ImageIndex;
			this.btFunctionRemoveFnCallArg.Click += (sender, e) => this.removeFunctionCallArgumentAction.CallBack();
			toolTips.SetToolTip( this.btFunctionRemoveFnCallArg, this.removeFunctionCallArgumentAction.Text );

			this.pnlFnCallArgButtons.Controls.Add( btFunctionAddFnCallArg );
			this.pnlFnCallArgButtons.Controls.Add( btFunctionRemoveFnCallArg );

			this.addFunctionCallArgumentAction.AddComponent( this.btFunctionAddFnCallArg );
			this.removeFunctionCallArgumentAction.AddComponent( this.btFunctionRemoveFnCallArg );
			this.pnlFnCallArgs.Controls.Add( this.pnlFnCallArgButtons );
			this.pnlFnCallArgs.Controls.Add( this.grdFnCallArgsList );
			this.pnlFnCallArgs.ResumeLayout( false );
		}

		private void BuildFunctionCallListTable()
		{
			var toolTips = new ToolTip();

			this.pnlFnCalls = new Panel();
			this.pnlFnCalls.AutoScroll = true;
			this.pnlFnCalls.SuspendLayout();
			this.pnlFnCalls.Dock = DockStyle.Fill;

			this.grdFnCallList = new DataGridView();
            this.grdFnCallList.Font = new Font( this.grdFnCallList.Font, FontStyle.Regular );
            this.grdFnCallList.BackgroundColor = Color.White;
			this.grdFnCallList.AllowUserToResizeRows = false;
			this.grdFnCallList.RowHeadersVisible = false;
			this.grdFnCallList.AutoGenerateColumns = false;
			this.grdFnCallList.AllowUserToAddRows = false;
			this.grdFnCallList.MultiSelect = false;
			this.grdFnCallList.Dock = DockStyle.Fill;
			this.grdFnCallList.AllowUserToOrderColumns = false;

			var textCellTemplate = new DataGridViewTextBoxCell();
			textCellTemplate.Style.BackColor = Color.Wheat;

            var textCellTemplateMonoSpaced = new DataGridViewTextBoxCell();
            textCellTemplateMonoSpaced.Style.BackColor = Color.Wheat;
            textCellTemplateMonoSpaced.Style.Font = new Font( FontFamily.GenericMonospace, 8 );

			var column0 = new DataGridViewTextBoxColumn();
			var column1 = new DataGridViewTextBoxColumn();
			var column2 = new DataGridViewTextBoxColumn();

			column0.CellTemplate = textCellTemplate;
			column1.CellTemplate = textCellTemplate;
            column2.CellTemplate = textCellTemplateMonoSpaced;

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

            this.grdFnCallList.RowEnter += (object sender, DataGridViewCellEventArgs e) => 
                this.PrepareArgsForCurrentCall( e.RowIndex );

			this.grdFnCallList.MinimumSize = new Size( 360, 100 );

			// Buttons panel
			this.pnlFnCallButtons = new FlowLayoutPanel();
			this.pnlFnCallButtons.AutoSize = true;
			this.pnlFnCallButtons.Dock = DockStyle.Bottom;
			this.btFunctionAddFnCall = new Button();
            this.btFunctionRemoveFnCall = new Button();

			this.btFunctionAddFnCall.Size = this.btFunctionAddFnCall.MinimumSize = 
				this.btFunctionAddFnCall.MaximumSize = new Size( 32, 32 );
			this.btFunctionAddFnCall.ImageList = UserAction.ImageList;
			this.btFunctionAddFnCall.ImageIndex = this.addFunctionCallAction.ImageIndex;
			this.btFunctionAddFnCall.Click += (sender, e) => this.addFunctionCallAction.CallBack();
			toolTips.SetToolTip( this.btFunctionAddFnCall, this.addFunctionCallAction.Text );

			this.btFunctionRemoveFnCall.Size = this.btFunctionRemoveFnCall.MinimumSize = 
				this.btFunctionRemoveFnCall.MaximumSize = new Size( 32, 32 );
			this.btFunctionRemoveFnCall.ImageList = UserAction.ImageList;
            this.btFunctionRemoveFnCall.ImageIndex = this.removeFunctionCallAction.ImageIndex;
			this.btFunctionRemoveFnCall.Click += (sender, e) => this.removeFunctionCallAction.CallBack();
			toolTips.SetToolTip( this.btFunctionRemoveFnCall, this.removeFunctionCallAction.Text );

			// Prepare
			this.addFunctionCallAction.AddComponent( this.btFunctionAddFnCall );
			this.removeFunctionCallAction.AddComponent( this.btFunctionRemoveFnCall );
			this.pnlFnCallButtons.Controls.Add( this.btFunctionAddFnCall );
			this.pnlFnCallButtons.Controls.Add( this.btFunctionRemoveFnCall );
			this.pnlFnCalls.Controls.Add( this.pnlFnCallButtons );
			this.pnlFnCalls.Controls.Add( this.grdFnCallList );
			this.pnlFnCalls.ResumeLayout( false );
		}

        private void PrepareArgsForCurrentCall(int rowIndex)
        {
            if ( this.Function.FunctionCallsArgumentList.Count > 0 ) {
                var call = (Function.CallArgument) this.Function.FunctionCallsArgumentList[ rowIndex ];

                this.grdFnCallArgsList.Hide();
                this.grdFnCallArgsList.Rows.Clear();

                foreach(Function.CallArgument.Arg arg in call.ArgumentList) {
                    int lastArgIndex = this.grdFnCallArgsList.Rows.Count;

                    this.grdFnCallArgsList.Rows.Add();
                    DataGridViewRow row = this.grdFnCallArgsList.Rows[ lastArgIndex ];
                    row.Cells[ 0 ].Value = arg.Name;
                    row.Cells[ 1 ].Value = arg.IsReadOnly;
                    row.Cells[ 2 ].Value = arg.Value;
                }

                this.grdFnCallArgsList.Show();
            }

            this.removeFunctionCallArgumentAction.Enabled =
                ( this.grdFnCallArgsList.Rows.Count > 0 );
            return;
        }
		
		/// <summary>
		/// Updates the information of the function call being modified.
		/// </summary>
		/// <param name="rowIndex">The row index, as an int, which gives the argument number.</param>
		/// <param name="colIndex">The col index, as an int, which gives the attribute of the argument.</param>
		private void OnFnCallCellEdited(int rowIndex, int colIndex)
		{
            DataGridViewRow row = this.grdFnCallList.Rows[ rowIndex ];
            string value = (string) row.Cells[ colIndex ].Value;
            var fnCall = (Function.CallArgument) this.Function.FunctionCallsArgumentList[ rowIndex ];

            if ( !string.IsNullOrWhiteSpace( value ) ) {
                if ( colIndex == 0 ) {
                    fnCall.Name = value;
                }
                else
                if ( colIndex == 1 ) {
                    fnCall.FunctionName = value;
                }
                else
                if ( colIndex == 2 ) {
                    fnCall.Variant = value;
                }
            }

            return;
		}

		/// <summary>
		/// Updates the information of the function call argument being modified.
		/// </summary>
		/// <param name="rowIndex">The row index, as an int, which gives the argument number.</param>
		/// <param name="colIndex">The col index, as an int, which gives the attribute of the argument.</param>
		private void OnFnCallArgCellEdited(int rowIndex, int colIndex)
		{
            DataGridViewRow row = this.grdFnCallArgsList.Rows[ rowIndex ];
            Function.CallArgument fnCall = this.GetCurrentFunctionCall();
            var arg = (Function.CallArgument.Arg) fnCall.ArgumentList[ rowIndex ];

            if ( fnCall != null ) {
                if ( colIndex != 1 ) {
                    string value = (string) row.Cells[ colIndex ].Value;

                    if( !string.IsNullOrWhiteSpace( value ) )
                    {
                        if ( colIndex == 0 ) {
                            arg.Name = value;
                        }
                        else
                        if ( colIndex == 2 ) {
                            arg.Value = value;
                        }
                    }
                }
                else
                if ( colIndex == 1 ) {
                    arg.IsReadOnly = (bool) row.Cells[ colIndex ].Value;
                }
            }

            return;
		}

        /// <summary>
        /// Gets the current function call that is selected.
        /// </summary>
        /// <returns>The current function call.</returns>
        private Function.CallArgument GetCurrentFunctionCall()
        {
            Function.CallArgument toret = null;
            DataGridViewCell cell = this.grdFnCallList.CurrentCell;

            if ( cell != null ) {
                toret = (Function.CallArgument)
                    this.Function.FunctionCallsArgumentList[ cell.RowIndex ];
            }

            return toret;
        }

		private void OnAddFunctionCall()
		{
            int lastCallIndex = this.grdFnCallList.Rows.Count;
            string name = "arg" + lastCallIndex;

            // Add in the UI
			this.grdFnCallList.Rows.Add();
            this.grdFnCallList.Rows[ lastCallIndex ].Cells[ 0 ].Value = name;

            // Add in the function
            this.Function.FunctionCallsArgumentList.Add(
                new Function.CallArgument( name, this.Function ) );

            this.removeFunctionCallAction.Enable();
            this.addFunctionCallArgumentAction.Enable();
		}

		private void OnRemoveFunctionCall()
		{
            DataGridViewCell cell = this.grdFnCallList.CurrentCell;

            if ( this.grdFnCallList.Rows.Count > 0 ) {
                if ( cell != null ) {
                    int rowIndex = cell.RowIndex;

                    // Remove in the UI
                    this.grdFnCallList.Rows.RemoveAt( rowIndex );

                    // Remove in the function
                    this.Function.FunctionCallsArgumentList.RemoveAt( rowIndex );
                }

                if ( this.grdFnCallList.Rows.Count == 0 ) {
                    this.removeFunctionCallAction.Disable();
                    this.addFunctionCallArgumentAction.Disable();
                }
            }

            return;
		}

		private void OnAddFunctionCallArgument()
		{
            Function.CallArgument currentCall = this.GetCurrentFunctionCall();

            if ( currentCall != null ) {
                int lastCallArgIndex = this.grdFnCallArgsList.Rows.Count;
                string name = currentCall.Name + ".arg" + lastCallArgIndex;

                // Add a new argument in the UI
    			this.grdFnCallArgsList.Rows.Add();
                this.grdFnCallArgsList.Rows[ lastCallArgIndex ].Cells[ 0 ].Value = name;

                // Add a new argument to the function call
                currentCall.ArgumentList.Add(
                    new Function.CallArgument.Arg( name, currentCall ) );

                this.removeFunctionCallArgumentAction.Enable();
            }

            return;
		}

		private void OnRemoveFunctionCallArgument()
		{
            DataGridViewCell cell = this.grdFnCallArgsList.CurrentCell;

            if ( this.grdFnCallArgsList.Rows.Count > 0 ) {
                if ( cell != null ) {
                    Function.CallArgument currentCall = this.GetCurrentFunctionCall();
                    int rowIndex = cell.RowIndex;

                    // Remove in UI
                    this.grdFnCallArgsList.Rows.RemoveAt( rowIndex );

                    // Remove in function
                    currentCall.ArgumentList.RemoveAt( rowIndex );
                }

                if ( this.grdFnCallArgsList.Rows.Count == 0 ) {
                    this.removeFunctionCallArgumentAction.Disable();
                }
            }

            return;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize( e );

			// Set splitter
			this.spFnCallLists.SplitterDistance = (int) ( this.pnlFnCallsLists.Width * 0.50 );

			// Set columns
			int widthPanel1 = this.spFnCallLists.Panel1.ClientSize.Width;
			int widthPanel2 = this.spFnCallLists.Panel2.ClientSize.Width;

			// FnCall: Name
			this.grdFnCallList.Columns[ 0 ].Width = (int) ( widthPanel1 * 0.33 );

			// FnCall: Function name
			this.grdFnCallList.Columns[ 1 ].Width = (int) ( widthPanel1 * 0.33 );

			// FnCall: Variant
			this.grdFnCallList.Columns[ 2 ].Width = (int) ( widthPanel1 * 0.34 );

			// Arg: Name
			this.grdFnCallArgsList.Columns[ 0 ].Width = (int) ( widthPanel2 * 0.40 );

            // Arg: Read only
            this.grdFnCallArgsList.Columns[ 1 ].Width = (int) ( widthPanel2 * 0.20 );

			// Arg: Value
			this.grdFnCallArgsList.Columns[ 2 ].Width = (int) ( widthPanel2 * 0.40 );
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

		private Function function;
	}
}

