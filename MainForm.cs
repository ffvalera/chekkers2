using System.Numerics;

namespace chekkers2
{
    public enum Player {Black = 0, White = 1};
    public enum State {Normal, King}

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
        Color canMove = Color.Aquamarine;
        Color Broun = Color.FromArgb(192, 64, 0);
        Image white = new Bitmap(new Bitmap("../../../Images/whiteChekker.png"), new Size(cellSize - 10, cellSize - 10));
        Image black = new Bitmap(new Bitmap("../../../Images/blackChekker.png"), new Size(cellSize - 10, cellSize - 10));
        Image whiteKing = new Bitmap(new Bitmap("../../../Images/white.jpg"), new Size(cellSize - 10, cellSize - 10));
        Image blackKing = new Bitmap(new Bitmap("../../../Images/black.jpg"), new Size(cellSize - 10, cellSize - 10));
        Point[] whiteMoves = { new Point(1, -1), new Point(-1, -1) };
        Point[] blackMoves = { new Point(1, 1), new Point(-1, 1) };
        Point[] takeMoves = { new Point(2, 2), new Point(-2, 2), new Point(-2, -2), new Point(2, -2) };       
        Point[] kingMoves = new Point[28];

        Player currentPlayer;
        Button[,] board = new Button[boardSize, boardSize];        
        bool isMoving = false;
        bool isLongTaking = false;
        Button? whosMoving = null;
        List<Button> colored = new List<Button>();
        
        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            for(int i = 1; i < boardSize; i++)
            {
                kingMoves[(i - 1) * 4] = new Point(i, i);
                kingMoves[(i - 1) * 4 + 1] = new Point(-i, i);
                kingMoves[(i - 1) * 4 + 2] = new Point(i, -i);
                kingMoves[(i - 1) * 4 + 3] = new Point(-i, -i);
            }
            currentPlayer = Player.Black;
            this.Text = "Chekkers";
            CreateBoard();
        }
        public void CreateBoard()
        {
            this.Width = boardSize * cellSize + 20;
            this.Height = boardSize * cellSize + 60;

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

            if (targetCell.y == 0 && targetCell.Player == Player.White || targetCell.y == 7 && targetCell.Player == Player.Black)
            {
                targetCell.State = State.King;
                target.Tag = targetCell;
                target.Image = targetCell.Player == Player.White ? whiteKing : blackKing;
            }
        }
        bool isSimpleMoveCorrect(cell start, cell target)
        {
            if (start.State == State.Normal)
                return target.Player == null
                    && (start.Player == Player.White && whiteMoves.Contains(new Point(target.x - start.x, target.y - start.y))
                    || (start.Player == Player.Black && blackMoves.Contains(new Point(target.x - start.x, target.y - start.y))));
            else
            {
                var dir = new Point(target.x - start.x, target.y - start.y);
                if (Math.Abs(dir.X) != Math.Abs(dir.Y) || Math.Abs(dir.X) == 0)
                    return false;
                dir.X /= Math.Abs(dir.X);
                dir.Y /= Math.Abs(dir.Y);
                var targ = new Point(target.x, target.y);
                while(targ.X != start.x && targ.Y != start.y)
                {
                    cell mid = (cell)board[targ.X, targ.Y].Tag;
                    if (mid.Player != null)
                        return false;
                    targ.X -= dir.X;
                    targ.Y -= dir.Y;
                }
                return true;
            }
        }
        Button isTakeMoveCorrect(cell start, cell target)
        {
            if (start.State != State.King)
            {
                cell mid = (cell)board[(start.x + target.x) / 2, (start.y + target.y) / 2].Tag;
                if (target.Player == null
                    && takeMoves.Contains(new Point(target.x - start.x, target.y - start.y))
                    && mid.Player != null && mid.Player != currentPlayer)                                    
                    return board[mid.x, mid.y];                
                else
                    return null;
            }
            else
            {
                var dir = new Point(target.x - start.x, target.y - start.y);
                cell dirCell = (cell)board[target.x, target.y].Tag;
                if (Math.Abs(dir.X) != Math.Abs(dir.Y) || Math.Abs(dir.X) == 0 || dirCell.Player != null)
                    return null;

                bool findChekker = false;
                Button chekker = null;

                dir.X /= Math.Abs(dir.X);
                dir.Y /= Math.Abs(dir.Y);
                var targ = new Point(target.x-dir.X, target.y-dir.Y);

                while (targ.X != start.x && targ.Y != start.y)
                {
                    cell mid = (cell)board[targ.X, targ.Y].Tag;
                    if (mid.Player == currentPlayer)
                        return null;
                    else if(mid.Player != null)
                    {
                        if (findChekker)
                            return null;
                        findChekker = true;
                        chekker = board[mid.x, mid.y];
                    }

                    targ.X -= dir.X;
                    targ.Y -= dir.Y;
                }
                return chekker;
            }
        }
        void Remove(Button button)
        {
            cell cell = (cell)button.Tag;
            cell.Player = null;
            cell.State = null;
            button.Tag = cell;
            button.Image = null;
        }
        bool canTake(cell cell)
        {
            var moves = takeMoves;
            if (cell.State == State.King)
                moves = kingMoves;

            bool ans = false;
            foreach (var move in moves)
            {
                if (cell.x + move.X < boardSize && cell.y + move.Y < boardSize && cell.x + move.X >= 0 && cell.y + move.Y >= 0)
                {
                    cell target = (cell)board[cell.x + move.X, cell.y + move.Y].Tag;
                    ans = ans || (isTakeMoveCorrect(cell, target) != null);
                }
            }
            return ans;
        }
        bool mustTake()
        {
            bool ans = false;
            foreach (var button in board)
            {
                cell cell = (cell)button.Tag;
                if(cell.Player == currentPlayer)
                {
                    ans = ans || canTake(cell);
                }
            }
            return ans;
        }
        void Moving(cell start, cell cell, Button button)
        {
            if (isSimpleMoveCorrect(start, cell) && !mustTake())
            {
                Move(whosMoving, button);

                SwitchPlayer();
                isMoving = false;
                whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                whosMoving = null;
            }
            else if (isTakeMoveCorrect(start, cell) != null)
            {
                Button mid = isTakeMoveCorrect(start, cell);
                Move(whosMoving, button);
                Remove(mid);
                cell = (cell)button.Tag;
                if (!canTake(cell))
                {
                    SwitchPlayer();
                    isMoving = false;
                    isLongTaking = false;
                    whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                    whosMoving = null;
                }
                else
                {
                    GetMoves(cell);
                    isLongTaking=true;
                    button.BackColor = currentChekker;
                    whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                    whosMoving = button;
                }
            }
            else if (!isLongTaking)
            {
                isMoving = false;
                whosMoving.BackColor = Color.FromArgb(192, 64, 0);
                whosMoving = null;
            }
        }
        void GetMoves(cell cell)
        {
            if (canTake(cell))
            {
                var moves = takeMoves;
                if (cell.State == State.King)
                    moves = kingMoves;


                foreach (var move in moves)
                {
                    if (cell.x + move.X < boardSize && cell.y + move.Y < boardSize && cell.x + move.X >= 0 && cell.y + move.Y >= 0)
                    {
                        cell target = (cell)board[cell.x + move.X, cell.y + move.Y].Tag;
                        if (isTakeMoveCorrect(cell, target) != null)
                        {
                            board[target.x, target.y].BackColor = canMove;
                            colored.Add(board[target.x, target.y]);
                        }
                    }
                }
            }
            else if(!mustTake())
            {
                var moves = cell.Player == Player.White ? whiteMoves : blackMoves;
                if (cell.State == State.King)
                    moves = kingMoves;
                foreach(var move in moves)
                {
                    if (cell.x + move.X < boardSize && cell.y + move.Y < boardSize && cell.x + move.X >= 0 && cell.y + move.Y >= 0)
                    {
                        cell target = (cell)board[cell.x + move.X, cell.y + move.Y].Tag;
                        if (isSimpleMoveCorrect(cell, target))
                        {
                            board[target.x, target.y].BackColor = canMove;
                            colored.Add(board[target.x, target.y]);
                        }
                    }
                }
            }
        }
        void DeleteMoves()
        {
            foreach(Button button in colored)
            {
                button.BackColor = Broun;
            }
            colored.Clear();
        }
        void Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            cell cell = (cell)button.Tag;       
            
            if (isMoving)
            {
                cell start = (cell)whosMoving.Tag;
                DeleteMoves();
                Moving(start, cell, button);                
            }
            else if (cell.Player == currentPlayer)
            {
                whosMoving = button;
                button.BackColor = currentChekker;
                isMoving = true;
                GetMoves(cell);
            }

            cell = (cell)button.Tag;
        
        }
    }
}
