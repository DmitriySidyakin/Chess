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
            
            // Заглушка для начальной расстановки
            /*start*/
            for (int i = 0; i < 8; i++)
            {
                DrawFigure(scale, Entity.Figures.Pawn, Entity.Side.Black, i, 1);
                DrawFigure(scale, Entity.Figures.Pawn, Entity.Side.White, i, 6);
                DrawFigure(scale, Entity.Figures.Rook, Entity.Side.White, i, 3);


            }

            DrawFigure(scale, Entity.Figures.Rook, Entity.Side.Black, 0, 0);
            DrawFigure(scale, Entity.Figures.Rook, Entity.Side.Black, 7, 0);
            DrawFigure(scale, Entity.Figures.Rook, Entity.Side.White, 0, 7);
            DrawFigure(scale, Entity.Figures.Rook, Entity.Side.White, 7, 7);

            DrawFigure(scale, Entity.Figures.Knight, Entity.Side.Black, 1, 0);
            DrawFigure(scale, Entity.Figures.Knight, Entity.Side.Black, 6, 0);
            DrawFigure(scale, Entity.Figures.Knight, Entity.Side.White, 1, 7);
            DrawFigure(scale, Entity.Figures.Knight, Entity.Side.White, 6, 7);

            /*end*/
        }

        private void DrawFigure(double scale, Chess.Entity.Figures figure, Entity.Side side, int col, int row, string strokeColor = null)
        {
            Path test = DrawFigure(scale, new(side, figure), strokeColor is null ? "#AAAAAA" : strokeColor);
            int testId = ChessBoard.Children.Add(test);
            Canvas.SetLeft(ChessBoard.Children[testId], (10) * scale + 47 * scale * col);
            Canvas.SetTop(ChessBoard.Children[testId], (10) * scale + 47 * scale * row);
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
                    int newBox = ChessBoard.Children.Add(new Rectangle() { Width = 47 * (size / 396), Height = 47 * (size / 396), Fill = (col + row) % 2 > 0 ? Brushes.Gray : Brushes.Azure, StrokeThickness = 0 });
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
          <PathFigure StartPoint='{(int)(5 * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)(5* scale)},{(int)(40 * scale)}' Point2='{(int)(8 * scale)},{(int)(40 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(17 * scale)},{(int)(41 * scale)}' Point2='{(int)(22 * scale)},{(int)(39 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(22 * scale)},{(int)(39 * scale)}' Point2='{(int)(22 * scale)},{(int)(15 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(19 * scale)},{(int)(11 * scale)}' Point2='{(int)(22 * scale)},{(int)(8 * scale)}' />

                <QuadraticBezierSegment Point1='{(int)(25 * scale)},{(int)(5 * scale)}' Point2='{(int)(28 * scale)},{(int)(8 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(31 * scale)},{(int)(11 * scale)}' Point2='{(int)(28 * scale)},{(int)(15 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(28 * scale)},{(int)(15 * scale)}' Point2='{(int)(28 * scale)},{(int)(39 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(33 * scale)},{(int)(41 * scale)}' Point2='{(int)(40 * scale)},{(int)(40 * scale)}' />

                <QuadraticBezierSegment Point1='{(int)(43 * scale)},{(int)(40 * scale)}' Point2='{(int)(43 * scale)},{(int)(43 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)(43 * scale)},{(int)(43 * scale)}' Point2='{(int)(4 * scale)},{(int)(43 * scale)}' />
                
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
