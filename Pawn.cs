using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChessGame
{
    public class Pawn : Piece
    {
        public bool hasDoubleMoved;
        public override void FindPseudoValidMoves(Piece[,] board, Position position)
        {
            ValidMoves = new List<Position>();
            // List of pseudo-legal moves (no verification if moves leave king in check)
            if (Color == Color.WHITE)
            {
                if (position.Rank == 1)
                {
                    if (board[position.File, 2] == null)
                    {
                        ValidMoves.Add(new Position(position.File, 2));
                        if (board[position.File, 3] == null)
                            ValidMoves.Add(new Position(position.File, 3));
                    }
                }
                else
                {
                    if (board[position.File, position.Rank + 1] == null)
                        ValidMoves.Add(new Position(position.File, position.Rank + 1));
                }
                if (position.File > 0 && board[position.File - 1, position.Rank + 1] != null
                    && board[position.File - 1, position.Rank + 1].Color == Color.BLACK)
                    ValidMoves.Add(new Position(position.File - 1, position.Rank + 1));
                if (position.File < 7 && board[position.File + 1, position.Rank + 1] != null
                    && board[position.File + 1, position.Rank + 1].Color == Color.BLACK)
                    ValidMoves.Add(new Position(position.File + 1, position.Rank + 1));
            }
            else
            {
                if (position.Rank == 6)
                {
                    if (board[position.File, 5] == null)
                    {
                        ValidMoves.Add(new Position(position.File, 5));
                        if (board[position.File, 4] == null)
                            ValidMoves.Add(new Position(position.File, 4));
                    }
                }
                else
                {
                    if (board[position.File, position.Rank - 1] == null)
                        ValidMoves.Add(new Position(position.File, position.Rank - 1));
                }
                if (position.File > 0 && board[position.File - 1, position.Rank - 1] != null
                    && board[position.File - 1, position.Rank - 1].Color == Color.WHITE)
                    ValidMoves.Add(new Position(position.File - 1, position.Rank - 1));
                if (position.File < 7 && board[position.File + 1, position.Rank - 1] != null
                    && board[position.File + 1, position.Rank - 1].Color == Color.WHITE)
                    ValidMoves.Add(new Position(position.File + 1, position.Rank - 1));
            }


        }
        
        public override void FindValidMoves(Piece[,] board, Position position, int turnCount)
        {
            FindPseudoValidMoves(board, position);
            //check for en passant
            //white pawn
            if (this.Color == Color.WHITE && position.Rank == 4)
            {
                //take left-side pawn
                if (position.File > 0 && board[position.File - 1, position.Rank] != null &&
                    board[position.File - 1, position.Rank].GetType() == typeof(Pawn) &&
                    board[position.File - 1, position.Rank].Color == Color.BLACK &&
                    board[position.File - 1, position.Rank].lastMoved == turnCount - 1)
                    ValidMoves.Add(new Position(position.File - 1, position.Rank + 1));
                //take right-side pawn
                if (position.File < 7 && board[position.File + 1, position.Rank] != null &&
                    board[position.File + 1, position.Rank].GetType() == typeof(Pawn) &&
                    board[position.File + 1, position.Rank].Color == Color.BLACK &&
                    board[position.File + 1, position.Rank].lastMoved == turnCount - 1)
                    ValidMoves.Add(new Position(position.File + 1, position.Rank + 1));
            }
            //black pawn
            if (this.Color == Color.BLACK && position.Rank == 3)
            {
                //take left-side pawn
                if (position.File > 0 && board[position.File - 1, position.Rank] != null &&
                    board[position.File - 1, position.Rank].GetType() == typeof(Pawn) &&
                    board[position.File - 1, position.Rank].Color == Color.WHITE &&
                    board[position.File - 1, position.Rank].lastMoved == turnCount - 1)
                    ValidMoves.Add(new Position(position.File - 1, position.Rank - 1));
                //take right-side pawn
                if (position.File < 7 && board[position.File + 1, position.Rank] != null &&
                    board[position.File + 1, position.Rank].GetType() == typeof(Pawn) &&
                    board[position.File + 1, position.Rank].Color == Color.WHITE &&
                    board[position.File + 1, position.Rank].lastMoved == turnCount - 1)
                    ValidMoves.Add(new Position(position.File + 1, position.Rank - 1));
            }

            
            // verify which enemy pieces attack your piece (before moving)
            List<Position> attackingPieces = new List<Position>();
            for (int f = 0; f < 8; f++)
            {
                for (int r = 0; r < 8; r++)
                {
                    if (board[f, r] != null && board[f, r].Color != this.Color)
                    {
                        /*
                        foreach (var move in board[f, r].ValidMoves)
                        {
                            if (position.File == move.File && position.Rank == move.Rank)
                            {
                                attackingPieces.Add(new Position(f, r));
                                break;
                            }
                        }*/
                        attackingPieces.Add(new Position(f, r));
                    }
                }
            }
            Position findKing()
            {
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (board[f, r] != null && board[f, r].GetType() == typeof(King) && board[f, r].Color == this.Color)
                        {
                            return new Position(f, r);
                        }
                    }
                }
                return null;
            }
            Position king = findKing();
            var movesToDelete = new List<Position>();
            foreach (var move in ValidMoves)
            {
                var newBoard = (Piece[,])board.Clone();
                newBoard[move.File, move.Rank] = this;
                newBoard[position.File, position.Rank] = null;
                foreach (var piece in attackingPieces)
                {
                    if (!(piece.File == move.File && piece.Rank == move.Rank))
                    {
                        newBoard[piece.File, piece.Rank].FindPseudoValidMoves(newBoard, piece);
                        foreach (var attack in newBoard[piece.File, piece.Rank].ValidMoves)
                        {
                            if (attack.File == king.File && attack.Rank == king.Rank)
                                if (!movesToDelete.Contains(move))
                                    movesToDelete.Add(move);
                        }
                        //board[piece.File, piece.Rank].FindPseudoValidMoves(board, piece);
                    }
                }
            }
            ValidMoves.RemoveAll(m => movesToDelete.Contains(m));
        }
        
        public Pawn(Color color):base(color) 
        {
            hasDoubleMoved = false;
        }
    }
}
