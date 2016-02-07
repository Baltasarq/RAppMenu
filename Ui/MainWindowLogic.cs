using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using RWABuilder.Core;
using CoreComponents = RWABuilder.Core.MenuComponents;
using UiComponents = RWABuilder.Ui.MenuComponentTreeNodes;

namespace RWABuilder.Ui {
	public partial class MainWindow {
		public MainWindow()
		{
			this.numMenus = 0;
			this.numFunctions = 0;
			this.numPDFs = 0;
			this.numGraphicMenus = 0;
			this.numGraphicMenuEntries = 0;
			this.copier = null;
			this.fileName = "";

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
				LocalStorageManager.PrepareAppConfigFolder(),
				LocalStorageManager.LogFile );

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
			if ( this.Package != null ) {
				this.EnsureEditingFinished();

				Trace.WriteLine( System.DateTime.Now + ": Closing document: "
					+ this.Package.Menu.Root.Name );

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

			this.Package = null;
			this.PrepareView( false );
		}

		private void PreparePackage(string fileName)
		{
			if ( string.IsNullOrWhiteSpace( fileName ) ) {
				this.Package = new Package();
			} else {
				this.Package = Package.Unpack( fileName );
			}

			this.copier = new MenuComponentClipboard();
			this.fileName = fileName;
		}

		private void PreparePackageFromMenuDesign(string fileName)
		{
			if ( string.IsNullOrWhiteSpace( fileName ) ) {
				this.Package = new Package();
			} else {
				this.Package = new Package( MenuDesign.LoadFromFile( fileName ) );
			}

			this.copier = new MenuComponentClipboard();
			this.fileName = fileName;
		}

		private void OnNew()
		{
			Trace.WriteLine( System.DateTime.Now + ": Creating document" );

			this.OnCloseDocument();

			this.SetStatus( "Preparing new document..." );
			this.PreparePackage( "" );

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
			dlg.DefaultExt = AppInfo.AppsExtension;
			dlg.CheckPathExists = true;
			dlg.InitialDirectory = this.ApplicationsFolder;
			dlg.Filter = AppInfo.AppsExtension + "|*." + AppInfo.AppsExtension
				+ "|All files|*";

			if ( dlg.ShowDialog() == DialogResult.OK ) {
				this.PrepareView( false );
				this.SetStatus( "Loading menu..." );
				this.SetToolbarForNumTasks( 2 );
				this.ApplicationsFolder = Path.GetDirectoryName( dlg.FileName );
				this.SetToolbarTaskFinished();

				try {
					this.PreparePackage( dlg.FileName );
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
				this.TreeMenuRoot.Text = this.Package.Menu.Root.Name;
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
			this.SetToolbarForNumTasks( this.Package.Menu.Root.MenuComponents.Count );
			this.CreateTreeNodesFor( this.TreeMenuRoot, this.Package.Menu.Root );

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
					var subNode = MenuComponentTreeNode.Create( pdfFile );
					subNode.Text = ( (CoreComponents.PdfFile) subNode.MenuComponent ).GetFileName();
					mctn.Nodes.Add( subNode );
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
				if ( menu == this.Package.Menu.Root ) {
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

			Trace.WriteLine( DateTime.Now + ": Saving " + this.Package.Menu.Root.Name );
			Trace.Indent();
			this.SetStatus( "Saving menu as..." );

			this.fileName = "";
			this.OnSave();

			Trace.Unindent();
		}

		private void OnSave()
		{
			this.EnsureEditingFinished();

			Trace.WriteLine( DateTime.Now + ": Saving " + this.Package.Menu.Root.Name );
			Trace.Indent();
			this.SetStatus( "Saving menu..." );

			if ( string.IsNullOrWhiteSpace( this.fileName ) ) {
				var dlg = new SaveFileDialog();

				dlg.Title = "Save menu";
				dlg.DefaultExt = AppInfo.AppsExtension;
				dlg.CheckPathExists = true;
				dlg.InitialDirectory = this.ApplicationsFolder;
				dlg.Filter = AppInfo.AppsExtension + "|*." + AppInfo.AppsExtension
					+ "|All files|*";
				dlg.FileName = this.Package.Menu.Root.Name;

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
				this.Package.Pack( this.fileName );
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
			this.TreeMenuRoot.Text = this.Package.Menu.Root.Name;

			End:
			this.BuildAppTitle();
		}

		private void OnExport()
		{
			string fileName = Path.GetFileNameWithoutExtension( this.Package.Menu.Root.Name ) + '.' + AppInfo.FileExtension;

			Trace.WriteLine( DateTime.Now + ": Exporting " + fileName );
			Trace.Indent();
			this.SetStatus( "Exporting menu design as xml..." );

			try {
				var dlg = new SaveFileDialog();

				dlg.Title = "Save menu";
				dlg.DefaultExt = AppInfo.FileExtension;
				dlg.CheckPathExists = true;
				dlg.InitialDirectory = this.ApplicationsFolder;
				dlg.Filter = AppInfo.FileExtension + "|*." + AppInfo.FileExtension
					+ "|All files|*";
				dlg.FileName = fileName;

				if ( dlg.ShowDialog() == DialogResult.OK ) {
					Trace.WriteLine( DateTime.Now + ": Packaged app file set: " + dlg.FileName );
					this.Package.Menu.SaveToFile( dlg.FileName );
				} else {
					Trace.WriteLine( DateTime.Now + ": Exporting cancelled" );
				}

				this.SetStatus();
			} catch(IOException exc) {
				this.SetErrorStatus( "Error creating XML file: " + exc.Message );
			}

			Trace.WriteLine( DateTime.Now + ": Finished exporting " + this.Package.Menu.Root.Name );
			Trace.Unindent();
			return;
		}

		private void OnImport() {
			this.OnCloseDocument();

			Trace.WriteLine( DateTime.Now + ": Importing " + fileName );
			Trace.Indent();
			this.SetStatus( "Importing menu design from xml..." );

			try {
				var dlg = new OpenFileDialog();

				dlg.Title = "Load menu";
				dlg.DefaultExt = AppInfo.FileExtension;
				dlg.CheckPathExists = true;
				dlg.InitialDirectory = this.ApplicationsFolder;
				dlg.Filter = AppInfo.FileExtension + "|*." + AppInfo.FileExtension
					+ "|All files|*";
				dlg.FileName = fileName;

				if ( dlg.ShowDialog() == DialogResult.OK ) {
					Trace.WriteLine( DateTime.Now + ": XML file set: " + dlg.FileName );
					this.PrepareView( false );
					this.SetStatus( "Loading menu..." );
					this.SetToolbarForNumTasks( 2 );
					this.SetToolbarTaskFinished();

					try {
						this.PreparePackageFromMenuDesign( dlg.FileName );
					}
					catch(XmlException exc)
					{
						this.SetErrorStatus( "Malformed XML: " + exc.Message );
						Trace.WriteLine( exc.Message );
						Trace.WriteLine( exc.StackTrace );
						return;
					}
					catch(Exception exc)
					{
						this.SetErrorStatus( "Unexpected error: " + exc.Message );
						Trace.WriteLine( exc.Message );
						Trace.WriteLine( exc.StackTrace );
						return;
					}
					finally {
						Trace.Unindent();
						this.fileName = "";
						this.SetToolbarTaskFinished();
					}

					this.PrepareViewStructuresForNewDocument();
					this.TreeMenuRoot.Text = this.Package.Menu.Root.Name;
					this.PrepareTreeNodesForDocument();
					this.PrepareView( true );
					Trace.WriteLine( DateTime.Now + ": Finished importing " + this.Package.Menu.Root.Name );
					Trace.Unindent();
				} else {
					Trace.WriteLine( DateTime.Now + ": Importing cancelled" );
				}

				this.SetStatus();
			} catch(IOException exc) {
				this.SetErrorStatus( "Error creating package file: " + exc.Message );
			}
				
			return;
		}

		private void OnProperties()
		{
			string oldEmail = this.Package.Menu.AuthorEmail;
			DateTime oldDate = this.Package.Menu.Date;
			string oldSourceCodePath = this.Package.Menu.SourceCodePath;
			string oldDocsPath = this.Package.Menu.WindowsBinariesPath;

			this.SetStatus( "Editing properties..." );

			var propertiesForm = new PropertiesWindow( this.Package.Menu, this.Icon );

			if ( propertiesForm.ShowDialog() == DialogResult.Cancel ) {
				this.Package.Menu.AuthorEmail = oldEmail;
				this.Package.Menu.Date = oldDate;
				this.Package.Menu.WindowsBinariesPath = oldDocsPath;
				this.Package.Menu.SourceCodePath = oldSourceCodePath;
			}

			this.SetStatus();
		}

		private void OnPreview()
		{
			this.SetStatus( "Creating preview..." );

			var previewForm = new PreviewWindow( this.Package.Menu, this.Icon );

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

		/// <summary>
		/// When the user wants to search...
		/// </summary>
		private void OnSearch()
		{
			this.tbbSearch.Focus();
		}

		/// <summary>
		/// When the user launches a search...
		/// </summary>
		/// <param name="e">Information, a KeyEventArgs object, about the key pressed.</param>
		private void OnSearchEntered(KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter
			  && this.Package != null )
			{
				this.SetStatus( false );

				bool wasFound = false;
				e.SuppressKeyPress = true;
				e.Handled = true;

				// Trigger search
				string searchTerm = this.tbbSearch.Text;
				this.tbbSearch.Text = "";

				var lNodes = new Stack<TreeNode>();
				lNodes.Push( this.GetSelectedTreeNode() );

				while ( lNodes.Count > 0 ) {
					TreeNode tr = lNodes.Pop();
					var mc = this.GetMenuComponentOfTreeNode( tr );

					// Add immediate subnodes
					foreach (TreeNode subNode in tr.Nodes) {
						lNodes.Push( subNode );
					}

					if ( tr.NextNode != null ) {
						lNodes.Push( tr.NextNode );
					}

					// Check
					if ( mc.LookInContentsFor( searchTerm ) ) {
						this.tvMenu.SelectedNode = tr;
						wasFound = true;
						break;
					}
				}

				if ( !wasFound ) {
					System.Media.SystemSounds.Exclamation.Play();
					this.SetErrorStatus( string.Format( "Not found: '{0}'", searchTerm ) );
				}
			}

			return;
		}

		private void PrepareViewStructuresForNewDocument()
		{
			// Remove all editors in the panel of properties
			this.pnlProperties.Controls.Clear();

			// Remove all nodes in the tree view and add a new root
			this.tvMenu.Nodes.Clear();
			this.tvMenu.Nodes.Add( new UiComponents.RootMenuTreeNode( this.Package.Menu.Root ) );
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
			this.searchAction.Enabled = view;

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

		private void SetStatus(bool processMessages = true)
		{
			this.lblStatus.ForeColor = Color.Black;
			this.lblStatus.Text = "Ready";

			if ( processMessages ) {
				Application.DoEvents();
			}
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

		/// <summary>
		/// Gets the package in which the menu desing is stored.
		/// </summary>
		/// <value>The package, as a Package object.</value>
		public Package Package {
			get; private set;
		}

		/// <summary>
		/// Gets or sets the applications folder.
		/// By default, it is "applications".
		/// </summary>
		/// <value>The applications folder, as a string.</value>
		public string ApplicationsFolder {
			get {
				return LocalStorageManager.ApplicationsFolder;
			}
			set {
				LocalStorageManager.ApplicationsFolder = value;
			}
		}

		/// <summary>
		/// Gets or sets the pdf folder.
		/// By default, it is "Pdf".
		/// </summary>
		/// <value>The pdf folder, as a string.</value>
		public string PdfFolder {
			get {
				return LocalStorageManager.PdfFolder;
			}
			set {
				LocalStorageManager.PdfFolder = value;
			}
		}

		/// <summary>
		/// Gets or sets the graphs folder.
		/// By default, it is "Graphs"
		/// </summary>
		/// <value>The graphs folder, as a string.</value>
		public string GraphsFolder {
			get {
				return LocalStorageManager.GraphsFolder;
			}
			set {
				LocalStorageManager.GraphsFolder = value;
			}
		}

		private MenuComponentClipboard copier;
		private string fileName;
		private int numMenus;
		private int numFunctions;
		private int numPDFs;
		private int numGraphicMenus;
		private int numGraphicMenuEntries;
	}
}

