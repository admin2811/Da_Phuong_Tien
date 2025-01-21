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
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public Form1()
        {
            InitializeComponent();
            container(new CreateGifForm());
        }
        private void CreateGifbtn_Click(object sender, EventArgs e)
        {
            container(new CreateGifForm());
        }
        private void container(object _form)
        {
            if (guna2Panel_container.Controls.Count > 0)
            {
                guna2Panel_container.Controls.Clear();
            }

            Form fm = _form as Form;
            fm.TopLevel = false;
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.Dock = DockStyle.Fill;
            guna2Panel_container.Controls.Add(fm);
            fm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void guna2Panel_container_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Searchbtn_Click(object sender, EventArgs e)
        {
            container(new SearchingGif());
        }
    }
}
