using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Ui.TreeNodes;

namespace RAppMenu.Ui {
	public class MainWindow: Form {
		public MainWindow()
		{
			this.doc = null;

			this.Build();
			this.PrepareView( false );
		}

		private void OnQuit()
		{
			this.Close();
		}

		private void OnShowWeb()
		{
			Process.Start( AppInfo.Web );
		}

		private void OnNew()
		{
			this.doc = new Document();

			this.PrepareView( true );
		}

		private void OnAddMenuEntry()
		{
            string id = "newMenuEntry";

            id += this.tree.GetNodeCount( true ).ToString();
            this.AddTreeNode( new MenuEntryTreeNode( id ) );
		}

        private void OnAddFunction()
        {
            string id = "newFunction";

            id += this.tree.GetNodeCount( true ).ToString();
            this.AddTreeNode( new FunctionTreeNode( id ) );
        }

        private void OnAddPdf()
        {
            string id = "newPdf";

            id += this.tree.GetNodeCount( true ).ToString();
            this.AddTreeNode( new PdfTreeNode( id ) );
        }

        private void OnAddSeparator()
        {
            string id = "newSeparator";

            id += this.tree.GetNodeCount( true ).ToString();
            this.AddTreeNode( new SeparatorTreeNode( id ) );
        }

        private void OnAddGraphicMenu()
        {
            string id = "newGraphicMenu";

            id += this.tree.GetNodeCount( true ).ToString();
            this.AddTreeNode( new GraphicMenuTreeNode( id ) );
        }

        /// <summary>
        /// Adds a given tree node to the tree.
        /// </summary>
        /// <param name="newNode">New node.</param>
        private void AddTreeNode(TreeNode newNode)
        {
            TreeNode tr = this.tree.SelectedNode;

            if ( tr == null ) {
                tr = this.tree.Nodes[ 0 ];
            }

            tr.Nodes.Add( newNode );
            tr.Expand();
        }

        /// <summary>
        /// Removes the selected node in the tree
        /// </summary>
        private void OnRemoveTreeNode()
        {
            TreeNode tr = this.tree.SelectedNode;

            if ( tr != null
              && tr != this.tree.Nodes[ 0 ] )
            {
                tr.Remove();
            }

            return;
        }

        private void OnUpTreeNode()
        {
            TreeNode tr = this.tree.SelectedNode;

            if ( tr != null
              && tr != this.tree.Nodes[ 0 ] )
            {

            }

            return;
        }

        private void OnDownTreeNode()
        {
            TreeNode tr = this.tree.SelectedNode;

            if ( tr != null
              && tr != this.tree.Nodes[ 0 ] )
            {

            }

            return;
        }

        private void OnTreeNodeSelected()
        {
            TreeNode tr = this.tree.SelectedNode;

            if ( tr != null ) {
                this.SetActionStatus( tr );
            }

            return;
        }

        private void SetActionStatus(TreeNode tr)
        {
            bool isTerminal = !( tr is MenuEntryTreeNode );

            Console.WriteLine( "Item clicked. Is terminal: {0}", isTerminal );

            // Actions
            this.btAddPdf.Enabled = !isTerminal;
            this.btAddSeparator.Enabled = !isTerminal;
            this.btAddMenuEntry.Enabled = !isTerminal;
            this.btAddFunction.Enabled = !isTerminal;
            this.btAddGraphicMenu.Enabled = !isTerminal;

            // Movements
            bool isRoot = ( tr == this.tree.Nodes[ 0 ] );

            this.btUp.Enabled = !isRoot;
            this.btDown.Enabled = !isRoot;
            this.btRemove.Enabled = !isRoot;
        }

		private void OnOpen()
		{
		}

		private void OnSave()
		{
		}

		private void OnSettings()
		{
		}

		private void BuildIcons()
		{
			this.appIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.r-editor.png" )
			);

			this.helpIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.help.png" )
			);

			this.addIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.add.png" )
			);

			this.checkIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.check.png" )
			);

			this.deleteIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.delete.png" )
			);

			this.editIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.edit.png" )
			);

			this.openIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.open.png" )
			);

			this.folderIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.folder.png" )
			);

			this.infoIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.info.png" )
			);

			this.newIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.new.png" )
			);

			this.saveIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.save.png" )
			);

			this.settingsIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.settings.png" )
			);

			this.quitIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.quit.png" )
			);

			this.menuIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.menu.png" )
			);

			this.functionIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.function.png" )
			);

			this.pdfIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.pdf.png" )
			);

			this.graphicIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.graphic.png" )
			);

            this.separatorIconBmp = new Bitmap(
                System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceStream( "RAppMenu.Res.separator.png" )
            );

            this.upIconBmp = new Bitmap(
                System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceStream( "RAppMenu.Res.up.png" )
            );

            this.downIconBmp = new Bitmap(
                System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceStream( "RAppMenu.Res.down.png" )
            );
		}

		private void BuildMenu()
		{
			this.opNew = new ToolStripMenuItem( "&New" );
			this.opNew.ShortcutKeys = Keys.Control | Keys.N;
			this.opNew.Click += (sender, e) => this.OnNew();
			this.opNew.Image = this.newIconBmp;

			this.opQuit = new ToolStripMenuItem( "&Quit" );
			this.opQuit.ShortcutKeys = Keys.Control | Keys.Q;
			this.opQuit.Click += (sender, e) => this.OnQuit();
			this.opQuit.Image = this.quitIconBmp;

			var opWeb = new ToolStripMenuItem( "&Web" );
			opWeb.Click += (sender, e) => this.OnShowWeb();
			opWeb.Image = this.infoIconBmp;

			this.mFile = new ToolStripMenuItem( "&File" );
			this.mHelp = new ToolStripMenuItem( "&Help" );

			this.mFile.DropDownItems.AddRange( new ToolStripItem[] {
				this.opNew, this.opQuit
			});

			this.mHelp.DropDownItems.AddRange( new ToolStripItem[]{
				opWeb
			});

			this.mMain = new MenuStrip();
			this.mMain.Items.AddRange( new ToolStripItem[] {
				this.mFile, this.mHelp }
			);

			this.MainMenuStrip = this.mMain;
			this.mMain.Dock = DockStyle.Top;
			return;
		}

		private void BuildTreePanel()
		{
			var imageList = new ImageList();
			var pnlActions = new FlowLayoutPanel();
            var pnlMovement = new FlowLayoutPanel();

            pnlActions.Dock = DockStyle.Bottom;
            pnlActions.Font = new Font( pnlActions.Font, FontStyle.Regular );
            pnlActions.AutoSize = true;
            pnlMovement.Dock = DockStyle.Right;
            pnlMovement.FlowDirection = FlowDirection.TopDown;
            pnlMovement.Font = new Font( pnlActions.Font, FontStyle.Regular );
            pnlMovement.AutoSize = true;

			imageList.ImageSize = new Size( 16, 16 );
			imageList.Images.AddRange( new Image[] {
				this.menuIconBmp, this.functionIconBmp,
				this.pdfIconBmp, this.graphicIconBmp,
                this.separatorIconBmp, this.downIconBmp,
                this.upIconBmp, this.deleteIconBmp
			} );

			this.btAddMenuEntry = new Button();
			this.btAddMenuEntry.Size = new Size( 32, 32 );
			this.btAddMenuEntry.ImageList = imageList;
			this.btAddMenuEntry.ImageIndex = 0;
			this.btAddMenuEntry.Click += (sender, e) => this.OnAddMenuEntry();
			pnlActions.Controls.Add( this.btAddMenuEntry );

			this.btAddFunction = new Button();
			this.btAddFunction.Size = new Size( 32, 32 );
			this.btAddFunction.ImageList = imageList;
			this.btAddFunction.ImageIndex = 1;
            this.btAddFunction.Click += (sender, e) => this.OnAddFunction();
			pnlActions.Controls.Add( this.btAddFunction );

			this.btAddPdf = new Button();
			this.btAddPdf.Size = new Size( 32, 32 );
			this.btAddPdf.ImageList = imageList;
			this.btAddPdf.ImageIndex = 2;
            this.btAddPdf.Click += (sender, e) => this.OnAddPdf();
			pnlActions.Controls.Add( this.btAddPdf );

            this.btAddGraphicMenu = new Button();
			this.btAddGraphicMenu.Size = new Size( 32, 32 );
			this.btAddGraphicMenu.ImageList = imageList;
			this.btAddGraphicMenu.ImageIndex = 3;
            this.btAddGraphicMenu.Click += (sender, e) => this.OnAddGraphicMenu();
			pnlActions.Controls.Add( this.btAddGraphicMenu );

            this.btAddSeparator = new Button();
            this.btAddSeparator.Size = new Size( 32, 32 );
            this.btAddSeparator.ImageList = imageList;
            this.btAddSeparator.ImageIndex = 4;
            this.btAddSeparator.Click += (sender, e) => this.OnAddSeparator();
            pnlActions.Controls.Add( this.btAddSeparator );

            this.btDown = new Button();
            this.btDown.Size = new Size( 32, 32 );
            this.btDown.Dock = DockStyle.Left;
            this.btDown.ImageList = imageList;
            this.btDown.ImageIndex = 6;
            pnlMovement.Controls.Add( this.btDown );
            this.btDown.Click += (sender, e) => this.OnDownTreeNode();

            this.btUp = new Button();
            this.btUp.Size = new Size( 32, 32 );
            this.btUp.Dock = DockStyle.Left;
            this.btUp.ImageList = imageList;
            this.btUp.ImageIndex = 5;
            pnlMovement.Controls.Add( this.btUp );
            this.btUp.Click += (sender, e) => this.OnUpTreeNode();

            this.btRemove = new Button();
            this.btRemove.Size = new Size( 32, 32 );
            this.btRemove.Dock = DockStyle.Left;
            this.btRemove.ImageList = imageList;
            this.btRemove.ImageIndex = 7;
            pnlMovement.Controls.Add( this.btRemove );
            this.btRemove.Click += (sender, e) => this.OnRemoveTreeNode();

            this.tree = new TreeView();
            this.tree.AfterSelect += (sender, e) => this.OnTreeNodeSelected();
            this.tree.Font = new Font( this.tree.Font, FontStyle.Regular );
            this.tree.Dock = DockStyle.Fill;
            this.tree.ImageList = imageList;
            this.tree.Nodes.Add( new MenuEntryTreeNode( "Root" ) );

            this.pnlTree = new GroupBox();
            this.pnlTree.Font = new Font( this.pnlTree.Font, FontStyle.Bold );
            this.pnlTree.Text = "Menu structure";
			this.pnlTree.Dock = DockStyle.Fill;
            this.pnlTree.Padding = new Padding( 10 );
			this.pnlTree.Controls.Add( this.tree );
			this.pnlTree.Controls.Add( pnlActions );
            this.pnlTree.Controls.Add( pnlMovement );

            this.splPanels.Panel1.Controls.Add( this.pnlTree );
		}

		private void BuildPropertiesPanel()
		{
			this.pnlProperties = new GroupBox();

			var pnlInnerProperties = new TableLayoutPanel();
			pnlInnerProperties.Font = new Font( this.pnlProperties.Font, FontStyle.Regular );
			pnlInnerProperties.Dock = DockStyle.Fill;
			this.pnlProperties.Controls.Add( pnlInnerProperties );

			this.pnlProperties.Text = "Item properties";
			this.pnlProperties.Font = new Font( this.pnlProperties.Font, FontStyle.Bold );
			this.pnlProperties.Dock = DockStyle.Fill;
			this.pnlProperties.Padding = new Padding( 5 );

			var pnlName = new Panel();
			pnlName.Dock = DockStyle.Top;
			this.lblName = new Label();
			this.lblName.Dock = DockStyle.Left;
			this.lblName.Text = "Name:";
			this.edName = new TextBox();
			this.edName.Dock = DockStyle.Fill;
			pnlName.Controls.Add( this.edName );
			pnlName.Controls.Add( this.lblName );
			pnlName.MaximumSize = new Size( int.MaxValue, this.edName.Height );
			pnlInnerProperties.Controls.Add( pnlName );

			this.splPanels.Panel2.Controls.Add( this.pnlProperties );
		}

		private void BuildStatus()
		{
			this.stStatus = new StatusStrip();

			this.lblStatus = new ToolStripStatusLabel();
			this.lblStatus.Text = "Ready";

			this.pbProgress = new ToolStripProgressBar();
			this.pbProgress.Visible = false;

			this.stStatus.Items.Add( this.lblStatus );
			this.stStatus.Items.Add( new ToolStripSeparator() );
			this.stStatus.Items.Add( this.pbProgress );
		}

		private void BuildToolBar()
		{
			this.tbBar = new ToolStrip();
			var imgList = new ImageList();
			this.tbBar.ImageList = imgList;

			// Populate image list
			imgList.ImageSize = new Size( 16, 16 );
			imgList.Images.AddRange( new Image[]{
				this.newIconBmp, this.openIconBmp,
				this.saveIconBmp, this.quitIconBmp,
			});

			// Buttons
			this.tbbNew = new ToolStripButton();
			this.tbbNew.ImageIndex = 0;
			this.tbbNew.ToolTipText = "New";
			this.tbbNew.Click += (sender, e) => this.OnNew();
			this.tbbOpen = new ToolStripButton();
			this.tbbOpen.ImageIndex = 1;
			this.tbbOpen.ToolTipText = "Open";
			this.tbbOpen.Click += (sender, e) => this.OnOpen();
			this.tbbSave = new ToolStripButton();
			this.tbbSave.ImageIndex = 2;
			this.tbbSave.ToolTipText = "Save";
			this.tbbOpen.Click += (sender, e) => this.OnSave();
			this.tbbQuit = new ToolStripButton();
			this.tbbQuit.ImageIndex = 3;
			this.tbbQuit.ToolTipText = "Quit";
			this.tbbQuit.Click += (sender, e) => this.OnQuit();

			// Polishing
			this.tbBar.Dock = DockStyle.Top;
			this.tbBar.BackColor = Color.DarkGray;
			this.tbBar.Items.AddRange( new ToolStripButton[] {
				this.tbbNew, this.tbbOpen, this.tbbSave,
				this.tbbQuit
			});
		}

		private void BuildSplitPanels()
		{
			this.splPanels = new SplitContainer();
			this.splPanels.Dock = DockStyle.Fill;

			this.splPanels.Resize += (sender, e) => {
				this.splPanels.SplitterDistance = this.ClientRectangle.Width / 2;
			};
		}

		private void Build()
		{
			this.BuildIcons();
			this.BuildMenu();
			this.BuildSplitPanels();
			this.BuildTreePanel();
			this.BuildPropertiesPanel();
			this.BuildStatus();
			this.BuildToolBar();

			this.Controls.Add( this.splPanels );
			this.Controls.Add( this.tbBar );
			this.Controls.Add( this.mMain );
            this.Controls.Add( this.stStatus );

			this.Text = AppInfo.Name;
			this.Icon = Icon.FromHandle( appIconBmp.GetHicon() );
			this.MinimumSize = new Size( 620, 460 );
		}

		private void PrepareView(bool view)
		{
            this.splPanels.Visible = view;

			this.SetStatus();
		}

		private void SetStatus()
		{
			this.SetStatus( "Ready" );
		}

		private void SetStatus(string msg)
		{
			this.lblStatus.Text = msg;
		}

		public Document Document {
			get {
				return this.doc;
			}
		}

		private TreeView tree;
		private SplitContainer splPanels;
		private GroupBox pnlProperties;
        private GroupBox pnlTree;
		private MenuStrip mMain;
		private ToolStripMenuItem mFile;
		private ToolStripMenuItem mHelp;
		private ToolStripMenuItem opQuit;
		private ToolStripMenuItem opNew;
		private Button btAddMenuEntry;
		private Button btAddFunction;
		private Button btAddPdf;
        private Button btAddSeparator;
		private Button btAddGraphicMenu;
		private Button btRemove;
        private Button btUp;
        private Button btDown;

		private StatusStrip stStatus;
		private ToolStripStatusLabel lblStatus;
		private ToolStripProgressBar pbProgress;

		private Label lblName;
		private TextBox edName;

		private ToolStrip tbBar;
		private ToolStripButton tbbNew;
		private ToolStripButton tbbOpen;
		private ToolStripButton tbbSave;
		private ToolStripButton tbbQuit;

		private Bitmap appIconBmp;
		private Bitmap helpIconBmp;
		private Bitmap addIconBmp;
		private Bitmap checkIconBmp;
		private Bitmap deleteIconBmp;
        private Bitmap downIconBmp;
		private Bitmap editIconBmp;
		private Bitmap folderIconBmp;
		private Bitmap functionIconBmp;
		private Bitmap pdfIconBmp;
		private Bitmap graphicIconBmp;
		private Bitmap menuIconBmp;
		private Bitmap openIconBmp;
		private Bitmap infoIconBmp;
		private Bitmap newIconBmp;
		private Bitmap saveIconBmp;
        private Bitmap separatorIconBmp;
		private Bitmap settingsIconBmp;
        private Bitmap upIconBmp;
		private Bitmap quitIconBmp;

		private Document doc;
	}
}

