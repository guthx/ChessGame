using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public class Queen : Piece
    {
        public override void FindPseudoValidMoves(Piece[,] board, Position position)
        {
            ValidMoves = new List<Position>();
            // Queen's valid moves consist of Rook's + Bishop's valid moves
            var rook = new Rook(this.Color);
            var bishop = new Bishop(this.Color);
            rook.FindPseudoValidMoves(board, position);
            bishop.FindPseudoValidMoves(board, position);
            ValidMoves.AddRange(rook.ValidMoves);
            ValidMoves.AddRange(bishop.ValidMoves);
        }

        public Queen(Color color) : base(color) { }
    }
}
