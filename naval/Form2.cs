﻿using System;
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
        public Form2() =>InitializeComponent();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Size = new Size(500, 800);
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
                        BackColor = flag.Equals(σημαία.Player) ? Color.CadetBlue : Color.DarkOliveGreen,
                        Anchor = (AnchorStyles)0xF, // Let table layout panel set the size
                        Hits = hits,
                        Padding = new Padding(0),
                        Margin = new Padding(1),
                        BorderStyle = BorderStyle.FixedSingle,
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
                    grid.Add(ship, column, row);
                }
            }
            grid.AddMisses();
        }

        private void onAnyShipClick(object sender, EventArgs e)
        {
            if (sender is Ship ship)
            {
                MessageBox.Show(ship.ToString());
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
        #region P R O P E R T I E S
        [Description("Type")]
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
                }
            }
        }
        τύπος _τύπος = 0;

        public bool Sunk { get; set; }

        [Description("Flag")]
        public σημαία  σημαία { get; set; }
        #endregion P R O P E R T I E S

        public Point[] Hits { get; set; } = new Point[0];
        public override string ToString() =>
            $"{σημαία} {τύπος} @ {((TableLayoutPanel)Parent)?.GetCellPosition(this)}";

        private readonly static string _imageDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
    }
    class TableLayoutPanelNaval : TableLayoutPanel
    {
        public void Add(Ship ship, int column, int row) 
        {
            Controls.Add(ship, column, row); 
        }

        internal void AddMisses()
        {
            var dim = Form2.Size_grid;
            for (int column = 0; column < dim; column++) for (int row = 0; row < dim; row++)
                {
                    if (GetControlFromPosition(column, row) == null)
                    {
                        var miss = new Panel 
                        { 
                            BackColor = Color.LightGray,
                            Padding = new Padding(0),
                            Margin = new Padding(1),
                            BorderStyle = BorderStyle.FixedSingle,
                        };
                        miss.Click += (sender, e) => ((Control)sender).BackColor = Color.DarkGray;
                        Controls.Add(miss, column, row);
                    }
                }
        }
    }
}
