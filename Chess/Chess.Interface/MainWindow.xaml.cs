using Chess.ComputerPlayer;
using Chess.Entity;
using Chess.InterfaceTranslation;
using Chess.Logging;
using Chess.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Свойства игры

        private Board board = new Board();

        public Logger Logger;

        private int logId = 0;

        private Dictionary<CellPoint, List<CellPoint>> availableSteps;

        private UIElement?[,] figurePathPositions = new UIElement?[Board.CellBoardSize, Board.CellBoardSize];

        public ILanguage language;

        private bool started = false;
        public bool blocked = false;

        private Side currentStepSide = Side.White;

        public GameSettings GameSettings = new GameSettings();

        public string logText = "";

        private Side CurrentStepSide
        {
            get
            {
                return currentStepSide;
            }

            set
            {
                currentStepSide = value;
            }
        }

        #endregion

        #region Forms and Windows

        NewGameSettings newGameSettings;

        #endregion

        private UIElement? activeFigureUIE = null;

        private object activateHover = new object();
        private CellPoint activeCellPoint = CellPoint.Unexisted;

        public ILanguage CurrentLanguage { get; set; }

        public CellPoint ActiveCellPoint
        {
            get { return activeCellPoint; }
            set
            {
                lock (activateHover)
                {
                    if (started && !blocked)
                    {
                        if (value.X != activeCellPoint.X || value.Y != activeCellPoint.Y)
                        {
                            activeCellPoint = value;
                            ActiveBoxChanged(activeCellPoint);
                        }
                    }
                }
            }
        }

        private void ActiveBoxChanged(CellPoint activeCellPointChanged)
        {
            if (activeFigureUIE is not null)
            {
                ChessBoard.Children.Remove(activeFigureUIE);
                activeFigureUIE = null;
            }

            if (activeCellPointChanged != CellPoint.Unexisted)
            {
                if ((started && board.Positions[activeCellPointChanged.X, activeCellPointChanged.Y].Side == currentStepSide) || !started)
                    DrawFigureBorder(activeCellPointChanged.X, activeCellPointChanged.Y, "Blue", ref activeFigureUIE);
            }

            if (activeCellPointChanged == CellPoint.Unexisted)
            {
                ChessBoard.Children.Remove(activeFigureUIE);
                activeFigureUIE = null;
            }
        }

        private List<UIElement> selectedFigureUIEStepsBoxes = new List<UIElement>();

        private UIElement? selectedFigureUIE = null;

        private object clickHover = new object();
        private CellPoint clickCellPoint = CellPoint.Unexisted;

        public CellPoint ClickCellPoint
        {
            get { return clickCellPoint; }
            set
            {
                lock (clickHover)
                {
                    if (started && !blocked)
                    {
                        if ((value.X != clickCellPoint.X || value.Y != clickCellPoint.Y))
                        {
                            if (IsAvailableStep(value.X, value.Y))
                            {
                                MakeStep(value.X, value.Y);
                                availableSteps = board.GetAvailableSteps(board.CurrentStepSide);

                                try
                                {
                                    if (this.GameSettings.Player2Black == PlayerType.Computer)
                                    {
                                        MakeComputerStep();
                                        currentStepSide = board.CurrentStepSide;
                                    }
                                }
                                catch (GameEndedException ex) { }
                            }

                            clickCellPoint = value;
                            ClickBoxChanged(clickCellPoint);
                        }
                        else
                        {
                            UnselectCurrent();
                        }
                    }
                }
            }
        }

        private void MakeStep(sbyte x, sbyte y)
        {
            blocked = true;
            bool eat = board.Positions[x, y].Man != Figures.Empty;
            board.MakeStepWithoutChecking(new CellPoint() { X = clickCellPoint.X, Y = clickCellPoint.Y }, new CellPoint() { X = x, Y = y });
            Logger.Add(new StepEntity(new Step(new CellPoint() { X = clickCellPoint.X, Y = clickCellPoint.Y }, new CellPoint() { X = x, Y = y }), Board.GetOppositeSide(board.CurrentStepSide), board.CurrentStepSide, board.Positions[x, y], eat, board.IsCheck(board.CurrentStepSide), board.IsMate(board.CurrentStepSide), board.IsCheckmate(board.CurrentStepSide), ++logId, DateTime.UtcNow));
            currentStepSide = board.CurrentStepSide;
            PrintLog();
            if (!CkeckState())
                blocked = false;

            UnselectCurrent();
            Redraw();
            try
            {
                if (this.GameSettings.Player1White == PlayerType.Computer && blocked)
                    MakeComputerStep();
            }
            catch (GameEndedException ex) { }
            /*UnselectCurrent();
            Redraw();*/
        }

        private void MakeComputerStep()
        {
            blocked = true;
            FiveStepPlayer computerPlayer = new(board);
            var step = computerPlayer.MakeStep();
            bool eat = board.Positions[step.End.X, step.End.Y].Man != Figures.Empty;
            board.MakeStepWithoutChecking(new CellPoint() { X = step.Start.X, Y = step.Start.Y }, new CellPoint() { X = step.End.X, Y = step.End.Y });
            Logger.Add(new StepEntity(new Step(new CellPoint() { X = step.Start.X, Y = step.Start.Y }, new CellPoint() { X = step.End.X, Y = step.End.Y }), Board.GetOppositeSide(board.CurrentStepSide), board.CurrentStepSide, board.Positions[step.End.X, step.End.Y], eat, board.IsCheck(board.CurrentStepSide), board.IsMate(board.CurrentStepSide), board.IsCheckmate(board.CurrentStepSide), ++logId, DateTime.UtcNow));
            currentStepSide = board.CurrentStepSide;
            PrintLog();
            if (!CkeckState())
                blocked = false;
            availableSteps = board.GetAvailableSteps(currentStepSide);
            UnselectCurrent();
            Redraw();
        }

        private string GetCurrentLogString()
        {
            return logText;
        }

        private void AddLogString(LogEntity e)
        {
            logText += language.MakeShortLogString(e);
        }

        private void PrintLog()
        {
            InfoDesk.Clear();
            logText = "";
            foreach (var e in Logger.log)
            {
                if (e is StepEntity)
                {
                    PrintStepLogEntity(e as StepEntity);
                }
            }
        }

        private void PrintStepLogEntity(StepEntity e)
        {
            var entityString = String.Empty;
            var stepPlayerName = e.StartSide == Side.White ? (GameSettings?.Player1WhiteName == null ? "" : GameSettings?.Player1WhiteName) : (GameSettings?.Player1WhiteName == null ? "" : GameSettings?.Player1WhiteName);
            entityString += language.MakeShortLogString(e);
            logText += entityString + Environment.NewLine;
            InfoDesk.Text += entityString + Environment.NewLine;
        }

        private bool CkeckState()
        {
            if (board.IsCheckmate(currentStepSide))
            {
                if (currentStepSide == Side.White)
                {
                    ShowText(CurrentLanguage.MessagesStrings["BlackIsOnCheckmate"]);
                    return true;
                }
                else
                {
                    ShowText(CurrentLanguage.MessagesStrings["WhiteIsOnCheckmate"]);
                    return true;
                }
            }

            if (board.IsCheck(currentStepSide))
            {
                if (currentStepSide == Side.White)
                {
                    ShowText(CurrentLanguage.MessagesStrings["BlackIsOnCheck"]);
                    return true;
                }
                else
                {
                    ShowText(CurrentLanguage.MessagesStrings["WhiteIsOnCheck"]);
                    return true;
                }
            }

            if (board.IsMate(currentStepSide))
            {
                if (currentStepSide == Side.White)
                {
                    ShowText(CurrentLanguage.MessagesStrings["BlackIsOnMate"]);
                    return true;
                }
                else
                {
                    ShowText(CurrentLanguage.MessagesStrings["WhiteIsOnMate"]);
                    return true;
                }
            }

            return false;
        }

        private void MoveFigurePath(CellPoint start, CellPoint end)
        {
            Canvas.SetLeft(ChessBoard.Children[ChessBoard.Children.IndexOf(figurePathPositions[start.X, start.Y])],
                (10) * GetScale() + 47 * GetScale() * end.X);
            Canvas.SetTop(ChessBoard.Children[ChessBoard.Children.IndexOf(figurePathPositions[start.X, start.Y])],
                (10) * GetScale() + 47 * GetScale() * end.Y);

            ChessBoard.Children.Remove(figurePathPositions[end.X, end.Y]);

            figurePathPositions[end.X, end.Y] = figurePathPositions[start.X, start.Y];
            figurePathPositions[start.X, start.Y] = null;
        }

        private bool IsAvailableStep(sbyte x, sbyte y)
        {
            return availableSteps.Count(s => s.Key.X == clickCellPoint.X && s.Key.Y == clickCellPoint.Y && s.Value.Count(v => v.X == x && v.Y == y) > 0) > 0;
        }

        private void UnselectCurrent()
        {
            if (started && (selectedFigureUIE is not null))
            {
                ChessBoard.Children.Remove(selectedFigureUIE);
                selectedFigureUIE = null;
                clickCellPoint = CellPoint.Unexisted;
                DeleteHighlightBoxes();
            }
        }

        private void ClickBoxChanged(CellPoint clickCellPointCurrent)
        {
            if (started && selectedFigureUIE is not null/* && clickCellPointCurrent != CellPoint.Unexisted*/)
            {
                ChessBoard.Children.Remove(selectedFigureUIE);
                selectedFigureUIE = null;
                DeleteHighlightBoxes();
            }

            if (clickCellPointCurrent != CellPoint.Unexisted)
            {
                if (started && board.Positions[clickCellPointCurrent.X, clickCellPointCurrent.Y].Side == currentStepSide)
                {
                    DrawFigureBorder(clickCellPointCurrent.X, clickCellPointCurrent.Y, "Green", ref selectedFigureUIE);
                    DrawHiglightBoxes(clickCellPointCurrent);
                }
            }
        }

        private void DeleteHighlightBoxes()
        {
            foreach (var box in selectedFigureUIEStepsBoxes)
                ChessBoard.Children.Remove(box);

            selectedFigureUIEStepsBoxes.Clear();
        }

        private void DrawHiglightBoxes(CellPoint clickCellPointCurrent)
        {
            if (availableSteps.Keys.Where(k => k.X == clickCellPointCurrent.X && k.Y == clickCellPointCurrent.Y).Count() > 0)
            {
                CellPoint startCellPoint = availableSteps.Keys.Where(k => k.X == clickCellPointCurrent.X && k.Y == clickCellPointCurrent.Y).First();
                foreach (var endStep in availableSteps[startCellPoint])
                {
                    DrawSquaresHighlighter(GetSize(), endStep.X, endStep.Y);
                }
            }

        }

        double size;

        public MainWindow()
        {
            newGameSettings = new NewGameSettings(this);

            InitializeComponent();

            this.Width = 800;
            this.Height = 450;
            this.MinWidth = 600;
            this.MinHeight = 450;
            Logger = new(GameSettings);
            availableSteps = new Dictionary<CellPoint, List<CellPoint>>();
            ChangeLanguage(new EnglishLanguage());
            DrawDesk(this.Height);
        }

        private void ChangeLanguage(ILanguage lng)
        {
            language = lng;
            language.MakeInterfaceTranslation(this, newGameSettings);

            foreach (MenuItem l in this.Language.Items)
                l.IsChecked = false;

            CurrentLanguage = language;

            if (language is EnglishLanguage)
            {
                ((MenuItem)Language.FindName("English")).IsChecked = true;
            }

            if (language is RussianTranslation)
            {
                ((MenuItem)Language.FindName("Russian")).IsChecked = true;
            }

            PrintLog();
        }

        public void ResetBoard()
        {
            this.board = new Board();
            availableSteps = board.GetAvailableStepsPre(currentStepSide);
            currentStepSide = Side.White;
            logText = String.Empty;
            Redraw();
        }

        public void StartGame()
        {
            BlackPlayerNameLabel.Content = GameSettings.Player2BlackName;
            WhitePlayerNameLabel.Content = GameSettings.Player1WhiteName;
            started = true;
            blocked = this.GameSettings.Player1White == PlayerType.Computer;
            Logger = new(GameSettings);
            logId = 0;
            ShowText(CurrentLanguage.MessagesStrings["TheGameIsStarted"]);
        }

        public void EndGame()
        {
            UnselectCurrent();
            ActiveBoxChanged(CellPoint.Unexisted);
            ClickBoxChanged(CellPoint.Unexisted);
            started = false;
            blocked = true;
        }

        Label gameInfoLabel = new Label() { Name = "LabelInfo", Content = "", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 48, FontWeight = FontWeights.Bold, Opacity = 0, Foreground = Brushes.Blue };
        public void ShowText(string text)
        {
            Grid? grid = (Grid?)ChessBoard.FindName("MainGrid");

            _ = grid?.Children.Add(gameInfoLabel);

            Grid.SetColumn(gameInfoLabel, 0);

            if (text != null)
            {
                gameInfoLabel.Content = text;
                gameInfoLabel.Opacity = 100;
                gameInfoLabel.FontSize = 28 * GetScale();
                DoubleAnimation textAnimation = new DoubleAnimation();
                textAnimation.From = 100;
                textAnimation.To = 0;
                textAnimation.Duration = TimeSpan.FromMilliseconds(3000);
                textAnimation.Completed += Window_CompliteShowText;
                gameInfoLabel.BeginAnimation(Label.OpacityProperty, textAnimation);
            }
        }

        private void Window_CompliteShowText(object? sender, EventArgs e)
        {
            Grid? grid = (Grid?)ChessBoard.FindName("MainGrid");
            grid?.Children.Remove(gameInfoLabel);
            blocked = this.GameSettings.Player1White == PlayerType.Computer;

            if (blocked)
                MakeComputerStep();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
            gameInfoLabel.FontSize = 48 * GetScale();
        }

        private void Redraw()
        {
            DrawDesk(GetSize());
            DrawSelectedAndHoveredFigures();
        }

        private void DrawSelectedAndHoveredFigures()
        {
            if (started && !blocked)
            {
                activeFigureUIE = null;
                selectedFigureUIE = null;
                ActiveBoxChanged(activeCellPoint);
                ClickBoxChanged(clickCellPoint);
            }
        }

        private void DrawDesk(double size)
        {
            this.size = size;
            ChessBoard.Width = size;
            ChessBoard.Height = ChessBoard.Width;

            DrawBoard(size);
            double scale = GetScale();
            DrawBoardFigures(scale);
        }

        private double GetSize()
        {

            //MainGrid.ColumnDefinitions[0]
            (var width, var height) = (MainGrid.ColumnDefinitions[0].ActualWidth - 150, this.ActualHeight - 100);

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
            for (int i = 0; i < Board.CellBoardSize; i++)
            {
                for (int j = 0; j < Board.CellBoardSize; j++)
                {
                    var figure = board.Positions[i, j];
                    if (figure.Man is not Figures.Empty)
                    {
                        figurePathPositions[i, j] = DrawFigure(scale, figure.Man, figure.Side, i, j);
                    }
                }
            }
        }

        private UIElement DrawFigure(double scale, Chess.Entity.Figures figure, Entity.Side side, int col, int row, string? strokeColor = null)
        {
            Path figureCurrent = DrawFigure(scale, new(side, figure), strokeColor is null ? "#AAAAAA" : strokeColor);
            figureCurrent.Name = $"Figure_{col}_{row}";
            ChessBoard.Children.Add(figureCurrent);
            Canvas.SetLeft(figureCurrent, (10) * scale + 47 * scale * col);
            Canvas.SetTop(figureCurrent, (10) * scale + 47 * scale * row);
            return figureCurrent;
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
                Canvas.SetTop(ChessBoard.Children[leftNum], 10 * (size / 396) + (47 / 2) * (size / 396) + 47 * (size / 396) * row - baseFontSize / 2 * (size / 396));
                int rightNum = ChessBoard.Children.Add(new Label() { Content = rowNum, Padding = new Thickness(0, 0, 0, 0), FontSize = baseFontSize * (size / 396) });
                Canvas.SetLeft(ChessBoard.Children[rightNum], 386 * (size / 396) + baseFontSize / 3 * (size / 396));
                Canvas.SetTop(ChessBoard.Children[rightNum], 10 * (size / 396) + (47 / 2) * (size / 396) + 47 * (size / 396) * row - baseFontSize / 2 * (size / 396));
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
            foreach (var letter in topPositions)
            {
                row = 0;
                foreach (var rowNum in leftPositions)
                {
                    int newBox = ChessBoard.Children.Add(new Rectangle() { Width = Math.Round(47 * (size / 396), 0), Height = Math.Round(47 * (size / 396), 0), Fill = (col + row) % 2 > 0 ? Brushes.Gray : Brushes.Azure, StrokeThickness = 1, Stroke = (col + row) % 2 > 0 ? Brushes.Gray : Brushes.Azure, Name = $"Cell_{col}_{row}" });
                    Canvas.SetLeft(ChessBoard.Children[newBox], Math.Round(10 * (size / 396) + 47 * (size / 396) * col, 0));
                    Canvas.SetTop(ChessBoard.Children[newBox], Math.Round(10 * (size / 396) + 47 * (size / 396) * row, 0));

                    row++;
                }
                col++;
            }
        }

        // Метод для подстветки возможных ходов
        private void DrawSquaresHighlighter(double size, int col, int row)
        {
            UIElement cellHighlight = new Rectangle() { Width = Math.Round(47 * (size / 396), 0) + 2, Height = Math.Round(47 * (size / 396), 0) + 2, Fill = Brushes.Green, StrokeThickness = 4, Stroke = Brushes.Red, Name = $"CellHighlight_{col}_{row}", Opacity = .5 };
            int newBox = ChessBoard.Children.Add(cellHighlight);
            selectedFigureUIEStepsBoxes.Add(cellHighlight);
            Canvas.SetLeft(ChessBoard.Children[newBox], Math.Round(10 * (size / 396) + 47 * (size / 396) * col, 0) - 1);
            Canvas.SetTop(ChessBoard.Children[newBox], Math.Round(10 * (size / 396) + 47 * (size / 396) * row, 0) - 1);

        }

        private void DrawFigureBorder(int col, int row, string color, ref UIElement? activeId)
        {
            if (col >= 0 && row >= 0)
            {
                if (!(board.Positions[col, row].Man is Figures.Empty))
                    activeId = DrawFigure(GetScale(), board.Positions[col, row].Man, board.Positions[col, row].Side, col, row, color);
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
StrokeThickness='{(int)(2 * scale)}' Fill='{GetHtmlColorOfFigureSide(figure.Side)}'>
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
          <PathFigure StartPoint='{(int)((9 + 1) * scale)},{(int)(43 * scale)}'>
            <PathFigure.Segments>
              <PathSegmentCollection>
                <QuadraticBezierSegment Point1='{(int)((9 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' />
                <QuadraticBezierSegment Point1='{(int)((15 + 1) * scale)},{(int)(35 * scale)}' Point2='{(int)((15 + 1) * scale)},{(int)(20 * scale)}' />
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
            //_ => throw new ArgumentOutOfRangeException(nameof(figure.Man)),
        };

        private string GetHtmlColorOfFigureSide(Chess.Entity.Side side) => side switch
        {
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

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            newGameSettings.Visibility = Visibility.Visible;
            newGameSettings.Focus();
            newGameSettings.Topmost = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            newGameSettings.IsMainWindowClosing = true;
            newGameSettings.Close();
        }

        private void ChessBoard_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventsSimple(sender, e, (c) => ActiveCellPoint = c, (c) => ActiveCellPoint = c);
        }

        private void ChessBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseEventsSimple(sender, e, (c) => ClickCellPoint = c, (c) => ClickCellPoint = c);
        }

        public delegate void MouseEventSimpleHandler(CellPoint cellPoint);

        public void MouseEventsSimple(object sender, MouseEventArgs e, MouseEventSimpleHandler selectHandler, MouseEventSimpleHandler unexistedHandler)
        {
            var mousePosition = e.GetPosition(ChessBoard);
            var boxSize = 47 * GetScale();
            var offset = 10 * GetScale();
            mousePosition.Offset(-offset, -offset);

            if (mousePosition.X >= 0 && mousePosition.X <= (boxSize * 8) && mousePosition.Y >= 0 && mousePosition.Y <= (boxSize * 8))
            {
                int cellColumn = (int)Math.Truncate(mousePosition.X / boxSize);
                int cellRow = (int)Math.Truncate(mousePosition.Y / boxSize);

                CellPoint cellPoint = new CellPoint() { X = (sbyte)cellColumn, Y = (sbyte)cellRow };

                selectHandler(cellPoint);

                InfoDesk.Text = logText + language.MakeMousePositionMessage(cellPoint);
            }
            else
            {
                InfoDesk.Text = logText;
                unexistedHandler(CellPoint.Unexisted);
            }
        }

        private void InfoDesk_TextChanged(object sender, TextChangedEventArgs e)
        {
            InfoDesk.ScrollToEnd();
        }

        private void ChessBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            InfoDesk.Text = logText;
        }

        private void English_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage(new EnglishLanguage());
        }

        private void Russian_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage(new RussianTranslation());
        }
    }
}
