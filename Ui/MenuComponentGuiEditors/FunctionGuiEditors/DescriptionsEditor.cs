using System;
using System.Drawing;
using System.Windows.Forms;
using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
	public class DescriptionsEditor: Form {
		public DescriptionsEditor(Function f)
		{
            this.Build();
            this.Function = f;
            this.Populate();

            this.grdDescs.CurrentCell = this.grdDescs.Rows[ 0 ].Cells[ 1 ];

            this.OnResize( null );
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
				this.grdDescs.CancelEdit();
                this.Close();
            };

            this.tbbSave = new ToolStripButton();
            this.tbbSave.ImageIndex = saveAction.ImageIndex;
            this.tbbSave.ToolTipText = saveAction.Text;
            this.tbbSave.Click += (sender, e) =>  {
                this.DialogResult = DialogResult.OK;
				this.grdDescs.EndEdit();
                this.Close();
            };

            this.tbToolbar.Items.Add( tbbQuit );
            this.tbToolbar.Items.Add( tbbSave );
        }

        private void BuildDescriptionsTable()
        {
            this.pnlDescsEditor = new GroupBox();
            this.pnlDescsEditor.SuspendLayout();
            this.pnlDescsEditor.Dock = DockStyle.Fill;
            this.pnlDescsEditor.Text = "Descriptions";
            this.pnlDescsEditor.Font = new Font( this.pnlDescsEditor.Font, FontStyle.Bold );

            this.grdDescs = new DataGridView();
            this.grdDescs.Font = new Font( this.grdDescs.Font, FontStyle.Regular );
            this.grdDescs.BackgroundColor = Color.White;
            this.grdDescs.AllowUserToResizeRows = false;
            this.grdDescs.RowHeadersVisible = false;
            this.grdDescs.AutoGenerateColumns = false;
            this.grdDescs.AllowUserToAddRows = false;
            this.grdDescs.MultiSelect = false;
            this.grdDescs.Dock = DockStyle.Fill;
            this.grdDescs.AllowUserToOrderColumns = false;

            var textCellTemplateMonoSpaced = new DataGridViewTextBoxCell();
            textCellTemplateMonoSpaced.Style.BackColor = Color.Wheat;
            textCellTemplateMonoSpaced.Style.Font = new Font( FontFamily.GenericMonospace, 8 );

            var column0 = new DataGridViewTextBoxColumn();
            var column1 = new DataGridViewTextBoxColumn();

            column0.HeaderText = "Argument";
            column0.CellTemplate = textCellTemplateMonoSpaced;
            column0.Width = 120;
            column0.SortMode = DataGridViewColumnSortMode.NotSortable;
            column0.ReadOnly = true;
            column1.HeaderText = "Description";
            column1.Width = 120;
            column1.SortMode = DataGridViewColumnSortMode.NotSortable;

            this.grdDescs.Columns.AddRange( new DataGridViewColumn[] {
                column0,
                column1,
            } );

            this.grdDescs.CellEndEdit +=
                (object sender, DataGridViewCellEventArgs evt) => {
                    if ( evt.ColumnIndex == 1
                      && evt.RowIndex >= 0 )
                    {
                        this.OnDescriptionEdited( evt.RowIndex );
                    }
            };

			this.grdDescs.LostFocus += (o, e) => this.grdDescs.EndEdit();

            this.pnlDescsEditor.Controls.Add( this.grdDescs );
            this.pnlDescsEditor.ResumeLayout( false );
        }

        private void Build()
        {
            var mainPanel = new Panel();
            mainPanel.SuspendLayout();
            mainPanel.Dock = DockStyle.Fill;

            this.BuildIcon();
            this.BuildToolbar();
            this.BuildDescriptionsTable();

            mainPanel.Controls.Add( this.pnlDescsEditor );
            mainPanel.Controls.Add( this.tbToolbar );
            mainPanel.ResumeLayout( false );

            this.Controls.Add( mainPanel );
            this.MinimumSize = new Size( 600, 400 );
            this.Text = "Descriptions Editor";
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
				} else {
					this.grdDescs.EndEdit();
				}
            }

            return;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize( e );

            // FnCall: Name
            this.grdDescs.Columns[ 0 ].Width = (int) ( this.grdDescs.ClientSize.Width * 0.20 );

            // FnCall: Function name
            this.grdDescs.Columns[ 1 ].Width = (int) ( this.grdDescs.ClientSize.Width * 0.79 );
        }

        private void Populate()
        {
            Function.ArgumentList argList = this.Function.RegularArgumentList;

            this.grdDescs.Rows.Clear();
            foreach(Function.Argument arg in argList) {
                this.grdDescs.Rows.Add( new string[] {
                    arg.Name, arg.Description } );
            }
        }

        private void OnDescriptionEdited(int argIndex)
        {
            var arg = (Function.Argument) this.Function.RegularArgumentList[ argIndex ];
            var text = (string) this.grdDescs.Rows[ argIndex ].Cells[ 1 ].Value;

            arg.Description = text;
        }

        /// <summary>
        /// Gets the function this editor is tied to.
        /// </summary>
        /// <value>The function, as a Function object.</value>
        public Function Function {
            get; set;
        }

        private ToolStrip tbToolbar;
        private ToolStripButton tbbQuit;
        private ToolStripButton tbbSave;
        private DataGridView grdDescs;
        private GroupBox pnlDescsEditor;
	}
}

