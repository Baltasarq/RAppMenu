using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using RWABuilder.Core;
using CoreComponents = RWABuilder.Core.MenuComponents;
using UiComponents = RWABuilder.Ui.MenuComponentTreeNodes;

namespace RWABuilder.Ui {
	public class MainWindow: Form {
		public MainWindow()
		{
			this.numMenus = 0;
			this.numFunctions = 0;
			this.numPDFs = 0;
			this.numGraphicMenus = 0;
			this.numGraphicMenuEntries = 0;
			this.doc = null;
			this.copier = null;

			this.Build();
			this.PrepareView( false );

            Trace.WriteLine( DateTime.Now + ": Ready" );
		}

		private void OnQuit()
		{
			this.OnCloseDocument();
			this.Close();
		}

		private void OnShowWeb()
		{
            Trace.WriteLine( System.DateTime.Now + ": Launching browser..." );

			this.SetStatus( "Launching external browser..." );
			Process.Start( AppInfo.Web );
			this.SetStatus();
		}

		private void OnShowHelp()
		{
			Trace.WriteLine( System.DateTime.Now + ": Launching browser..." );

			this.SetStatus( "Launching external browser..." );
			Process.Start( AppInfo.Help );
			this.SetStatus();
		}

        private void OnShowLog()
        {
            Trace.WriteLine( System.DateTime.Now + ": Showing log..." );
            Trace.Flush();

            string logPath = Path.Combine(
                AppInfo.PrepareAppConfigFolder(),
                AppInfo.LogFile );

            this.SetStatus( "Launching external browser..." );
            Process.Start( logPath );
            this.SetStatus();
        }

        private void OnAbout()
        {
            var formAbout = new Form();

            formAbout.Icon = this.Icon;
			formAbout.Text = AppInfo.Name;
            formAbout.Padding = new Padding( 10 );
            formAbout.FormBorderStyle = FormBorderStyle.FixedSingle;

            var pnlText = new Panel();
			pnlText.Padding = new Padding( 10 );
			pnlText.AutoSize = true;
            pnlText.Dock = DockStyle.Fill;

            var lblInfo = new Label();
            lblInfo.AutoSize = true;
            lblInfo.Padding = new Padding( 5 );
            lblInfo.Font = new Font( FontFamily.GenericSansSerif, 14 );
            lblInfo.Font = new Font( lblInfo.Font, FontStyle.Bold );
            lblInfo.Dock = DockStyle.Top;
            lblInfo.Text = AppInfo.Name + " " + AppInfo.Version;

            var lblDesc = new Label();
            lblDesc.Padding = new Padding( 5 );
            lblDesc.Font = new Font( FontFamily.GenericSansSerif, 12 );
            lblDesc.Dock = DockStyle.Fill;
            lblDesc.Text = "This is a companion tool for RWizard, "
                + "to help make applications easily.";

            pnlText.Controls.Add( lblDesc );
            pnlText.Controls.Add( lblInfo );

            var picIcon = new PictureBox();
			picIcon.Padding = new Padding( 10 );
            picIcon.Dock = DockStyle.Left;
            picIcon.Image = this.appIconBmp;
            picIcon.MinimumSize = picIcon.MaximumSize = new Size(
                this.appIconBmp.Width + 10,
                this.appIconBmp.Height + 10 );

            var pnlAbout = new Panel();
			pnlAbout.AutoSize = true;
            pnlAbout.Dock = DockStyle.Fill;
            pnlAbout.Controls.Add( pnlText );
            pnlAbout.Controls.Add( picIcon );

            formAbout.Controls.Add( pnlAbout );
            formAbout.StartPosition = FormStartPosition.CenterParent;
			formAbout.MinimumSize = new Size( 500, 100 );
            formAbout.ShowDialog();
        }

        /// <summary>
        /// Prepares the following menu to be closed.
        /// It saves it if needed;
        /// </summary>
        private void OnCloseDocument()
        {
            if ( this.Document != null
              && this.Document.NeedsSave )
            {
				this.EnsureEditingFinished();

                Trace.WriteLine( System.DateTime.Now + ": Closing document: "
                                                + this.Document.Root.Name );

                DialogResult result =
                    MessageBox.Show( this,
					    "Do you want to save?", 
                        "Closing menu", 
                        MessageBoxButtons.YesNo,
					    MessageBoxIcon.Question,
					    MessageBoxDefaultButton.Button1 );

                if ( result == DialogResult.Yes ) {
                    this.OnSave();
                }
            }

            this.doc = null;
			this.PrepareView( false );
        }

		private void PrepareMenuDesign(string fileName)
		{
			if ( string.IsNullOrWhiteSpace( fileName ) ) {
				this.doc = new MenuDesign();
			} else {
				this.doc = MenuDesign.LoadFromFile( fileName );
			}

            this.copier = new MenuComponentClipboard();
			this.fileName = fileName;
		}

		private void OnNew()
		{
            Trace.WriteLine( System.DateTime.Now + ": Creating document" );

            this.OnCloseDocument();

			this.SetStatus( "Preparing new document..." );
			this.PrepareMenuDesign( "" );

            this.PrepareViewStructuresForNewDocument();
			this.PrepareView( true );
		}

		private void OnAddMenu()
		{
			this.EnsureEditingFinished();

            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newMenuEntry";

			this.SetStatus( "Creating menu..." );
            id += ( ++this.numMenus ).ToString();
            var menu = new CoreComponents.RegularMenu( id, (CoreComponents.Menu) parentMc );
            this.AddTreeNode( new UiComponents.MenuTreeNode( menu ) );
		}

        private void OnAddFunction()
        {
			this.EnsureEditingFinished();

			MenuComponentTreeNode tn;
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
			var parentImagesMenu = parentMc as CoreComponents.GraphicMenu;
			string id;

			this.SetStatus( "Creating function..." );
			if ( parentImagesMenu != null ) {
				id = "newGraphIdFunction" + ( ++this.numGraphicMenuEntries );
                var ime = new CoreComponents.GraphicEntry( id, parentImagesMenu );
				tn = new UiComponents.GraphicEntryTreeNode( ime );
			} else {
				id = "newFunction" + ( ++this.numFunctions );
                var f = new CoreComponents.Function( id, (CoreComponents.Menu) parentMc );
				tn = new UiComponents.FunctionTreeNode( f );
			}

            this.AddTreeNode( tn );
        }

        private void OnAddPdf()
		{
			this.EnsureEditingFinished();
			
            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "file" + ( ++this.numPDFs ) + ".pdf";

			this.SetStatus( "Creating pdf..." );
            var pf = new CoreComponents.PdfFile( id, (CoreComponents.Menu) parentMc );
			this.AddTreeNode( new UiComponents.PdfFileTreeNode( pf ) );
        }

        private void OnAddSeparator()
        {
			this.EnsureEditingFinished();

            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();

            var sep = new CoreComponents.Separator( (CoreComponents.Menu) parentMc );
            this.AddTreeNode( new UiComponents.SeparatorTreeNode( sep ) );
        }

        private void OnAddGraphicMenu()
        {
			this.EnsureEditingFinished();

            MenuComponent parentMc = this.GetMenuComponentOfTreeNode();
            string id = "newGraphicMenu";

			this.SetStatus( "Creating graphic menu..." );
            id += ( ++this.numGraphicMenus ).ToString();
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
			this.EnsureEditingFinished();

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
			this.EnsureEditingFinished();

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
			this.EnsureEditingFinished();

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
			this.EnsureEditingFinished();

            MenuComponentTreeNode tr = this.GetSelectedTreeNode();

            if ( tr != null ) {
                this.SetActionStatusForTreeNode( tr );
            }

            return;
        }

		private void OnOpen()
		{
            Trace.WriteLine( System.DateTime.Now + ": Opening document" );
            Trace.Indent();

            this.OnCloseDocument();

            this.SetStatus( "Select menu..." );

            var dlg = new OpenFileDialog();

            dlg.Title = "Load menu";
            dlg.DefaultExt = AppInfo.FileExtension;
            dlg.CheckPathExists = true;
            dlg.InitialDirectory = this.ApplicationsFolder;
            dlg.Filter = AppInfo.FileExtension + "|*." + AppInfo.FileExtension
                + "|All files|*";

            if ( dlg.ShowDialog() == DialogResult.OK ) {
                this.PrepareView( false );
                this.SetStatus( "Loading menu..." );
                this.SetToolbarForNumTasks( 2 );
                this.ApplicationsFolder = Path.GetDirectoryName( dlg.FileName );
                this.SetToolbarTaskFinished();

                try {
					this.PrepareMenuDesign( dlg.FileName );
                }
                catch(XmlException exc)
                {
                    this.SetErrorStatus( "Malformed XML: " + exc.Message );
					this.fileName = "";
                    Trace.WriteLine( exc.Message );
                    Trace.WriteLine( exc.StackTrace );
                    return;
                }
                catch(Exception exc)
                {
                    this.SetErrorStatus( "Unexpected error: " + exc.Message );
					this.fileName = "";
                    Trace.WriteLine( exc.Message );
                    Trace.WriteLine( exc.StackTrace );
                    return;
                }
                finally {
                    Trace.Unindent();
                    this.SetToolbarTaskFinished();
                }
                    
                this.PrepareViewStructuresForNewDocument();
                this.TreeMenuRoot.Text = this.Document.Root.Name;
				this.PrepareTreeNodesForDocument();
                this.PrepareView( true );
            }

            this.SetStatus();
		}

		/// <summary>
		/// Prepares the editors for document.
		/// Makes the view reflect the document structure.
		/// Useful when loading.
		/// </summary>
		private void PrepareTreeNodesForDocument()
		{
            this.SetStatus( "Preparing editors..." );
			this.SetToolbarForNumTasks( this.Document.Root.MenuComponents.Count );
			this.CreateTreeNodesFor( this.TreeMenuRoot, this.Document.Root );

            this.TreeMenuRoot.ExpandAll();
		}

		private void CreateTreeNodesFor(MenuComponentTreeNode mctn, CoreComponents.Menu menu)
		{
			foreach(MenuComponent submc in menu.MenuComponents) {
				var separator = submc as CoreComponents.Separator;
				var pdfFile = submc as CoreComponents.PdfFile;
				var function = submc as CoreComponents.Function;
                var grphMenu = submc as CoreComponents.GraphicMenu;
                var subMenu = submc as CoreComponents.RegularMenu;

				if ( separator != null ) {
					mctn.Nodes.Add( MenuComponentTreeNode.Create( separator ) );
				}
				else
				if ( pdfFile != null ) {
					mctn.Nodes.Add( MenuComponentTreeNode.Create( pdfFile ) );
				}
				else
				if ( function != null ) {
					mctn.Nodes.Add( MenuComponentTreeNode.Create( function ) );
				}
                else
                if ( grphMenu != null ) {
                    var mtn = MenuComponentTreeNode.Create( grphMenu );

                    // Prepare tree menu and editor for graphic menu
                    mctn.Nodes.Add( mtn );

                    // Prepare tree menu and editor for each graphic menu entry
                    foreach(CoreComponents.GraphicEntry grme in grphMenu.MenuComponents) {
						mtn.Nodes.Add( MenuComponentTreeNode.Create( grme ) );
                    }
                }
                else
                if ( subMenu != null ) {
                    var mtn = MenuComponentTreeNode.Create( subMenu );

                    mctn.Nodes.Add( mtn );
                    this.CreateTreeNodesFor( mtn, subMenu );
                }

				// One step more
				if ( menu == this.Document.Root ) {
					this.SetToolbarTaskFinished();
				}
			}

			return;
		}

		private void EnsureEditingFinished() {
			this.GetSelectedTreeNode().GetEditor( this.pnlProperties ).FinishEditing();
		}

		private void OnSaveAs()
		{
			this.EnsureEditingFinished();

			Trace.WriteLine( DateTime.Now + ": Saving " + this.Document.Root.Name );
			Trace.Indent();
			this.SetStatus( "Saving menu as..." );

			this.fileName = "";
			this.OnSave();

			Trace.Unindent();
		}

		private void OnSave()
		{
			this.EnsureEditingFinished();

            Trace.WriteLine( DateTime.Now + ": Saving " + this.Document.Root.Name );
            Trace.Indent();
            this.SetStatus( "Saving menu..." );

            if ( string.IsNullOrWhiteSpace( this.fileName ) ) {
                var dlg = new SaveFileDialog();

                dlg.Title = "Save menu";
                dlg.DefaultExt = AppInfo.FileExtension;
                dlg.CheckPathExists = true;
                dlg.InitialDirectory = this.ApplicationsFolder;
                dlg.Filter = AppInfo.FileExtension + "|*." + AppInfo.FileExtension
                    + "|All files|*";
    			dlg.FileName = this.Document.Root.Name;

                if ( dlg.ShowDialog() == DialogResult.OK ) {
                    this.fileName = dlg.FileName;
                    Trace.WriteLine( DateTime.Now + ": File set: " + this.fileName );
                } else {
                    Trace.WriteLine( DateTime.Now + ": Saving cancelled" );
                    goto End;
                }
            }

            this.SetToolbarForNumTasks( 2 );
            this.ApplicationsFolder = Path.GetDirectoryName( this.fileName );
            this.SetToolbarTaskFinished();

            try {
                this.Document.SaveToFile( this.fileName );
				this.SetStatus();
            } catch(XmlException exc)
            {
                this.SetErrorStatus( "Malformed XML: " + exc.Message );
                Trace.WriteLine( exc.Message );
                Trace.WriteLine( exc.StackTrace );
            }
            catch(Exception exc)
            {
                this.SetErrorStatus( "Unexpected error: " + exc.Message );
                Trace.WriteLine( exc.Message );
                Trace.WriteLine( exc.StackTrace );
            }
            finally {
                this.SetToolbarTaskFinished();
                Trace.Unindent();
            }

			this.GetSelectedTreeNode().GetEditor( this.pnlProperties ).Show();
			this.TreeMenuRoot.Text = this.Document.Root.Name;

            End:
			this.BuildAppTitle();
		}

		private void OnExport()
		{
			Packager packr;

			Trace.WriteLine( DateTime.Now + ": Exporting " + this.Document.Root.Name );
			Trace.Indent();
			this.SetStatus( "Exporting app..." );

			try {
				packr = new Packager( this.Document );
				string fileName = Path.GetFileNameWithoutExtension( this.fileName ) + '.' + AppInfo.AppsExtension;

				var dlg = new SaveFileDialog();

				dlg.Title = "Save menu";
				dlg.DefaultExt = AppInfo.AppsExtension;
				dlg.CheckPathExists = true;
				dlg.InitialDirectory = this.ApplicationsFolder;
				dlg.Filter = AppInfo.AppsExtension + "|*." + AppInfo.AppsExtension
					+ "|All files|*";
				dlg.FileName = fileName;

				if ( dlg.ShowDialog() == DialogResult.OK ) {
					Trace.WriteLine( DateTime.Now + ": Packaged app file set: " + dlg.FileName );
					packr.Package( dlg.FileName );
				} else {
					Trace.WriteLine( DateTime.Now + ": Exporting cancelled" );
				}

				SetStatus();
			} catch(IOException exc) {
				this.SetErrorStatus( "Error creating zip file: " + exc.Message );
			}

			Trace.WriteLine( DateTime.Now + ": Finished exporting " + this.Document.Root.Name );
			Trace.Unindent();
			return;
		}

		private void OnProperties()
		{
            string oldEmail = this.Document.AuthorEmail;
            DateTime oldDate = this.Document.Date;
			string oldSourceCodePath = this.Document.SourceCodePath;
			string oldDocsPath = this.Document.DocsPath;

			this.SetStatus( "Editing properties..." );

			var propertiesForm = new PropertiesWindow( this.Document, this.Icon );
			
            if ( propertiesForm.ShowDialog() == DialogResult.Cancel ) {
                this.Document.AuthorEmail = oldEmail;
                this.Document.Date = oldDate;
				this.Document.DocsPath = oldDocsPath;
				this.Document.SourceCodePath = oldSourceCodePath;
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

		/// <summary>
		/// Copies the current menu component
		/// </summary>
		private void OnCopy()
		{
            this.copier.MenuComponent = this.GetMenuComponentOfTreeNode( this.GetSelectedTreeNode() );
		}

		private void OnPaste()
		{
			// Something to copy?
            if ( this.copier.MenuComponent != null ) {
				TreeNode treeNode = this.GetSelectedTreeNode();

				// Which tree node?
				if ( treeNode != null ) {
					MenuComponent mc = this.GetMenuComponentOfTreeNode( treeNode );

					// Only copy to (or paste in) a menu
					if ( mc is Core.MenuComponents.Menu ) {
						var menu = mc as Core.MenuComponents.Menu;
                        MenuComponent toUse = this.copier.MenuComponent;

						// Add the new menu component to the model
						menu.Add( toUse );

						// Create the new tree node
						this.AddTreeNode( MenuComponentTreeNode.Create( toUse ) );
					}
				}
			}

			return;
		}

		private void BuildIcons()
		{
			System.Reflection.Assembly entryAssembly;

			try {
				entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

				this.appIconBmp = new Bitmap(
					entryAssembly.GetManifestResourceStream( "RWABuilder.Res.appIcon.png" )
				);
			} catch(Exception) {
				throw new ArgumentException( "Unable to load embedded icons" );
			}

			this.addIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.add.png" )
			);

            this.checkIconBmp = new Bitmap(
                entryAssembly.GetManifestResourceStream( "RWABuilder.Res.check.png" )
            );

			this.editIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.edit.png" )
			);

			this.notepadIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.notepad.png" )
			);

			this.editFnCallsIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.editFnCalls.png" )
			);

			this.deleteIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.delete.png" )
			);

			this.openIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.open.png" )
			);

			this.helpIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.help.png" )
			);

			this.infoIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.info.png" )
			);

			this.newIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.new.png" )
			);

			this.saveIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.save.png" )
			);

			this.saveAsIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.save-as.png" )
			);

			this.quitIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.quit.png" )
			);

			this.menuIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.menu.png" )
			);

			this.functionIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.function.png" )
			);

			this.pdfIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.pdf.png" )
			);

			this.graphicIconBmp = new Bitmap(
				System.Reflection.Assembly.GetEntryAssembly().
				GetManifestResourceStream( "RWABuilder.Res.graphic.png" )
			);

            this.separatorIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.separator.png" )
            );

            this.upIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.up.png" )
            );

            this.downIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.down.png" )
            );

			this.playIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.play.png" )
			);

			this.exportIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.export.png" )
			);

			this.copyIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.copy.png" )
			);

			this.pasteIconBmp = new Bitmap(
				entryAssembly.GetManifestResourceStream( "RWABuilder.Res.paste.png" )
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
            this.opSave.Image = UserAction.ImageList.Images[ this.saveAction.ImageIndex ];

			this.opSaveAs = new ToolStripMenuItem( "&" + this.saveAsAction.Text );
			this.opSaveAs.Click += (sender, e) => this.saveAsAction.CallBack();
			this.opSaveAs.Image = UserAction.ImageList.Images[ this.saveAsAction.ImageIndex ];

			this.opExport = new ToolStripMenuItem( "&" + this.exportAction.Text );
			this.opExport.Click += (sender, e) => this.exportAction.CallBack();
			this.opExport.Image = UserAction.ImageList.Images[ this.exportAction.ImageIndex ];

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

			this.opCopy = new ToolStripMenuItem( this.copyEntryAction.Text );
			this.opCopy.Click += (sender, e) => this.copyEntryAction.CallBack();
			this.opCopy.Image = UserAction.ImageList.Images[ this.copyEntryAction.ImageIndex ];

			this.opPaste = new ToolStripMenuItem( this.pasteEntryAction.Text );
			this.opPaste.Click += (sender, e) => this.pasteEntryAction.CallBack();
			this.opPaste.Image = UserAction.ImageList.Images[ this.pasteEntryAction.ImageIndex ];

			this.opRemove = new ToolStripMenuItem( removeEntryAction.Text );
			this.opRemove.Click += (sender, e) => this.removeEntryAction.CallBack();
			this.opRemove.Image = UserAction.ImageList.Images[ this.removeEntryAction.ImageIndex ];

			this.opRemove = new ToolStripMenuItem( removeEntryAction.Text );
			this.opRemove.Click += (sender, e) => this.removeEntryAction.CallBack();
			this.opRemove.Image = UserAction.ImageList.Images[ this.removeEntryAction.ImageIndex ];

			this.opProperties = new ToolStripMenuItem( propertiesAction.Text );
			this.opProperties.Click += (sender, e) => this.propertiesAction.CallBack();
			this.opProperties.Image = UserAction.ImageList.Images[ this.propertiesAction.ImageIndex ];

			this.opPreview = new ToolStripMenuItem( previewAction.Text );
			this.opPreview.Click += (sender, e) => this.previewAction.CallBack();
			this.opPreview.Image = UserAction.ImageList.Images[ this.previewAction.ImageIndex ];

			var opWeb = new ToolStripMenuItem( "&Web" );
			opWeb.Click += (sender, e) => this.OnShowWeb();
			opWeb.Image = this.infoIconBmp;

			var opHelp = new ToolStripMenuItem( "&Help..." );
			opHelp.Click += (sender, e) => this.OnShowHelp();
			opHelp.Image = this.helpIconBmp;

            var opLog = new ToolStripMenuItem( "&Show log..." );
            opLog.Click += (sender, e) => this.OnShowLog();

            var opAbout = new ToolStripMenuItem( "&About..." );
            opAbout.Click += (sender, e) => this.OnAbout();

			this.mFile = new ToolStripMenuItem( "&File" );
			this.mEdit = new ToolStripMenuItem( "&Edit" );
			this.mTools = new ToolStripMenuItem( "&Tools" );
			this.mHelp = new ToolStripMenuItem( "&Help" );

			this.mFile.DropDownItems.AddRange( new ToolStripItem[] {
                this.opNew, this.opLoad,
                this.opSave, this.opSaveAs,
				this.opExport, this.opQuit
			});

			this.mEdit.DropDownItems.AddRange( new ToolStripItem[] {
				this.opAddMenu, this.opAddFunction,
				this.opAddPdf, this.opAddSeparator,
				this.opAddGraphicMenu, new ToolStripSeparator(),
				this.opCopy, this.opPaste,
				this.opMoveEntryUp, this.opMoveEntryDown, this.opRemove
			});

			this.mTools.DropDownItems.AddRange( new ToolStripItem[]{
				opProperties, opPreview
			});

			this.mHelp.DropDownItems.AddRange( new ToolStripItem[]{
                opHelp, opWeb, opLog, opAbout
			});

            // User actions
            this.newAction.AddComponent( this.opNew );
            this.quitAction.AddComponent( this.opQuit );
            this.loadAction.AddComponent( this.opLoad );
            this.saveAction.AddComponent( this.opSave );
			this.saveAsAction.AddComponent( this.opSaveAs );
			this.addMenuAction.AddComponent( this.opAddMenu );
			this.addGraphicMenuAction.AddComponent( this.opAddGraphicMenu );
			this.addFunctionAction.AddComponent( this.opAddFunction );
			this.addPdfAction.AddComponent( this.opAddPdf );
			this.addSeparatorAction.AddComponent( this.opAddSeparator );
			this.moveEntryDownAction.AddComponent( this.opMoveEntryDown );
			this.moveEntryUpAction.AddComponent( this.opMoveEntryUp );
			this.removeEntryAction.AddComponent( this.opRemove );
			this.previewAction.AddComponent( this.opPreview );
			this.propertiesAction.AddComponent( this.opProperties );
			this.copyEntryAction.AddComponent( this.opCopy );
			this.pasteEntryAction.AddComponent( this.opPaste );

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
			this.tvMenu.NodeMouseClick += (object sender, TreeNodeMouseClickEventArgs e) => {
				if ( e.Button == MouseButtons.Right ) {
					this.tvMenu.SelectedNode = e.Node;
				}
			};
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
			this.tbbSaveAs = new ToolStripButton();
			this.tbbSaveAs.ImageIndex = this.saveAsAction.ImageIndex;
			this.tbbSaveAs.ToolTipText = this.saveAsAction.Text;
			this.tbbSaveAs.Click += (sender, e) => this.saveAsAction.CallBack();
			this.tbbQuit = new ToolStripButton();
            this.tbbQuit.ImageIndex = this.quitAction.ImageIndex;
            this.tbbQuit.ToolTipText = this.quitAction.Text;
            this.tbbQuit.Click += (sender, e) => this.quitAction.CallBack();
			this.tbbPreview = new ToolStripButton();
			this.tbbPreview.ImageIndex = this.previewAction.ImageIndex;
			this.tbbPreview.ToolTipText = this.previewAction.Text;
			this.tbbPreview.Click += (sender, e) => this.previewAction.CallBack();
			this.tbbProperties = new ToolStripButton();
			this.tbbProperties.ImageIndex = this.propertiesAction.ImageIndex;
			this.tbbProperties.ToolTipText = this.propertiesAction.Text;
			this.tbbProperties.Click += (sender, e) => this.propertiesAction.CallBack();

			// Polishing
			this.tbBar.Dock = DockStyle.Top;
			this.tbBar.BackColor = Color.DarkGray;
			this.tbBar.Items.AddRange( new ToolStripButton[] {
				this.tbbNew, this.tbbOpen, this.tbbSave,
				this.tbbSaveAs, this.tbbPreview, this.tbbProperties, this.tbbQuit
			});

            // User actions
            this.newAction.AddComponent( this.tbbNew );
            this.loadAction.AddComponent( this.tbbOpen );
            this.saveAction.AddComponent( this.tbbSave );
			this.saveAsAction.AddComponent( this.tbbSaveAs );
            this.quitAction.AddComponent( this.tbbQuit );
			this.previewAction.AddComponent( this.tbbPreview );
			this.propertiesAction.AddComponent( this.tbbProperties );
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
				this.playIconBmp, this.addIconBmp, this.editFnCallsIconBmp,
                this.saveAsIconBmp, this.checkIconBmp, this.editIconBmp,
				this.notepadIconBmp, this.exportIconBmp,
				this.copyIconBmp, this.pasteIconBmp,
            });

            this.newAction = new UserAction( "New", 0, this.OnNew );
			this.loadAction = new UserAction( "Open", 1, this.OnOpen );
			this.saveAction = new UserAction( "Save", 2, this.OnSave );
			this.saveAsAction = new UserAction( "Save as", 15, this.OnSaveAs );
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
			this.propertiesAction = new UserAction( "Properties", 17, this.OnProperties );
			this.exportAction = new UserAction( "Export", 19, this.OnExport );
			this.copyEntryAction = new UserAction( "Copy", 20, this.OnCopy );
			this.pasteEntryAction = new UserAction( "Paste", 21, this.OnPaste );

			// For the function GUI editor
			new UserAction( "Add function argument", 13, null );
			new UserAction( "Edit function call arguments", 14, null );
			new UserAction( "Edit descriptions", 18, null );
			new UserAction( "Remove function argument", 9, null );
			new UserAction( "Add function call argument", 13, null );
			new UserAction( "Remove function call argument", 9, null );
			new UserAction( "Add argument to function call", 13, null );
			new UserAction( "Remove argument from function call", 9, null );

            // For the CSV editor
            new UserAction( "Verify", 16, null );

			// For the color picker editor
			new UserAction( "Add color", 13, null );
			new UserAction( "Remove color", 9, null );
		}

		private void BuildContextMenu()
		{
			var menu = new ContextMenuStrip();
			menu.ImageList = UserAction.ImageList;
			this.tvMenu.ContextMenuStrip = menu;

			var cmMoveUp = new ToolStripMenuItem( this.moveEntryUpAction.Text ) {
				ImageIndex = this.moveEntryUpAction.ImageIndex,
			};
			cmMoveUp.Click += (sender, e) => this.moveEntryUpAction.CallBack();
			this.moveEntryUpAction.AddComponent( cmMoveUp );

			var cmMoveDown = new ToolStripMenuItem( this.moveEntryDownAction.Text ) {
				ImageIndex = this.moveEntryDownAction.ImageIndex,
			};
			cmMoveDown.Click += (sender, e) => this.moveEntryDownAction.CallBack();
			this.moveEntryDownAction.AddComponent( cmMoveDown );

			var cmCopy = new ToolStripMenuItem( this.copyEntryAction.Text ) {
				ImageIndex = this.copyEntryAction.ImageIndex,
			};
			cmCopy.Click += (sender, e) => this.copyEntryAction.CallBack();
			this.copyEntryAction.AddComponent( cmCopy );

			var cmPaste = new ToolStripMenuItem( this.pasteEntryAction.Text ) {
				ImageIndex = this.pasteEntryAction.ImageIndex,
			};
			cmPaste.Click += (sender, e) => this.pasteEntryAction.CallBack();
			this.pasteEntryAction.AddComponent( cmPaste );

			var cmRemove = new ToolStripMenuItem( this.removeEntryAction.Text ) {
				ImageIndex = this.removeEntryAction.ImageIndex,
			};
			cmRemove.Click += (sender, e) => this.removeEntryAction.CallBack();
			this.removeEntryAction.AddComponent( cmRemove );

			menu.Items.AddRange( new ToolStripMenuItem[] {
				cmMoveUp, cmMoveDown,
				cmCopy, cmPaste, cmRemove,
			} );

			return;
		}

		private void BuildAppTitle()
		{
			StringBuilder title = new StringBuilder();

			title.Append( AppInfo.Name );
			title.Append( ' ' );
			title.Append( AppInfo.Version );

			if ( !string.IsNullOrWhiteSpace( this.fileName ) ) {
				title.Insert( 0, Path.GetFileName( this.fileName ) + @" - " );
			}

			this.Text = title.ToString();
			return;
		}

		private void Build()
		{
            Trace.WriteLine( "Building Gui..." );

            this.BuildIcons();
            this.BuildUserActions();
			this.BuildMenu();
			this.BuildSplitPanels();
			this.BuildTreePanel();
			this.BuildPropertiesPanel();
			this.BuildStatus();
			this.BuildToolBar();
			this.BuildContextMenu();

			this.SetStatus( "Preparing user interface..." );
			this.Controls.Add( this.splPanels );
			this.Controls.Add( this.tbBar );
			this.Controls.Add( this.mMain );
            this.Controls.Add( this.stStatus );

			this.BuildAppTitle();
			this.FormClosing += (sender, e) => this.OnCloseDocument();
			this.Icon = Icon.FromHandle( appIconBmp.GetHicon() );
			this.MinimumSize = new Size( 1000, 740 );
            this.Size = this.MinimumSize;

            Trace.WriteLine( "Gui Built." );
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
			// Actions
			this.saveAction.Enabled = view;
			this.saveAsAction.Enabled = view;
			this.exportAction.Enabled = view;
			this.addMenuAction.Enabled = view;
			this.addGraphicMenuAction.Enabled = view;
			this.addSeparatorAction.Enabled = view;
			this.addPdfAction.Enabled = view;
			this.addFunctionAction.Enabled = view;
			this.moveEntryDownAction.Enabled = view;
			this.moveEntryUpAction.Enabled = view;
			this.copyEntryAction.Enabled = view;
			this.pasteEntryAction.Enabled = view;
			this.removeEntryAction.Enabled = view;
			this.previewAction.Enabled = view;
			this.propertiesAction.Enabled = view;

			// Polish
			if ( view ) {
				this.SetActionStatusForTreeNode( this.TreeMenuRoot );
                this.tvMenu.SelectedNode = this.TreeMenuRoot;
				this.BuildAppTitle();
			}

            // Widgets
            this.splPanels.Visible = view;
			this.SetStatus();
		}

		private void SetActionStatusForTreeNode(MenuComponentTreeNode mctr)
		{
			MenuComponent mc = this.GetMenuComponentOfTreeNode( mctr );
			bool isRegularMenu = mc is Core.MenuComponents.RegularMenu;
			bool isMenu = ( ( mc is Core.MenuComponents.GraphicMenu ) || isRegularMenu );
			bool isRoot = ( mctr == this.TreeMenuRoot );
			bool hasNext = ( mctr.NextNode != null );
			bool hasPrev = ( mctr.PrevNode != null );

			this.addPdfAction.Enabled = isRegularMenu;
			this.addSeparatorAction.Enabled = isRegularMenu;
			this.addMenuAction.Enabled = isRegularMenu;
			this.addFunctionAction.Enabled = isMenu;
			this.addGraphicMenuAction.Enabled = isRegularMenu;
			this.moveEntryUpAction.Enabled = ( !isRoot && hasPrev );
			this.moveEntryDownAction.Enabled = ( !isRoot && hasNext );
			this.copyEntryAction.Enabled = !isRoot;
            this.pasteEntryAction.Enabled = ( isMenu && ( this.copier.HasContentsFor( mc ) ) );
			this.removeEntryAction.Enabled = !isRoot;

            this.splPanels.Panel2.Hide();
			mctr.GetEditor( this.pnlProperties ).Show();
            this.splPanels.Panel2.Show();
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

		public MenuDesign Document {
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
		private ToolStripMenuItem opSaveAs;
		private ToolStripMenuItem opExport;
		private ToolStripMenuItem opNew;
		private ToolStripMenuItem opAddMenu;
		private ToolStripMenuItem opAddFunction;
		private ToolStripMenuItem opAddGraphicMenu;
		private ToolStripMenuItem opAddSeparator;
		private ToolStripMenuItem opAddPdf;
		private ToolStripMenuItem opCopy;
		private ToolStripMenuItem opPaste;
		private ToolStripMenuItem opRemove;
		private ToolStripMenuItem opMoveEntryUp;
		private ToolStripMenuItem opMoveEntryDown;
		private ToolStripMenuItem opProperties;
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
		private ToolStripButton tbbSaveAs;
		private ToolStripButton tbbQuit;
		private ToolStripButton tbbProperties;
		private ToolStripButton tbbPreview;

		private Bitmap appIconBmp;
		private Bitmap addIconBmp;
        private Bitmap checkIconBmp;
		private Bitmap editIconBmp;
		private Bitmap exportIconBmp;
		private Bitmap notepadIconBmp;
		private Bitmap editFnCallsIconBmp;
		private Bitmap deleteIconBmp;
        private Bitmap downIconBmp;
		private Bitmap functionIconBmp;
		private Bitmap pdfIconBmp;
		private Bitmap playIconBmp;
		private Bitmap graphicIconBmp;
		private Bitmap menuIconBmp;
		private Bitmap openIconBmp;
		private Bitmap helpIconBmp;
		private Bitmap infoIconBmp;
		private Bitmap newIconBmp;
		private Bitmap saveIconBmp;
		private Bitmap saveAsIconBmp;
        private Bitmap separatorIconBmp;
        private Bitmap upIconBmp;
		private Bitmap quitIconBmp;
		private Bitmap copyIconBmp;
		private Bitmap pasteIconBmp;

		private UserAction quitAction;
		private UserAction newAction;
		private UserAction loadAction;
		private UserAction saveAction;
		private UserAction saveAsAction;
		private UserAction exportAction;

		private UserAction addMenuAction;
		private UserAction addFunctionAction;
		private UserAction addPdfAction;
		private UserAction addSeparatorAction;
		private UserAction removeEntryAction;
		private UserAction moveEntryUpAction;
		private UserAction moveEntryDownAction;
		private UserAction addGraphicMenuAction;
		private UserAction previewAction;
		private UserAction propertiesAction;
		private UserAction copyEntryAction;
		private UserAction pasteEntryAction;

		private MenuDesign doc;
        private MenuComponentClipboard copier;
        private string fileName;
		private int numMenus;
		private int numFunctions;
		private int numPDFs;
		private int numGraphicMenus;
		private int numGraphicMenuEntries;
	}
}
