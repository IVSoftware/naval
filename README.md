One approach that you might find helpful would be to bundle up all the information about a `Ship` into a class. This is an [abstraction](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop) that could make it easier for **displaying ship names when they are sunk**. At the same time, use [inheritance](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop) so that a `Ship` is still a `PictureBox` with all the functionality that implies.
***
**Ship minimal class example**

Member properties tell us what we know about a ship. Use `enum` values to make the intent perfectly clear. 

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
        public σημαία σημαία
        {
            get => _σημαία;
            set
            {
                _σημαία = value;
                onUpdateColor();
            }
        }
        σημαία _σημαία = σημαία.Player;

        private void onUpdateColor()
        {
            var color =
                Sunk ? Color.Red :
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
        #endregion P R O P E R T I E S

        public Point[] Hits { get; set; } = new Point[0];
        public override string ToString() =>
            $"{σημαία} {τύπος} @ {((TableLayoutPanel)Parent)?.GetCellPosition(this)}";

        private readonly static string _imageDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
    }

***
_Where `enum` values are:_

    enum Direction
    {
        Horizontal,
        Vertical,
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

 ***
 **Displaying ships names when they are sunk** 

 When the inherited `Ship` version of `PictureBox` is clicked the information is now available.

    private void onAnyShipClick(object sender, EventArgs e)
    {
        if (sender is Ship ship)
        {
            MessageBox.Show(ship.ToString());
        }
    }