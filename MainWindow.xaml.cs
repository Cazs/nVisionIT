using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace nVisionLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public class Cell
    {
        public Cell(System.Drawing.Point cellPosition, bool isCellAlive) {
            this.Position = cellPosition;
            this.Alive = isCellAlive;
        }

        public bool Alive { get; set; }

        public System.Drawing.Point Position { get; set; }
    }

    public partial class MainWindow : Window
    {
        // TODO: CMD inputs or Form Control inputs?
        static int gridW = 120;
        static int gridH = 75;
        const int REFRESH_RATE = 2;
        const int ODDS_OF_BIRTH = 120;
        bool run = false;
        Cell[,] cells = new Cell[gridW, gridH];

        public MainWindow()
        {
            InitializeComponent();
            this.run = true; // TODO: Pause/Stop Evolution logic
            this.initCanvas();
        }

        private void initCanvas()
        {
            this.WindowState = WindowState.Maximized;

            this.lifeCanvas.Background = Brushes.Blue;

            Canvas.SetTop(this.lifeCanvas, 50);
            Canvas.SetLeft(this.lifeCanvas, 50);

            this.StartGameLoopAsync();
        }

        private async Task StartGameLoopAsync()
        {
            int gridElemCount = gridH * gridW;

            // Initialise cell/population grid
            for (int y = 0; y < gridH; y++)
            {
                for (int x = 0; x < gridW; x++)
                {
                    // Get a random number from 1 till width multiplied by height
                    int randomNumber = getRandomNumber(1, gridElemCount);
                    int odds = Math.DivRem(gridElemCount, randomNumber).Remainder; // TODO: Odds of life formula

                    cells[x, y] = new Cell(new System.Drawing.Point(x, y), odds <= ODDS_OF_BIRTH ? true : false);
                }
            }

            // Start Game Loop
            while (true)
            {
                await DrawRectanglesAsync(this.lifeCanvas);
                await Task.Delay(REFRESH_RATE);
                Thread.Sleep(REFRESH_RATE);
            }
        }

        public async Task DrawRectanglesAsync(Canvas cgolCanvas)
        {
            // Clear canvas for the next generation of cells
            cgolCanvas.Children.Clear();

            var size = 10;
            var space = 2;

            for (int j = 0; j < gridH; j++)
            {
                for (int i = 0; i < gridW; i++)
                {
                    if (cells[i, j].Alive)
                    {
                        // Cell is alive!

                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
                        {
                            Height = size,
                            Width = size,
                        };

                        rectangle.Stroke = new SolidColorBrush(Colors.White);
                        rectangle.Fill = new SolidColorBrush(Colors.Black);
                        cgolCanvas.Children.Add(rectangle);

                        Canvas.SetLeft(rectangle, i * (size + space));
                        Canvas.SetTop(rectangle, j * (size + space));

                        // Check if cell has 2 or 3 neighbours
                        int liveNeighboursCount = getLiveNeighbours(cells[i, j]).Length;
                        cells[i, j].Alive = (liveNeighboursCount >= 2 && liveNeighboursCount <= 3);

                        await Task.Delay(REFRESH_RATE);
                    } else
                    {
                        // Dead cell, check if has 3 live neighbours
                        int liveNeighboursCount = getLiveNeighbours(cells[i, j]).Length;
                        cells[i, j].Alive = (liveNeighboursCount == 3);
                    }
                }
            }
        }
        
        private Cell[] getLiveNeighbours(Cell cell)
        {
            Cell[] neighbouringCells = {
                cell.Position.X - 1 >= 0 ? cells[cell.Position.X - 1, cell.Position.Y] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.X - 1 >= 0 && cell.Position.Y - 1 >= 0 ? cells[cell.Position.X - 1, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.X - 1 >= 0 && cell.Position.Y + 1 < gridH ? cells[cell.Position.X - 1, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.Y - 1 >= 0 ? cells[cell.Position.X, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), false),
                //
                cell.Position.X + 1 < gridW ? cells[cell.Position.X + 1, cell.Position.Y] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.X + 1 < gridW && cell.Position.Y + 1 < gridH ? cells[cell.Position.X + 1, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.X + 1 < gridW && cell.Position.Y - 1 >= 0 ? cells[cell.Position.X + 1, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.Y + 1 < gridH ? cells[cell.Position.X, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), false),
            };

            return neighbouringCells.Where(x => x.Alive).ToArray();
        }

        private int getRandomNumber(int min, int max)
        {
            Random random = new Random();
            int number = random.Next(min, max);
            return number;
        }
    }
}
