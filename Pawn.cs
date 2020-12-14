using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChessGame
{
    public class Pawn : Piece
    {
        public override bool IsMoveValid(Position src, Position dst, Piece dstPiece)
        {
            if(Color == Color.WHITE)
            {
                if (src.File == dst.File)
                {
                    if (src.Rank == 1 && (dst.Rank == 2 || dst.Rank == 3) && dstPiece == null)
                        return true;
                    else if (src.Rank != 1 && dst.Rank - src.Rank == 1 && dstPiece == null)
                        return true;
                    else return false;
                }
                else if ((dst.File - src.File == 1 || dst.File - src.File == -1)
                  && dst.Rank - src.Rank == 1 && dstPiece != null)
                    return true;
                else return false;
            } else
            {
                if (src.File == dst.File)
                {
                    if (src.Rank == 6 && (dst.Rank == 5 || dst.Rank == 4) && dstPiece == null)
                        return true;
                    else if (src.Rank != 6 && dst.Rank - src.Rank == -1 && dstPiece == null)
                        return true;
                    else return false;
                }
                else if ((dst.File - src.File == 1 || dst.File - src.File == -1)
                  && dst.Rank - src.Rank == -1 && dstPiece != null)
                    return true;
                else return false;
            }
        }
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
                    && board[position.File - 1, position.Rank - 1].Color == Color.BLACK)
                    ValidMoves.Add(new Position(position.File - 1, position.Rank - 1));
                if (position.File < 7 && board[position.File + 1, position.Rank - 1] != null
                    && board[position.File + 1, position.Rank - 1].Color == Color.BLACK)
                    ValidMoves.Add(new Position(position.File + 1, position.Rank - 1));
            }
        }
        public override void FindValidMoves(Piece[,] board, Position position)
        {
            FindPseudoValidMoves(board, position);
            // verify which enemy pieces attack your piece (before moving)
            List<Position> attackingPieces = new List<Position>();
            for (int f = 0; f<8; f++)
            {
                for(int r=0; r<8; r++)
                {
                    if(board[f, r] != null && board[f, r].Color != this.Color)
                    {
                        foreach(var move in board[f, r].ValidMoves)
                        {
                            if(position.File == move.File && position.Rank == move.Rank)
                            {
                                attackingPieces.Add(new Position(f, r));
                                break;
                            }
                        }
                    }
                }
            }
            Position findKing()
            {
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (board[f, r].GetType() == typeof(King) && board[f, r].Color == this.Color)
                        {
                            return new Position(f, r);
                        }
                    }
                }
                return null;
            }
            Position king = findKing();
           
            foreach(var move in ValidMoves)
            {
                var newBoard = (Piece[,])board.Clone();
                newBoard[move.File, move.Rank] = this;
                foreach(var piece in attackingPieces)
                {
                    newBoard[piece.File, piece.Rank].FindPseudoValidMoves(newBoard, piece);
                    foreach(var attack in newBoard[piece.File, piece.Rank].ValidMoves)
                    {
                        if (attack.File == king.File && attack.Rank == king.Rank)
                            ValidMoves.Remove(move);
                    }
                }
            }
        }
        public Pawn(Color color):base(color) { }
    }
}
