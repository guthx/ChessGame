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

        public Gamestate()
        {
            ToMove = Color.WHITE;
            Board = new Piece[8, 8];
            //initialize white side
            for(int f=0; f<8; f++)
            {
                Board[f, 1] = new Pawn(Color.WHITE);
            }
            Board[4, 0] = new King(Color.WHITE);

            //initialize black side
            for(int f=0; f<8; f++)
            {
                Board[f, 6] = new Pawn(Color.BLACK);
            }
            Board[4, 7] = new King(Color.BLACK);

            for(int f=0; f<8; f++)
            {
                for(int r=0; r<8; r++)
                {
                    if (Board[f, r] != null)
                    {
                        Board[f, r].ValidMoves = new List<Position>();
                        Board[f, r].FindValidMoves(Board, new Position(f, r));
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
                                Board[f, r].FindValidMoves(Board, new Position(f, r));
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
                                Board[f, r].FindValidMoves(Board, new Position(f, r));
                        }
                    }
                }
                    
                return MoveResult.MOVED;
            }
        }
    }
}
