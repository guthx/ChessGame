using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public enum MoveResult { MOVED, NO_PIECE, WRONG_COLOR, INVALID_MOVE, AWAITING_PROMOTION, CHECKMATE };
    public enum PieceType { KNIGHT, BISHOP, ROOK, QUEEN, PAWN, KING }
    public class Gamestate
    {
        public Color ToMove;
        public Piece[,] Board;
        public int TurnCount;
        public Position awaitingPromotion;
        public bool GameOver;
        protected Position WhiteKing;
        protected Position BlackKing;
        protected List<Position> whitePiecesPositions;
        protected List<Position> blackPiecesPositions;
        private bool Checkmate;
        public Gamestate()
        {
            GameOver = false; 
            Checkmate = false;
            TurnCount = 1;
            ToMove = Color.WHITE;
            Board = new Piece[8, 8];
            whitePiecesPositions = new List<Position>();
            blackPiecesPositions = new List<Position>();
            //initialize white side
            for(int f=0; f<8; f++)
            {
                Board[f, 1] = new Pawn(Color.WHITE);
                whitePiecesPositions.Add(new Position(f, 1));
            }
            Board[4, 0] = new King(Color.WHITE);
            whitePiecesPositions.Add(new Position(4, 0));
            Board[0, 0] = new Rook(Color.WHITE);
            whitePiecesPositions.Add(new Position(0, 0));
            Board[7, 0] = new Rook(Color.WHITE);
            whitePiecesPositions.Add(new Position(7, 0));
            Board[1, 0] = new Knight(Color.WHITE);
            whitePiecesPositions.Add(new Position(1, 0));
            Board[6, 0] = new Knight(Color.WHITE);
            whitePiecesPositions.Add(new Position(6, 0));
            Board[2, 0] = new Bishop(Color.WHITE);
            whitePiecesPositions.Add(new Position(2, 0));
            Board[5, 0] = new Bishop(Color.WHITE);
            whitePiecesPositions.Add(new Position(5, 0));
            Board[3, 0] = new Queen(Color.WHITE);
            whitePiecesPositions.Add(new Position(3, 0));
            WhiteKing = new Position(4, 0);
            //initialize black side
            for(int f=0; f<8; f++)
            {
                Board[f, 6] = new Pawn(Color.BLACK);
                blackPiecesPositions.Add(new Position(f, 6));
            }
            Board[4, 7] = new King(Color.BLACK);
            blackPiecesPositions.Add(new Position(4, 7));
            Board[0, 7] = new Rook(Color.BLACK);
            blackPiecesPositions.Add(new Position(0, 7));
            Board[7, 7] = new Rook(Color.BLACK);
            blackPiecesPositions.Add(new Position(7, 7));
            Board[1, 7] = new Knight(Color.BLACK);
            blackPiecesPositions.Add(new Position(1, 7));
            Board[6, 7] = new Knight(Color.BLACK);
            blackPiecesPositions.Add(new Position(6, 7));
            Board[2, 7] = new Bishop(Color.BLACK);
            blackPiecesPositions.Add(new Position(2, 7));
            Board[5, 7] = new Bishop(Color.BLACK);
            blackPiecesPositions.Add(new Position(5, 7));
            Board[3, 7] = new Queen(Color.BLACK);
            blackPiecesPositions.Add(new Position(3, 7));
            BlackKing = new Position(4, 7);
            for(int f=0; f<8; f++)
            {
                for(int r=0; r<8; r++)
                {
                    if (Board[f, r] != null)
                    {
                        Board[f, r].ValidMoves = new List<Position>();
                        if (r == 0 || r == 1)
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount, WhiteKing, blackPiecesPositions);
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
                if (ToMove == Color.WHITE)
                {
                    whitePiecesPositions.Remove(src);
                    whitePiecesPositions.Add(dst);
                    blackPiecesPositions.Remove(dst);
                } else
                {
                    blackPiecesPositions.Remove(src);
                    blackPiecesPositions.Add(dst);
                    whitePiecesPositions.Remove(dst);
                }
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
                        if (ToMove == Color.WHITE)
                        {
                            whitePiecesPositions.Remove(new Position(7, src.Rank));
                            whitePiecesPositions.Add(new Position(5, src.Rank));
                        } else
                        {
                            blackPiecesPositions.Remove(new Position(7, src.Rank));
                            blackPiecesPositions.Add(new Position(5, src.Rank));
                        }
                    }
                    //queenside castle
                    if(dst.File - src.File == -2)
                    {
                        Board[3, src.Rank] = Board[0, src.Rank];
                        Board[0, src.Rank] = null;
                        Board[3, src.Rank].lastMoved = TurnCount;
                        if (ToMove == Color.WHITE)
                        {
                            whitePiecesPositions.Remove(new Position(0, src.Rank));
                            whitePiecesPositions.Add(new Position(3, src.Rank));
                        }
                        else
                        {
                            blackPiecesPositions.Remove(new Position(0, src.Rank));
                            blackPiecesPositions.Add(new Position(3, src.Rank));
                        }
                    }
                    if (piece.Color == Color.WHITE)
                        WhiteKing = dst;
                    else
                        BlackKing = dst;
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
                        if (ToMove == Color.WHITE)
                        {
                            blackPiecesPositions.Remove(new Position(dst.File, src.Rank));
                        } else
                        {
                            whitePiecesPositions.Remove(new Position(dst.File, src.Rank));
                        }
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
                /*
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.WHITE)
                            Board[f, r].FindPseudoValidMoves(Board, new Position(f, r));
                    }
                }
                */
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.BLACK)
                        {
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount, BlackKing, whitePiecesPositions);
                            if (Board[f, r].ValidMoves.Count > 0)
                                Checkmate = false;
                        }

                    }
                }
            }

            else
            {
                ToMove = Color.WHITE;
                /*
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.BLACK)
                            Board[f, r].FindPseudoValidMoves(Board, new Position(f, r));
                    }
                }
                */
                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.WHITE)
                        {
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount, WhiteKing, blackPiecesPositions);
                            if (Board[f, r].ValidMoves.Count > 0)
                                Checkmate = false;
                        }

                    }
                }
            }
            if (Checkmate == false)
                return MoveResult.MOVED;
            else
            {
                GameOver = true;
                return MoveResult.CHECKMATE;
            }
                
        }
    }
}
