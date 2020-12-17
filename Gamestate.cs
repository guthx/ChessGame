using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public enum MoveResult { MOVED, NO_PIECE, WRONG_COLOR, INVALID_MOVE, AWAITING_PROMOTION, CHECKMATE };
    public enum PieceType { KNIGHT, BISHOP, ROOK, QUEEN }
    public class Gamestate
    {
        public Color ToMove;
        public Piece[,] Board;
        public int TurnCount;
        public Position awaitingPromotion;
        private bool Checkmate;
        public Gamestate()
        {
            Checkmate = false;
            TurnCount = 1;
            ToMove = Color.WHITE;
            Board = new Piece[8, 8];
            //initialize white side
            for(int f=0; f<8; f++)
            {
                Board[f, 1] = new Pawn(Color.WHITE);
            }
            Board[4, 0] = new King(Color.WHITE);
            Board[0, 0] = new Rook(Color.WHITE);
            Board[7, 0] = new Rook(Color.WHITE);
            Board[1, 0] = new Knight(Color.WHITE);
            Board[6, 0] = new Knight(Color.WHITE);
            Board[2, 0] = new Bishop(Color.WHITE);
            Board[5, 0] = new Bishop(Color.WHITE);
            Board[3, 0] = new Queen(Color.WHITE);
            //initialize black side
            for(int f=0; f<8; f++)
            {
                Board[f, 6] = new Pawn(Color.BLACK);
            }
            Board[4, 7] = new King(Color.BLACK);
            Board[0, 7] = new Rook(Color.BLACK);
            Board[7, 7] = new Rook(Color.BLACK);
            Board[1, 7] = new Knight(Color.BLACK);
            Board[6, 7] = new Knight(Color.BLACK);
            Board[2, 7] = new Bishop(Color.BLACK);
            Board[5, 7] = new Bishop(Color.BLACK);
            Board[3, 7] = new Queen(Color.BLACK);
            for(int f=0; f<8; f++)
            {
                for(int r=0; r<8; r++)
                {
                    if (Board[f, r] != null)
                    {
                        Board[f, r].ValidMoves = new List<Position>();
                        Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount);
                    }
                        
                }
            }

        }

        public MoveResult Promote(PieceType piece)
        {
            if(awaitingPromotion != null)
            {
                switch (piece)
                {
                    case PieceType.KNIGHT:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Knight(ToMove);
                        break;
                    case PieceType.BISHOP:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Bishop(ToMove);
                        break;
                    case PieceType.ROOK:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Rook(ToMove);
                        break;
                    case PieceType.QUEEN:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Queen(ToMove);
                        break;
                }
                awaitingPromotion = null;
                return SwapPlayer();
            }
            return MoveResult.NO_PIECE;
        } 
        public MoveResult Move(Position src, Position dst)
        {
            var piece = Board[src.File, src.Rank];
            if (piece == null)
                return MoveResult.NO_PIECE;
            else if (piece.Color != ToMove)
                return MoveResult.WRONG_COLOR;
            else if (piece.ValidMoves.Find(p => p.File == dst.File && p.Rank == dst.Rank) == null)
                return MoveResult.INVALID_MOVE;
            else
            {
                Board[dst.File, dst.Rank] = piece;
                Board[src.File, src.Rank] = null;
                piece.lastMoved = TurnCount;
                //check if castling and move rook if necessarry
                if (piece.GetType() == typeof(King))
                {
                    //kingside castle
                    if(dst.File - src.File == 2)
                    {
                        Board[5, src.Rank] = Board[7, src.Rank];
                        Board[7, src.Rank] = null;
                        Board[5, src.Rank].lastMoved = TurnCount;
                    }
                    //queenside castle
                    if(dst.File - src.File == -2)
                    {
                        Board[3, src.Rank] = Board[0, src.Rank];
                        Board[0, src.Rank] = null;
                        Board[3, src.Rank].lastMoved = TurnCount;
                    }
                }
                
                if(piece.GetType() == typeof(Pawn))
                {
                    //check for en-passant
                    int fOffset = dst.File - src.File;
                    if(fOffset != 0 && Board[dst.File, src.Rank] != null &&
                        Board[dst.File, src.Rank].GetType() == typeof(Pawn) &&
                        Board[dst.File, src.Rank].Color != piece.Color &&
                        Board[dst.File, src.Rank].lastMoved == TurnCount - 1 &&
                        ((Pawn)Board[dst.File, src.Rank]).hasDoubleMoved)
                    {
                        Board[dst.File, src.Rank] = null;
                    }

                    //flag pawns that moved two squares
                    if (Math.Abs(src.Rank - dst.Rank) == 2)
                        ((Pawn)piece).hasDoubleMoved = true;
                    else
                        ((Pawn)piece).hasDoubleMoved = false;

                    //check for promotion
                    if(dst.Rank == 0 || dst.Rank == 7)
                    {
                        awaitingPromotion = new Position(dst.File, dst.Rank);
                        return MoveResult.AWAITING_PROMOTION;
                    }
                }

                return SwapPlayer();

                
            }
        }

        private MoveResult SwapPlayer()
        {
            Checkmate = true;
            TurnCount++;
            if (ToMove == Color.WHITE)
            {
                ToMove = Color.BLACK;
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.WHITE)
                            Board[f, r].FindPseudoValidMoves(Board, new Position(f, r));
                    }
                }
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.BLACK)
                        {
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount);
                            if (Board[f, r].ValidMoves.Count > 0)
                                Checkmate = false;
                        }

                    }
                }
            }

            else
            {
                ToMove = Color.WHITE;
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.BLACK)
                            Board[f, r].FindPseudoValidMoves(Board, new Position(f, r));
                    }
                }
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.WHITE)
                        {
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount);
                            if (Board[f, r].ValidMoves.Count > 0)
                                Checkmate = false;
                        }

                    }
                }
            }
            if (Checkmate == false)
                return MoveResult.MOVED;
            else
                return MoveResult.CHECKMATE;
        }
    }
}
