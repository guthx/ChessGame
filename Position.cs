﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    public class Position
    {
        public int File;
        public int Rank;
        
        public Position(char file, int rank)
        {
            File = char.ToUpper(file) - 'A';
            Rank = rank - 1;
        }
        public Position(int file, int rank)
        {
            File = file;
            Rank = rank;
        }
    }
}