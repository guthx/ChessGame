using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ChessGame
{
    public enum GameResult { WHITE_WIN, BLACK_WIN, STALEMATE, DRAW, ACTIVE }
    public enum MoveResult { MOVED, NO_PIECE, WRONG_COLOR, INVALID_MOVE, AWAITING_PROMOTION, GAME_OVER };
    public enum PieceType { KNIGHT, BISHOP, ROOK, QUEEN, PAWN, KING }
    public class Gamestate
    {
        public Color ToMove;
        public Piece[,] Board;
        public int TurnCount;
        public Position awaitingPromotion;
        public bool GameOver;
        public GameResult GameResult;
        public Stopwatch WhiteStopwatch;
        public Stopwatch BlackStopwatch;
        protected Position WhiteKing;
        protected Position BlackKing;
        protected List<Position> whitePiecesPositions;
        protected List<Position> blackPiecesPositions;
        public bool WhiteCanCastleQueenside;
        public bool WhiteCanCastleKingside;
        public bool BlackCanCastleQueenside;
        public bool BlackCanCastleKingside;
        public Position EnPassantPosition;
        public int HalfMoveCount;
        public List<string> PositionHistory;
        private Dictionary<string, int> positionRepetitionCount;
        private bool Checkmate;
        public Gamestate()
        {
            HalfMoveCount = 0;
            GameResult = GameResult.ACTIVE;
            GameOver = false; 
            Checkmate = false;
            TurnCount = 1;
            ToMove = Color.WHITE;
            Board = new Piece[8, 8];
            WhiteStopwatch = new Stopwatch();
            BlackStopwatch = new Stopwatch();
            whitePiecesPositions = new List<Position>();
            blackPiecesPositions = new List<Position>();
            PositionHistory = new List<string>();
            positionRepetitionCount = new Dictionary<string, int>();
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
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount, WhiteKing, blackPiecesPositions, ref EnPassantPosition);
                    }
                        
                }
            }
            string fen = FEN.GamstateToFEN(this);
            PositionHistory.Add(fen);
            positionRepetitionCount.Add(fen, 1);
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
            if (GameOver)
                return MoveResult.GAME_OVER;

            var piece = Board[src.File, src.Rank];
            if (piece == null)
                return MoveResult.NO_PIECE;
            else if (piece.Color != ToMove)
                return MoveResult.WRONG_COLOR;
            else if (piece.ValidMoves.Find(p => p.File == dst.File && p.Rank == dst.Rank) == null)
                return MoveResult.INVALID_MOVE;
            else
            {
                
                if (Board[dst.File, dst.Rank] != null)
                    HalfMoveCount = 0;
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
                    /*
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
                    */
                    if (dst.Equals(EnPassantPosition))
                    {
                        if (ToMove == Color.WHITE)
                        {
                            Board[dst.File, dst.Rank - 1] = null;
                            blackPiecesPositions.Remove(new Position(dst.File, dst.Rank - 1));
                        }
                        else
                        {
                            Board[dst.File, dst.Rank + 1] = null;
                            whitePiecesPositions.Remove(new Position(dst.File, dst.Rank + 1));
                        }
                        HalfMoveCount = 0;
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
            EnPassantPosition = null;
            Checkmate = true;
            TurnCount++;
            HalfMoveCount++;
            if (ToMove == Color.WHITE)
            {
                WhiteStopwatch.Stop();
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
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount, BlackKing, whitePiecesPositions, ref EnPassantPosition);
                            if (Board[f, r].ValidMoves.Count > 0)
                                Checkmate = false;
                        }

                    }
                }
                King whiteKing = (King)Board[WhiteKing.File, WhiteKing.Rank];
                bool k, q;
                (k, q) = whiteKing.CanCastle(WhiteKing, Board, blackPiecesPositions);
                WhiteCanCastleKingside = k;
                WhiteCanCastleQueenside = q;
                BlackStopwatch.Restart();
            }

            else
            {
                BlackStopwatch.Stop();
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
                            Board[f, r].FindValidMoves(Board, new Position(f, r), TurnCount, WhiteKing, blackPiecesPositions, ref EnPassantPosition);
                            if (Board[f, r].ValidMoves.Count > 0)
                                Checkmate = false;
                        }

                    }
                }
                King blackKing = (King)Board[BlackKing.File, BlackKing.Rank];
                bool k, q;
                (k, q) = blackKing.CanCastle(BlackKing, Board, whitePiecesPositions);
                BlackCanCastleKingside = k;
                BlackCanCastleQueenside = q;
                WhiteStopwatch.Restart();
            }
            string fen = FEN.GamstateToFEN(this);
            PositionHistory.Add(fen);
            if (positionRepetitionCount.ContainsKey(fen))
                positionRepetitionCount[fen] += 1;
            else
                positionRepetitionCount[fen] = 1;
            if (Checkmate == false)
            {
                if (positionRepetitionCount[fen] == 3 || HalfMoveCount >= 50)
                {
                    GameOver = true;
                    GameResult = GameResult.DRAW;
                    return MoveResult.GAME_OVER;
                }
                else 
                    return MoveResult.MOVED;
            }
                
            else
            {
                bool stalemate = true;
                if (ToMove == Color.BLACK)
                {
                    foreach(var piece in whitePiecesPositions)
                    {
                        if (Board[piece.File, piece.Rank].IsAttackingSquare(piece, BlackKing, Board))
                        {
                            stalemate = false;
                            break;
                        }
                    }
                } else
                {
                    foreach (var piece in blackPiecesPositions)
                    {
                        if (Board[piece.File, piece.Rank].IsAttackingSquare(piece, WhiteKing, Board))
                        {
                            stalemate = false;
                            break;
                        }
                    }
                }
                GameOver = true;
                if (stalemate)
                    GameResult = GameResult.STALEMATE;
                else if (ToMove == Color.BLACK)
                    GameResult = GameResult.WHITE_WIN;
                else
                    GameResult = GameResult.BLACK_WIN;
                    
                return MoveResult.GAME_OVER;
            }
                
        }
    }
}
