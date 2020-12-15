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

        public Rook(Color color) : base(color) { }
    }
}
