using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public enum Color { WHITE, BLACK };
    abstract public class Piece
    {
        public Color Color { get; }
        public List<Position> ValidMoves;
        public bool hasMoved;
        //public Position Position;
        abstract public void FindPseudoValidMoves(Piece[,] board, Position position);
        virtual public void FindValidMoves(Piece[,] board, Position position)
        {
            FindPseudoValidMoves(board, position);
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
        public Piece(Color color)
        {
            Color = color;
            ValidMoves = new List<Position>();
            hasMoved = false;
        }
    }
}
