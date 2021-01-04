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
        public int lastMoved;
        //public Position Position;

        abstract public bool IsAttackingSquare(Position position, Position square, Piece[,] board);
        abstract public void FindPseudoValidMoves(Piece[,] board, Position position);
        virtual public void FindValidMoves(Piece[,] board, Position position, int turnCount, Position king, List<Position> attackingPieces)
        {
            FindPseudoValidMoves(board, position);
            // verify which enemy pieces attack your piece (before moving)
            /*
            List<Position> attackingPieces = new List<Position>();
            for (int f = 0; f < 8; f++)
            {
                for (int r = 0; r < 8; r++)
                {
                    if (board[f, r] != null && board[f, r].Color != this.Color)
                    {
                        
                        foreach (var move in board[f, r].ValidMoves)
                        {
                            if (position.File == move.File && position.Rank == move.Rank)
                            {
                                attackingPieces.Add(new Position(f, r));
                                break;
                            }
                        }
                        attackingPieces.Add(new Position(f, r));
                    }
                }
            }
            */
            var movesToDelete = new List<Position>();
            /*
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
            */
            board[position.File, position.Rank] = null;
            foreach (var move in ValidMoves)
            {
                var previousPiece = board[move.File, move.Rank];
                board[move.File, move.Rank] = this;
                foreach(var piece in attackingPieces)
                {
                    if (!(piece.File == move.File && piece.Rank == move.Rank) && board[piece.File, piece.Rank].GetType() != typeof(Pawn))
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
        public Piece(Color color)
        {
            Color = color;
            ValidMoves = new List<Position>();
            lastMoved = 0;
        }
    }
}
