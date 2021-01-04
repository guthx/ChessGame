﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public class King : Piece
    {
        public override void FindPseudoValidMoves(Piece[,] board, Position position)
        {
            ValidMoves = new List<Position>();

            for(int f=-1; f<=1; f++)
            {
                for(int r=-1; r<=1; r++)
                {
                    if (!(f == 0 && r == 0) && position.File + f >= 0 && position.File + f <= 7 &&
                        position.Rank + r >= 0 && position.Rank + r <= 7 &&
                        (board[position.File + f, position.Rank + r] == null || board[position.File + f, position.Rank + r].Color != this.Color))
                        ValidMoves.Add(new Position(position.File + f, position.Rank + r));
                }
            }
        }

        public override void FindValidMoves(Piece[,] board, Position position, int turnCount, Position king, List<Position> attackingPieces)
        {
            FindPseudoValidMoves(board, position);
       
            var movesToDelete = new List<Position>();

            board[position.File, position.Rank] = null;
            foreach(var move in ValidMoves)
            {
                var previousPiece = board[move.File, move.Rank];
                board[move.File, move.Rank] = this;
                foreach(var piece in attackingPieces)
                {
                    if(!(piece.File == move.File && piece.Rank == move.Rank))
                    {
                        if (board[piece.File, piece.Rank].IsAttackingSquare(piece, move, board))
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

            /* check for castling
             * rules: 
             * king moves 2 squares left or right and rook is moved to the square that king crossed
             * neither the king nor rook has moved this game
             * no pieces between king and rook
             * king is not in check
             * king does not pass by square attacked by enemy piece
             * king does not end up in check
            */
            /*
            bool isChecked()
            {
                foreach(var piece in enemyPieces)
                {
                    board[piece.File, piece.Rank].FindPseudoValidMoves(board, piece);
                    if (board[piece.File, piece.Rank].ValidMoves.Find(m => m.File == position.File && m.Rank == position.Rank) != null)
                        return true;
                }
                return false;
            }
            */
            bool areSquaresAttacked(Position[] squares)
            {
                foreach(var piece in attackingPieces)
                {
                    board[piece.File, piece.Rank].FindPseudoValidMoves(board, piece);
                    foreach(var square in squares)
                    {
                        if (board[piece.File, piece.Rank].ValidMoves.Find(m => m.File == square.File && m.Rank == square.Rank) != null)
                            return true;
                    }
                }
                return false;
            }
            if (lastMoved == 0)
            {
                //white king kingside castle
                var squares = new Position[3] { new Position('E', 1), new Position('F', 1), new Position('G', 1) };
                if (this.Color == Color.WHITE && board[5, 0] == null && board[6, 0] == null &&
                    board[7, 0] != null && board[7, 0].GetType() == typeof(Rook) &&
                    board[7, 0].Color == Color.WHITE && board[7, 0].lastMoved == 0 &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('G', 1));
                //white king queenside castle
                squares[1] = new Position('D', 1);
                squares[2] = new Position('C', 1);
                if (this.Color == Color.WHITE && board[1, 0] == null && board[2, 0] == null && board[3, 0] == null &&
                    board[0, 0] != null && board[0, 0].GetType() == typeof(Rook) &&
                    board[0, 0].Color == Color.WHITE && board[0, 0].lastMoved == 0 &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('C', 1));
                //black king kingside castle
                squares[0] = new Position('E', 8);
                squares[1] = new Position('F', 8); 
                squares[2] = new Position('G', 8);
                if (this.Color == Color.BLACK && board[5, 7] == null && board[6, 7] == null &&
                    board[7, 7] != null && board[7, 7].GetType() == typeof(Rook) &&
                    board[7, 7].Color == Color.BLACK && board[7, 7].lastMoved == 0 &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('G', 8));
                //black king queenside castle
                squares[1] = new Position('D', 8);
                squares[2] = new Position('C', 8);
                if (this.Color == Color.BLACK && board[1, 7] == null && board[2, 7] == null && board[3, 7] == null &&
                    board[0, 7] != null && board[0, 7].GetType() == typeof(Rook) &&
                    board[0, 7].Color == Color.BLACK && board[0, 7].lastMoved == 0 &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('C', 8));

            }

        }

        public override bool IsAttackingSquare(Position position, Position square, Piece[,] board)
        {
            int fileOffset = position.File - square.File;
            int rankOffset = position.Rank - square.Rank;
            if (fileOffset >= -1 && fileOffset <= 1 && rankOffset >= -1 && rankOffset <= 1)
                return true;
            else return false;
        }
        public King(Color color) : base(color) { }
    }
}
