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
        public MainWindow()
        {
            InitializeComponent();
            DrawBoard(396);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChessBoard.Width = ChessBoard.ActualHeight;
            
            DrawBoard(ChessBoard.Width);
            double scale = (ChessBoard.Width / 396);
            Path test = DrawFigure(scale, new(Entity.Side.White, Entity.Figures.Pawn), GetHtmlColorOfFigureSideBorder(Entity.Side.White));
            int testId = ChessBoard.Children.Add(test);
            Canvas.SetLeft(ChessBoard.Children[testId], 10* scale + 47 * scale);
            Canvas.SetTop(ChessBoard.Children[testId], 10* scale + 47 * scale);
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
                    int newBox = ChessBoard.Children.Add(new Rectangle() { Width = 47 * (size / 396), Height = 47 * (size / 396), Stroke = Brushes.Black, Fill = (col + row) % 2 > 0 ? Brushes.Black : Brushes.Azure });
                    Canvas.SetLeft(ChessBoard.Children[newBox], 10 * (size / 396) + 47 * (size / 396) * col);
                    Canvas.SetTop(ChessBoard.Children[newBox], 10 * (size / 396) + 47 * (size / 396) * row);
                    row++;
                }
                col++;
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
          <PathFigure StartPoint='{(int)(5 * scale)},{(int)(45 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)(5* scale)},{(int)(42 * scale)}' Point2='{(int)(5 * scale)},{(int)(40 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(5 * scale)},{(int)(40 * scale)}' Point2='{(int)(17 * scale)},{(int)(40 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(17 * scale)},{(int)(40 * scale)}' Point2='{(int)(18 * scale)},{(int)(20 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(15 * scale)},{(int)(18 * scale)}' Point2='{(int)(18 * scale)},{(int)(15 * scale)}' />

                <QuadraticBezierSegment Point1='{(int)(42 * scale)},{(int)(45 * scale)}' Point2='{(int)(5 * scale)},{(int)(45 * scale)}' />
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

    }
}
