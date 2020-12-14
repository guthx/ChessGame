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
        //public Position Position;
        abstract public void FindPseudoValidMoves(Piece[,] board, Position position);
        abstract public void FindValidMoves(Piece[,] board, Position position);
        public Piece(Color color)
        {
            Color = color;
            ValidMoves = new List<Position>();
        }
    }
}
