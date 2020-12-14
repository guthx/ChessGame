using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public class King : Piece
    {
        public override void FindPseudoValidMoves(Piece[,] board, Position position)
        {
            throw new NotImplementedException();
        }

        public override void FindValidMoves(Piece[,] board, Position position)
        {
            throw new NotImplementedException();
        }

        public override bool IsMoveValid(Position src, Position dst, Piece dstPiece)
        {
            throw new NotImplementedException();
        }
        public King(Color color) : base(color) { }
    }
}
