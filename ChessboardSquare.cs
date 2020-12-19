using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGame
{
    public class ChessboardSquare : PictureBox
    {
        public Position Position;
        public ChessboardSquare(int f, int r) : base()
        {
            Position = new Position(f, r);
        }
    }
}
