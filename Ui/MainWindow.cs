using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;

using RAppMenu.Core;
using CoreComponents = RAppMenu.Core.MenuComponents;
using UiComponents = RAppMenu.Ui.MenuComponentTreeNodes;

namespace RAppMenu.Ui {
	public class MainWindow: Form {
		public MainWindow()
		{
			this.doc = null;
            this.ApplicationsFolder = "Applications";
            this.PdfFolder = "Pdf";
			this.GraphsFolder = "Graphs";

			this.Build();
			this.PrepareView( false );
		}

		private void OnQuit()
		{
			this.Close();
		}

		private void OnShowWeb()
		{
			this.SetStatus( "Launching external browser..." );
			Process.Start( AppInfo.Web );
			this.SetStatus();
		}

		private void OnNew()
		{
			this.SetStatus( "Preparing new document..." );
			this.doc = new DesignOfUserMenu();
            this.fileNameSet = false;

            this.PrepareViewStructuresForNewDocument();
			this.PrepareView( true );
		}

		private void OnAddMenu()
		{
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newMenuEntry";

			this.SetStatus( "Creating menu..." );
            id += this.tvMenu.GetNodeCount( true ).ToString();
            var menu = new CoreComponents.Menu( id, (CoreComponents.Menu) parentMc );
            this.AddTreeNode( new UiComponents.MenuTreeNode( menu ) );
		}

        private void OnAddFunction()
        {
			MenuComponentTreeNode tn;
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
			var parentImagesMenu = parentMc as CoreComponents.GraphicMenu;
			string id = this.tvMenu.GetNodeCount( true ).ToString();

			this.SetStatus( "Creating function..." );
			if ( parentImagesMenu != null ) {
				id = "newGraphIdFunction" + id;
                var ime = new CoreComponents.GraphicMenuEntry( id, parentImagesMenu );
				tn = new UiComponents.GraphicMenuEntryTreeNode( ime );
			} else {
				id = "newFunction" + id;
                var f = new CoreComponents.Function( id, (CoreComponents.Menu) parentMc );
				tn = new UiComponents.FunctionTreeNode( f );
			}

            this.AddTreeNode( tn );
        }

        private void OnAddPdf()
        {
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "file" + this.tvMenu.GetNodeCount( true ) + ".pdf";

			this.SetStatus( "Creating pdf..." );
            var pf = new CoreComponents.PdfFile( id, (CoreComponents.Menu) parentMc );
			this.AddTreeNode( new UiComponents.PdfFileTreeNode( pf ) );
        }

        private void OnAddSeparator()
        {
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();

            var sep = new CoreComponents.Separator( (CoreComponents.Menu) parentMc );
            this.AddTreeNode( new UiComponents.SeparatorTreeNode( sep ) );
        }

        private void OnAddGraphicMenu()
        {
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newGraphicMenu";

			this.SetStatus( "Creating graphic menu..." );
            id += this.tvMenu.GetNodeCount( true ).ToString();
            var gm = new CoreComponents.GraphicMenu( id, (CoreComponents.Menu) parentMc );
            this.AddTreeNode( new UiComponents.GraphicMenuTreeNode( gm ) );
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
			this.SetStatus();
        }

		private MenuComponentTreeNode GetSelectedTreeNode()
		{
			TreeNode toret = this.tvMenu.SelectedNode;

            if ( toret == null ) {
                toret = this.TreeMenuRoot;
				this.tvMenu.SelectedNode = toret;
			}

			return (MenuComponentTreeNode) toret;
		}

		/// <summary>
		/// Gets the menu component of tree current node.
		/// </summary>
		/// <returns>
		/// The menu component of tree node, as a <see cref="MenuComponent"/> object.
		/// </returns>
        private MenuComponent GetMenuComponentOfTreeNode()
        {
            return this.GetMenuComponentOfTreeNode( this.GetSelectedTreeNode() );
        }

		/// <summary>
		/// Gets the menu component of tree current node.
		/// </summary>
		/// <param name="tr">
		/// The tree node to get the menu component from.
		/// </param>
		/// <returns>
		/// The menu component of tree node, as a <see cref="MenuComponent"/> object.
		/// </returns>
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
            MenuComponentTreeNode mctr = this.GetSelectedTreeNode();

            if ( mctr != null
              && mctr != this.TreeMenuRoot )
            {
                // Remove in the document structure
                mctr.MenuComponent.Remove();

                // Remove in UI
                this.tvMenu.SelectedNode = ( mctr.NextNode ?? mctr.PrevNode ) ?? mctr.Parent;
                mctr.Remove();
            }

            return;
        }

        private void OnUpTreeNode()
        {
            TreeNode tr = this.GetSelectedTreeNode();

            if ( tr != this.TreeMenuRoot) {
				TreeNode trPrev = tr.PrevNode;
				TreeNode parent = tr.Parent;

				if ( parent != null
				  && trPrev != null
	              && tr != this.TreeMenuRoot )
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
	              && tr != this.TreeMenuRoot )
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
            MenuComponentTreeNode tr = this.GetSelectedTreeNode();

            if ( tr != null ) {
                this.SetActionStatusForTreeNode( tr );
            }

            return;
        }

		private void OnOpen()
		{
            this.SetStatus( "Loading menu..." );

            var dlg = new OpenFileDialog();

            dlg.Title = "Load menu";
            dlg.DefaultExt = AppInfo.FileExtension;
            dlg.CheckPathExists = true;
            dlg.InitialDirectory = this.ApplicationsFolder;
            dlg.Filter = AppInfo.FileExtension + "|*." + AppInfo.FileExtension
                + "|All files|*";

            if ( dlg.ShowDialog() == DialogResult.OK ) {
                this.SetToolbarForNumTasks( 2 );
                this.ApplicationsFolder = Path.GetDirectoryName( dlg.FileName );
                this.SetToolbarTaskFinished();
                this.doc = DesignOfUserMenu.LoadFromFile( dlg.FileName );
                this.SetToolbarTaskFinished();
                this.PrepareViewStructuresForNewDocument();
                this.TreeMenuRoot.Text = this.Document.Root.Name;
                this.fileNameSet = true;
				this.PrepareEditorsForDocument();
            }

            this.SetStatus();
		}

		/// <summary>
		/// Prepares the editors for document.
		/// Makes the view reflect the document structure.
		/// Useful when loading.
		/// </summary>
		private void PrepareEditorsForDocument()
		{
			this.PrepareView( false );
			this.SetToolbarForNumTasks( this.Document.Root.MenuComponents.Count );
			this.CreateEditorsFor( this.TreeMenuRoot, this.Document.Root );

			this.TreeMenuRoot.GetEditor( this.pnlProperties ).Show();
			this.TreeMenuRoot.ExpandAll();
			this.PrepareView( true );
		}

		private void CreateEditorsFor(MenuComponentTreeNode mctn, CoreComponents.Menu menu)
		{
			foreach(MenuComponent submc in menu.MenuComponents) {
				var separator = submc as CoreComponents.Separator;
				var pdfFile = submc as CoreComponents.PdfFile;
				var function = submc as CoreComponents.Function;
                var grphMenuEntry = submc as CoreComponents.GraphicMenuEntry;
                var grphMenu = submc as CoreComponents.GraphicMenu;
                var subMenu = submc as CoreComponents.Menu;

				if ( separator != null ) {
					var mtn = new MenuComponentTreeNodes.SeparatorTreeNode( separator );

					mctn.Nodes.Add( mtn );
					mtn.GetEditor( this.pnlProperties ).ReadDataFromComponent();
				}
				else
				if ( pdfFile != null ) {
					var mtn = new MenuComponentTreeNodes.PdfFileTreeNode( pdfFile );

					mctn.Nodes.Add( mtn );
					mtn.GetEditor( this.pnlProperties ).ReadDataFromComponent();
				}
				else
				if ( function != null ) {
					var mtn = new MenuComponentTreeNodes.FunctionTreeNode( function );

					mctn.Nodes.Add( mtn );
					mtn.GetEditor( this.pnlProperties ).ReadDataFromComponent();
				}
                else
                if ( grphMenu != null ) {
                    var mtn = new MenuComponentTreeNodes.GraphicMenuTreeNode( grphMenu );

                    mctn.Nodes.Add( mtn );
                    mtn.GetEditor( this.pnlProperties ).ReadDataFromComponent();
                    this.CreateEditorsFor( mtn, grphMenu );
                }
                else
                if ( grphMenuEntry != null ) {
                    var mtn = new MenuComponentTreeNodes.GraphicMenuEntryTreeNode( grphMenuEntry );

                    mctn.Nodes.Add( mtn );
                    mtn.GetEditor( this.pnlProperties ).ReadDataFromComponent();
                }
                else
                if ( subMenu != null ) {
                    var mtn = new MenuComponentTreeNodes.MenuTreeNode( subMenu );

                    mctn.Nodes.Add( mtn );
                    mtn.GetEditor( this.pnlProperties ).ReadDataFromComponent();
                    this.CreateEditorsFor( mtn, subMenu );
                }

				// One step more
				if ( menu == this.Document.Root ) {
					this.SetToolbarTaskFinished();
				}
			}

			return;
		}

		private void OnSave()
		{
            this.SetStatus( "Saving menu..." );

            if ( !fileNameSet ) {
                var dlg = new SaveFileDialog();

                dlg.Title = "Save menu";
                dlg.DefaultExt = AppInfo.FileExtension;
                dlg.CheckPathExists = true;
                dlg.InitialDirectory = this.ApplicationsFolder;
                dlg.Filter = AppInfo.FileExtension + "|*." + AppInfo.FileExtension
                    + "|All files|*";
    			dlg.FileName = this.Document.Root.Name;

                if ( dlg.ShowDialog() == DialogResult.OK ) {
                    this.SetToolbarForNumTasks( 2 );
                    this.ApplicationsFolder = Path.GetDirectoryName( dlg.FileName );
                    this.SetToolbarTaskFinished();
                    this.Document.SaveToFile( dlg.FileName );
                    this.SetToolbarTaskFinished();
    				this.TreeMenuRoot.Text = this.Document.Root.Name;
                    this.fileNameSet = true;
                }
            } else {
                string fileName = this.Document.Root.Name + '.' + AppInfo.FileExtension;
                this.Document.SaveToFile( Path.Combine( this.ApplicationsFolder, fileName ) );
            }

			this.SetStatus();
		}

		private void OnPreview()
		{
			this.SetStatus( "Creating preview..." );

            var previewForm = new PreviewWindow( this.Document, this.Icon );

			this.SetStatus( "Showing preview..." );
            previewForm.ShowDialog();
			this.SetStatus();
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

			this.addIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.add.png" )
				);

			this.deleteIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.delete.png" )
			);

			this.openIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RAppMenu.Res.open.png" )
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
				this.opAddGraphicMenu, new ToolStripSeparator(),
				this.opMoveEntryUp, this.opMoveEntryDown, this.opRemove
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
			pnlActions.SuspendLayout();
            var pnlMovement = new FlowLayoutPanel();
			pnlMovement.SuspendLayout();
			var toolTips = new ToolTip();

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
            toolTips.SetToolTip( this.btAddMenuEntry, this.addMenuAction.Text );
			pnlActions.Controls.Add( this.btAddMenuEntry );

			this.btAddFunction = new Button();
			this.btAddFunction.Size = new Size( 32, 32 );
            this.btAddFunction.ImageList = UserAction.ImageList;
            this.btAddFunction.ImageIndex = this.addFunctionAction.ImageIndex;
            this.btAddFunction.Click += (sender, e) => this.addFunctionAction.CallBack();
			toolTips.SetToolTip( this.btAddFunction, this.addFunctionAction.Text );
			pnlActions.Controls.Add( this.btAddFunction );

			this.btAddPdf = new Button();
			this.btAddPdf.Size = new Size( 32, 32 );
            this.btAddPdf.ImageList = UserAction.ImageList;
            this.btAddPdf.ImageIndex = this.addPdfAction.ImageIndex;
            this.btAddPdf.Click += (sender, e) => this.addPdfAction.CallBack();
			toolTips.SetToolTip( this.btAddPdf, this.addPdfAction.Text );
			pnlActions.Controls.Add( this.btAddPdf );

            this.btAddGraphicMenu = new Button();
			this.btAddGraphicMenu.Size = new Size( 32, 32 );
            this.btAddGraphicMenu.ImageList = UserAction.ImageList;
            this.btAddGraphicMenu.ImageIndex = this.addGraphicMenuAction.ImageIndex;
            this.btAddGraphicMenu.Click += (sender, e) => this.addGraphicMenuAction.CallBack();
			toolTips.SetToolTip( this.btAddGraphicMenu, this.addGraphicMenuAction.Text );
			pnlActions.Controls.Add( this.btAddGraphicMenu );

            this.btAddSeparator = new Button();
            this.btAddSeparator.Size = new Size( 32, 32 );
            this.btAddSeparator.ImageList = UserAction.ImageList;
            this.btAddSeparator.ImageIndex = this.addSeparatorAction.ImageIndex;
            this.btAddSeparator.Click += (sender, e) => this.addSeparatorAction.CallBack();
			toolTips.SetToolTip( this.btAddSeparator, this.addSeparatorAction.Text );
            pnlActions.Controls.Add( this.btAddSeparator );

            this.btUp = new Button();
            this.btUp.Size = new Size( 32, 32 );
            this.btUp.Dock = DockStyle.Left;
            this.btUp.ImageList = UserAction.ImageList;
            this.btUp.ImageIndex = this.moveEntryUpAction.ImageIndex;
            toolTips.SetToolTip( this.btUp, this.moveEntryUpAction.Text );
            pnlMovement.Controls.Add( this.btUp );
            this.btUp.Click += (sender, e) => this.moveEntryUpAction.CallBack();

			this.btDown = new Button();
            this.btDown.Size = new Size( 32, 32 );
            this.btDown.Dock = DockStyle.Left;
            this.btDown.ImageList = UserAction.ImageList;
            this.btDown.ImageIndex = this.moveEntryDownAction.ImageIndex;
            toolTips.SetToolTip( this.btDown, this.moveEntryDownAction.Text );
            pnlMovement.Controls.Add( this.btDown );
            this.btDown.Click += (sender, e) => this.moveEntryDownAction.CallBack();

            this.btRemove = new Button();
            this.btRemove.Size = new Size( 32, 32 );
            this.btRemove.Dock = DockStyle.Left;
            this.btRemove.ImageList = UserAction.ImageList;
            this.btRemove.ImageIndex = this.removeEntryAction.ImageIndex;
            toolTips.SetToolTip( this.btRemove, this.removeEntryAction.Text );
            pnlMovement.Controls.Add( this.btRemove );
            this.btRemove.Click += (sender, e) => this.removeEntryAction.CallBack();

            this.tvMenu = new TreeView();
            this.tvMenu.HideSelection = false;
            this.tvMenu.AfterSelect += (sender, e) => this.OnTreeNodeSelected();
            this.tvMenu.Font = new Font( this.tvMenu.Font, FontStyle.Regular );
            this.tvMenu.Dock = DockStyle.Fill;
            this.tvMenu.ImageList = UserAction.ImageList;

            this.pnlTree = new GroupBox();
			this.pnlTree.SuspendLayout();
            this.pnlTree.Font = new Font( this.pnlTree.Font, FontStyle.Bold );
            this.pnlTree.Text = "Menu structure";
			this.pnlTree.Dock = DockStyle.Fill;
            this.pnlTree.Padding = new Padding( 10 );
			this.pnlTree.Controls.Add( this.tvMenu );
			this.pnlTree.Controls.Add( pnlActions );
            this.pnlTree.Controls.Add( pnlMovement );

            this.splPanels.Panel1.Controls.Add( this.pnlTree );
			this.pnlTree.ResumeLayout( false );
			pnlMovement.ResumeLayout( false );
			pnlActions.ResumeLayout( false );

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
			this.pnlGroupProperties = new GroupBox();

            this.pnlProperties = new TableLayoutPanel();
			this.pnlProperties.SuspendLayout();
			this.pnlProperties.AutoSize = false;
			this.pnlProperties.AutoScroll = true;
			this.pnlProperties.Font = new Font( this.pnlProperties.Font, FontStyle.Regular );
			this.pnlProperties.Dock = DockStyle.Fill;
			this.pnlGroupProperties.Controls.Add( this.pnlProperties );

			this.pnlGroupProperties.Text = "Properties";
			this.pnlGroupProperties.Font = new Font( this.pnlProperties.Font, FontStyle.Bold );
			this.pnlGroupProperties.Dock = DockStyle.Fill;
			this.pnlGroupProperties.Padding = new Padding( 5 );
			this.splPanels.Panel2.Controls.Add( this.pnlGroupProperties );
			this.pnlProperties.ResumeLayout( false );
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
				if ( this.WindowState != FormWindowState.Minimized ) {
	                int distance = this.ClientRectangle.Width;

	                if ( distance >= 800 ) {
	                    distance /= 3;
	                } else {
	                    distance /= 2;
	                }

					this.splPanels.SplitterDistance = distance;
				}
			};

			this.splPanels.IsSplitterFixed = true;
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
				this.playIconBmp, this.addIconBmp
            });

            this.newAction = new UserAction( "New", 0, this.OnNew );
			this.loadAction = new UserAction( "Open", 1, this.OnOpen );
			this.saveAction = new UserAction( "Save", 2, this.OnSave );
            this.quitAction = new UserAction( "Quit", 3, this.OnQuit );

            this.addMenuAction = new UserAction( "Add menu", 4, this.OnAddMenu );
			this.addGraphicMenuAction = new UserAction( "Add graphic menu", 5, this.OnAddGraphicMenu );
			this.addFunctionAction = new UserAction( "Add function", 6, this.OnAddFunction );
			this.addPdfAction = new UserAction( "Add pdf file path", 7, this.OnAddPdf );
			this.addSeparatorAction = new UserAction( "Add separator", 8, this.OnAddSeparator );

			this.removeEntryAction = new UserAction( "Remove entry", 9, this.OnRemoveTreeNode );
			this.moveEntryUpAction = new UserAction( "Move entry up", 10, this.OnUpTreeNode );
			this.moveEntryDownAction = new UserAction( "Move entry down", 11, this.OnDownTreeNode );
			this.previewAction = new UserAction( "Preview", 12, this.OnPreview );

			// For the function GUI editor
			new UserAction( "Add function argument", 13, null );
			new UserAction( "Remove function argument", 9, null );
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

			this.SetStatus( "Preparing user interface..." );
			this.Controls.Add( this.splPanels );
			this.Controls.Add( this.tbBar );
			this.Controls.Add( this.mMain );
            this.Controls.Add( this.stStatus );

			this.Text = AppInfo.Name;
			this.Icon = Icon.FromHandle( appIconBmp.GetHicon() );
			this.MinimumSize = new Size( 1000, 740 );
            this.Size = this.MinimumSize;
		}

        private void PrepareViewStructuresForNewDocument()
        {
            // Remove all editors in the panel of properties
            this.pnlProperties.Controls.Clear();

            // Remove all nodes in the tree view and add a new root
            this.tvMenu.Nodes.Clear();
            this.tvMenu.Nodes.Add( new UiComponents.RootMenuTreeNode( this.doc.Root ) );
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
				this.SetActionStatusForTreeNode( this.TreeMenuRoot );
			}

			this.SetStatus();
		}

		private void SetActionStatusForTreeNode(MenuComponentTreeNode mctr)
		{
			bool isTerminal = !( mctr is UiComponents.MenuTreeNode );
			bool isGraphicMenu = mctr is UiComponents.GraphicMenuTreeNode;
			bool isRoot = ( mctr == this.TreeMenuRoot );
			bool hasNext = ( mctr.NextNode != null );
			bool hasPrev = ( mctr.PrevNode != null );

			this.addPdfAction.Enabled = !isTerminal;
			this.addSeparatorAction.Enabled = !isTerminal;
			this.addMenuAction.Enabled = !isTerminal;
			this.addFunctionAction.Enabled = !isTerminal || isGraphicMenu;
			this.addGraphicMenuAction.Enabled = !isTerminal;
			this.moveEntryUpAction.Enabled = ( !isRoot && hasPrev );
			this.moveEntryDownAction.Enabled = ( !isRoot && hasNext );
			this.removeEntryAction.Enabled = !isRoot;

			mctr.GetEditor( this.pnlProperties ).Show();
		}

		private void SetToolbarForNumTasks(int numTasks)
		{
			this.pbProgress.Visible = true;
			this.pbProgress.Minimum = 0;
			this.pbProgress.Step = 1;
			this.pbProgress.Maximum = numTasks;

			Application.DoEvents();
		}

		private void SetToolbarTaskFinished()
		{
			this.pbProgress.PerformStep();

			if ( this.pbProgress.Value == this.pbProgress.Maximum ) {
				this.pbProgress.Visible = false;
			}

			Application.DoEvents();
		}

		private void SetStatus()
		{
			this.lblStatus.ForeColor = Color.Black;
			this.lblStatus.Text = "Ready";

			Application.DoEvents();
		}

		private void SetStatus(string msg)
		{
			this.lblStatus.ForeColor = Color.Blue;
			this.lblStatus.Text = msg;

			Application.DoEvents();
		}

		private void SetErrorStatus(string msg)
		{
			this.lblStatus.ForeColor = Color.DarkRed;
			this.lblStatus.Text = msg;

			Application.DoEvents();
		}

		public MenuComponentTreeNode TreeMenuRoot {
			get {
				return (MenuComponentTreeNode) this.tvMenu.Nodes[ 0 ];
			}
		}

		public DesignOfUserMenu Document {
			get {
				return this.doc;
			}
		}

        /// <summary>
        /// Gets or sets the applications folder.
        /// By default, it is "applications".
        /// </summary>
        /// <value>The applications folder, as a string.</value>
        public string ApplicationsFolder {
			get {
				return AppInfo.ApplicationsFolder;
			}
			set {
				AppInfo.ApplicationsFolder = value;
			}
        }

        /// <summary>
        /// Gets or sets the pdf folder.
        /// By default, it is "Pdf".
        /// </summary>
        /// <value>The pdf folder, as a string.</value>
        public string PdfFolder {
            get {
				return AppInfo.PdfFolder;
			}
			set {
				AppInfo.PdfFolder = value;
			}
        }

		/// <summary>
		/// Gets or sets the graphs folder.
		/// By default, it is "Graphs"
		/// </summary>
		/// <value>The graphs folder, as a string.</value>
		public string GraphsFolder {
			get {
				return AppInfo.GraphsFolder;
			}
			set {
				AppInfo.GraphsFolder = value;
			}
		}

		private TreeView tvMenu;
		private SplitContainer splPanels;
		private GroupBox pnlGroupProperties;
		private TableLayoutPanel pnlProperties;
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

		private ToolStrip tbBar;
		private ToolStripButton tbbNew;
		private ToolStripButton tbbOpen;
		private ToolStripButton tbbSave;
		private ToolStripButton tbbQuit;
		private ToolStripButton tbbPreview;

		private Bitmap appIconBmp;
		private Bitmap addIconBmp;
		private Bitmap deleteIconBmp;
        private Bitmap downIconBmp;
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

		private DesignOfUserMenu doc;
        private bool fileNameSet;
	}
}

