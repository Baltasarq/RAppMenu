using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using RAppMenu.Core;

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
			TreeNode tr = this.tree.SelectedNode;

			if ( tr == null ) {
				tr = this.tree.Nodes[ 0 ];
			}
			var node = new TreeNode( "nuevo", 0, 0 );
			tr.Nodes.Add( node );
			tr.Expand();
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

			pnlActions.Dock = DockStyle.Bottom;

			imageList.ImageSize = new Size( 16, 16 );
			imageList.Images.AddRange( new Image[] {
				this.menuIconBmp, this.functionIconBmp,
				this.pdfIconBmp, this.graphicIconBmp,
				this.deleteIconBmp
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
			pnlActions.Controls.Add( this.btAddFunction );

			this.btAddPdf = new Button();
			this.btAddPdf.Size = new Size( 32, 32 );
			this.btAddPdf.ImageList = imageList;
			this.btAddPdf.ImageIndex = 2;
			pnlActions.Controls.Add( this.btAddPdf );

			this.btAddGraphic = new Button();
			this.btAddGraphic.Size = new Size( 32, 32 );
			this.btAddGraphic.ImageList = imageList;
			this.btAddGraphic.ImageIndex = 3;
			pnlActions.Controls.Add( this.btAddGraphic );

			this.btRemove = new Button();
			this.btRemove.Size = new Size( 32, 32 );
			this.btRemove.Dock = DockStyle.Left;
			this.btRemove.ImageList = imageList;
			this.btRemove.ImageIndex = 4;
			pnlActions.Controls.Add( this.btRemove );

			this.pnlTree = new Panel();
			this.pnlTree.Dock = DockStyle.Fill;
			pnlActions.MaximumSize = new Size( int.MaxValue, this.btRemove.Height * 2 );
			this.pnlTree.Padding = new Padding( 10 );

			this.tree = new TreeView();
			this.tree.Dock = DockStyle.Fill;
			this.tree.ImageList = imageList;
			this.tree.Nodes.Add( "Root" );
			this.tree.Nodes[ 0 ].ImageIndex = 0;
			this.pnlTree.Controls.Add( this.tree );
			this.pnlTree.Controls.Add( pnlActions );

			this.splPanels.Panel1.Controls.Add( this.pnlTree );
		}

		private void BuildPropertiesPanel()
		{
			this.pnlProperties = new GroupBox();

			var pnlInnerProperties = new TableLayoutPanel();
			pnlInnerProperties.Font = new Font( this.pnlProperties.Font, FontStyle.Regular );
			pnlInnerProperties.Dock = DockStyle.Fill;
			this.pnlProperties.Controls.Add( pnlInnerProperties );

			this.pnlProperties.Text = "Properties";
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
			this.pnlTree.Visible = view;
			this.pnlProperties.Visible = view;

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
		private Panel pnlTree;
		private MenuStrip mMain;
		private ToolStripMenuItem mFile;
		private ToolStripMenuItem mHelp;
		private ToolStripMenuItem opQuit;
		private ToolStripMenuItem opNew;
		private Button btAddMenuEntry;
		private Button btAddFunction;
		private Button btAddPdf;
		private Button btAddGraphic;
		private Button btRemove;

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
		private Bitmap settingsIconBmp;
		private Bitmap quitIconBmp;

		private Document doc;
	}
}

