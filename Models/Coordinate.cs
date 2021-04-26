﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    public class Coordinate
    {
        private int row;

        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        private int col;

        public int Col
        {
            get { return col; }
            set { col = value; }
        }

        public Coordinate(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}
