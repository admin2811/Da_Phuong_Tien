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
    public partial class FontFamilySelector : Form
    {
        public string SelectedFont { get; private set; } = "Arial";
        public int SelectedSize { get; private set; } = 12;
        public string SelectedPosition { get; private set; } = "top";

        public FontFamilySelector()
        {
            InitializeComponent();
        }

        private void FontFamilySelector_Load(object sender, EventArgs e)
        {
            foreach (FontFamily font in FontFamily.Families)
            {
                comboBoxFont.Items.Add(font.Name);
            }

            for (int i = 8; i <= 72; i += 2)
            {
                comboBoxSize.Items.Add(i);
            }

            comboBoxPosition.Items.AddRange(new string[] { "top", "middle", "bottom" });
        }

        private void comboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedFont = comboBoxFont.SelectedItem?.ToString() ?? "Arial";
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(comboBoxSize.SelectedItem?.ToString(), out int size))
            {
                SelectedSize = size;
            }
            else
            {
                SelectedSize = 12;
            }
        }

        private void comboBoxPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedPosition = comboBoxPosition.SelectedItem?.ToString() ?? "top";
        }

        private void FontFamilySelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedFont))
            {
                SelectedFont = "Arial";
            }

            if (SelectedSize == 0)
            {
                SelectedSize = 12;
            }

            if (string.IsNullOrEmpty(SelectedPosition))
            {
                SelectedPosition = "top";
            }
        }

        private void btnChon_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}