using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;
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

            this.tvMenu.Nodes.Clear();
			this.tvMenu.Nodes.Add( new RootMenuEntryTreeNode( this.doc.Root ) );
			this.PrepareView( true );
		}

		private void OnAddMenuEntry()
		{
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newMenuEntry";

            id += this.tvMenu.GetNodeCount( true ).ToString();
            this.AddTreeNode( new MenuEntryTreeNode( id, (MenuEntry) parentMc ) );
		}

        private void OnAddFunction()
        {
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newFunction";

            id += this.tvMenu.GetNodeCount( true ).ToString();
            this.AddTreeNode( new FunctionTreeNode( id, (MenuEntry) parentMc ) );
        }

        private void OnAddPdf()
        {
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newPdf";

            id += this.tvMenu.GetNodeCount( true ).ToString();
            this.AddTreeNode( new PdfTreeNode( id, (MenuEntry) parentMc ) );
        }

        private void OnAddSeparator()
        {
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newSeparator";

            id += this.tvMenu.GetNodeCount( true ).ToString();
            this.AddTreeNode( new SeparatorTreeNode( id, (MenuEntry) parentMc ) );
        }

        private void OnAddGraphicMenu()
        {
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newGraphicMenu";

            id += this.tvMenu.GetNodeCount( true ).ToString();
            this.AddTreeNode( new GraphicMenuTreeNode( id, (MenuEntry) parentMc ) );
        }

        /// <summary>
        /// Adds a given tree node to the tree.
        /// </summary>
        /// <param name="newNode">New node.</param>
        private void AddTreeNode(MenuComponentTreeNode newNode)
        {
			TreeNode tr = this.GetSelectedTreeNode();

            tr.Nodes.Add( newNode );
            tr.Expand();
        }

		private MenuComponentTreeNode GetSelectedTreeNode()
		{
			TreeNode toret = this.tvMenu.SelectedNode;

            if ( toret == null ) {
                toret = this.tvMenu.Nodes[ 0 ];
				this.tvMenu.SelectedNode = toret;
			}

			return (MenuComponentTreeNode) toret;
		}

        private MenuComponent GetMenuComponentOfTreeNode()
        {
            return this.GetMenuComponentOfTreeNode( this.GetSelectedTreeNode() );
        }

        private MenuComponent GetMenuComponentOfTreeNode(TreeNode tr)
		{
            if ( tr == null ) {
                tr = this.GetSelectedTreeNode();
            }

            return ( (MenuComponentTreeNode) tr ).MenuComponent;
		}

        /// <summary>
        /// Removes the selected node in the tree
        /// </summary>
        private void OnRemoveTreeNode()
        {
            TreeNode tr = this.GetSelectedTreeNode();

            if ( tr != null
              && tr != this.tvMenu.Nodes[ 0 ] )
            {
                // Remove in the document structure
                ( (MenuComponentTreeNode) tr).MenuComponent.Remove();

                // Remove in UI
                this.tvMenu.SelectedNode = ( tr.NextNode ?? tr.PrevNode ) ?? tr.Parent;
                tr.Remove();
            }

            return;
        }

        private void OnUpTreeNode()
        {
            TreeNode tr = this.GetSelectedTreeNode();

            if ( tr != this.tvMenu.Nodes[ 0 ] ) {
				TreeNode trPrev = tr.PrevNode;
				TreeNode parent = tr.Parent;

				if ( parent != null
				  && trPrev != null
	              && tr != this.tvMenu.Nodes[ 0 ] )
	            {
					int trPrevIndex = parent.Nodes.IndexOf( trPrev );

                    // Swap on the document
                    ( (MenuComponentTreeNode) tr ).MenuComponent.SwapPrevious();

                    // Swap on the tree
                    parent.Nodes.RemoveAt( trPrevIndex );
                    parent.Nodes.RemoveAt( trPrevIndex );
                    parent.Nodes.Insert( trPrevIndex, trPrev );
                    parent.Nodes.Insert( trPrevIndex, tr );
					this.tvMenu.SelectedNode = tr;
	            }
			}

            return;
        }

        private void OnDownTreeNode()
        {
            TreeNode tr = this.GetSelectedTreeNode();

			if ( tr != null ) {
				TreeNode trNext = tr.NextNode;
				TreeNode parent = tr.Parent;

	            if ( tr != null
				  && parent != null
				  && trNext != null
	              && tr != this.tvMenu.Nodes[ 0 ] )
	            {
					int trIndex = parent.Nodes.IndexOf( tr );

                    // Swap on the document
                    ( (MenuComponentTreeNode) tr ).MenuComponent.SwapNext();

                    // Swap on the tree
                    parent.Nodes.RemoveAt( trIndex );
                    parent.Nodes.RemoveAt( trIndex );
                    parent.Nodes.Insert( trIndex, tr );
                    parent.Nodes.Insert( trIndex, trNext );
					this.tvMenu.SelectedNode = tr;
	            }
			}

            return;
        }

        private void OnTreeNodeSelected()
        {
            TreeNode tr = this.GetSelectedTreeNode();

            if ( tr != null ) {
                this.SetActionStatus( tr );
            }

            return;
        }

        private void SetActionStatus(TreeNode tr)
        {
            bool isTerminal = !( tr is MenuEntryTreeNode );
			bool isRoot = ( tr == this.tvMenu.Nodes[ 0 ] );
			bool hasNext = ( tr.NextNode != null );
			bool hasPrev = ( tr.PrevNode != null );

            this.addPdfAction.Enabled = !isTerminal;
            this.addSeparatorAction.Enabled = !isTerminal;
            this.addMenuAction.Enabled = !isTerminal;
            this.addFunctionAction.Enabled = !isTerminal;
            this.addGraphicMenuAction.Enabled = !isTerminal;
            this.moveEntryUpAction.Enabled = ( !isRoot && hasPrev );
			this.moveEntryDownAction.Enabled = ( !isRoot && hasNext );
            this.removeEntryAction.Enabled = !isRoot;
        }

		private void OnOpen()
		{
		}

		private void OnSave()
		{
            var dlg = new SaveFileDialog();

            dlg.Title = "Save menu";
            dlg.DefaultExt = AppInfo.FileExtension;
            dlg.CheckPathExists = true;
            dlg.InitialDirectory = this.ApplicationsFolder;

            if ( dlg.ShowDialog() == DialogResult.OK ) {
                this.ApplicationsFolder = Path.GetDirectoryName( dlg.FileName );
                this.Document.SaveToFile( dlg.FileName );
            }

            return;
		}

		private void OnPreview()
		{
			var previewForm = new Form();

			previewForm.FormBorderStyle = FormBorderStyle.FixedSingle;
			previewForm.Icon = this.Icon;
			previewForm.Text = AppInfo.Name + " preview";
			previewForm.MinimumSize = new Size( 320, 240 );
			previewForm.ShowDialog();
		}

		private void BuildIcons()
		{
			try {
				this.appIconBmp = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly().
					GetManifestResourceStream( "RAppMenu.Res.r-editor.png" )
				);
			} catch(Exception) {
				throw new ArgumentException( "Unable to load embedded icons" );
			}

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

			this.playIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.play.png" )
			);
		}

		private void BuildMenu()
		{
            this.opNew = new ToolStripMenuItem( "&" + newAction.Text );
			this.opNew.ShortcutKeys = Keys.Control | Keys.N;
            this.opNew.Click += (sender, e) => this.newAction.CallBack();
            this.opNew.Image = UserAction.ImageList.Images[ this.newAction.ImageIndex ];

            this.opLoad = new ToolStripMenuItem( "&" + loadAction.Text );
			this.opLoad.ShortcutKeys = Keys.Control | Keys.O;
            this.opLoad.Click += (sender, e) => this.loadAction.CallBack();
            this.opLoad.Image = UserAction.ImageList.Images[ this.loadAction.ImageIndex ];

            this.opSave = new ToolStripMenuItem( "&" + saveAction.Text );
			this.opSave.ShortcutKeys = Keys.Control | Keys.S;
            this.opSave.Click += (sender, e) => this.saveAction.CallBack();
            this.opSave.Image = UserAction.ImageList.Images[ this.saveAction.ImageIndex ];;

            this.opQuit = new ToolStripMenuItem( "&" + quitAction.Text );
			this.opQuit.ShortcutKeys = Keys.Control | Keys.Q;
            this.opQuit.Click += (sender, e) => this.quitAction.CallBack();
            this.opQuit.Image = UserAction.ImageList.Images[ this.quitAction.ImageIndex ];

			this.opAddMenu = new ToolStripMenuItem( addMenuAction.Text );
			this.opAddMenu.Click += (sender, e) => this.addMenuAction.CallBack();
			this.opAddMenu.Image = UserAction.ImageList.Images[ this.addMenuAction.ImageIndex ];

			this.opAddGraphicMenu = new ToolStripMenuItem( addGraphicMenuAction.Text );
			this.opAddGraphicMenu.Click += (sender, e) => this.addGraphicMenuAction.CallBack();
			this.opAddGraphicMenu.Image = UserAction.ImageList.Images[ this.addGraphicMenuAction.ImageIndex ];

			this.opAddFunction = new ToolStripMenuItem( addFunctionAction.Text );
			this.opAddFunction.Click += (sender, e) => this.addFunctionAction.CallBack();
			this.opAddFunction.Image = UserAction.ImageList.Images[ this.addFunctionAction.ImageIndex ];

			this.opAddPdf = new ToolStripMenuItem( addPdfAction.Text );
			this.opAddPdf.Click += (sender, e) => this.addPdfAction.CallBack();
			this.opAddPdf.Image = UserAction.ImageList.Images[ this.addPdfAction.ImageIndex ];

			this.opAddSeparator = new ToolStripMenuItem( addSeparatorAction.Text );
			this.opAddSeparator.Click += (sender, e) => this.addSeparatorAction.CallBack();
			this.opAddSeparator.Image = UserAction.ImageList.Images[ this.addSeparatorAction.ImageIndex ];

			this.opMoveEntryUp = new ToolStripMenuItem( moveEntryUpAction.Text );
			this.opMoveEntryUp.Click += (sender, e) => this.moveEntryUpAction.CallBack();
			this.opMoveEntryUp.Image = UserAction.ImageList.Images[ this.moveEntryUpAction.ImageIndex ];

			this.opMoveEntryDown = new ToolStripMenuItem( moveEntryDownAction.Text );
			this.opMoveEntryDown.Click += (sender, e) => this.moveEntryDownAction.CallBack();
			this.opMoveEntryDown.Image = UserAction.ImageList.Images[ this.moveEntryDownAction.ImageIndex ];

			this.opRemove = new ToolStripMenuItem( removeEntryAction.Text );
			this.opRemove.Click += (sender, e) => this.removeEntryAction.CallBack();
			this.opRemove.Image = UserAction.ImageList.Images[ this.removeEntryAction.ImageIndex ];

			this.opPreview = new ToolStripMenuItem( previewAction.Text );
			this.opPreview.Click += (sender, e) => this.previewAction.CallBack();
			this.opPreview.Image = UserAction.ImageList.Images[ this.previewAction.ImageIndex ];

			var opWeb = new ToolStripMenuItem( "&Web" );
			opWeb.Click += (sender, e) => this.OnShowWeb();
			opWeb.Image = this.infoIconBmp;

			this.mFile = new ToolStripMenuItem( "&File" );
			this.mEdit = new ToolStripMenuItem( "&Edit" );
			this.mTools = new ToolStripMenuItem( "&Tools" );
			this.mHelp = new ToolStripMenuItem( "&Help" );

			this.mFile.DropDownItems.AddRange( new ToolStripItem[] {
                this.opNew, this.opLoad,
                this.opSave, this.opQuit
			});

			this.mEdit.DropDownItems.AddRange( new ToolStripItem[] {
				this.opAddMenu, this.opAddFunction,
				this.opAddPdf, this.opAddSeparator,
				this.opAddGraphicMenu, this.opMoveEntryUp,
				this.opMoveEntryDown, this.opRemove
			});

			this.mTools.DropDownItems.AddRange( new ToolStripItem[]{
				opPreview
			});

			this.mHelp.DropDownItems.AddRange( new ToolStripItem[]{
				opWeb
			});

            // User actions
            this.newAction.AddComponent( this.opNew );
            this.quitAction.AddComponent( this.opQuit );
            this.loadAction.AddComponent( this.opLoad );
            this.saveAction.AddComponent( this.opSave );
			this.addMenuAction.AddComponent( this.opAddMenu );
			this.addGraphicMenuAction.AddComponent( this.opAddGraphicMenu );
			this.addFunctionAction.AddComponent( this.opAddFunction );
			this.addPdfAction.AddComponent( this.opAddPdf );
			this.addSeparatorAction.AddComponent( this.opAddSeparator );
			this.moveEntryDownAction.AddComponent( this.opMoveEntryDown );
			this.moveEntryUpAction.AddComponent( this.opMoveEntryUp );
			this.removeEntryAction.AddComponent( this.opRemove );
			this.previewAction.AddComponent( this.opPreview );

            // Insert in form
			this.mMain = new MenuStrip();
            this.mMain.ImageList = UserAction.ImageList;
			this.mMain.Items.AddRange( new ToolStripItem[] {
				this.mFile, this.mEdit, this.mTools, this.mHelp }
			);

			this.MainMenuStrip = this.mMain;
			this.mMain.Dock = DockStyle.Top;
			return;
		}

		private void BuildTreePanel()
		{
			var pnlActions = new FlowLayoutPanel();
			var toolTipActions = new ToolTip();
            var pnlMovement = new FlowLayoutPanel();
			var toolTipMovement = new ToolTip();

            pnlActions.Dock = DockStyle.Bottom;
            pnlActions.Font = new Font( pnlActions.Font, FontStyle.Regular );
            pnlActions.AutoSize = true;
            pnlMovement.Dock = DockStyle.Right;
            pnlMovement.FlowDirection = FlowDirection.TopDown;
            pnlMovement.Font = new Font( pnlActions.Font, FontStyle.Regular );
            pnlMovement.AutoSize = true;

			this.btAddMenuEntry = new Button();
			this.btAddMenuEntry.Size = new Size( 32, 32 );
            this.btAddMenuEntry.ImageList = UserAction.ImageList;
            this.btAddMenuEntry.ImageIndex = this.addMenuAction.ImageIndex;
            this.btAddMenuEntry.Click += (sender, e) => this.addMenuAction.CallBack();
            toolTipActions.SetToolTip( this.btAddMenuEntry, this.addMenuAction.Text );
			pnlActions.Controls.Add( this.btAddMenuEntry );

			this.btAddFunction = new Button();
			this.btAddFunction.Size = new Size( 32, 32 );
            this.btAddFunction.ImageList = UserAction.ImageList;
            this.btAddFunction.ImageIndex = this.addFunctionAction.ImageIndex;
            this.btAddFunction.Click += (sender, e) => this.addFunctionAction.CallBack();
            toolTipActions.SetToolTip( this.btAddFunction, this.addFunctionAction.Text );
			pnlActions.Controls.Add( this.btAddFunction );

			this.btAddPdf = new Button();
			this.btAddPdf.Size = new Size( 32, 32 );
            this.btAddPdf.ImageList = UserAction.ImageList;
            this.btAddPdf.ImageIndex = this.addPdfAction.ImageIndex;
            this.btAddPdf.Click += (sender, e) => this.addPdfAction.CallBack();
            toolTipActions.SetToolTip( this.btAddPdf, this.addPdfAction.Text );
			pnlActions.Controls.Add( this.btAddPdf );

            this.btAddGraphicMenu = new Button();
			this.btAddGraphicMenu.Size = new Size( 32, 32 );
            this.btAddGraphicMenu.ImageList = UserAction.ImageList;
            this.btAddGraphicMenu.ImageIndex = this.addGraphicMenuAction.ImageIndex;
            this.btAddGraphicMenu.Click += (sender, e) => this.addGraphicMenuAction.CallBack();
            toolTipActions.SetToolTip( this.btAddGraphicMenu, this.addGraphicMenuAction.Text );
			pnlActions.Controls.Add( this.btAddGraphicMenu );

            this.btAddSeparator = new Button();
            this.btAddSeparator.Size = new Size( 32, 32 );
            this.btAddSeparator.ImageList = UserAction.ImageList;
            this.btAddSeparator.ImageIndex = this.addSeparatorAction.ImageIndex;
            this.btAddSeparator.Click += (sender, e) => this.addSeparatorAction.CallBack();
            toolTipActions.SetToolTip( this.btAddSeparator, this.addSeparatorAction.Text );
            pnlActions.Controls.Add( this.btAddSeparator );

            this.btUp = new Button();
            this.btUp.Size = new Size( 32, 32 );
            this.btUp.Dock = DockStyle.Left;
            this.btUp.ImageList = UserAction.ImageList;
            this.btUp.ImageIndex = this.moveEntryUpAction.ImageIndex;
            toolTipMovement.SetToolTip( this.btUp, this.moveEntryUpAction.Text );
            pnlMovement.Controls.Add( this.btUp );
            this.btUp.Click += (sender, e) => this.moveEntryUpAction.CallBack();

			this.btDown = new Button();
            this.btDown.Size = new Size( 32, 32 );
            this.btDown.Dock = DockStyle.Left;
            this.btDown.ImageList = UserAction.ImageList;
            this.btDown.ImageIndex = this.moveEntryDownAction.ImageIndex;
            toolTipMovement.SetToolTip( this.btDown, this.moveEntryDownAction.Text );
            pnlMovement.Controls.Add( this.btDown );
            this.btDown.Click += (sender, e) => this.moveEntryDownAction.CallBack();

            this.btRemove = new Button();
            this.btRemove.Size = new Size( 32, 32 );
            this.btRemove.Dock = DockStyle.Left;
            this.btRemove.ImageList = UserAction.ImageList;
            this.btRemove.ImageIndex = this.removeEntryAction.ImageIndex;
            toolTipMovement.SetToolTip( this.btRemove, this.removeEntryAction.Text );
            pnlMovement.Controls.Add( this.btRemove );
            this.btRemove.Click += (sender, e) => this.removeEntryAction.CallBack();

            this.tvMenu = new TreeView();
            this.tvMenu.HideSelection = false;
            this.tvMenu.AfterSelect += (sender, e) => this.OnTreeNodeSelected();
            this.tvMenu.Font = new Font( this.tvMenu.Font, FontStyle.Regular );
            this.tvMenu.Dock = DockStyle.Fill;
            this.tvMenu.ImageList = UserAction.ImageList;

            this.pnlTree = new GroupBox();
            this.pnlTree.Font = new Font( this.pnlTree.Font, FontStyle.Bold );
            this.pnlTree.Text = "Menu structure";
			this.pnlTree.Dock = DockStyle.Fill;
            this.pnlTree.Padding = new Padding( 10 );
			this.pnlTree.Controls.Add( this.tvMenu );
			this.pnlTree.Controls.Add( pnlActions );
            this.pnlTree.Controls.Add( pnlMovement );

            this.splPanels.Panel1.Controls.Add( this.pnlTree );

            // User actions
            this.addMenuAction.AddComponent( this.btAddMenuEntry );
            this.addGraphicMenuAction.AddComponent( this.btAddGraphicMenu );
            this.addFunctionAction.AddComponent( this.btAddFunction );
            this.addPdfAction.AddComponent( this.btAddPdf );
            this.removeEntryAction.AddComponent( this.btRemove );
            this.moveEntryUpAction.AddComponent( this.btUp );
            this.moveEntryDownAction.AddComponent( this.btDown );
            this.addSeparatorAction.AddComponent( this.btAddSeparator );
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
            this.tbBar.ImageList = UserAction.ImageList;

			// Buttons
			this.tbbNew = new ToolStripButton();
            this.tbbNew.ImageIndex = this.newAction.ImageIndex;
            this.tbbNew.ToolTipText = this.newAction.Text;
            this.tbbNew.Click += (sender, e) => this.newAction.CallBack();
			this.tbbOpen = new ToolStripButton();
            this.tbbOpen.ImageIndex = this.loadAction.ImageIndex;
            this.tbbOpen.ToolTipText = this.loadAction.Text;
            this.tbbOpen.Click += (sender, e) => this.loadAction.CallBack();
			this.tbbSave = new ToolStripButton();
            this.tbbSave.ImageIndex = this.saveAction.ImageIndex;
            this.tbbSave.ToolTipText = this.saveAction.Text;
            this.tbbSave.Click += (sender, e) => this.saveAction.CallBack();
			this.tbbQuit = new ToolStripButton();
            this.tbbQuit.ImageIndex = this.quitAction.ImageIndex;
            this.tbbQuit.ToolTipText = this.quitAction.Text;
            this.tbbQuit.Click += (sender, e) => this.quitAction.CallBack();
			this.tbbPreview = new ToolStripButton();
			this.tbbPreview.ImageIndex = this.previewAction.ImageIndex;
			this.tbbPreview.ToolTipText = this.previewAction.Text;
			this.tbbPreview.Click += (sender, e) => this.previewAction.CallBack();

			// Polishing
			this.tbBar.Dock = DockStyle.Top;
			this.tbBar.BackColor = Color.DarkGray;
			this.tbBar.Items.AddRange( new ToolStripButton[] {
				this.tbbNew, this.tbbOpen, this.tbbSave,
				this.tbbPreview, this.tbbQuit
			});

            // User actions
            this.newAction.AddComponent( this.tbbNew );
            this.loadAction.AddComponent( this.tbbOpen );
            this.saveAction.AddComponent( this.tbbSave );
            this.quitAction.AddComponent( this.tbbQuit );
			this.previewAction.AddComponent( this.tbbPreview );
		}

		private void BuildSplitPanels()
		{
			this.splPanels = new SplitContainer();
			this.splPanels.Dock = DockStyle.Fill;

			this.splPanels.Resize += (sender, e) => {
				this.splPanels.SplitterDistance = this.ClientRectangle.Width / 2;
			};
		}

		private void BuildUserActions()
		{
            UserAction.ImageList.Images.Clear();
            UserAction.ImageList.ImageSize = new Size( 16, 16 );
            UserAction.ImageList.Images.AddRange( new Image[] {
                this.newIconBmp, this.openIconBmp, this.saveIconBmp,
                this.quitIconBmp,
                this.menuIconBmp, this.graphicIconBmp, this.functionIconBmp,
                this.pdfIconBmp, this.separatorIconBmp,
                this.deleteIconBmp, this.upIconBmp, this.downIconBmp,
				this.playIconBmp
            });

            this.newAction = new UserAction( "New", 0, this.OnNew );
			this.loadAction = new UserAction( "Open", 1, this.OnOpen );
			this.saveAction = new UserAction( "Save", 2, this.OnSave );
            this.quitAction = new UserAction( "Quit", 3, this.OnQuit );

            this.addMenuAction = new UserAction( "Add menu", 4, this.OnAddMenuEntry );
			this.addGraphicMenuAction = new UserAction( "Add graphic menu", 5, this.OnAddGraphicMenu );
			this.addFunctionAction = new UserAction( "Add function", 6, this.OnAddFunction );
			this.addPdfAction = new UserAction( "Add pdf file path", 7, this.OnAddPdf );
			this.addSeparatorAction = new UserAction( "Add separator", 8, this.OnAddSeparator );

			this.removeEntryAction = new UserAction( "Remove entry", 9, this.OnRemoveTreeNode );
			this.moveEntryUpAction = new UserAction( "Move entry up", 10, this.OnUpTreeNode );
			this.moveEntryDownAction = new UserAction( "Move entry down", 11, this.OnDownTreeNode );
			this.previewAction = new UserAction( "Preview", 12, this.OnPreview );
		}

		private void Build()
		{
            this.BuildIcons();
            this.BuildUserActions();
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
			// Widgets
            this.splPanels.Visible = view;

			// Actions
			this.saveAction.Enabled = view;
			this.addMenuAction.Enabled = view;
			this.addGraphicMenuAction.Enabled = view;
			this.addSeparatorAction.Enabled = view;
			this.addPdfAction.Enabled = view;
			this.addFunctionAction.Enabled = view;
			this.moveEntryDownAction.Enabled = view;
			this.moveEntryUpAction.Enabled = view;
			this.removeEntryAction.Enabled = view;
			this.previewAction.Enabled = view;

			// Polish
			if ( view ) {
				this.SetActionStatus( this.tvMenu.Nodes[ 0 ] );
			}

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

        public string ApplicationsFolder {
            get; set;
        }

		private TreeView tvMenu;
		private SplitContainer splPanels;
		private GroupBox pnlProperties;
        private GroupBox pnlTree;
		private MenuStrip mMain;
		private ToolStripMenuItem mFile;
		private ToolStripMenuItem mEdit;
		private ToolStripMenuItem mTools;
		private ToolStripMenuItem mHelp;
		private ToolStripMenuItem opQuit;
		private ToolStripMenuItem opLoad;
		private ToolStripMenuItem opSave;
		private ToolStripMenuItem opNew;
		private ToolStripMenuItem opAddMenu;
		private ToolStripMenuItem opAddFunction;
		private ToolStripMenuItem opAddGraphicMenu;
		private ToolStripMenuItem opAddSeparator;
		private ToolStripMenuItem opAddPdf;
		private ToolStripMenuItem opRemove;
		private ToolStripMenuItem opMoveEntryUp;
		private ToolStripMenuItem opMoveEntryDown;
		private ToolStripMenuItem opPreview;

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
		private ToolStripButton tbbPreview;

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
		private Bitmap playIconBmp;
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

		private UserAction quitAction;
		private UserAction newAction;
		private UserAction loadAction;
		private UserAction saveAction;

		private UserAction addMenuAction;
		private UserAction addFunctionAction;
		private UserAction addPdfAction;
		private UserAction addSeparatorAction;
		private UserAction removeEntryAction;
		private UserAction moveEntryUpAction;
		private UserAction moveEntryDownAction;
		private UserAction addGraphicMenuAction;

		private UserAction previewAction;

		private Document doc;
	}
}

