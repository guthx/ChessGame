using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public List<string> PositionHistory;
        public (Position, Position) LastMove;
        public Position WhiteKing;
        public Position BlackKing;
        public List<Position> whitePiecesPositions;
        public List<Position> blackPiecesPositions;
        public bool WhiteCanCastleQueenside;
        public bool WhiteCanCastleKingside;
        public bool BlackCanCastleQueenside;
        public bool BlackCanCastleKingside;
        public Position EnPassantPosition;
        private int HalfMoveCount;
        private Dictionary<string, int> positionRepetitionCount;

        public Gamestate()
        {
            HalfMoveCount = 0;
            GameResult = GameResult.ACTIVE;
            GameOver = false; 
            TurnCount = 1;
            ToMove = Color.WHITE;
            Board = new Piece[8, 8];
            WhiteStopwatch = new Stopwatch();
            BlackStopwatch = new Stopwatch();
            whitePiecesPositions = new List<Position>();
            blackPiecesPositions = new List<Position>();
            PositionHistory = new List<string>();
            positionRepetitionCount = new Dictionary<string, int>();
            WhiteCanCastleKingside = true;
            WhiteCanCastleQueenside = true;
            BlackCanCastleKingside = true;
            BlackCanCastleQueenside = true;
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
                            Board[f, r].FindValidMoves(this, new Position(f, r));
                    }
                        
                }
            }
            string fen = GamstateToFEN(this);
            PositionHistory.Add(fen);
            positionRepetitionCount.Add(String.Join(" ", fen.Split(' ').Take(4)), 1);
        }

        public Gamestate(string fen, Gamestate gamestate = null)
        {
            GameResult = GameResult.ACTIVE;
            GameOver = false;
            Board = new Piece[8, 8];
            WhiteStopwatch = new Stopwatch();
            BlackStopwatch = new Stopwatch();
            positionRepetitionCount = new Dictionary<string, int>();
            if (gamestate == null)
            {
                PositionHistory = new List<string>();
                PositionHistory.Add(fen);
            }
            else
            {
                int halfTurnCount = gamestate.TurnCount * 2;
                if (gamestate.ToMove == Color.WHITE)
                    halfTurnCount--;
                PositionHistory = gamestate.PositionHistory.Take(halfTurnCount).ToList();
                foreach(var position in PositionHistory)
                {
                    string fenNoMoveCount = String.Join(" ", position.Split(' ').Take(4));
                    if (positionRepetitionCount.ContainsKey(fenNoMoveCount))
                        positionRepetitionCount[fenNoMoveCount] += 1;
                    else
                        positionRepetitionCount[fenNoMoveCount] = 1;
                }
            }
            whitePiecesPositions = new List<Position>();
            blackPiecesPositions = new List<Position>();
            
            var splitFen = fen.Split(' ');
            int f = 0, r = 7;
            foreach (char square in splitFen[0])
            {
                switch (square)
                {
                    case 'Q':
                        Board[f, r] = new Queen(Color.WHITE);
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'q':
                        Board[f, r] = new Queen(Color.BLACK);
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'K':
                        Board[f, r] = new King(Color.WHITE);
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'k':
                        Board[f, r] = new King(Color.BLACK);
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'R':
                        Board[f, r] = new Rook(Color.WHITE);
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'r':
                        Board[f, r] = new Rook(Color.BLACK);
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'N':
                        Board[f, r] = new Knight(Color.WHITE);
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'n':
                        Board[f, r] = new Knight(Color.BLACK);
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'B':
                        Board[f, r] = new Bishop(Color.WHITE);
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'b':
                        Board[f, r] = new Bishop(Color.BLACK);
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'P':
                        Board[f, r] = new Pawn(Color.WHITE);
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'p':
                        Board[f, r] = new Pawn(Color.BLACK);
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case '/':
                        f = 0;
                        r--;
                        break;
                    default:
                        int n = int.Parse(square.ToString());
                        for (int i = 0; i < n; i++)
                            f++;
                        break;
                }
            };

            // active color
            switch (splitFen[1])
            {
                case "w":
                    ToMove = Color.WHITE;
                    break;
                case "b":
                    ToMove = Color.BLACK;
                    break;
            }

            // castling availability
            WhiteCanCastleKingside = false;
            WhiteCanCastleQueenside = false;
            BlackCanCastleKingside = false;
            BlackCanCastleQueenside = false;
            foreach (char c in splitFen[2])
            {
                switch (c)
                {
                    case 'K':
                        WhiteCanCastleKingside = true;
                        break;
                    case 'Q':
                        WhiteCanCastleQueenside = true;
                        break;
                    case 'k':
                        BlackCanCastleKingside = true;
                        break;
                    case 'q':
                        BlackCanCastleQueenside = true;
                        break;
                }
            };

            // en-passant target square
            switch (splitFen[3])
            {
                case "-":
                    EnPassantPosition = null;
                    break;
                default:
                    EnPassantPosition = new Position(splitFen[3][0], int.Parse(splitFen[3][1].ToString()));
                    break;
            }

            // number of halfmoves since last capture or pawn move
            HalfMoveCount = int.Parse(splitFen[4]);
            // turn count
            TurnCount = int.Parse(splitFen[5]);

            for (f = 0; f < 8; f++)
            {
                for (r = 0; r < 8; r++)
                {
                    if (Board[f, r] != null)
                    {
                        Board[f, r].ValidMoves = new List<Position>();
                        if (r == 0 || r == 1)
                            Board[f, r].FindValidMoves(this, new Position(f, r));
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
                Position enPassantPosition = null;
                if (EnPassantPosition != null)
                    enPassantPosition = new Position(EnPassantPosition.File, EnPassantPosition.Rank);
                EnPassantPosition = null;
                LastMove = (src, dst);
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
                //check if castling and move rook if necessarry
                if (piece.Type == PieceType.KING)
                {
                    if (piece.Color == Color.WHITE)
                    {
                        WhiteCanCastleKingside = false;
                        WhiteCanCastleQueenside = false;
                    }
                    else
                    {
                        BlackCanCastleKingside = false;
                        BlackCanCastleQueenside = false;
                    }
                    //kingside castle
                    if(dst.File - src.File == 2)
                    {
                        Board[5, src.Rank] = Board[7, src.Rank];
                        Board[7, src.Rank] = null;
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
                
                if(piece.Type == PieceType.PAWN)
                {
                    HalfMoveCount = 0;
                    //check if move is en-passant
                    if (dst.Equals(enPassantPosition))
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
                    }

                    //check if pawn moved two squares
                    else if (Math.Abs(src.Rank - dst.Rank) == 2)
                    {
                        if (piece.Color == Color.WHITE)
                            EnPassantPosition = new Position(dst.File, dst.Rank - 1);
                        else
                            EnPassantPosition = new Position(dst.File, dst.Rank + 1);
                    }
                        
                    //check for promotion
                    if(dst.Rank == 0 || dst.Rank == 7)
                    {
                        awaitingPromotion = new Position(dst.File, dst.Rank);
                        return MoveResult.AWAITING_PROMOTION;
                    }
                }

                // if a rook was moved for the first time, flag appropriate castling as illegal
                if (piece.Type == PieceType.ROOK)
                {
                    if (piece.Color == Color.WHITE)
                    {
                        if (src.Equals(new Position('A', 1)))
                            WhiteCanCastleQueenside = false;
                        else if (src.Equals(new Position('H', 1)))
                            WhiteCanCastleKingside = false;
                    }
                    else
                    {
                        if (src.Equals(new Position('A', 8)))
                            BlackCanCastleQueenside = false;
                        else if (src.Equals(new Position('H', 8)))
                            BlackCanCastleKingside = false;
                    }
                }

                return SwapPlayer();

                
            }
        }

        public static string GamstateToFEN(Gamestate gamestate)
        {
            var builder = new StringBuilder();
            int r, f, empty;
            //gamestate.Board state
            for (r = 7; r >= 0; r--)
            {
                empty = 0;
                for (f = 0; f < 8; f++)
                {
                    if (gamestate.Board[f, r] != null)
                    {
                        if (empty > 0)
                        {
                            builder.Append(empty);
                            empty = 0;
                        }
                        builder.Append(gamestate.Board[f, r].Symbol);
                    }
                    else
                        empty++;
                }
                if (empty > 0)
                    builder.Append(empty);
                builder.Append('/');
            }
            builder.Append(' ');

            //to move
            if (gamestate.ToMove == Color.WHITE)
                builder.Append('w');
            else
                builder.Append('b');
            builder.Append(' ');

            //possibility of castling
            string castling = "";
            if (gamestate.WhiteCanCastleKingside)
                castling.Append('K');
            if (gamestate.WhiteCanCastleQueenside)
                castling.Append('Q');
            if (gamestate.BlackCanCastleKingside)
                castling.Append('k');
            if (gamestate.BlackCanCastleQueenside)
                castling.Append('q');
            if (castling == "")
                castling = "-";
            builder.Append(castling);
            builder.Append(' ');

            //possiblity of en-passant
            if (gamestate.EnPassantPosition != null)
                builder.Append(gamestate.EnPassantPosition.ToString());
            else
                builder.Append('-');
             builder.Append(' ');

            
            //number of half-moves since last take or pawn move
            builder.Append(gamestate.HalfMoveCount);
            builder.Append(' ');

            //number of full-moves
            builder.Append((gamestate.TurnCount - 1) / 2);
            
            return builder.ToString();
        }

        

        private MoveResult SwapPlayer()
        {
            var gameOver = true;
            
            HalfMoveCount++;
            if (ToMove == Color.WHITE)
            {
                WhiteStopwatch.Stop();
                ToMove = Color.BLACK;

                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.BLACK)
                        {
                            Board[f, r].FindValidMoves(this, new Position(f, r));
                            if (Board[f, r].ValidMoves.Count > 0)
                                gameOver = false;
                        }

                    }
                }

                BlackStopwatch.Restart();
            }

            else
            {
                TurnCount++;
                BlackStopwatch.Stop();
                ToMove = Color.WHITE;

                for (int f = 0; f < 8; f++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (Board[f, r] != null && Board[f, r].Color == Color.WHITE)
                        {
                            Board[f, r].FindValidMoves(this, new Position(f, r));
                            if (Board[f, r].ValidMoves.Count > 0)
                                gameOver = false;
                        }

                    }
                }
                WhiteStopwatch.Restart();
            }
            string fen = GamstateToFEN(this);
            string fenNoMoveCount = String.Join(" ", fen.Split(' ').Take(4));
            PositionHistory.Add(fen);
            if (positionRepetitionCount.ContainsKey(fenNoMoveCount))
                positionRepetitionCount[fenNoMoveCount] += 1;
            else
                positionRepetitionCount[fenNoMoveCount] = 1;
            if (gameOver == false)
            {
                if (positionRepetitionCount[fenNoMoveCount] == 3 || HalfMoveCount >= 50)
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
