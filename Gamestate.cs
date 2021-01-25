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
        public bool Check;
        public GameResult GameResult;
        public Stopwatch WhiteStopwatch;
        public Stopwatch BlackStopwatch;
        public List<string> PositionHistory;
        public List<string> MoveHistory;
        public List<(Position from, Position to)> LastMoves;
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
            MoveHistory = new List<string>();
            LastMoves = new List<(Position from, Position to)>();
            positionRepetitionCount = new Dictionary<string, int>();
            WhiteCanCastleKingside = true;
            WhiteCanCastleQueenside = true;
            BlackCanCastleKingside = true;
            BlackCanCastleQueenside = true;
            Check = false;
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
            LastMoves = new List<(Position from, Position to)>();
            if (gamestate == null)
            {
                PositionHistory = new List<string>();
                PositionHistory.Add(fen);
                MoveHistory = new List<string>();
            }
            else
            {
                int halfTurnCount = (gamestate.TurnCount-1) * 2;
                if (gamestate.ToMove == Color.BLACK)
                    halfTurnCount++;
                PositionHistory = gamestate.PositionHistory.Take(halfTurnCount).ToList();
                MoveHistory = gamestate.MoveHistory.Take(halfTurnCount - 1).ToList();
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
                        Board[f, r].ValidMoves = new List<Position>();
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'q':
                        Board[f, r] = new Queen(Color.BLACK);
                        Board[f, r].ValidMoves = new List<Position>();
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'K':
                        Board[f, r] = new King(Color.WHITE);
                        Board[f, r].ValidMoves = new List<Position>();
                        whitePiecesPositions.Add(new Position(f, r));
                        WhiteKing = new Position(f, r);
                        f++;
                        break;
                    case 'k':
                        Board[f, r] = new King(Color.BLACK);
                        Board[f, r].ValidMoves = new List<Position>();
                        blackPiecesPositions.Add(new Position(f, r));
                        BlackKing = new Position(f, r);
                        f++;
                        break;
                    case 'R':
                        Board[f, r] = new Rook(Color.WHITE);
                        Board[f, r].ValidMoves = new List<Position>();
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'r':
                        Board[f, r] = new Rook(Color.BLACK);
                        Board[f, r].ValidMoves = new List<Position>();
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'N':
                        Board[f, r] = new Knight(Color.WHITE);
                        Board[f, r].ValidMoves = new List<Position>();
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'n':
                        Board[f, r] = new Knight(Color.BLACK);
                        Board[f, r].ValidMoves = new List<Position>();
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'B':
                        Board[f, r] = new Bishop(Color.WHITE);
                        Board[f, r].ValidMoves = new List<Position>();
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'b':
                        Board[f, r] = new Bishop(Color.BLACK);
                        Board[f, r].ValidMoves = new List<Position>();
                        blackPiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'P':
                        Board[f, r] = new Pawn(Color.WHITE);
                        Board[f, r].ValidMoves = new List<Position>();
                        whitePiecesPositions.Add(new Position(f, r));
                        f++;
                        break;
                    case 'p':
                        Board[f, r] = new Pawn(Color.BLACK);
                        Board[f, r].ValidMoves = new List<Position>();
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
                        Board[f, r].FindValidMoves(this, new Position(f, r));
                    }

                }
            }
        }

        public static Piece[,] GetBoardFromFen(string fen)
        {
            var board = new Piece[8, 8];
            var splitFen = fen.Split(' ');
            int f = 0, r = 7;
            foreach (char square in splitFen[0])
            {
                switch (square)
                {
                    case 'Q':
                        board[f, r] = new Queen(Color.WHITE);
                        f++;
                        break;
                    case 'q':
                        board[f, r] = new Queen(Color.BLACK);
                        f++;
                        break;
                    case 'K':
                        board[f, r] = new King(Color.WHITE);
                        f++;
                        break;
                    case 'k':
                        board[f, r] = new King(Color.BLACK);
                        f++;
                        break;
                    case 'R':
                        board[f, r] = new Rook(Color.WHITE);
                        break;
                    case 'r':
                        board[f, r] = new Rook(Color.BLACK);
                        break;
                    case 'N':
                        board[f, r] = new Knight(Color.WHITE);
                        break;
                    case 'n':
                        board[f, r] = new Knight(Color.BLACK);
                        break;
                    case 'B':
                        board[f, r] = new Bishop(Color.WHITE);
                        break;
                    case 'b':
                        board[f, r] = new Bishop(Color.BLACK);
                        break;
                    case 'P':
                        board[f, r] = new Pawn(Color.WHITE);
                        break;
                    case 'p':
                        board[f, r] = new Pawn(Color.BLACK);
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
            return board;
        }
        public MoveResult Promote(PieceType piece)
        {
            if(awaitingPromotion != null)
            {
                switch (piece)
                {
                    case PieceType.KNIGHT:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Knight(ToMove);
                        MoveHistory[MoveHistory.Count - 1] = MoveHistory.Last() + 'N';
                        break;
                    case PieceType.BISHOP:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Bishop(ToMove);
                        MoveHistory[MoveHistory.Count - 1] = MoveHistory.Last() + 'B';
                        break;
                    case PieceType.ROOK:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Rook(ToMove);
                        MoveHistory[MoveHistory.Count - 1] = MoveHistory.Last() + 'R';
                        break;
                    case PieceType.QUEEN:
                        Board[awaitingPromotion.File, awaitingPromotion.Rank] = new Queen(ToMove);
                        MoveHistory[MoveHistory.Count - 1] = MoveHistory.Last() + 'R';
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
                LastMoves.Clear();
                LastMoves.Add((src, dst));
                Check = false;
                bool takes = false, sameFile = false, sameRank = false, qsCastle = false, ksCastle = false;
                List<Position> conflictingPieces;
                if (piece.Color == Color.WHITE)
                {
                    conflictingPieces = whitePiecesPositions
                        .FindAll(p => Board[p.File, p.Rank] != piece &&
                        Board[p.File, p.Rank].Type == piece.Type && 
                        Board[p.File, p.Rank].ValidMoves.Contains(dst));
                    
                }
                else
                {
                    conflictingPieces = blackPiecesPositions
                        .FindAll(p => Board[p.File, p.Rank] != piece &&
                        Board[p.File, p.Rank].Type == piece.Type &&
                        Board[p.File, p.Rank].ValidMoves.Contains(dst));
                }
                foreach (var conflictingPiece in conflictingPieces)
                {
                    if (conflictingPiece.File == src.File)
                        sameFile = true;
                    if (conflictingPiece.Rank == src.Rank)
                        sameRank = true;
                }
                StringBuilder moveNotation = new StringBuilder();
                HalfMoveCount++;
                Position enPassantPosition = null;
                if (EnPassantPosition != null)
                    enPassantPosition = new Position(EnPassantPosition.File, EnPassantPosition.Rank);
                EnPassantPosition = null;
                if (Board[dst.File, dst.Rank] != null)
                {
                    HalfMoveCount = 0;
                    takes = true;
                }
                    
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
                        ksCastle = true;
                        LastMoves.Add((new Position(7, src.Rank), new Position(5, src.Rank)));
                    }
                    //queenside castle
                    else if(dst.File - src.File == -2)
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
                        qsCastle = true;
                        LastMoves.Add((new Position(0, src.Rank), new Position(3, src.Rank)));
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
                            LastMoves.Add((new Position(dst.File, dst.Rank - 1), null));
                        }
                        else
                        {
                            Board[dst.File, dst.Rank + 1] = null;
                            whitePiecesPositions.Remove(new Position(dst.File, dst.Rank + 1));
                            LastMoves.Add((new Position(dst.File, dst.Rank + 1), null));
                        }
                        takes = true;
                        
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

                if (ksCastle)
                    moveNotation.Append("0-0");
                else if (qsCastle)
                    moveNotation.Append("0-0-0");
                else
                {
                    if (piece.Type != PieceType.PAWN)
                        moveNotation.Append(char.ToUpper(piece.Symbol));
                    else if (takes)
                        sameRank = true;
                    if (sameRank)
                        moveNotation.Append(char.ToLower((char)(src.File + 65)));
                    if (sameFile)
                        moveNotation.Append(src.Rank + 1);
                    if (takes)
                        moveNotation.Append('x');
                    moveNotation.Append(char.ToLower((char)(dst.File + 65)));
                    moveNotation.Append(dst.Rank + 1);
                }
                MoveHistory.Add(moveNotation.ToString());

                if (awaitingPromotion != null)
                    return MoveResult.AWAITING_PROMOTION;
                else
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
            StringBuilder castling = new StringBuilder();
            if (gamestate.WhiteCanCastleKingside)
                castling.Append('K');
            if (gamestate.WhiteCanCastleQueenside)
                castling.Append('Q');
            if (gamestate.BlackCanCastleKingside)
                castling.Append('k');
            if (gamestate.BlackCanCastleQueenside)
                castling.Append('q');
            if (castling.Length == 0)
                castling.Append('-');
            builder.Append(castling.ToString());
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
            builder.Append(gamestate.TurnCount);
            
            return builder.ToString();
        }

        

        private MoveResult SwapPlayer()
        {
            var gameOver = true;
            
            
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

                if (Board[BlackKing.File, BlackKing.Rank].IsAttacked(this, BlackKing))
                {
                    Check = true;
                    MoveHistory[MoveHistory.Count - 1] = MoveHistory.Last() + '+';
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
                if (Board[WhiteKing.File, WhiteKing.Rank].IsAttacked(this, WhiteKing))
                {
                    Check = true;
                    MoveHistory[MoveHistory.Count - 1] = MoveHistory.Last() + '+';
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
