using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphMenuUtils {
  class GraphicsMenuTable {
        public enum SizeModeStyle { StrechImage, ZoomImage };

        ToolStripMenuItem StartmenuItem;
        ToolStripDropDown graphmenu;
        EventHandler ClickOnGraphMenuItem_Click;
        Font menuItemsFont;
        int margen = 20;
        float itemHeight;
        float itemWidth;
        int NumColumnasManual = 0;
        string VoidString =new string('X', 1000);
        SizeModeStyle sizeMode = SizeModeStyle.StrechImage;



        private GraphicsMenuTable(ToolStripMenuItem StartmenuItem)
        {
            menuItemsFont = new Font(FontFamily.GenericSerif, 1000);
            graphmenu = new ToolStripDropDown();
            graphmenu.Name = "";
            graphmenu.Font = menuItemsFont;
            this.StartmenuItem = StartmenuItem;
            StartmenuItem.DropDown = graphmenu;
            ItemHeight = 60;
            ItemWidth = 60;
        }

        public SizeModeStyle SizeMode
        {
            get { return sizeMode; }
            set
            {
                sizeMode = value;
            }
        }

        public int MargenTotal
        {
            get { return margen; }
            set
            {
                margen = value;
                UpdateLayout();
            }
        }

        public int NumColumns
        {
            get { return NumColumnasManual; }
            set
            {
                NumColumnasManual = value;
                UpdateLayout();
            }
        }

        public float ItemHeight
        {
            get { return itemHeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("ItemHeight must be >=0 in GraphicsMenuTable");
                if (itemHeight < 250)
                {
                    itemHeight = value;
                    UpdateLayout();
                }
            }
        }

        public float ItemWidth
        {
            get { return itemWidth; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("ItemHeight must be >=0 in GraphicsMenuTable");
                if (itemWidth < 350)
                {
                    itemWidth = value;
                    UpdateLayout();
                }
            }
        }
    

        private void internAddGraphItem(GraphMenuItemData item)
        {
            ToolStripMenuItem menuitem = new ToolStripMenuItem();
            menuitem.Text = VoidString;
            menuitem.Tag = item;
            menuitem.ToolTipText = item.ToolTip;
            menuitem.Paint += GraphToolStripMenuItem1_Paint;
            menuitem.Font = menuItemsFont;
            //menuitem.Click += ClickOnGraphMenuItem_Click;
            menuitem.MouseHover += RepaintGraphMenuItem_Click;
            menuitem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            graphmenu.Items.Add(menuitem);
        }


        public void AddGraphItem(GraphMenuItemData item)
        {
            internAddGraphItem(item);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            graphmenu.LayoutStyle = ToolStripLayoutStyle.Table;
            TableLayoutSettings tablelayout = (TableLayoutSettings)graphmenu.LayoutSettings;
            Size filasColumnas=CalcularFilasColumnasMenu(graphmenu.Items.Count, menuItemsFont);
            tablelayout.ColumnCount = filasColumnas.Width;
            tablelayout.RowCount = filasColumnas.Height;
            for (int i = 0; i < tablelayout.ColumnCount; i++)
            {
                if (tablelayout.ColumnStyles.Count > i)
                    tablelayout.ColumnStyles[i] = new ColumnStyle(SizeType.Absolute, itemWidth + margen);
                else
                    tablelayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, itemWidth + margen));
            }
            for (int i = 0; i < tablelayout.RowCount; i++)
            {
                if (tablelayout.RowStyles.Count > i)
                    tablelayout.RowStyles[i] = new RowStyle(SizeType.Absolute, itemHeight + margen);
                else
                    tablelayout.RowStyles.Add(new RowStyle(SizeType.Absolute, itemHeight + margen));
            }
        }

        public void AddGraphItems(List<GraphMenuItemData> ListamenuItems)
        {
            foreach (GraphMenuItemData item in ListamenuItems)
            {
                internAddGraphItem(item);
            }
            UpdateLayout();
        }

        public void SetClickEvent(EventHandler ClickOnGraphMenuItem_Click)
        {
            this.ClickOnGraphMenuItem_Click = ClickOnGraphMenuItem_Click;
            foreach (ToolStripItem item in graphmenu.Items)
            {
                item.Click += ClickOnGraphMenuItem_Click;
            }
        }

        public static GraphicsMenuTable AddGraphMenuTable(ToolStripMenuItem StartmenuItem, List<GraphMenuItemData> ListamenuItems)
        {
            GraphicsMenuTable result = new GraphicsMenuTable(StartmenuItem);
            result.AddGraphItems(ListamenuItems);
            return result;
        }


        private Size CalcularFilasColumnasMenu(int numItems, Font menuItemsFont)
        {
            Size r = new Size( 60, 60 );
            int extramargen = 100;
            float totalHeight = ((itemHeight+margen) * numItems) + extramargen;
            Rectangle screenSize = Screen.PrimaryScreen.Bounds;
            int minnumColumns = ((int)totalHeight / screenSize.Height);
            if (totalHeight % screenSize.Height > 0) minnumColumns++;
            int numColumns = Math.Max(NumColumnasManual, minnumColumns);
            NumColumnasManual = numColumns;
            r.Width = numColumns;
            r.Height = (numItems / numColumns) + (numItems % numColumns > 0?1:0);
            return r;
        }



        private void RepaintGraphMenuItem_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem)) return;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.OwnerItem.Invalidate();
            foreach (ToolStripMenuItem t in (item.OwnerItem as ToolStripMenuItem).DropDownItems)
            {
                t.Invalidate();
            }
            Application.DoEvents();
        }

        private void GraphToolStripMenuItem1_Paint(object sender, PaintEventArgs e)
        {
            if (!(sender is ToolStripMenuItem)) return;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            string f = (item.Tag as GraphMenuItemData).ImageFileName;
            Image icono=Image.FromFile(f);

            if (sizeMode == SizeModeStyle.StrechImage)
            {
                e.Graphics.DrawImage(icono, margen / 2, margen / 2, e.ClipRectangle.Width - margen, e.ClipRectangle.Height - margen);
            }
            else if (sizeMode== SizeModeStyle.ZoomImage)
            {
                Rectangle imagerect = new Rectangle(e.ClipRectangle.X + margen / 2, e.ClipRectangle.Y + margen / 2, e.ClipRectangle.Width - margen, e.ClipRectangle.Height - margen);
                float ratioImage = (float)icono.Width / (float)icono.Height;
                float ratioRect = (float)imagerect.Width / (float)imagerect.Height;
                int rHeight=0;
                int rWidth=0;
                if (ratioRect > ratioImage)
                {
                    rHeight = imagerect.Height;
                    rWidth = (int)((float)rHeight * ratioImage);
                }
                else
                {
                    rWidth = imagerect.Width;
                    rHeight = (int)((float)rWidth / ratioImage);
                }
                imagerect = new Rectangle(imagerect.X + (imagerect.Width - rWidth) / 2,
                    imagerect.Y + (imagerect.Height - rHeight) / 2, rWidth, rHeight);
                if (!e.ClipRectangle.Contains(imagerect))
                    return;
                e.Graphics.DrawImage(icono, imagerect.X, imagerect.Y, imagerect.Width, imagerect.Height);
            }
        }


        public class GraphMenuItemData
        {
            public string ImageFileName;
            public string ToolTip;
            public Object Tag;

            public GraphMenuItemData(string FileName, string ToolTip, Object Tag)
            {
                this.ImageFileName = FileName;
                this.ToolTip = ToolTip;
                this.Tag = Tag;
            }
        }

    }
}
