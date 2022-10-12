using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics;

namespace chekkers2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = "ABOBA";
            this.BackColor = Color.Aquamarine;
            this.Height = 250;
            this.Width = 250;
            this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            this.BackgroundImage = Image.FromFile("C:\\Users\\valer\\OneDrive\\Документы\\обои\\Screenshot_7-1.jpg");
            this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Congrutilation");
                
        }
    }
}