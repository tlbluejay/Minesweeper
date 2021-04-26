using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    public class Board : INotifyPropertyChanged
    {
        private List<Tile> tiles;

        public List<Tile> Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        private int mines;

        public int Mines
        {
            get { return mines; }
            set { mines = value; FieldChanged(); }
        }

        public Board()
        {
            Tiles = new List<Tile>();
            Mines = 0;
        }

        public List<Tile> GetAdjacentTiles(Tile origin)
        {
            List<Tile> adjTiles = new List<Tile>();
            for (int i = 0; i < Tiles.Count && adjTiles.Count < 8; i++)
            {
                if (origin.Coordinate.Col - 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row - 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col + 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row + 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col - 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row + 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if(origin.Coordinate.Col + 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row - 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if(origin.Coordinate.Col == Tiles[i].Coordinate.Col && origin.Coordinate.Row - 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if(origin.Coordinate.Col == Tiles[i].Coordinate.Col && origin.Coordinate.Row + 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if(origin.Coordinate.Col - 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if(origin.Coordinate.Col + 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
            }
            return adjTiles;
        }

        public int GetAdjacentMines(List<Tile> adjTiles)
        {
            int count = 0;
            foreach (Tile t in adjTiles)
            {
                if (t.IsMine)
                {
                    count++;
                }
            }
            return count;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FieldChanged([CallerMemberName] string field = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(field));
        }
    }
}
