using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectAssignment
{
    public partial class ColorSelector : Form
    {
        private string hexColor1;
        private string hexColor2;
        private string hexColor3;
        public ColorSelector()
        {
            InitializeComponent();
        }

        private void ColorSelector_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Bitmap pixelData = (Bitmap)pictureBox1.Image;
            Color clr = pixelData.GetPixel(e.X, e.Y);
            lblSmallScreen.BackColor = clr;
            lblRGBvalue.Text = "R: " + clr.R.ToString() + " G: " + clr.G.ToString() + " B: " + clr.B.ToString();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Bitmap pixelData = (Bitmap)pictureBox1.Image;
            Color clr = pixelData.GetPixel(e.X, e.Y);

            string hexColor = $"#{clr.R:X2}{clr.G:X2}{clr.B:X2}";

            txtRedValue.Text = clr.R.ToString();
            txtGreenValue.Text = clr.G.ToString();
            txtBlueValue.Text = clr.B.ToString();
            hexColor1 = hexColor;
            pnlSelectedScreen.BackColor = clr;
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            Bitmap pixelData = (Bitmap)pictureBox2.Image;
            Color clr = pixelData.GetPixel(e.X, e.Y);
            lblSmallScreen2.BackColor = clr;
            lblRGBvalue2.Text = "R: " + clr.R.ToString() + " G: " + clr.G.ToString() + " B: " + clr.B.ToString();

        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            Bitmap pixelData = (Bitmap)pictureBox2.Image;
            Color clr = pixelData.GetPixel(e.X, e.Y);

            string hexColor = $"#{clr.R:X2}{clr.G:X2}{clr.B:X2}";

            txtRedValue2.Text = clr.R.ToString();
            txtGreenValue2.Text = clr.G.ToString();
            txtBlueValue2.Text = clr.B.ToString();
            hexColor2 = hexColor;
            pnlSelectedScreen2.BackColor = clr;

        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            Bitmap pixelData = (Bitmap)pictureBox3.Image;
            Color clr = pixelData.GetPixel(e.X, e.Y);
            lblSmallScreen3.BackColor = clr;
            lblRGBvalue3.Text = "R: " + clr.R.ToString() + " G: " + clr.G.ToString() + " B: " + clr.B.ToString();

        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            Bitmap pixelData = (Bitmap)pictureBox3.Image;
            Color clr = pixelData.GetPixel(e.X, e.Y);

            string hexColor = $"#{clr.R:X2}{clr.G:X2}{clr.B:X2}";

            txtRedValue3.Text = clr.R.ToString();
            txtGreenValue3.Text = clr.G.ToString();
            txtBlueValue3.Text = clr.B.ToString();
            hexColor3 = hexColor;
            pnlSelectedScreen3.BackColor = clr;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Tag = new string[] { hexColor1, hexColor2, hexColor3 };
            this.DialogResult = DialogResult.OK;
            this.Close();

            string hexColors = hexColor1 + "," + hexColor2 + "," + hexColor3;

        }
    }
}
