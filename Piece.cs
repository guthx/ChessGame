using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public enum Color { WHITE, BLACK };
    abstract public class Piece
    {
        public Color Color { get; }
        public PieceType Type;
        public char Symbol;
        public List<Position> ValidMoves;
        //public Position Position;

        abstract public bool IsAttackingSquare(Position position, Position square, Piece[,] board);
        abstract public void FindPseudoValidMoves(Piece[,] board, Position position);
        virtual public void FindValidMoves(Gamestate gamestate, Position position)
        {
            FindPseudoValidMoves(gamestate.Board, position);
            var attackingPieces = this.Color == Color.WHITE ? gamestate.blackPiecesPositions : gamestate.whitePiecesPositions;
            var king = this.Color == Color.WHITE ? gamestate.WhiteKing : gamestate.BlackKing;
            var movesToDelete = new List<Position>();
   
            gamestate.Board[position.File, position.Rank] = null;
            foreach (var move in ValidMoves)
            {
                var previousPiece = gamestate.Board[move.File, move.Rank];
                gamestate.Board[move.File, move.Rank] = this;
                foreach(var piece in attackingPieces)
                {
                    if (!(piece.File == move.File && piece.Rank == move.Rank) && gamestate.Board[piece.File, piece.Rank].GetType() != typeof(Pawn))
                    {
                        if (gamestate.Board[piece.File, piece.Rank].IsAttackingSquare(piece, king, gamestate.Board))
                        {
                            movesToDelete.Add(move);
                            break;
                        }
                            
                    }
                }
                gamestate.Board[move.File, move.Rank] = previousPiece;
            }
            gamestate.Board[position.File, position.Rank] = this;
            ValidMoves.RemoveAll(m => movesToDelete.Contains(m));
        }
        public Piece(Color color)
        {
            Color = color;
            ValidMoves = new List<Position>();
        }
    }
}
