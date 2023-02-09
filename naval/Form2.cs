using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;

namespace naval
{
    public partial class Form2 : Form
    {
        internal const int Size_grid = 10;
        internal Color ColorUp = Color.LightGray;
        internal Color ColorDown = Color.DarkGray;
        public Form2() =>InitializeComponent();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            buttonNewGame.Click += onNewGame;
            buttonNewGame.PerformClick();
        }

        private void onNewGame(object sender, EventArgs e)
        {
            playerBoard.Visible = false;
            opponentBoard.Visible = false;
            playerBoard.Controls.Clear();
            opponentBoard.Controls.Clear();
            PlaceShips(grid: playerBoard, flag: σημαία.Player);
            PlaceShips(grid: opponentBoard, flag: σημαία.Opponent);
            playerBoard.Visible = true;
            opponentBoard.Visible = true;
        }

#if DEBUG
        private readonly Random _random = new Random(10);
#else
        // Random, by default, seeds using the current DateTime.
        private readonly Random _random = new Random();
#endif
        private void PlaceShips(TableLayoutPanelNaval grid, σημαία flag)
        {
            foreach (τύπος shipType in Enum.GetValues(typeof(τύπος)))
            {
                const int MAX_TRIES = 100;
                int tries = 1;
                Direction direction = (Direction)_random.Next(2);
             retry:
                if(tries < MAX_TRIES)
                {
                    int column, row;
                    switch (direction)
                    {
                        case Direction.Horizontal:
                            row = _random.Next(Size_grid);
                            column = _random.Next(Size_grid - (int)shipType);
                            break;
                        case Direction.Vertical:
                            row = _random.Next(Size_grid - (int)shipType);
                            column = _random.Next(Size_grid);
                            break;
                        default: throw new NotImplementedException();
                    }

                    int span = (int)shipType;
                    Point[] hits;
                    switch (direction)
                    {
                        case Direction.Horizontal:
                            hits = Enumerable.Range(column, span).Select(_ => new Point(_, row)).ToArray();
                            break;
                        case Direction.Vertical:
                            hits = Enumerable.Range(row, span).Select(_ => new Point(column, _)).ToArray();
                            break;
                        default: throw new NotImplementedException();
                    }

                    foreach (var hit in hits)
                    {
                        if (grid.GetControlFromPosition(hit.X, hit.Y) != null)
                        {
                            tries++;
                            goto retry;
                        }
                    }
                    Ship ship = new Ship
                    {
                        τύπος = shipType,
                        σημαία = flag,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackColor = flag.Equals(σημαία.Player) ? Color.CadetBlue : Color.LightSalmon,
                        Anchor = (AnchorStyles)0xF, // Let table layout panel set the size
                        Hits = hits,
                    };
                    switch (direction)
                    {
                        case Direction.Horizontal: grid.SetColumnSpan(ship, span); break;
                        case Direction.Vertical:
                            grid.SetRowSpan(ship, span);
                            ship.Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        default: throw new NotImplementedException();
                    }
                    ship.Click += onAnyShipClick;
                    grid.Add(ship, column, row, hidden: flag.Equals(σημαία.Opponent));
                }
            }
            grid.AddMisses();
        }

        private void onAnyShipClick(object sender, EventArgs e)
        {
            if (sender is Ship ship)
            {
                if (ship.σημαία.Equals(σημαία.Opponent) && ship.Hits.Any())
                {
                    Point position = ship.Hits[0];
                    opponentBoard.Visible = false;
                    ship.Sunk = true;
                    foreach (var hit in ship.Hits)
                    {
                        opponentBoard.Controls.Remove(opponentBoard.GetControlFromPosition(hit.X, hit.Y));
                    }
                    ship.Hits = new Point[0];
                    opponentBoard.Controls.Add(ship, position.X, position.Y);
                    opponentBoard.Visible = true;
                }
                else
                {
                    MessageBox.Show(ship.ToString());
                }
            }
        }
    }
    enum τύπος
    {
        [Description("Aircraft Carrier")]
        Αεροπλανοφόρο = 5,

        [Description("Destroyer")]
        Αντιτορπιλικό = 4,

        [Description("Military")]
        Πολεμικό = 3,

        [Description("Submarine")]
        Υποβρύχιο = 2,
    }
    /// <summary>
    /// Flag
    /// </summary>
    enum σημαία
    {
        Player,
        Opponent,
    }
    enum Direction
    {
        Horizontal,
        Vertical,
    }
    class Ship : PictureBox
    {
        τύπος _τύπος = 0;
        public τύπος τύπος
        {
            get => _τύπος;
            set
            {
                if (!Equals(_τύπος, value))
                {
                    _τύπος = value;
                    switch (_τύπος)
                    {
                        case τύπος.Αεροπλανοφόρο: Image = Image.FromFile(Path.Combine(_imageDir, "aircraft-carrier.png")); break;
                        case τύπος.Αντιτορπιλικό: Image = Image.FromFile(Path.Combine(_imageDir, "destroyer.png")); break;
                        case τύπος.Πολεμικό: Image = Image.FromFile(Path.Combine(_imageDir, "military.png")); break;
                        case τύπος.Υποβρύχιο: Image = Image.FromFile(Path.Combine(_imageDir, "submarine.png")); break;
                    }
                    OnPropertyChanged();
                }
            }
        }

        [Description("Flag")]
        σημαία _σημαία = 0;
        public σημαία  σημαία 
        {
            get => _σημαία ;
            set
            {
                if (!Equals(_σημαία , value))
                {
                    _σημαία  = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _sunk = false;
        public bool Sunk
        {
            get => _sunk;
            set
            {
                if (!Equals(_sunk, value))
                {
                    _sunk = value;
                    OnPropertyChanged();
                }
            }
        }

        private void onUpdateColor()
        {
            var color = 
                Sunk? Color.Red :
                    σημαία.Equals(σημαία.Player) ? 
                        Color.Navy : 
                        Color.DarkOliveGreen;
            for (int x = 0; x < Image.Width; x++) for (int y = 0; y < Image.Height; y++)
                {
                    Bitmap bitmap = (Bitmap)Image;
                    if (bitmap.GetPixel(x, y).R < 0x80)
                    {
                        bitmap.SetPixel(x, y, color);
                    }
                }
            Refresh();
        }
        public void PerformClick() => base.OnClick(EventArgs.Empty);
        public Point[] Hits { get; set; } = new Point[0];
        public override string ToString() =>
            $"{σημαία} {τύπος} @ {((TableLayoutPanel)Parent)?.GetCellPosition(this)}";
        private readonly static string _imageDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            onUpdateColor();
        }
    }
    class TableLayoutPanelNaval : TableLayoutPanel
    {
        public void Add(Ship ship, int column, int row, bool hidden) 
        {
            switch (ship.σημαία)
            {
                case σημαία.Player: Controls.Add(ship, column, row); break;
                case σημαία.Opponent:
                    foreach (var point in ship.Hits)
                    {
                        Panel hit = new Panel { BackColor = Color.Aqua };
                        // Forward to hidden ship
                        hit.Click += (sender, e) => ship.PerformClick();
                        Controls.Add(hit, point.X, point.Y);
                    }
                    break;
                default: throw new NotImplementedException();
            }
        }

        internal void AddMisses()
        {
            var dim = Form2.Size_grid;
            for (int column = 0; column < dim; column++) for (int row = 0; row < dim; row++)
                {
                    if (GetControlFromPosition(column, row) == null)
                    {
                        var miss = new Panel { BackColor = Color.LightGray };
                        miss.Click += (sender, e) => ((Control)sender).BackColor = Color.DarkGray;
                        Controls.Add(miss, column, row);
                    }
                }
        }
    }
}
