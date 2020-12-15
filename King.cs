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

        public override void FindValidMoves(Piece[,] board, Position position)
        {
            FindPseudoValidMoves(board, position);
            var enemyPieces = new List<Position>();
            for(int f=0; f<8; f++)
            {
                for(int r=0; r<8; r++)
                {
                    if (board[f, r] != null && board[f, r].Color != this.Color)
                        enemyPieces.Add(new Position(f, r));
                }
            }
            var movesToDelete = new List<Position>();
            foreach(var move in ValidMoves)
            {
                var newBoard = (Piece[,])board.Clone();
                newBoard[move.File, move.Rank] = this;
                newBoard[position.File, position.Rank] = null;
                foreach(var piece in enemyPieces)
                {
                    if(!(piece.File == move.File && piece.Rank == move.Rank))
                    {
                        newBoard[piece.File, piece.Rank].FindPseudoValidMoves(newBoard, piece);
                        if (newBoard[piece.File, piece.Rank].ValidMoves.Find(m => m.File == move.File && m.Rank == move.Rank) != null)
                        {
                            if(movesToDelete.Find(m => m.File == move.File && m.Rank == move.Rank) == null)
                                movesToDelete.Add(move);
                        }
                            
                    }
                }
            }
            ValidMoves.RemoveAll(m => movesToDelete.Contains(m));

        }
        public King(Color color) : base(color) { }
    }
}
