using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public enum MoveResult { MOVED, NO_PIECE, WRONG_COLOR, INVALID_MOVE };
    public class Gamestate
    {
        public Color ToMove;
        public Piece[,] Board;
        public int TurnCount;

        public Gamestate()
        {
            TurnCount = 1;
            ToMove = Color.WHITE;
            Board = new Piece[8, 8];
            //initialize white side
            for(int f=0; f<8; f++)
            {
                Board[f, 1] = new Pawn(Color.WHITE);
            }
            Board[4, 0] = new King(Color.WHITE);
            Board[0, 0] = new Rook(Color.WHITE);
            Board[7, 0] = new Rook(Color.WHITE);
            Board[1, 0] = new Knight(Color.WHITE);
            Board[6, 0] = new Knight(Color.WHITE);
            Board[2, 0] = new Bishop(Color.WHITE);
            Board[5, 0] = new Bishop(Color.WHITE);
            Board[3, 0] = new Queen(Color.WHITE);
            //initialize black side
            for(int f=0; f<8; f++)
            {
                Board[f, 6] = new Pawn(Color.BLACK);
            }
            Board[4, 7] = new King(Color.BLACK);
            Board[0, 7] = new Rook(Color.BLACK);
            Board[7, 7] = new Rook(Color.BLACK);
            Board[1, 7] = new Knight(Color.BLACK);
            Board[6, 7] = new Knight(Color.BLACK);
            Board[2, 7] = new Bishop(Color.BLACK);
            Board[5, 7] = new Bishop(Color.BLACK);
            Board[3, 7] = new Queen(Color.BLACK);
            for(int f=0; f<8; f++)
            {
                for(int r=0; r<8; r++)
                {
                    if (Board[f, r] != null)
                    {
                        Board[f, r].ValidMoves = new List<Position>();
                        Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount);
                    }
                        
                }
            }

        }

        public MoveResult Move(Position src, Position dst)
        {
            var piece = Board[src.File, src.Rank];
            if (piece == null)
                return MoveResult.NO_PIECE;
            else if (piece.Color != ToMove)
                return MoveResult.WRONG_COLOR;
            else if (piece.ValidMoves.Find(p => p.File == dst.File && p.Rank == dst.Rank) == null)
                return MoveResult.INVALID_MOVE;
            else
            {
                Board[dst.File, dst.Rank] = piece;
                Board[src.File, src.Rank] = null;
                piece.lastMoved = TurnCount;
                //check if castling and move rook if necessarry
                if (piece.GetType() == typeof(King))
                {
                    //kingside castle
                    if(dst.File - src.File == 2)
                    {
                        Board[5, src.Rank] = Board[7, src.Rank];
                        Board[7, src.Rank] = null;
                        Board[5, src.Rank].lastMoved = TurnCount;
                    }
                    //queenside castle
                    if(dst.File - src.File == -2)
                    {
                        Board[3, src.Rank] = Board[0, src.Rank];
                        Board[0, src.Rank] = null;
                        Board[3, src.Rank].lastMoved = TurnCount;
                    }
                }
                //check for en-passant
                if(piece.GetType() == typeof(Pawn))
                {
                    int fOffset = dst.File - src.File;
                    if(fOffset != 0 && Board[dst.File, src.Rank] != null &&
                        Board[dst.File, src.Rank].GetType() == typeof(Pawn) &&
                        Board[dst.File, src.Rank].Color != piece.Color &&
                        Board[dst.File, src.Rank].lastMoved == TurnCount - 1 &&
                        ((Pawn)Board[dst.File, src.Rank]).hasDoubleMoved)
                    {
                        Board[dst.File, src.Rank] = null;
                    }
                }

                //flag pawn for doublemove
                if(piece.GetType() == typeof(Pawn))
                {
                    if (Math.Abs(src.Rank - dst.Rank) == 2)
                        ((Pawn)piece).hasDoubleMoved = true;
                    else
                        ((Pawn)piece).hasDoubleMoved = false;
                }
                TurnCount++;
                if (ToMove == Color.WHITE)
                {
                    ToMove = Color.BLACK;
                    for (int f = 0; f < 8; f++)
                    {
                        for (int r = 0; r < 8; r++)
                        {
                            if (Board[f, r] != null && Board[f, r].Color == Color.WHITE)
                                Board[f, r].FindPseudoValidMoves(Board, new Position(f, r));
                        }
                    }
                    for (int f = 0; f < 8; f++)
                    {
                        for (int r = 0; r < 8; r++)
                        {
                            if (Board[f, r] != null && Board[f, r].Color == Color.BLACK)
                                Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount);
                        }
                    }
                }

                else
                {
                    ToMove = Color.WHITE;
                    for (int f = 0; f < 8; f++)
                    {
                        for (int r = 0; r < 8; r++)
                        {
                            if (Board[f, r] != null && Board[f, r].Color == Color.BLACK)
                                Board[f, r].FindPseudoValidMoves(Board, new Position(f, r));
                        }
                    }
                    for (int f = 0; f < 8; f++)
                    {
                        for (int r = 0; r < 8; r++)
                        {
                            if (Board[f, r] != null && Board[f, r].Color == Color.WHITE)
                                Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount);
                        }
                    }
                }
                
                return MoveResult.MOVED;
            }
        }
    }
}
