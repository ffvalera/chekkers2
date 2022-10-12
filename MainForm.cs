using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chekkers2
{
    public partial class MainForm : Form
    {
        const int boardSize = 8;
        const int cellSize = 50;
        int[,] board;
        Image white;
        Image black;
        public MainForm()
        {
            InitializeComponent();

            white = new Bitmap("../../../Images/white.jpg");
            black = new Bitmap("../../../Images/black.jpg");
            this.Text = "Chekkers";
            Init();
        }
        public void Init()
        {
            board = new int[boardSize, boardSize]
            {
                {1, 0, 1, 0, 1, 0, 1, 0 },
                {0, 1, 0, 1, 0, 1, 0, 1 },
                {1, 0, 1, 0, 1, 0, 1, 0 },
                {0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 2, 0, 2, 0, 2, 0, 2 },
                {2, 0, 2, 0, 2, 0, 2, 0 },
                {0, 2, 0, 2, 0, 2, 0, 2 },
            };
            CreateBoard();
        }
        public void CreateBoard()
        {
            this.Width = (boardSize +1)* cellSize;
            this.Height = (boardSize+1) * cellSize;

            for(int i = 0; i < boardSize; i++)
                for(int j = 0; j < boardSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(i*cellSize,j*cellSize);
                    button.Size = new Size(cellSize,cellSize);
                    if (board[i, j] == 1)
                        button.BackgroundImage = black;
                    else if (board[i, j] == 2)
                        button.BackgroundImage = white;
                    button.BackgroundImageLayout = ImageLayout.Zoom;
                    this.Controls.Add(button);

                }
        }
            
    }
}
