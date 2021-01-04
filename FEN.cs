using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public static class FEN
    {

        public static string GamstateToFEN(Gamestate gamestate)
        {
            var builder = new StringBuilder();
            int r, f, empty;
            //board state
            for(r=7; r>=0; r--)
            {
                empty = 0;
                for (f=0; f<8; f++)
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
           // builder.Append(' ');

            /*
            //number of half-moves since last take or pawn move
            builder.Append(gamestate.HalfMoveCount);
            builder.Append(' ');

            //number of full-moves
            builder.Append((gamestate.TurnCount - 1) / 2);
            */
            return builder.ToString();
        }
    }
}
