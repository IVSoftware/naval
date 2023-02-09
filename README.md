One approach that you might find helpful would be to bundle up all the information about a `Ship` into a class. This is an [abstraction](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop) that could make it easier for **displaying ship names when they are sunk**. At the same time, use [inheritance](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop) so that a `Ship` is still a `PictureBox` with all the functionality that implies.
***
**Ship minimal class example**

Member properties tell us what we know about a ship. Use `enum` values to make the intent perfectly clear. 

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
                }
            }
        }

        [Description("Flag")]
        public σημαία σημαία { get; set; }
        public bool Sunk { get; set; }

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
Another thing would be to combine the functionality of things like `playerBoard` and `playerShips` into another inherited class that is all-in-one. 

    // Initially configured as a 10 x 10 grid.
    class TableLayoutPanelNaval : TableLayoutPanel
    {
        public Ship this[int column, int row]
        {
            get
            {
                return (Ship)GetControlFromPosition(column, row);
            }
        }
    }

 ***
 **Initialize board and game**

 By adding a `Click` handler to every `Ship`, the information you require to sink a ship is readily available.









