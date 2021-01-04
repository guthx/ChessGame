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
        
        public override void FindValidMoves(Piece[,] board, Position position, int turnCount, Position king, List<Position> attackingPieces)
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
   
            var movesToDelete = new List<Position>();
            board[position.File, position.Rank] = null;
            foreach (var move in ValidMoves)
            {
                var previousPiece = board[move.File, move.Rank];
                board[move.File, move.Rank] = this;
                foreach (var piece in attackingPieces)
                {
                    if (!(piece.File == move.File && piece.Rank == move.Rank))
                    {
                        if (board[piece.File, piece.Rank].IsAttackingSquare(piece, king, board))
                        {
                            movesToDelete.Add(move);
                            break;
                        }

                    }
                }
                board[move.File, move.Rank] = previousPiece;
            }
            board[position.File, position.Rank] = this;
            ValidMoves.RemoveAll(m => movesToDelete.Contains(m));
        }

        public override bool IsAttackingSquare(Position position, Position square, Piece[,] board)
        {
            if (board[position.File, position.Rank].Color == Color.WHITE)
            {
                int fileOffset = position.File - square.File;
                if ((fileOffset == -1 || fileOffset == 1) && square.Rank - position.Rank == 1)
                    return true;
                else return false;
            } else
            {
                int fileOffset = position.File - square.File;
                if ((fileOffset == -1 || fileOffset == 1) && square.Rank - position.Rank == -1)
                    return true;
                else return false;
            }
        }

        public Pawn(Color color):base(color) 
        {
            hasDoubleMoved = false;
        }
    }
}
