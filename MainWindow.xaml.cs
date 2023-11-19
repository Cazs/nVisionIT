using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace nVisionLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class Cell
    {
        public Cell(System.Drawing.Point cellPosition, Rectangle gfx, bool isCellAlive)
        {
            this.Position = cellPosition;
            this.Alive = isCellAlive;
            this.CellGfx = gfx;
        }

        public Rectangle CellGfx { get; set; }
        
        public bool Alive { get; set; }

        public System.Drawing.Point Position { get; set; }
    }

    public partial class MainWindow : Window
    {
        // TODO: CMD inputs or Form Control inputs?
        static int MAP_GRID_WIDTH = 120;
        static int MAP_GRID_HEIGHT = 60;
        const int MAP_REFRESH_RATE = 1;
        const int CELL_ODDS_OF_LIFE = 70;
        const int CELL_SIZE = 10;
        const int CELL_SPACING = 2;

        bool run = false;
        Cell[,] cells = new Cell[MAP_GRID_WIDTH, MAP_GRID_HEIGHT];

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

            Canvas.SetTop(this.lifeCanvas, CELL_SIZE);
            Canvas.SetLeft(this.lifeCanvas, CELL_SIZE);

            this.StartGameLoopAsync();
        }
        private async void StartGameLoopAsync()
        {
            int gridElemCount = MAP_GRID_HEIGHT * MAP_GRID_WIDTH;

            // Initialise cell/population grid
            for (int y = 0; y < MAP_GRID_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_GRID_WIDTH; x++)
                {
                    // Get a random number from 1 till width multiplied by height
                    int randomNumber = getRandomNumber(1, gridElemCount);
                    int odds = Math.DivRem(gridElemCount, randomNumber).Remainder; // TODO: Odds of life formula

                    cells[x, y] = new Cell(new System.Drawing.Point(x, y),
                        new Rectangle
                        {
                            Height = CELL_SIZE,
                            Width = CELL_SIZE,
                        },
                        odds <= CELL_ODDS_OF_LIFE ? true : false
                    );

                    this.lifeCanvas.Children.Add(cells[x, y].CellGfx);
                }
            }

            // Start Game Loop
            while (true)
            {
                await DrawRectanglesAsync();
            }
        }

        public async Task DrawRectanglesAsync()
        {   
            for (int j = 0; j < MAP_GRID_HEIGHT; j++)
            {
                for (int i = 0; i < MAP_GRID_WIDTH; i++)
                {
                    if (cells[i, j].Alive)
                    {
                        // Cell is alive!
                        Canvas.SetLeft(cells[i, j].CellGfx, i * (CELL_SIZE + CELL_SPACING));
                        Canvas.SetTop(cells[i, j].CellGfx, j * (CELL_SIZE + CELL_SPACING));

                        cells[i, j].CellGfx.Stroke = new SolidColorBrush(Colors.White);
                        cells[i, j].CellGfx.Fill = new SolidColorBrush(Colors.Black);

                        // Check if cell has 2 or 3 neighbours
                        int liveNeighboursCount = getLiveNeighbours(cells[i, j]).Length;
                        cells[i, j].Alive = (liveNeighboursCount >= 2 && liveNeighboursCount <= 3);

                        await Task.Delay(MAP_REFRESH_RATE);
                    }
                    else if(!cells[i, j].Alive)
                    {
                        // Dead cell, check if has 3 live neighbours
                        Canvas.SetLeft(cells[i, j].CellGfx, i * (CELL_SIZE + CELL_SPACING));
                        Canvas.SetTop(cells[i, j].CellGfx, j * (CELL_SIZE + CELL_SPACING));

                        cells[i, j].CellGfx.Stroke = new SolidColorBrush(Colors.White);
                        cells[i, j].CellGfx.Fill = new SolidColorBrush(Colors.DarkGray);

                        int liveNeighboursCount = getLiveNeighbours(cells[i, j]).Length;
                        cells[i, j].Alive = (liveNeighboursCount == 3);
                    }
                }
            }
        }

        private Cell[] getLiveNeighbours(Cell cell)
        {
            Cell[] neighbouringCells = {
                cell.Position.X - 1 >= 0 ? cells[cell.Position.X - 1, cell.Position.Y] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
                cell.Position.X - 1 >= 0 && cell.Position.Y - 1 >= 0 ? cells[cell.Position.X - 1, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
                cell.Position.X - 1 >= 0 && cell.Position.Y + 1 < MAP_GRID_HEIGHT ? cells[cell.Position.X - 1, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
                cell.Position.Y - 1 >= 0 ? cells[cell.Position.X, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
                //
                cell.Position.X + 1 < MAP_GRID_WIDTH ? cells[cell.Position.X + 1, cell.Position.Y] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
                cell.Position.X + 1 < MAP_GRID_WIDTH && cell.Position.Y + 1 < MAP_GRID_HEIGHT ? cells[cell.Position.X + 1, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
                cell.Position.X + 1 < MAP_GRID_WIDTH && cell.Position.Y - 1 >= 0 ? cells[cell.Position.X + 1, cell.Position.Y - 1] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
                cell.Position.Y + 1 < MAP_GRID_HEIGHT ? cells[cell.Position.X, cell.Position.Y + 1] : new Cell(new System.Drawing.Point(0, 0), new Rectangle(), false),
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
