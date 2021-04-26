using Minesweeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Minesweeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Board board = new Board();
        private Random rand = new Random();
        private int rows, cols, mineCount;
        private bool GameOver;
        private bool firstClick = true;
        public MainPage()
        {
            this.InitializeComponent();

            MineCounterTextBlock.DataContext = board;
        }

        private async void setup()
        {
            GameOver = false;
            firstClick = true;
            MessageDialog md = new MessageDialog("Select a difficulty.");
            md.Commands.Add(new UICommand("Easy", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            md.Commands.Add(new UICommand("Medium", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            md.Commands.Add(new UICommand("Hard", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            md.DefaultCommandIndex = 0;
            md.CancelCommandIndex = 2;
            _ = await md.ShowAsync();
            initBoard();
            initButtons();
        }

        private void OnNewGame(object sender, RoutedEventArgs e)
        {
            setup();
        }

        private async void WinnerWinnerChickenDinner()
        {
            MessageDialog md = new MessageDialog("Congrats you found all the mines!");
            md.Commands.Add(new UICommand("Nice!", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            md.DefaultCommandIndex = 0;
            md.CancelCommandIndex = 0;
            _ = await md.ShowAsync();
        }

        private void OnBoardButtonTapped(object sender, TappedRoutedEventArgs trea)
        {
            //get the button that was clicked
            //if that button is unopened 
            //it is a mine, then perish
            //on loss prompt for new game or exit application
            if (!GameOver)
            {
                int col = Grid.GetColumn(sender as Button);
                int row = Grid.GetRow(sender as Button);
                Tile tile = board.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault();
                if (firstClick)
                {
                    SetupMines(tile);
                    firstClick = false;
                }
                LeftClickLogic(tile);
            }
        }

        private void OnBoardButtonRightTapped(object sender, RightTappedRoutedEventArgs rtrea)
        {
            if (!GameOver)
            {
                //get the buttons position
                int col = Grid.GetColumn(sender as Button);
                int row = Grid.GetRow(sender as Button);
                //check the current state of the tile at the same coordinate
                //update it through the phases of unopened -> flagged -> ambiguous
                //flagged decrements the mineCounter, unflagging increments the mineCounter
                Tile tile = board.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault();
                if (tile.IsLeftClicked)
                {
                    if (tile.AdjacentMines == board.GetAdjacentTiles(tile).Where(t => t.IsRightClicked == true).Count())
                    {
                        board.GetAdjacentTiles(tile).Where(t => t.IsRightClicked != true).ToList().ForEach(ti => LeftClickLogic(ti));
                    }
                }
                else
                {
                    if (tile.IsRightClicked == false)
                    {
                        tile.IsRightClicked = true;
                        board.Mines--;
                    }
                    else if (tile.IsRightClicked == true)
                    {
                        tile.IsRightClicked = null;
                        board.Mines++;
                    }
                    else
                    {
                        tile.IsRightClicked = false;
                    }
                }
            }
        }

        private void initButtons()
        {
            BoardGrid.Children.Clear();
            Converters.TileToImageSource converter = new Converters.TileToImageSource();
            foreach (Tile t in board.Tiles)
            {
                Button b = new Button();
                BoardGrid.HorizontalAlignment = HorizontalAlignment.Center;
                BoardGrid.VerticalAlignment = VerticalAlignment.Center;
                b.Margin = new Thickness(0);
                b.Padding = new Thickness(0);

                Binding bi = new Binding();
                bi.Path = new PropertyPath("ImageURI");
                bi.Mode = BindingMode.OneWay;
                bi.Source = t;
                bi.Converter = converter;

                Image img = new Image();
                img.SetBinding(Image.SourceProperty, bi);

                b.Content = img;
                Grid.SetColumn(b, t.Coordinate.Col);
                Grid.SetRow(b, t.Coordinate.Row);
                b.Tapped += OnBoardButtonTapped;
                b.RightTapped += OnBoardButtonRightTapped;
                BoardGrid.Children.Add(b);
            }

        }

        private void initBoard()
        {
            board.Tiles.Clear();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Tile t = new Tile(new Coordinate(i, j), false);
                    board.Tiles.Add(t);
                }
            }
        }

        private void SetupMines(Tile tile)
        {
            board.Mines = 0;
            List<Coordinate> coordinates = new List<Coordinate>();
            coordinates.Add(tile.Coordinate);
            foreach (Tile ti in board.GetAdjacentTiles(tile))
            {
                coordinates.Add(ti.Coordinate);
            }

            foreach (Tile t in board.Tiles)
            { 
                t.IsMine = false; 
            }
            while (mineCount > 0)
            {
                int col = rand.Next(cols);
                int row = rand.Next(rows);

                bool validated = true;
                for (int i = 0; i < coordinates.Count && validated; i++)
                {
                    validated = !(coordinates[i].Col == col && coordinates[i].Row == row);
                }

                if (!board.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault().IsMine && validated)
                {
                    board.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault().IsMine = true;
                    board.Mines++;
                    mineCount--;
                }
            }

            foreach (Tile t in board.Tiles)
            {
                t.AdjacentMines = board.GetAdjacentMines(board.GetAdjacentTiles(t));
            }
        }

        private void LeftClickLogic(Tile tile)
        {
            if (tile.IsRightClicked != true)
            {
                tile.IsLeftClicked = true;

                if (tile.IsMine)
                {
                    board.Tiles.Where(t => t.IsMine).ToList().ForEach(t => t.IsLeftClicked = true);
                    GameOver = true;
                }
                else
                {
                    if (tile.AdjacentMines == 0)
                    {
                        foreach (Tile t in board.GetAdjacentTiles(tile))
                        {
                            if (!t.IsMine && !t.IsLeftClicked)
                            {
                                LeftClickLogic(t);
                            }
                        }
                    }
                }
                //do this: check for all mines found
                if (board.Tiles.Where(t => t.IsLeftClicked && !t.IsMine).Count() == ((cols * rows) - board.Tiles.Where(ti => ti.IsMine).Count()))
                {
                    GameOver = true;
                    WinnerWinnerChickenDinner();
                }
            }

        }

        private void CommandInvokedHandler(IUICommand command)
        {
            switch (command.Label.ToLower())
            {
                case "easy":
                    mineCount = 10;
                    cols = 9;
                    rows = 9;
                    break;
                case "medium":
                    mineCount = 40;
                    cols = 16;
                    rows = 16;
                    break;
                case "hard":
                    mineCount = 99;
                    cols = 30;
                    rows = 16;
                    break;
                case "yes":
                    setup();
                    break;
                case "no":
                    Application.Current.Exit();
                    break;
                default:
                    break;

            }
        }
    }
}
