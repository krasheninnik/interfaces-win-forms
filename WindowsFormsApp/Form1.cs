using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.ShowDialog();
        }

        int x, y;
        bool isRed = true;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            var pen = new Pen(isRed ? Color.FromName("Red") :
            Color.FromName("Green"), 3);
            int l = ClientRectangle.Left;
            int t = ClientRectangle.Top + menuStrip1.Height;
            int r = ClientRectangle.Right;
            int b = ClientRectangle.Bottom;
            e.Graphics.DrawLine(pen, l, t, r, b);
            e.Graphics.DrawLine(pen, r, t, l, b);
            e.Graphics.DrawString($"({x},{y})", DefaultFont,
            new SolidBrush(Color.Black), x, y);
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                isRed = !isRed;
                Invalidate();
            }

        }
    }
}
