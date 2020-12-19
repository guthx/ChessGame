using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGame
{
    public partial class Form1 : Form
    {
        private FlowLayoutPanel Chessboard;
        private ChessboardSquare[,] Squares;
        private Gamestate Gamestate;
        private List<Position> SelectedValidMoves;
        private Position SelectedPiece;
        private TableLayoutPanel TextPanel; 
        private TextBox ToMove;
        private TextBox TurnCount;
        private TableLayoutPanel MainLayout;
        private MoveResult result;
        private void RefreshColors()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0)
                        Squares[i, j].BackColor = System.Drawing.Color.AliceBlue;
                    else
                        Squares[i, j].BackColor = System.Drawing.Color.LightGreen;
                }
        }
        private void UpdateBoard()
        {
            RefreshColors();
            for(int i=0; i<8; i++)
                for(int j=0; j<8; j++)
                {
                    Squares[i, j].Image = null;
                    if(Gamestate.Board[i, j] != null)
                    {
                        var pieceName = Gamestate.Board[i, j].Color.ToString() + "_" + Gamestate.Board[i, j].GetType().Name + ".png";
                        Squares[i, j].Image = Image.FromFile(@"Resources/" + pieceName);
                        Squares[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            ToMove.Text = Gamestate.ToMove + " to move";
            TurnCount.Text = "Turn " + Gamestate.TurnCount.ToString();
        }

        private void ShowValidMoves(object sender, EventArgs e)
        {
            
            RefreshColors();
            var squarePosition = ((ChessboardSquare)sender).Position;
            if (result == MoveResult.MOVED)
            {
                if (Gamestate.Board[squarePosition.File, squarePosition.Rank] != null &&
                Gamestate.Board[squarePosition.File, squarePosition.Rank].Color == Gamestate.ToMove)
                {
                    SelectedValidMoves.Clear();
                    var piece = Gamestate.Board[squarePosition.File, squarePosition.Rank];
                    foreach (var move in piece.ValidMoves)
                    {
                        Squares[move.File, move.Rank].BackColor = System.Drawing.Color.Red;
                        SelectedValidMoves.Add(Squares[move.File, move.Rank].Position);
                    }
                    SelectedPiece = squarePosition;
                }
                else if (SelectedValidMoves.Contains(squarePosition))
                {
                    result = Gamestate.Move(SelectedPiece, squarePosition);
                    if (result == MoveResult.AWAITING_PROMOTION)
                    {
                        if (Gamestate.ToMove == Color.WHITE)
                        {
                            for(int i=0; i<4; i++)
                                Squares[squarePosition.File, squarePosition.Rank - i].BackColor = System.Drawing.Color.Blue;
                            Squares[squarePosition.File, squarePosition.Rank].Image = Image.FromFile(@"Resources/WHITE_Queen.png");
                            Squares[squarePosition.File, squarePosition.Rank - 1].Image = Image.FromFile(@"Resources/WHITE_Rook.png");
                            Squares[squarePosition.File, squarePosition.Rank - 2].Image = Image.FromFile(@"Resources/WHITE_Bishop.png");
                            Squares[squarePosition.File, squarePosition.Rank - 3].Image = Image.FromFile(@"Resources/WHITE_Knight.png");
                        } else
                        {
                            for (int i = 0; i < 4; i++)
                                Squares[squarePosition.File, squarePosition.Rank + i].BackColor = System.Drawing.Color.Blue;
                            Squares[squarePosition.File, squarePosition.Rank].Image = Image.FromFile(@"Resources/BLACK_Queen.png");
                            Squares[squarePosition.File, squarePosition.Rank + 1].Image = Image.FromFile(@"Resources/BLACK_Rook.png");
                            Squares[squarePosition.File, squarePosition.Rank + 2].Image = Image.FromFile(@"Resources/BLACK_Bishop.png");
                            Squares[squarePosition.File, squarePosition.Rank + 3].Image = Image.FromFile(@"Resources/BLACK_Knight.png");
                        }
                        SelectedPiece = squarePosition;
                    }
                    else
                    {
                        SelectedPiece = null;
                        UpdateBoard();
                    }
                        
                    SelectedValidMoves.Clear();
                    
                }
                else
                {
                    SelectedPiece = null;
                    SelectedValidMoves.Clear();
                }
            } else if (result == MoveResult.AWAITING_PROMOTION)
            {
                if (squarePosition.Rank == 7 || squarePosition.Rank == 0)
                    result = Gamestate.Promote(PieceType.QUEEN);
                else if (squarePosition.Rank == 6 || squarePosition.Rank == 1)
                    result = Gamestate.Promote(PieceType.ROOK);
                else if (squarePosition.Rank == 5 || squarePosition.Rank == 2)
                    result = Gamestate.Promote(PieceType.BISHOP);
                else if (squarePosition.Rank == 4 || squarePosition.Rank == 3)
                    result = Gamestate.Promote(PieceType.KNIGHT);
                if (result == MoveResult.MOVED || result == MoveResult.CHECKMATE)
                    UpdateBoard();
            }
            
        }
       
        public Form1()
        {
            InitializeComponent();
            Gamestate = new Gamestate();
            SelectedValidMoves = new List<Position>();
            result = MoveResult.MOVED;
            Chessboard = new FlowLayoutPanel();
            Chessboard.Size = new Size(320, 320);
            Chessboard.FlowDirection = FlowDirection.BottomUp;
            Squares = new ChessboardSquare[8, 8];
            //Controls.Add(Chessboard);
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    Squares[i, j] = new ChessboardSquare(i, j);
                    if ((i + j) % 2 == 0)
                        Squares[i, j].BackColor = System.Drawing.Color.AliceBlue;
                    else
                        Squares[i, j].BackColor = System.Drawing.Color.LightGreen;
                    Squares[i, j].Size = new Size(40, 40);
                    Squares[i, j].Margin = new Padding(0);
                    Squares[i, j].Click += new EventHandler(ShowValidMoves);
                    Chessboard.Controls.Add(Squares[i, j]);
                }
            
            TextPanel = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 1,
                AutoSize = true
            };
            ToMove = new TextBox
            {
                Text = Gamestate.ToMove.ToString() + " to move",
                BorderStyle = BorderStyle.None,
                BackColor = this.BackColor,
                Enabled = false
            };
            TurnCount = new TextBox
            {
                Text = "Turn " + Gamestate.TurnCount.ToString(),
                BorderStyle = BorderStyle.None,
                BackColor = this.BackColor,
                Enabled = false
            };
            TextPanel.Controls.Add(ToMove, 0, 0);
            TextPanel.Controls.Add(TurnCount, 0, 1);
            //Controls.Add(TextPanel);
            MainLayout = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 2,
                AutoSize = true
            };
            MainLayout.Controls.Add(Chessboard, 0, 0);
            MainLayout.Controls.Add(TextPanel, 1, 0);
            Controls.Add(MainLayout);
            UpdateBoard();
        }
    }
}
