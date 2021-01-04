using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public class Rook : Piece
    {
        public override void FindPseudoValidMoves(Piece[,] board, Position position)
        {
            ValidMoves = new List<Position>();

            for(int i=position.File+1; i<8; i++)
            {
                if (board[i, position.Rank] == null)
                {
                    ValidMoves.Add(new Position(i, position.Rank));
                }
                else if (board[i, position.Rank].Color != this.Color)
                {
                    ValidMoves.Add(new Position(i, position.Rank));
                    break;
                }
                else break;
            }
            for (int i = position.File - 1; i >= 0; i--)
            {
                if (board[i, position.Rank] == null)
                {
                    ValidMoves.Add(new Position(i, position.Rank));
                }
                else if (board[i, position.Rank].Color != this.Color)
                {
                    ValidMoves.Add(new Position(i, position.Rank));
                    break;
                }
                else break;
            }
            for (int i = position.Rank + 1; i < 8; i++)
            {
                if (board[position.File, i] == null)
                {
                    ValidMoves.Add(new Position(position.File, i));
                }
                else if (board[position.File, i].Color != this.Color)
                {
                    ValidMoves.Add(new Position(position.File, i));
                    break;
                }
                else break;
            }
            for (int i = position.Rank - 1; i >= 0; i--)
            {
                if (board[position.File, i] == null)
                {
                    ValidMoves.Add(new Position(position.File, i));
                }
                else if (board[position.File, i].Color != this.Color)
                {
                    ValidMoves.Add(new Position(position.File, i));
                    break;
                }
                else break;
            }
        }

        public override bool IsAttackingSquare(Position position, Position square, Piece[,] board)
        {
            int fileOffset = square.File - position.File;
            int rankOffset = square.Rank - position.Rank;
            if (fileOffset == 0 && rankOffset != 0)
            {
                if (rankOffset > 0)
                {
                    for (int i=position.Rank+1; i<square.Rank; i++)
                    {
                        if (board[square.File, i] != null)
                            return false;
                    }
                    return true;
                }
                else
                {
                    for (int i = position.Rank - 1; i > square.Rank; i--)
                    {
                        if (board[square.File, i] != null)
                            return false;
                    }
                    return true;
                }
            }
            else if (fileOffset != 0 && rankOffset == 0)
            {
                if (fileOffset > 0)
                {
                    for (int i = position.File + 1; i < square.File; i++)
                    {
                        if (board[i, square.Rank] != null)
                            return false;
                    }
                    return true;
                }
                else
                {
                    for (int i = position.File - 1; i > square.File; i--)
                    {
                        if (board[i, square.Rank] != null)
                            return false;
                    }
                    return true;
                }
            }
            else return false;
        }

        public Rook(Color color) : base(color)
        {
            Type = PieceType.ROOK;
            if (color == Color.WHITE)
                Symbol = 'R';
            else
                Symbol = 'r';
        }
    }
}
