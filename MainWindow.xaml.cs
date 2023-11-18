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
        // TODO: CMD inputs or Form Control inputs for constant and static vars
        static int WORLD_GRID_WIDTH = 130;
        static int WORLD_GRID_HEIGHT = 60;

        const int WORLD_REFRESH_RATE = 3;
        const int CELL_ODDS_OF_LIFE = 130;
        const int CELL_SIZE = 10;
        const int CELL_SPACING = 2;

        Cell[,] cells = new Cell[WORLD_GRID_WIDTH, WORLD_GRID_HEIGHT];
        bool run = false;

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

            Canvas.SetTop(this.lifeCanvas, 10);
            Canvas.SetLeft(this.lifeCanvas, 10);

            _ = this.StartGameLoopAsync();
        }

        private async Task StartGameLoopAsync()
        {
            int gridElemCount = WORLD_GRID_HEIGHT * WORLD_GRID_WIDTH;

            // Initialise cell/population grid
            for (int y = 0; y < WORLD_GRID_HEIGHT; y++)
            {
                for (int x = 0; x < WORLD_GRID_WIDTH; x++)
                {
                    // Get a random number from 1 till width multiplied by height
                    int randomNumber = getRandomNumber(1, gridElemCount);
                    int odds = Math.DivRem(gridElemCount, randomNumber).Remainder; // TODO: Odds of life formula

                    cells[x, y] = new Cell(new System.Drawing.Point(x, y), odds <= CELL_ODDS_OF_LIFE ? true : false);
                }
            }

            // Start Game Loop
            while (true)
            {
                await DrawRectanglesAsync(this.lifeCanvas);
                await Task.Delay(WORLD_REFRESH_RATE);
                // Thread.Sleep(WORLD_REFRESH_RATE);
            }
        }

        public async Task DrawRectanglesAsync(Canvas cgolCanvas)
        {
            // Clear canvas for the next generation of cells
            cgolCanvas.Children.Clear();

            for (int j = 0; j < WORLD_GRID_HEIGHT; j++)
            {
                for (int i = 0; i < WORLD_GRID_WIDTH; i++)
                {
                    if (cells[i, j].Alive)
                    {
                        // Cell is alive!

                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
                        {
                            Height = CELL_SIZE,
                            Width = CELL_SIZE,
                        };

                        rectangle.Stroke = new SolidColorBrush(Colors.White);
                        rectangle.Fill = new SolidColorBrush(Colors.Black);
                        cgolCanvas.Children.Add(rectangle);

                        Canvas.SetLeft(rectangle, i * (CELL_SIZE + CELL_SPACING));
                        Canvas.SetTop(rectangle, j * (CELL_SIZE + CELL_SPACING));

                        // Check if cell has 2 or 3 neighbours
                        int liveNeighboursCount = getLiveNeighbours(cells[i, j]).Length;
                        cells[i, j].Alive = (liveNeighboursCount >= 2 && liveNeighboursCount <= 3);

                        await Task.Delay(WORLD_REFRESH_RATE);
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
                cell.Position.X - 1 >= 0 && cell.Position.Y + 1 < WORLD_GRID_HEIGHT ? cells[cell.Position.X - 1, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.Y - 1 >= 0 ? cells[cell.Position.X, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), false),
                //
                cell.Position.X + 1 < WORLD_GRID_WIDTH ? cells[cell.Position.X + 1, cell.Position.Y] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.X + 1 < WORLD_GRID_WIDTH && cell.Position.Y + 1 < WORLD_GRID_HEIGHT ? cells[cell.Position.X + 1, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.X + 1 < WORLD_GRID_WIDTH && cell.Position.Y - 1 >= 0 ? cells[cell.Position.X + 1, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), false),
                cell.Position.Y + 1 < WORLD_GRID_HEIGHT ? cells[cell.Position.X, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), false),
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
