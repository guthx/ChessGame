using System;
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

        public override void FindValidMoves(Gamestate gamestate, Position position)
        {
            FindPseudoValidMoves(gamestate.Board, position);
            var attackingPieces = this.Color == Color.WHITE ? gamestate.blackPiecesPositions : gamestate.whitePiecesPositions;
            var movesToDelete = new List<Position>();

            gamestate.Board[position.File, position.Rank] = null;
            foreach(var move in ValidMoves)
            {
                var previousPiece = gamestate.Board[move.File, move.Rank];
                gamestate.Board[move.File, move.Rank] = this;
                foreach(var piece in attackingPieces)
                {
                    if(!(piece.File == move.File && piece.Rank == move.Rank))
                    {
                        if (gamestate.Board[piece.File, piece.Rank].IsAttackingSquare(piece, move, gamestate.Board))
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
            CanCastle(gamestate);

           

        }

        public override bool IsAttackingSquare(Position position, Position square, Piece[,] board)
        {
            int fileOffset = position.File - square.File;
            int rankOffset = position.Rank - square.Rank;
            if (fileOffset >= -1 && fileOffset <= 1 && rankOffset >= -1 && rankOffset <= 1)
                return true;
            else return false;
        }

        public void CanCastle(Gamestate gamestate)
        {
            var attackingPieces = this.Color == Color.WHITE ? gamestate.blackPiecesPositions : gamestate.whitePiecesPositions;
            bool areSquaresAttacked(Position[] squares)
            {
                foreach (var piece in attackingPieces)
                {
                    foreach (var square in squares)
                    {
                        if (gamestate.Board[piece.File, piece.Rank].IsAttackingSquare(piece, square, gamestate.Board))
                            return true;
                    }
                }
                return false;
            }
            if (this.Color == Color.WHITE)
            {
                var squares = new Position[3] { new Position('E', 1), new Position('F', 1), new Position('G', 1) };
                if (gamestate.WhiteCanCastleKingside && 
                    gamestate.Board[5, 0] == null && 
                    gamestate.Board[6, 0] == null &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('G', 1));

                squares[1] = new Position('D', 1);
                squares[2] = new Position('C', 1);
                if (gamestate.WhiteCanCastleQueenside &&
                    gamestate.Board[1, 0] == null &&
                    gamestate.Board[2, 0] == null &&
                    gamestate.Board[3, 0] == null &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('C', 1));
            }
            else
            {
                var squares = new Position[3] { new Position('E', 8), new Position('F', 8), new Position('G', 8) };
                if (gamestate.BlackCanCastleKingside &&
                    gamestate.Board[5, 7] == null &&
                    gamestate.Board[6, 7] == null &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('G', 8));

                squares[1] = new Position('D', 8);
                squares[2] = new Position('C', 8);
                if (gamestate.BlackCanCastleQueenside &&
                    gamestate.Board[1, 7] == null &&
                    gamestate.Board[2, 7] == null &&
                    gamestate.Board[3, 7] == null &&
                    !areSquaresAttacked(squares))
                    ValidMoves.Add(new Position('C', 8));
            }

        }
        public King(Color color) : base(color)
        {
            Type = PieceType.KING;
            if (color == Color.WHITE)
                Symbol = 'K';
            else
                Symbol = 'k';
        }
    }
}
