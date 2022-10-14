using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace chekkers2

{
    public enum Player
    {
        Black = 0, White = 1
    };
    public enum State
    {
        Normal, King
    }

    public struct cell
    {
        public int x;
        public int y;
        public Player? Player;
        public State? State;
        public cell(int x, int y)
        {
            this.x = x;
            this.y = y;
            Player = null;
            State = null;
        }
    }
    public partial class MainForm : Form
    {
        const int boardSize = 8;
        const int cellSize = 60;
        Color currentChekker = Color.Gold;
        Image white = new Bitmap(new Bitmap("../../../Images/whiteChekker.png"), new Size(cellSize - 10, cellSize - 10));
        Image black = new Bitmap(new Bitmap("../../../Images/blackChekker.png"), new Size(cellSize - 10, cellSize - 10));
        Image whiteKing = new Bitmap(new Bitmap("../../../Images/white.jpg"), new Size(cellSize - 10, cellSize - 10));
        Image blackKing = new Bitmap(new Bitmap("../../../Images/black.jpg"), new Size(cellSize - 10, cellSize - 10));
        Point[] whiteMoves = { new Point(1, -1), new Point(-1, -1) };
        Point[] blackMoves = { new Point(1, 1), new Point(-1, 1) };
        Point[] takeMoves = { new Point(2, 2), new Point(-2, 2), new Point(-2, -2), new Point(2, -2) };

        Player currentPlayer;
        Button[,] board = new Button[boardSize, boardSize];        
        bool isMoving = false;
        Button? whosMoving = null;
        
        
        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            currentPlayer = Player.White;
            this.Text = "Chekkers";
            CreateBoard();
        }
        public void CreateBoard()
        {
            this.Width = boardSize * cellSize + 20;
            this.Height = boardSize * cellSize + 40;

            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                {
                    Button button = new Button();

                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.Click += new EventHandler(Click);
                    cell cell = new cell(j, i);

                    if ((i + j) % 2 == 1)
                        button.BackColor = Color.White;
                    else
                    {
                        if (i < 3)
                        {
                            cell.Player = Player.Black;
                            button.Image = black;
                            cell.State = State.Normal;
                        }
                        else if (i >= 5)
                        {
                            cell.Player = Player.White;
                            button.Image = white;
                            cell.State = State.Normal;
                        }

                        button.BackColor = Color.FromArgb(192, 64, 0);
                    }
                    
                    button.Tag = cell;
                    board[j, i] = button;
                    this.Controls.Add(button);
                }
            string letters = "abcdefgh";
            for(int i = 0; i < boardSize; i++)
            {
                Label label = new Label();
                label.Text = letters[i].ToString();
                label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                label.Location = new Point(i * cellSize, boardSize * cellSize);
                label.Size = new Size(cellSize, cellSize/2);
                this.Controls.Add(label);
            }
        }

        public void SwitchPlayer()
        {
            currentPlayer = 1 - currentPlayer;
        }
        void Move(Button start, Button target)
        {
            cell targetCell = (cell)target.Tag;
            cell startCell = (cell)start.Tag;

            targetCell.Player = startCell.Player;            
            startCell.Player = null;

            targetCell.State = startCell.State;
            startCell.State = null;

            target.Tag = targetCell;
            start.Tag = startCell;

            target.Image = start.Image;
            start.Image = null;
        }
        bool isSimpleMoveCorrect(cell start, cell target)
        {
            return target.Player == null
                && (start.Player == Player.White && whiteMoves.Contains(new Point( target.x- start.x, target.y - start.y))
                || (start.Player == Player.Black && blackMoves.Contains(new Point( target.x- start.x,  target.y- start.y))));
        }
        bool isTakeMoveCorrect(cell start, cell target)
        {
            cell mid = (cell)board[(start.x + target.x) / 2, (start.y + target.y) / 2].Tag;
            return target.Player == null
                && takeMoves.Contains(new Point(target.x - start.x, target.y - start.y))
                && mid.Player != null && mid.Player != currentPlayer;

        }
        void Remove(Button button)
        {
            cell cell = (cell)button.Tag;
            cell.Player = null;
            button.Tag = cell;
            button.Image = null;
        }
        bool canTake(cell cell)
        {
            bool ans = false;
            foreach (var move in takeMoves)
            {
                if (cell.x + move.X < boardSize && cell.y + move.Y < boardSize && cell.x + move.X >= 0 && cell.y + move.Y >= 0)
                {
                    cell target = (cell)board[cell.x + move.X, cell.y + move.Y].Tag;
                    ans = ans || isTakeMoveCorrect(cell, target);
                }
            }
            return ans;
        }
        void normalMoving(cell start, cell cell, Button button)
        {
            if (isSimpleMoveCorrect(start, cell) && !canTake(start))
            {
                Move(whosMoving, button);

                SwitchPlayer();
                isMoving = false;
                whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                whosMoving = null;
            }
            else if (isTakeMoveCorrect(start, cell))
            {
                Button mid = board[(start.x + cell.x) / 2, (start.y + cell.y) / 2];
                Move(whosMoving, button);
                Remove(mid);
                if (!canTake(cell))
                {
                    SwitchPlayer();
                    isMoving = false;
                    whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                    whosMoving = null;
                }
                else
                {
                    button.BackColor = currentChekker;
                    whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                    whosMoving = button;
                }
            }
            else if (!canTake(start))
            {
                isMoving = false;
                whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                whosMoving = null;
            }

        }
        void KingMoving(cell start, cell cell, Button button)
        {

        }
        void Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            cell cell = (cell)button.Tag;       
            
            if (isMoving)
            {
                cell start = (cell)whosMoving.Tag;
                if (start.State == State.Normal)
                {
                    normalMoving(start, cell, button);
                }
                else
                {
                    KingMoving(start, cell, button);
                }
            }
            else if (cell.Player == currentPlayer)
            {
                whosMoving = button;
                button.BackColor = currentChekker;
                isMoving = true;
            }

            cell = (cell)button.Tag;
            if(cell.y == 0 && cell.Player == Player.White || cell.y == 7 && cell.Player == Player.Black)
            {
                cell.State = State.King;
                button.Tag = cell;
                button.Image = cell.Player == Player.White?whiteKing:blackKing;
            }
        }
    }
}
