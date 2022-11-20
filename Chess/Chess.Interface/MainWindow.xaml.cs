﻿using Chess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess.Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Свойства игры

        private Board board = new Board();

        private bool started = false;

        private Side currentStepSide = Side.White;

        #endregion

        #region Forms and Windows
        
        NewGameSettings newGameSettings;
        
        #endregion

        int ActiveFigurePathId = -1;

        double size;

        public MainWindow()
        {
            newGameSettings = new NewGameSettings(this);

            InitializeComponent();

            this.Width = 800;
            this.Height = 450;
            this.MinWidth = 600;
            this.MinHeight = 450;

            DrawDesk(this.Height);
        }        

        public void ResetBoard()
        {
            this.board = new Board();
            currentStepSide = Side.White;
        }

        public void StartGame() => started = true;

        public void EndGame() => started = false;



        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawDesk(GetSize());
        }

        private void DrawDesk(double size)
        {
            ChessBoard.Width = size;
            ChessBoard.Height = ChessBoard.Width;

            DrawBoard(size);
            double scale = GetScale();
            DrawBoardFigures(scale);
        }

        private double GetSize()
        {
            (var width, var height) = (this.ActualWidth - 50, this.ActualHeight - 100);

            double size;

            if (width.CompareTo(height) >= 0)
                size = height;
            else
                size = width;
            return size;
        }

        private double GetScale()
        {
            return (ChessBoard.Width / 396);
        }

        private void DrawBoardFigures(double scale)
        {
            for (int i = 0; i < Board.BoardCellSize; i++)
            {
                for (int j = 0; j < Board.BoardCellSize; j++)
                {
                    var figure = board.Positions[i, j];
                    if (figure.Man is not Figures.Empty)
                    {
                        var figurePathId = DrawFigure(scale, figure.Man, figure.Side, i, j);
                        ChessBoard.Children[figurePathId].MouseEnter += ChessBoadFigure_MouseEnter;
                    }
                }
            }
        }

        private int DrawFigure(double scale, Chess.Entity.Figures figure, Entity.Side side, int col, int row, string strokeColor = null)
        {
            Path figureCurrent = DrawFigure(scale, new(side, figure), strokeColor is null ? "#AAAAAA" : strokeColor);
            figureCurrent.Name = $"Figure_{col}_{row}";
            int figurePathId = ChessBoard.Children.Add(figureCurrent);
            Canvas.SetLeft(ChessBoard.Children[figurePathId], (10) * scale + 47 * scale * col);
            Canvas.SetTop(ChessBoard.Children[figurePathId], (10) * scale + 47 * scale * row);
            return figurePathId;
        }

        private void ChessBoadFigure_MouseLeave(object sender, MouseEventArgs e)
        {
            ChessBoadCellBox_MouseLeave(sender, e);
        }

        private void ChessBoadFigure_MouseEnter(object sender, MouseEventArgs e)
        {
            ChessBoadFigure_MouseLeave(sender, e);
            int col, row;
            GetColAndRowOfFigure(e, out col, out row);
            DrawFigureBorder(col, row, "Blue");

            ChessBoard.Children[ActiveFigurePathId].MouseLeave += ChessBoadFigure_MouseLeave;
        }
        private static void GetColAndRowOfFigure(MouseEventArgs e, out int col, out int row)
        {
            (col, row) = (-1, -1);
            if (e.Source is Path)
            {
                var path = e.Source as Path;
                if (path is not null && path.Name.StartsWith("Figure_"))
                {
                    string cellNumber = path.Name.Substring(7, path.Name.Length - 7);
                    var colRowNums = cellNumber.Split('_');
                    col = int.Parse(colRowNums[0]);
                    row = int.Parse(colRowNums[1]);
                }
            }
        }

        public void DrawBoard(double size)
        {
            ChessBoard.Children.Clear();

            string leftPositions = "87654321";
            string topPositions = "abcdefgh";
            int baseFontSize = 8;

            DrawSquares(size, leftPositions, topPositions);
            DrawLetters(size, topPositions, baseFontSize);
            DrawDigits(size, leftPositions, baseFontSize);
        }

        private void DrawDigits(double size, string leftPositions, int baseFontSize)
        {
            int row = 0;
            foreach (var rowNum in leftPositions)
            {
                int leftNum = ChessBoard.Children.Add(new Label() { Content = rowNum, Padding = new Thickness(0, 0, 0, 0), FontSize = baseFontSize * (size / 396) });
                Canvas.SetLeft(ChessBoard.Children[leftNum], baseFontSize / 3 * (size / 396));
                Canvas.SetTop(ChessBoard.Children[leftNum], 10 * (size / 396) + (47 / 2) * (size / 396) + 47 * (size / 396) * row - baseFontSize/2 * (size / 396));
                int rightNum = ChessBoard.Children.Add(new Label() { Content = rowNum, Padding = new Thickness(0, 0, 0, 0), FontSize = baseFontSize * (size / 396) });
                Canvas.SetLeft(ChessBoard.Children[rightNum], 386 * (size / 396) + baseFontSize / 3 * (size / 396));
                Canvas.SetTop(ChessBoard.Children[rightNum], 10 * (size / 396) + (47 / 2) * (size / 396) + 47 * (size / 396) * row - baseFontSize/2 * (size / 396));
                row++;
            }
        }

        private void DrawLetters(double size, string topPositions, int baseFontSize)
        {
            int col = 0;
            foreach (var letter in topPositions)
            {
                int topLetter = ChessBoard.Children.Add(new Label() { Content = letter, Padding = new Thickness(0, 0, 0, 0), FontSize = baseFontSize * (size / 396) });
                Canvas.SetLeft(ChessBoard.Children[topLetter], 10 * (size / 396) + (47 / 2) * (size / 396) + 47 * (size / 396) * col);
                Canvas.SetTop(ChessBoard.Children[topLetter], 0);
                int bottomLetter = ChessBoard.Children.Add(new Label() { Content = letter, Padding = new Thickness(0, 0, 0, 0), FontSize = baseFontSize * (size / 396) });
                Canvas.SetLeft(ChessBoard.Children[bottomLetter], 10 * (size / 396) + (47 / 2) * (size / 396) + 47 * (size / 396) * col);
                Canvas.SetTop(ChessBoard.Children[bottomLetter], 386 * (size / 396));
                col++;
            }
        }

        private void DrawSquares(double size, string leftPositions, string topPositions)
        {
            int col, row;
            col = 0;
            row = 0;
            foreach (var letter in topPositions)
            {
                row = 0;
                foreach (var rowNum in leftPositions)
                {
                    int newBox = ChessBoard.Children.Add(new Rectangle() { Width = 47 * (size / 396), Height = 47 * (size / 396), Fill = (col + row) % 2 > 0 ? Brushes.Gray : Brushes.Azure, StrokeThickness = 0, Name = $"Cell_{col}_{row}"});
                    Canvas.SetLeft(ChessBoard.Children[newBox], 10 * (size / 396) + 47 * (size / 396) * col);
                    Canvas.SetTop(ChessBoard.Children[newBox], 10 * (size / 396) + 47 * (size / 396) * row);
                    ChessBoard.Children[newBox].MouseEnter += ChessBoadCellBox_MouseEnter;
                    ChessBoard.Children[newBox].MouseLeave += ChessBoadCellBox_MouseLeave;

                    row++;
                }
                col++;
            }
        }

        private void ChessBoadCellBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if(ActiveFigurePathId > -1)
            {
                ChessBoard.Children.RemoveAt(ActiveFigurePathId);
                ActiveFigurePathId = -1;
            } 
        }

        private void ChessBoadCellBox_MouseEnter(object sender, MouseEventArgs e)
        {
            ChessBoadCellBox_MouseLeave(sender, e);
            int col, row;
            GetColAndRowOfRectangle(e, out col, out row);
            DrawFigureBorder(col, row, "Blue");
        }

        private void DrawFigureBorder(int col, int row, string color)
        {
            if (col >= 0 && row >= 0)
            {
                if (!(board.Positions[col, row].Man is Figures.Empty))
                    ActiveFigurePathId = DrawFigure(GetScale(), board.Positions[col, row].Man, board.Positions[col, row].Side, col, row, color);
            }
        }

        private static void GetColAndRowOfRectangle(MouseEventArgs e, out int col, out int row)
        {
            (col, row) = (-1, -1);
            if (e.Source is Rectangle)
            {
                var rectangle = e.Source as Rectangle;
                if (rectangle is not null && rectangle.Name.StartsWith("Cell_"))
                {
                    string cellNumber = rectangle.Name.Substring(5, rectangle.Name.Length - 5);
                    var colRowNums = cellNumber.Split('_');
                    col = int.Parse(colRowNums[0]);
                    row = int.Parse(colRowNums[1]);
                }
            }
        }

        private Path DrawFigure(double scale, Chess.Entity.Figure figure, string strokeColor) => figure.Man switch
        {
            Entity.Figures.Pawn => (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Parse(
                @$"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Stroke='{strokeColor}'
StrokeThickness='{(int)(2*scale)}' Fill='{GetHtmlColorOfFigureSide(figure.Side)}'>
  <Path.Data>
    <PathGeometry>
      <PathGeometry.Figures>
        <PathFigureCollection>
          <PathFigure StartPoint='{(int)((3 + 1) * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((3 + 1) * scale)},{(int)(40 * scale)}' Point2='{(int)((6 + 1) * scale)},{(int)(40 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((15 + 1) * scale)},{(int)(41 * scale)}' Point2='{(int)((20 + 1) * scale)},{(int)(39 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((20 + 1) * scale)},{(int)(27 * scale)}' Point2='{(int)((20 + 1) * scale)},{(int)(15 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((17 + 1) * scale)},{(int)(11 * scale)}' Point2='{(int)((20 + 1) * scale)},{(int)(8 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((23 + 1) * scale)},{(int)(5 * scale)}' Point2='{(int)((26 + 1) * scale)},{(int)(8 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((29 + 1) * scale)},{(int)(11 * scale)}' Point2='{(int)((26 + 1) * scale)},{(int)(15 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((26 + 1) * scale)},{(int)(27 * scale)}' Point2='{(int)((26 + 1) * scale)},{(int)(39 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((31 + 1) * scale)},{(int)(41 * scale)}' Point2='{(int)((40 + 1) * scale)},{(int)(40 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((43 + 1) * scale)},{(int)(40 * scale)}' Point2='{(int)((43 + 1) * scale)},{(int)(43 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((23.5 + 1) * scale)},{(int)(43 * scale)}' Point2='{(int)((2 + 1) * scale)},{(int)(43 * scale)}' />
              </PathSegmentCollection>
            </PathFigure.Segments>
          </PathFigure>
        </PathFigureCollection>
      </PathGeometry.Figures>
    </PathGeometry>
  </Path.Data>
</Path>"),
            Entity.Figures.Rook => (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Parse(
                @$"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Stroke='{strokeColor}'
StrokeThickness='{(int)(2 * scale)}' Fill='{GetHtmlColorOfFigureSide(figure.Side)}'>
  <Path.Data>
    <PathGeometry>
      <PathGeometry.Figures>
        <PathFigureCollection>
          <PathFigure StartPoint='{(int)((9+1) * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((9 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1)  * scale)},{(int)(20 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((15 + 1) * scale)},{(int)(20 * scale)}' Point2='{(int)((12 + 1) * scale)},{(int)(20 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((12 + 1) * scale)},{(int)(20 * scale)}' Point2='{(int)((12 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((12 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((16 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((16 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((16 + 1) * scale)},{(int)(11 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((16 + 1) * scale)},{(int)(11 * scale)}' Point2='{(int)((20 + 1) * scale)},{(int)(11 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((20 + 1) * scale)},{(int)(11 * scale)}' Point2='{(int)((20 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((20 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((26 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((26 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((26 + 1) * scale)},{(int)(11 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((26 + 1) * scale)},{(int)(11 * scale)}' Point2='{(int)((30 + 1) * scale)},{(int)(11 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((30 + 1) * scale)},{(int)(11 * scale)}' Point2='{(int)((30 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((30 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((34 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((34 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((34 + 1) * scale)},{(int)(20 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((34 + 1) * scale)},{(int)(20 * scale)}' Point2='{(int)((31 + 1) * scale)},{(int)(20 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((31 + 1) * scale)},{(int)(20 * scale)}' Point2='{(int)((31 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((37 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((37 + 1) * scale)},{(int)(43 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((37 + 1) * scale)},{(int)(43 * scale)}' Point2='{(int)((8 + 1) * scale)},{(int)(43 * scale)}' />
              </PathSegmentCollection>
            </PathFigure.Segments>
          </PathFigure>
        </PathFigureCollection>
      </PathGeometry.Figures>
    </PathGeometry>
  </Path.Data>
</Path>"),
            Entity.Figures.Knight => (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Parse(
                @$"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Stroke='{strokeColor}'
StrokeThickness='{(int)(2 * scale)}' Fill='{GetHtmlColorOfFigureSide(figure.Side)}'>
  <Path.Data>
    <PathGeometry>
      <PathGeometry.Figures>
		<PathFigureCollection>
          <PathFigure StartPoint='{(int)((9 + 1) * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((9 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((16.5 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((18 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((18 + 1) * scale)},{(int)(26 * scale)}' Point2='{(int)((18 + 1) * scale)},{(int)(17 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((12 + 1) * scale)},{(int)(20 * scale)}' Point2='{(int)((10 + 1) * scale)},{(int)(17 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((12 + 1) * scale)},{(int)(16 * scale)}' Point2='{(int)((14 + 1) * scale)},{(int)(15 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((11.5 + 1) * scale)},{(int)(15.5 * scale)}' Point2='{(int)((9 + 1) * scale)},{(int)(16 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((9 + 1) * scale)},{(int)(11 * scale)}' Point2='{(int)((16 + 1) * scale)},{(int)(10 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((16 + 1) * scale)},{(int)(6 * scale)}' Point2='{(int)((21 + 1) * scale)},{(int)(6 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((26 + 1) * scale)},{(int)(6 * scale)}' Point2='{(int)((27 + 1) * scale)},{(int)(8 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((25.5 + 1) * scale)},{(int)(9 * scale)}' Point2='{(int)((34 + 1) * scale)},{(int)(10 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((30.5 + 1) * scale)},{(int)(10 * scale)}' Point2='{(int)((27 + 1) * scale)},{(int)(10 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((29 + 1) * scale)},{(int)(12 * scale)}' Point2='{(int)((28 + 1) * scale)},{(int)(15 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((31 + 1) * scale)},{(int)(25 * scale)}' Point2='{(int)((28 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((29.5 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((31 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((37 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((37 + 1) * scale)},{(int)(43 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((23 + 1) * scale)},{(int)(43 * scale)}' Point2='{(int)((8 + 1) * scale)},{(int)(43 * scale)}' />

              </PathSegmentCollection>
            </PathFigure.Segments>
          </PathFigure>
		  <PathFigure StartPoint='{(int)((19 + 1) * scale)},{(int)(11 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((19 + 1) * scale)},{(int)(9 * scale)}' Point2='{(int)((21 + 1) * scale)},{(int)(10 * scale)}' />
              </PathSegmentCollection>
            </PathFigure.Segments>
          </PathFigure>
        </PathFigureCollection>
      </PathGeometry.Figures>
    </PathGeometry>
  </Path.Data>
</Path>"),
            Entity.Figures.Bishop => (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Parse(
                @$"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Stroke='{strokeColor}'
StrokeThickness='{(int)(2 * scale)}' Fill='{GetHtmlColorOfFigureSide(figure.Side)}'>
  <Path.Data>
    <PathGeometry>
      <PathGeometry.Figures>
        <PathFigureCollection>
          <PathFigure StartPoint='{(int)((9 + 1) * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((9 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((17 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((19 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((16 + 1) * scale)},{(int)(32 * scale)}' Point2='{(int)((19 + 1) * scale)},{(int)(29 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((19 + 1) * scale)},{(int)(24.5 * scale)}' Point2='{(int)((19 + 1) * scale)},{(int)(20 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((16 + 1 - 1) * scale)},{(int)(12 * scale)}' Point2='{(int)((23 + 1) * scale)},{(int)(6 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((31 + 1) * scale)},{(int)(12 * scale)}' Point2='{(int)((27 + 1) * scale)},{(int)(20 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((27 + 1) * scale)},{(int)(24.5 * scale)}' Point2='{(int)((27 + 1) * scale)},{(int)(29 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((30 + 1) * scale)},{(int)(32 * scale)}' Point2='{(int)((27 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((29.5 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((32 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((38 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((38 + 1) * scale)},{(int)(43 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((23 + 1) * scale)},{(int)(43 * scale)}' Point2='{(int)((8 + 1) * scale)},{(int)(43 * scale)}' />
              </PathSegmentCollection>
            </PathFigure.Segments>
          </PathFigure>
        </PathFigureCollection>
      </PathGeometry.Figures>
    </PathGeometry>
  </Path.Data>
</Path>"),
            Entity.Figures.King => (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Parse(
                @$"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Stroke='{strokeColor}'
StrokeThickness='{(int)(2 * scale)}' Fill='{GetHtmlColorOfFigureSide(figure.Side)}'>
  <Path.Data>
    <PathGeometry>
      <PathGeometry.Figures>
        <PathFigureCollection>
          <PathFigure StartPoint='{(int)((9 + 1) * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((9 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((3 + 1) * scale)},{(int)(18 * scale)}' Point2='{(int)((11 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((20 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((20 + 1) * scale)},{(int)(18 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((20 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((23 + 1) * scale)},{(int)(6 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((26 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((26 + 1) * scale)},{(int)(18 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((26 + 1) * scale)},{(int)(7 * scale)}' Point2='{(int)((35 + 1) * scale)},{(int)(7 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((43 + 1) * scale)},{(int)(18 * scale)}' Point2='{(int)((31 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((37 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((37 + 1) * scale)},{(int)(43 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((23 + 1) * scale)},{(int)(43 * scale)}' Point2='{(int)((8 + 1) * scale)},{(int)(43 * scale)}' />
			  </PathSegmentCollection>
            </PathFigure.Segments>
          </PathFigure>
        </PathFigureCollection>
      </PathGeometry.Figures>
    </PathGeometry>
  </Path.Data>
</Path>"),
            Entity.Figures.Queen => (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Parse(
                @$"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Stroke='{strokeColor}'
StrokeThickness='{(int)(2 * scale)}' Fill='{GetHtmlColorOfFigureSide(figure.Side)}'>
  <Path.Data>
    <PathGeometry>
      <PathGeometry.Figures>
        <PathFigureCollection>
          <PathFigure StartPoint='{(int)((9 + 1) * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((9 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((12 + 1) * scale)},{(int)(30 * scale)}' Point2='{(int)((9 + 1) * scale)},{(int)(25 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((13 + 1) * scale)},{(int)(28 * scale)}' Point2='{(int)((17 + 1) * scale)},{(int)(31 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((13 + 1) * scale)},{(int)(20.5 * scale)}' Point2='{(int)((9 + 1) * scale)},{(int)(10 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((15 + 1) * scale)},{(int)(20.5 * scale)}' Point2='{(int)((21 + 1) * scale)},{(int)(31 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((21 + 1) * scale)},{(int)(24.5 * scale)}' Point2='{(int)((21 + 1) * scale)},{(int)(18 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((19 + 1) * scale)},{(int)(10 * scale)}' Point2='{(int)((23 + 1) * scale)},{(int)(6 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((27 + 1) * scale)},{(int)(10 * scale)}' Point2='{(int)((25 + 1) * scale)},{(int)(18 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((25 + 1) * scale)},{(int)(24.5 * scale)}' Point2='{(int)((25 + 1) * scale)},{(int)(31 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((31 + 1) * scale)},{(int)(20.5 * scale)}' Point2='{(int)((37 + 1) * scale)},{(int)(10 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((33 + 1) * scale)},{(int)(20.5 * scale)}' Point2='{(int)((29 + 1) * scale)},{(int)(31 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((33 + 1) * scale)},{(int)(28 * scale)}' Point2='{(int)((37 + 1) * scale)},{(int)(25 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((34 + 1) * scale)},{(int)(30 * scale)}' Point2='{(int)((31 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((37 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((37 + 1) * scale)},{(int)(43 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((24.5 + 1) * scale)},{(int)(43 * scale)}' Point2='{(int)((8 + 1) * scale)},{(int)(43 * scale)}' />
			  </PathSegmentCollection>
            </PathFigure.Segments>
          </PathFigure>
        </PathFigureCollection>
      </PathGeometry.Figures>
    </PathGeometry>
  </Path.Data>
</Path>"),
        };

        private string GetHtmlColorOfFigureSide(Chess.Entity.Side side) => side switch {
            Entity.Side.Black => "#000000",
            Entity.Side.White => "#FFFFFF",
            _ => throw new ArgumentOutOfRangeException()
        };

        private string GetHtmlColorOfFigureSideBorder(Chess.Entity.Side side) => side switch
        {
            Entity.Side.Black => "#FFFFFF",
            Entity.Side.White => "#000000",
            _ => throw new ArgumentOutOfRangeException()
        };

        private void ChessBoard_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            newGameSettings.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            newGameSettings.IsMainWindowClosing = true;
            newGameSettings.Close();
        }
    }
}
