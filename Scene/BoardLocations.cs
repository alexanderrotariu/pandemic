using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class City
{
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public DiseaseType Region { get; set; }
    public List<string> ConnectedCities { get; set; }
    public int DiseaseCount { get; set; }

    public City(string name, Vector2 position, DiseaseType region)
    {
        Name = name;
        Position = position;
        Region = region;
        ConnectedCities = new List<string>();
        DiseaseCount = 0;
    }
}

public enum DiseaseType
{
    Blue,    // North America & Europe
    Yellow,  // South America & Africa
    Black,   // Asia/Middle East
    Red      // East Asia & Oceania
}

public partial class BoardLocations : Node2D
{
    private Dictionary<string, City> cities;
    private Dictionary<DiseaseType, Color> diseaseColors;
    
    // Cache for optimizing drawing operations
    private List<(Vector2 start, Vector2 end, Color color)> connectionLines;
    private bool needsConnectionUpdate = true;


    public override void _Ready()
    {
        InitializeColors();
        InitializeCities();
        CreateConnections();
    }

    private void InitializeColors()
    {
        diseaseColors = new Dictionary<DiseaseType, Color>
        {
            { DiseaseType.Blue, Colors.Blue },
            { DiseaseType.Yellow, Colors.Yellow },
            { DiseaseType.Black, Colors.Black },
            { DiseaseType.Red, Colors.Red }
        };
    }

    private void InitializeCities()
    {
        cities = new Dictionary<string, City>();

        // Get all city nodes from the scene tree
        var groups = new[] { "BLUE_CITIES", "YELLOW_CITIES", "BLACK_CITIES", "RED_CITIES" };
        foreach (var groupPath in groups)
        {
            var group = GetNode<CanvasGroup>(groupPath);
            var diseaseType = GetDiseaseTypeFromGroup(groupPath);

            foreach (Node cityNode in group.GetChildren())
            {
                var sprite = cityNode as Sprite2D;
                if (sprite != null)
                {
                    cities.Add(cityNode.Name, new City(
                        cityNode.Name,
                        sprite.Position,
                        diseaseType
                    ));
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        if (needsConnectionUpdate)
        {
            UpdateConnectionLines();
            needsConnectionUpdate = false;
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (connectionLines == null) return;

        foreach (var line in connectionLines)
        {
            DrawLine(line.start, line.end, line.color, 3, true);
        }
    }

    private void UpdateConnectionLines()
    {
        connectionLines = new List<(Vector2, Vector2, Color)>();

        foreach (var city in cities.Values)
        {
            foreach (var connectedCityName in city.ConnectedCities)
            {
                if (cities.TryGetValue(connectedCityName, out var connectedCity))
                {
                    // Calculate the direct path vector
                    Vector2 directPath = connectedCity.Position - city.Position;
                    Vector2 start = city.Position;
                    Vector2 end = connectedCity.Position;

                    // If the path is too long (crosses too much of the board), flip it
                    if (Math.Abs(directPath.X) > 1200) // Threshold for when to flip
                    {
                        // Flip the X coordinates
                        if (start.X < 0)
                        {
                            start.X = start.X - 800; // Push further left
                            end.X = end.X + 800;     // Push further right
                        }
                        else
                        {
                            start.X = start.X + 800; // Push further right
                            end.X = end.X - 800;     // Push further left
                        }
                    }

                    // Handle cities from different regions
                    if (city.Region != connectedCity.Region)
                    {
                        var midpoint = (start + end) / 2;
                        connectionLines.Add((
                            start,
                            midpoint,
                            diseaseColors[city.Region]
                        ));
                        connectionLines.Add((
                            midpoint,
                            end,
                            diseaseColors[connectedCity.Region]
                        ));
                    }
                    // Cities from same region
                    else
                    {
                        connectionLines.Add((
                            start,
                            end,
                            diseaseColors[city.Region]
                        ));
                    }
                }
            }
        }
    }

    private DiseaseType GetDiseaseTypeFromGroup(string groupPath)
    {
        return groupPath switch
        {
            "BLUE_CITIES" => DiseaseType.Blue,     // The "BLUE_GROUP" in your scene actually represents Red cities
            "YELLOW_CITIES" => DiseaseType.Yellow,
            "BLACK_CITIES" => DiseaseType.Black,    // The "RED_GROUP" represents Black cities
            "RED_CITIES" => DiseaseType.Red,   // The "GREEN_GROUP" represents Blue cities
            _ => throw new ArgumentException($"Unknown group: {groupPath}")
        };
    }

    private void CreateConnections()
    {
        CreateBlueRegionConnections();      // North America & Europe (GREEN_GROUP)
        CreateYellowRegionConnections();    // South America & Africa (YELLOW_GROUP)
        CreateRedRegionConnections();       // East Asia & Oceania (BLUE_GROUP)
        CreateBlackRegionConnections();     // Middle East *
        
        ValidateConnections();
        needsConnectionUpdate = true;
    }

    private void AddBidirectionalConnection(string city1, string city2)
    {
        if (!cities.ContainsKey(city1) || !cities.ContainsKey(city2))
        {
            GD.PrintErr($"Attempted to connect nonexistent cities: {city1} - {city2}");
            return;
        }

        cities[city1].ConnectedCities.Add(city2);
        cities[city2].ConnectedCities.Add(city1);
    }

    private void CreateBlueRegionConnections()
    {
        var blueConnections = new (string, string)[]
        {
            // Blue City Connections
            ("SanFrancisco", "Chicago"),
            //("SanFrancisco", "Manila"),
            //("SanFrancisco", "Tokyo"),
            ("SanFrancisco", "LosAngeles"),

            ("Chicago", "Montreal"),
            ("Chicago", "Atlanta"),
            ("Chicago", "MexicoCity"),
            ("Chicago", "LosAngeles"),

            ("Montreal", "NewYork"),
            ("Montreal", "Washington"),

            ("Atlanta","Washington"),

            ("NewYork", "Washington"),
            ("NewYork", "London"),
            ("NewYork", "Madrid"),

            ("London", "Madrid"),
            ("London", "Paris"),
            ("London", "Essen"),

            ("Madrid", "Paris"),
            ("Madrid", "Algiers"),
            ("Madrid", "SaoPaulo"),

            ("Paris", "Essen"),
            ("Paris", "Milan"),
            ("Paris", "Algiers"),

            ("Essen", "Milan"),
            ("Essen", "StPetersburg"),

            ("Milan", "Istanbul"),

            ("StPetersburg", "Moscow"),
            ("StPetersburg", "Istanbul"),
        };

        foreach (var (city1, city2) in blueConnections)
        {
            AddBidirectionalConnection(city1, city2);
        }
    }

    private void CreateYellowRegionConnections()
    {
        var yellowConnections = new (string, string)[]
        {
            //Yellow City Connections
            ("LosAngeles", "MexicoCity"),
            //("LosAngeles", "Sydney"),

            ("MexicoCity", "Miami"),
            ("MexicoCity", "Bogota"),

            ("MexicoCity", "Lima"),

            ("Miami", "Bogota"),
            ("Miami", "Washington"),
            ("Miami", "Atlanta"),

            ("Bogota", "Lima"),
            ("Bogota", "SaoPaulo"),

            ("Lima", "Santiago"),

            ("BuenosAires", "SaoPaulo"),
            ("BuenosAires", "Bogota"),

            ("SaoPaulo", "Lagos"),


            ("Lagos", "Khartoum"),
            ("Lagos", "Kinshasa"),

            ("Kinshasa", "Khartoum"),
            ("Kinshasa", "Johannesburg"),

            ("Khartoum", "Cairo"),
            ("Khartoum", "Johannesburg")
        };

        foreach (var (city1, city2) in yellowConnections)
        {
            AddBidirectionalConnection(city1, city2);
        }
    }

    private void CreateBlackRegionConnections()
    {
        var blackConnections = new (string, string)[]
        {
            //Black City Ocnnections
            ("Moscow", "Istanbul"),
            ("Moscow", "Tehran"),

            ("Istanbul", "Baghdad"),
            ("Istanbul", "Cairo"),
            ("Istanbul", "Algiers"),

            ("Cairo", "Algiers"),
            ("Cairo", "Baghdad"),
            ("Cairo", "Khartoum"),
            ("Cairo", "Riyadh"),

            ("Baghdad", "Tehran"),
            ("Baghdad", "Karachi"),
            ("Baghdad", "Riyadh"),

            ("Riyadh", "Karachi"),

            ("Tehran", "Karachi"),
            ("Tehran", "Delhi"),

            ("Karachi", "Delhi"),
            ("Karachi", "Mumbai"),

            ("Mumbai", "Delhi"),
            ("Mumbai", "Chennai"),

            ("Delhi", "Chennai"),
            ("Delhi", "Kolkata"),

            ("Chennai", "Kolkata"),
            ("Chennai", "Bangkok"),
            ("Chennai", "Jakarta"),

            ("Kolkata", "Bangkok"),
            ("Kolkata", "HongKong")
        };

        foreach (var (city1, city2) in blackConnections)
        {
            AddBidirectionalConnection(city1, city2);
        }
    }

    private void CreateRedRegionConnections()
    {
        var redConnections = new (string, string)[]
        {
            //Red City Connections
            ("Beijing", "Seoul"),
            ("Beijing", "Shanghai"),

            ("Seoul", "Shanghai"),
            ("Seoul", "Tokyo"),

            ("Tokyo", "Shanghai"),
            ("Tokyo", "Osaka"),

            ("Shanghai", "Taipei"),
            ("Shanghai", "HongKong"),

            ("HongKong", "Taipei"),
            ("HongKong", "Manila"),
            ("HongKong", "HoChiMinhCity"),
            ("HongKong", "Bangkok"),

            ("Taipei", "Manila"),
            ("Taipei", "Osaka"),

            ("Bangkok", "HoChiMinhCity"),
            ("Bangkok", "Jakarta"),

            ("Jakarta", "HoChiMinhCity"),
            ("Jakarta", "Sydney"),

            ("HoChiMinhCity", "Manila"),

            ("Manila", "Sydney")
        };

        foreach (var (city1, city2) in redConnections)
        {
            AddBidirectionalConnection(city1, city2);
        }
    }

    private void ValidateConnections()
    {
        // Validate all cities have at least one connection
        foreach (var city in cities.Values)
        {
            if (city.ConnectedCities.Count == 0)
            {
                GD.PrintErr($"Warning: City {city.Name} has no connections!");
            }
        }

        // Validate bidirectional connections
        foreach (var city in cities.Values)
        {
            foreach (var connectedCity in city.ConnectedCities)
            {
                if (!cities[connectedCity].ConnectedCities.Contains(city.Name))
                {
                    GD.PrintErr($"Warning: One-way connection detected: {city.Name} -> {connectedCity}");
                }
            }
        }
    }

    // Helper method to get all cities connected to a specific city
    public List<(string cityName, DiseaseType region)> GetConnectedCitiesWithRegions(string cityName)
    {
        if (!cities.TryGetValue(cityName, out var city))
        {
            return new List<(string, DiseaseType)>();
        }

        return city.ConnectedCities
            .Where(connectedCity => cities.ContainsKey(connectedCity))
            .Select(connectedCity => (connectedCity, cities[connectedCity].Region))
            .ToList();
    }

    // Helper method to check if a path exists between two cities
    public bool PathExists(string startCity, string targetCity, int maxSteps = 100)
    {
        if (!cities.ContainsKey(startCity) || !cities.ContainsKey(targetCity))
            return false;

        var visited = new HashSet<string>();
        var queue = new Queue<(string city, int steps)>();
        queue.Enqueue((startCity, 0));

        while (queue.Count > 0)
        {
            var (currentCity, steps) = queue.Dequeue();
            
            if (steps > maxSteps) continue;
            if (currentCity == targetCity) return true;
            
            if (!visited.Contains(currentCity))
            {
                visited.Add(currentCity);
                foreach (var nextCity in cities[currentCity].ConnectedCities)
                {
                    if (!visited.Contains(nextCity))
                    {
                        queue.Enqueue((nextCity, steps + 1));
                    }
                }
            }
        }

        return false;
    }
}