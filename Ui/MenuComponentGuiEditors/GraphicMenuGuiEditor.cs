using System;
using System.Drawing;
using System.Windows.Forms;

using RWABuilder.Core;
using CoreComponents = RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui.MenuComponentGuiEditors {
	public class GraphicMenuGuiEditor: MenuGuiEditor {
		public GraphicMenuGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
			: base( panel, mctn, mc )
		{
			this.Build();
		}

		public override void Show()
		{
			base.Show();
			this.pnlMeasures.Show();
		}

		private void Build()
		{
            this.OnBuilding = true;
            this.Panel.SuspendLayout();

			// Panel
			this.pnlMeasures = new FlowLayoutPanel();
            this.pnlMeasures.AutoSize = true;
			this.pnlMeasures.SuspendLayout();
			this.pnlMeasures.Dock = DockStyle.Top;
			this.Panel.Controls.Add( this.pnlMeasures );

			// Image width
			var lblImageWidth = new Label();
			lblImageWidth.Text = "Image width:";
			lblImageWidth.AutoSize = false;
			lblImageWidth.TextAlign = ContentAlignment.MiddleLeft;
			this.udImageWidth = new NumericUpDown();
			this.udImageWidth.TextAlign = HorizontalAlignment.Right;
			this.udImageWidth.Font = new Font( this.udImageWidth.Font, FontStyle.Bold );
			this.udImageWidth.ValueChanged += (sender, e) => this.OnValuesChanged();
			this.pnlMeasures.Controls.Add( lblImageWidth );
			this.pnlMeasures.Controls.Add( this.udImageWidth );

			// Image height
			var lblImageHeight = new Label();
			lblImageHeight.Text = "Image height:";
			lblImageHeight.AutoSize = false;
			lblImageHeight.TextAlign = ContentAlignment.MiddleLeft;
			this.udImageHeight = new NumericUpDown();
			this.udImageHeight.TextAlign = HorizontalAlignment.Right;
			this.udImageHeight.Font = new Font( this.udImageHeight.Font, FontStyle.Bold );
			this.udImageHeight.ValueChanged += (sender, e) => this.OnValuesChanged();
			this.pnlMeasures.Controls.Add( lblImageHeight );
			this.pnlMeasures.Controls.Add( this.udImageHeight );

			// Minimum number of columns
			var lblMinCols = new Label();
			lblMinCols.Text = "Minimum number of columns:";
			lblMinCols.AutoSize = true;
			lblMinCols.TextAlign = ContentAlignment.MiddleLeft;
			this.udMinimumColumns = new NumericUpDown();
			this.udMinimumColumns.TextAlign = HorizontalAlignment.Right;
			this.udMinimumColumns.Font = new Font( this.udImageHeight.Font, FontStyle.Bold );
			this.udMinimumColumns.ValueChanged += (sender, e) => this.OnValuesChanged();
			this.pnlMeasures.Controls.Add( lblMinCols );
			this.pnlMeasures.Controls.Add( this.udMinimumColumns );

			// Sizes for controls
			Graphics grf = new Form().CreateGraphics();
			SizeF fontSize = grf.MeasureString( "W", this.udImageHeight.Font );
			int charWidth = (int) fontSize.Width + 5;
			this.udImageWidth.MaximumSize = new Size( charWidth * 3, this.udImageWidth.Height );
			this.udImageHeight.MaximumSize = new Size( charWidth * 3, this.udImageHeight.Height );

			// Limits
			this.udImageWidth.Minimum = CoreComponents.GraphicMenu.MinimumGraphicSize;
			this.udImageWidth.Maximum = CoreComponents.GraphicMenu.MaximumGraphicSize;
			this.udImageHeight.Minimum = CoreComponents.GraphicMenu.MinimumGraphicSize;
			this.udImageHeight.Maximum = CoreComponents.GraphicMenu.MaximumGraphicSize;
			this.udMinimumColumns.Minimum = CoreComponents.GraphicMenu.MinimumColumns;
			this.udMinimumColumns.Maximum = CoreComponents.GraphicMenu.MaximumColumns;

			this.pnlMeasures.ResumeLayout( false );
            this.Panel.ResumeLayout( false );
            this.OnBuilding = false;
		}

        public override void ReadDataFromComponent()
        {
            base.ReadDataFromComponent();
            this.OnBuilding = true;

            this.udImageWidth.Value = this.GraphicMenu.ImageWidth;
            this.udImageHeight.Value = this.GraphicMenu.ImageHeight;
            this.udMinimumColumns.Value = this.GraphicMenu.MinimumNumberOfColumns;

            this.OnBuilding = false;
        }

		private void OnValuesChanged()
		{
            if ( !this.OnBuilding ) {
    			var graphicMenu = (Core.MenuComponents.GraphicMenu) this.MenuComponent;

    			graphicMenu.ImageHeight = (int) this.udImageHeight.Value;
    			graphicMenu.ImageWidth = (int) this.udImageWidth.Value;
            }

            return;
		}

		public CoreComponents.GraphicMenu GraphicMenu {
			get {
				return (CoreComponents.GraphicMenu) this.MenuComponent;
			}
		}

		private Panel pnlMeasures;
		private NumericUpDown udImageWidth;
		private NumericUpDown udImageHeight;
		private NumericUpDown udMinimumColumns;
	}
}

