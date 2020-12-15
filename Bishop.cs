using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public class Bishop : Piece
    {
        public override void FindPseudoValidMoves(Piece[,] board, Position position)
        {
            ValidMoves = new List<Position>();
            int f, r;
            //up-right movement
            f = 1;
            r = 1;
            while(position.File + f < 8 && position.Rank + r < 8)
            {
                int nFile = position.File + f;
                int nRank = position.Rank + r;
                if (board[nFile, nRank] == null)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                }
                else if (board[nFile, nRank].Color != this.Color)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                    break;
                }
                else break;
                f++;
                r++;
            }
            //up-left movement
            f = -1;
            r = 1;
            while (position.File + f >= 0 && position.Rank + r < 8)
            {
                int nFile = position.File + f;
                int nRank = position.Rank + r;
                if (board[nFile, nRank] == null)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                }
                else if (board[nFile, nRank].Color != this.Color)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                    break;
                }
                else break;
                f--;
                r++;
            }
            //down-right movement
            f = 1;
            r = -1;
            while (position.File + f < 8 && position.Rank + r >= 0)
            {
                int nFile = position.File + f;
                int nRank = position.Rank + r;
                if (board[nFile, nRank] == null)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                }
                else if (board[nFile, nRank].Color != this.Color)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                    break;
                }
                else break;
                f++;
                r--;
            }
            //down-left movement
            f = -1;
            r = -1;
            while (position.File + f >= 0 && position.Rank + r >= 0)
            {
                int nFile = position.File + f;
                int nRank = position.Rank + r;
                if (board[nFile, nRank] == null)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                }
                else if (board[nFile, nRank].Color != this.Color)
                {
                    ValidMoves.Add(new Position(nFile, nRank));
                    break;
                }
                else break;
                f--;
                r--;
            }
        }

        public Bishop(Color color) : base(color) { }
    }
}
