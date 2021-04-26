using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    public class Tile : INotifyPropertyChanged
    {
        private Coordinate coordinate;

        public Coordinate Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
        }

        private bool isMine;

        public bool IsMine
        {
            get { return isMine; }
            set { isMine = value; OnBoolChanged(); }
        }

        private bool? isRightClicked;

        public bool? IsRightClicked
        {
            get { return isRightClicked; }
            set { isRightClicked = value; OnBoolChanged(); }
        }

        private bool isLeftClicked;

        public bool IsLeftClicked
        {
            get { return isLeftClicked; }
            set { isLeftClicked = value; OnBoolChanged(); }
        }

        private int adjacentMines;

        public int AdjacentMines
        {
            get { return adjacentMines; }
            set { adjacentMines = value; }
        }

        private string imageURI;

        public string ImageURI
        {
            get { return imageURI; }
            set { imageURI = value; FieldChanged(); }
        }

        public Tile(Coordinate coordinate, bool isMine)
        {
            Coordinate = coordinate;
            IsMine = isMine;
            IsLeftClicked = false;
            IsRightClicked = false;
        }



        //use this -> ms-appx:///Assets/Images/

        public void OnBoolChanged()
        {
            StringBuilder sb = new StringBuilder("ms-appx:///Assets/Images/");
            _ = (IsRightClicked == true && !IsLeftClicked) ? sb.Append("flag.svg")
                : (IsRightClicked == false && !IsLeftClicked) ? sb.Append("unopened.svg")
                : (IsRightClicked is null && !IsLeftClicked) ? sb.Append("questionmark.svg")
                : (IsMine) ? sb.Append("mine.png")
                : sb.Append($"Minesweeper_{AdjacentMines}.svg");
            ImageURI = sb.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FieldChanged([CallerMemberName] string field = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(field));
        }
    }
}
