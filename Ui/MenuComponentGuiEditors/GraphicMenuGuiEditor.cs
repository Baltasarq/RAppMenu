using System;
using System.Drawing;
using System.Windows.Forms;

using RAppMenu.Core;

namespace RAppMenu.Ui.MenuComponentGuiEditors {
	public class GraphicMenuGuiEditor: MenuGuiEditor {
		public GraphicMenuGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
			: base( panel, mctn, mc )
		{
			this.Build();
		}

		public override void Show()
		{
			base.Show();
			this.pnlImageWidth.Show();
			this.pnlImageHeight.Show();
		}

		private void Build()
		{
			// Image width
			this.pnlImageWidth = new Panel();
			this.pnlImageWidth.Dock = DockStyle.Top;
			var lblImageWidth = new Label();
			lblImageWidth.Text = "Image width:";
			lblImageWidth.AutoSize = false;
			lblImageWidth.TextAlign = ContentAlignment.MiddleLeft;
			lblImageWidth.Dock = DockStyle.Left;
			this.udImageWidth = new NumericUpDown();
			this.udImageWidth.Dock = DockStyle.Fill;
			this.udImageWidth.Font = new Font( this.udImageWidth.Font, FontStyle.Bold );
			this.udImageWidth.ValueChanged += (sender, e) => this.OnValuesChanged();
			this.pnlImageWidth.Controls.Add( this.udImageWidth );
			this.pnlImageWidth.Controls.Add( lblImageWidth );
			this.pnlImageWidth.MaximumSize = new Size( int.MaxValue, this.udImageWidth.Height );
			this.Panel.Controls.Add( this.pnlImageWidth );

			// Image height
			this.pnlImageHeight = new Panel();
			this.pnlImageHeight.Dock = DockStyle.Top;
			var lblImageHeight = new Label();
			lblImageHeight.Text = "Image height:";
			lblImageHeight.AutoSize = false;
			lblImageHeight.TextAlign = ContentAlignment.MiddleLeft;
			lblImageHeight.Dock = DockStyle.Left;
			this.udImageHeight = new NumericUpDown();
			this.udImageHeight.Dock = DockStyle.Fill;
			this.udImageHeight.Font = new Font( this.udImageHeight.Font, FontStyle.Bold );
			this.udImageHeight.ValueChanged += (sender, e) => this.OnValuesChanged();
			this.pnlImageHeight.Controls.Add( this.udImageHeight );
			this.pnlImageHeight.Controls.Add( lblImageHeight );
			this.pnlImageHeight.MaximumSize = new Size( int.MaxValue, this.udImageWidth.Height );
			this.Panel.Controls.Add( this.pnlImageHeight );

			// Limits
			this.udImageWidth.Minimum = 16;
			this.udImageWidth.Maximum = 250;
			this.udImageHeight.Minimum = 16;
			this.udImageHeight.Maximum = 250;
		}

		private void OnValuesChanged()
		{
			var graphicMenu = (Core.MenuComponents.ImagesMenu) this.MenuComponent;

			graphicMenu.ImageHeight = (int) this.udImageHeight.Value;
			graphicMenu.ImageWidth = (int) this.udImageWidth.Value;
		}

		private Panel pnlImageWidth;
		private Panel pnlImageHeight;
		private NumericUpDown udImageWidth;
		private NumericUpDown udImageHeight;
	}
}

