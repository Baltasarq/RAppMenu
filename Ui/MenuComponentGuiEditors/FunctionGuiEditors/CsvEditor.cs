using System;
using System.Drawing;
using System.Windows.Forms;

namespace RAppMenu.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
    public class CsvEditor: Form {
        public enum EndResult { Quit, Save }

        public CsvEditor()
        {
            this.Build();
        }

        private void BuildToolbar()
        {
            var quitAction = UserAction.LookUp( "quit" );
            var saveAction = UserAction.LookUp( "save" );
            this.checkAction = UserAction.LookUp( "check" );

            this.tbToolbar = new ToolStrip();
            this.tbToolbar.BackColor = Color.DarkGray;
            this.tbToolbar.Dock = DockStyle.Top;
            this.tbToolbar.ImageList = UserAction.ImageList;

            // Buttons
            this.tbbQuit = new ToolStripButton();
            this.tbbQuit.ImageIndex = quitAction.ImageIndex;
            this.tbbQuit.ToolTipText = quitAction.Text;
            this.tbbQuit.Click += (sender, e) => {
                this.Result = EndResult.Quit; this.Close();
            };

            this.tbbSave = new ToolStripButton();
            this.tbbSave.ImageIndex = saveAction.ImageIndex;
            this.tbbSave.ToolTipText = saveAction.Text;
            this.tbbSave.Click += (sender, e) =>  {
                this.Result = EndResult.Save; this.Close();
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
                this.checkAction.Enabled = ( this.edData.Text.Length > 0 );
            };
        }

        private void Build()
        {
            var mainPanel = new TableLayoutPanel();
            mainPanel.SuspendLayout();

            this.BuildToolbar();
            this.BuildEditor();

            mainPanel.Controls.Add( this.tbToolbar );
            mainPanel.Controls.Add( this.edData );
            mainPanel.ResumeLayout( false );
        }

        /// <summary>
        /// Checks the data entered in the editor.
        /// </summary>
        public void OnCheckData()
        {
        }

        /// <summary>
        /// Gets or sets the result of the form.
        /// </summary>
        /// <value>The result.</value>
        public EndResult Result {
            get; set;
        }

        private ToolStrip tbToolbar;
        private ToolStripButton tbbQuit;
        private ToolStripButton tbbSave;
        private ToolStripButton tbbCheck;
        private TextBox edData;

        private UserAction checkAction;
    }


}

