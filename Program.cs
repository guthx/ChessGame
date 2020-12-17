using System;
using System.Text.RegularExpressions;

namespace ChessGame
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var state = new Gamestate();

            MoveResult promote()
            {
                Console.WriteLine("Which piece type to promote to? (N, B, R, Q)");
                var piece = Console.ReadLine();
                bool promoted = false;
                MoveResult response = MoveResult.NO_PIECE;
                while (!promoted)
                {
                    switch (piece)
                    {
                        case "N":
                            response = state.Promote(PieceType.KNIGHT);
                            promoted = true;
                            break;
                        case "B":
                            response = state.Promote(PieceType.BISHOP);
                            promoted = true;
                            break;
                        case "R":
                            response = state.Promote(PieceType.ROOK);
                            promoted = true;
                            break;
                        case "Q":
                            response = state.Promote(PieceType.QUEEN);
                            promoted = true;
                            break;
                        default:
                            promoted = false;
                            break;
                    }
                }
                if (response != MoveResult.NO_PIECE)
                    return response;
                else throw new Exception("Attempted promotion with no promoting pawn");
            }

            
            bool checkmated = false;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            while (true)
            {
                if (!checkmated)
                {
                    for (int r = 7; r >= 0; r--)
                    {
                        for (int f = 0; f < 8; f++)
                        {

                            char symbol = ' ';
                            if (state.Board[f, r] != null)
                            {

                                switch (state.Board[f, r].GetType().ToString())
                                {
                                    case "ChessGame.Pawn":
                                        symbol = 'P';
                                        break;
                                    case "ChessGame.King":
                                        symbol = 'K';
                                        break;
                                    case "ChessGame.Rook":
                                        symbol = 'R';
                                        break;
                                    case "ChessGame.Knight":
                                        symbol = 'N';
                                        break;
                                    case "ChessGame.Bishop":
                                        symbol = 'B';
                                        break;
                                    case "ChessGame.Queen":
                                        symbol = 'Q';
                                        break;
                                }
                                if (state.Board[f, r].Color == Color.WHITE)
                                    Console.ForegroundColor = ConsoleColor.White;
                                else
                                    Console.ForegroundColor = ConsoleColor.Black;
                            }

                            Console.Write(symbol + " ");
                        }
                        Console.Write('\n');
                    }
                    bool moved = false;
                    while (!moved)
                    {
                        var move = Console.ReadLine();
                        string pattern = @"([A-H][1-8])\s([A-H][1-8])";
                        var match = Regex.Match(move, pattern);
                        if (match.Success)
                        {
                            var src = new Position(match.Groups[1].Value[0], int.Parse(match.Groups[1].Value[1].ToString()));
                            var dst = new Position(match.Groups[2].Value[0], int.Parse(match.Groups[2].Value[1].ToString()));
                            var result = state.Move(src, dst);
                            switch (result)
                            {
                                case MoveResult.NO_PIECE:
                                    Console.WriteLine("Didn't select a piece");
                                    break;
                                case MoveResult.INVALID_MOVE:
                                    Console.WriteLine("Invalid move");
                                    break;
                                case MoveResult.WRONG_COLOR:
                                    Console.WriteLine("Cannot move enemy piece");
                                    break;
                                case MoveResult.MOVED:
                                    moved = true;
                                    break;
                                case MoveResult.AWAITING_PROMOTION:
                                    var promoteResult = promote();
                                    if (promoteResult == MoveResult.MOVED)
                                        moved = true;
                                    else if (promoteResult == MoveResult.CHECKMATE)
                                    {
                                        checkmated = true;
                                        moved = true;
                                    }    
                                    else throw new Exception();
                                    break;
                                case MoveResult.CHECKMATE:
                                    checkmated = true;
                                    moved = true;
                                    break;
                            }
                        }
                        else
                        {
                            string pattern2 = @"[A-H][1-8]";
                            match = Regex.Match(move, pattern2);
                            if (match.Success)
                            {
                                var piece = new Position(match.Value[0], int.Parse(match.Value[1].ToString()));
                                if (state.Board[piece.File, piece.Rank] != null && state.Board[piece.File, piece.Rank].Color == state.ToMove)
                                {
                                    var validMoves = state.Board[piece.File, piece.Rank].ValidMoves;
                                    if (validMoves.Count != 0)
                                    {
                                        Console.WriteLine($"Valid moves for {match.Value}:");
                                        foreach (var validMove in validMoves)
                                        {
                                            Console.Write(validMove.ToString() + " ");
                                        }
                                        Console.Write("\n");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"No valid moves for {match.Value}");
                                    }


                                }
                                else
                                {
                                    Console.WriteLine($"No ally piece on {match.Value}");
                                }
                            }
                        }
                    }
                } else
                {
                    string winner;
                    if (state.ToMove == Color.WHITE)
                        winner = "Black";
                    else
                        winner = "White";
                    Console.WriteLine($"Checkmate! {winner} wins!");
                    Console.ReadLine();
                }
            } 
        }
    }
}
