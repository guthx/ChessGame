using System;
using System.Text.RegularExpressions;

namespace ChessGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new Gamestate();
            while (true)
            {
                for(int r=7; r>=0; r--)
                {
                    for(int f=0; f<8; f++)
                    {
                       
                        char symbol = 'x';
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
                            }
                        }
                        Console.Write(symbol + " ");
                    }
                    Console.Write('\n');
                }
                var moved = false;
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
                        }
                    }
                }
            }
            Console.WriteLine("Hello World!");
        }
    }
}
