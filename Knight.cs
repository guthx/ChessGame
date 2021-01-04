using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public class Knight : Piece
    {
        private int[,] moveOffsets;
        public override void FindPseudoValidMoves(Piece[,] board, Position position)
        {
            ValidMoves = new List<Position>();

            

            for(int i=0; i<8; i++)
            {
                int newFile = position.File + moveOffsets[i, 0];
                int newRank = position.Rank + moveOffsets[i, 1];
                if (newFile >= 0 && newFile < 8 && newRank >= 0 && newRank < 8 &&
                    (board[newFile, newRank] == null || board[newFile, newRank].Color != this.Color))
                    ValidMoves.Add(new Position(newFile, newRank));
            }
        }

        public override bool IsAttackingSquare(Position position, Position square, Piece[,] board)
        {
            int absFileOffset = Math.Abs(position.File - square.File);
            int absRankOffset = Math.Abs(position.File - square.File);
            if ((absFileOffset == 2 && absRankOffset == 1) || (absFileOffset == 1 && absRankOffset == 2))
                return true;
            else return false;
        }

        public Knight(Color color) : base(color) 
        {
            moveOffsets = new int[8, 2] {
                { -2, -1 },
                { -2, 1 },
                { -1, -2 },
                { -1, 2 },
                { 1, -2 },
                { 1, 2 },
                { 2, -1 },
                { 2, 1 }
            };
        }
    }
}
