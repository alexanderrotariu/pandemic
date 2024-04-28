using System;
using System.Collections.Generic;
using Godot;

public partial class BoardLocations : Node2D
{
	Dictionary<string, List<string>> connections = new Dictionary<string, List<string>>();

	//Declaring sprite names being looked for
		string[] greenNames = { "GREEN_1",
								"GREEN_2",
								"GREEN_3",
								"GREEN_4",			
								"GREEN_5",
								"GREEN_6",
								"GREEN_7",
								"GREEN_8",
								"GREEN_9",
								"GREEN_10",
								"GREEN_11",
								"GREEN_12",};

		string[] yellowNames = { "YELLOW_1",
								 "YELLOW_2",
								 "YELLOW_3",
								 "YELLOW_4",			
								 "YELLOW_5",
								 "YELLOW_6",
								 "YELLOW_7",
								 "YELLOW_8",
								 "YELLOW_9",
								 "YELLOW_10",
								 "YELLOW_11",
								 "YELLOW_12",};

		string[] redNames = { 	"RED_1",
								"RED_2",
								"RED_3",
								"RED_4",			
								"RED_5",
								"RED_6",
								"RED_7",
								"RED_8",
								"RED_9",
								"RED_10",
								"RED_11",
								"RED_12",};

		string[] blueNames = {  "BLUE_1",
								"BLUE_2",
								"BLUE_3",
								"BLUE_4",			
								"BLUE_5",
								"BLUE_6",
								"BLUE_7",
								"BLUE_8",
								"BLUE_9",
								"BLUE_10",
								"BLUE_11",};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Console.WriteLine("Starting...");

		createConnections();
		
		//Get all sprite locations
		Sprite2D[] greenSpriteLocations = getAllSprites(greenNames);
		Sprite2D[] yellowSpriteLocations = getAllSprites(yellowNames);
		Sprite2D[] redSpriteLocations = getAllSprites(redNames);
		Sprite2D[] blueSpriteLocations = getAllSprites(blueNames);
	}



	 public override void _Draw()
    {
        //drawLineBetweenSprite(getAllSprites(greenNames));
		//drawLineBetweenSprite(getAllSprites(yellowNames));
        //drawLineBetweenSprite(getAllSprites(redNames));
        //drawLineBetweenSprite(getAllSprites(blueNames));

		foreach (var connection in connections)
		{
			Sprite2D source = GetNode<Sprite2D>(connection.Key);

			foreach (string targetName in connection.Value)
			{
				Sprite2D target = GetNode<Sprite2D>(targetName);

				try 
				{
					string origin = source.Name.ToString().Substring(0, connection.Key.IndexOf("_"));
					string destination = targetName.Substring(0, targetName.IndexOf("_"));

					if(origin == destination)
					{
						switch(origin)
						{
							case "GREEN":
								DrawLine(source.Position, target.Position, Colors.Green, 3, true);
								break;

							case "YELLOW":
								DrawLine(source.Position, target.Position, Colors.Yellow, 3, true);
								break;

							case "BLUE":
								DrawLine(source.Position, target.Position, Colors.Blue, 3, true);
								break;

							case "RED":
								DrawLine(source.Position, target.Position, Colors.Red, 3, true);
								break;
						}
					}
					else
					{
						var midpointX = (source.Position.X + target.Position.X) / 2;
						var midpointY = (source.Position.Y + target.Position.Y) / 2;;

						Vector2 halfTarget = new Vector2(midpointX, midpointY);

						switch(origin)
						{
							case "GREEN":
								DrawLine(source.Position, halfTarget, Colors.Green, 3, true);
								break;

							case "YELLOW":
								DrawLine(source.Position, halfTarget, Colors.Yellow, 3, true);
								break;

							case "BLUE":
								DrawLine(source.Position, halfTarget, Colors.Blue, 3, true);
								break;

							case "RED":
								DrawLine(source.Position, halfTarget, Colors.Red, 3, true);
								break;
						}

						switch(destination)
						{
							case "GREEN":
								DrawLine(halfTarget, target.Position, Colors.Green, 3, true);
								break;

							case "YELLOW":
								DrawLine(halfTarget, target.Position, Colors.Yellow, 3, true);
								break;

							case "BLUE":
								DrawLine(halfTarget, target.Position, Colors.Blue, 3, true);
								break;

							case "RED":
								DrawLine(halfTarget, target.Position, Colors.Red, 3, true);
								break;
						}


					}
				}
				catch
				{
					string destination = targetName;
					switch(destination)
					{
						case "Y1":
							DrawLine(source.Position, target.Position, Colors.Yellow, 3, true);
							break;

						case"G1":
							DrawLine(source.Position, target.Position, Colors.Green, 3, true);
							break;

						case "B9":
						case "B12":
						case "B10":
							DrawLine(source.Position, target.Position, Colors.Blue, 3, true);
							break;
					}
				}
				
				
				
			}
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public Sprite2D[] getAllSprites(string[] locationNames)
	{
        //Declaring an array of Sprite2D's 
		Sprite2D[] sprites = new Sprite2D[locationNames.Length];

		
		for (int i=0; i < locationNames.Length; i++)
		{
			string spriteName = locationNames[i];

			NodePath currentNode = new NodePath(spriteName);

			sprites[i] = GetNode<Sprite2D>(currentNode);
			
			Console.WriteLine($"{sprites[i].Name} is at position {sprites[i].Position}");
		}

		return sprites;
	}

	// //Pass in an array of sprite references
	// public void drawLineBetweenSprite(Sprite2D[] sprites)
	// {
	// 	//For each sprite excluding the last one
	// 	for(int i=0; i < sprites.Length-1; i++)
	// 	{
	// 		//Get current sprite location
	// 		Vector2 origin = sprites[i].Position;
	// 		//Get next sprites location
	// 		Vector2 destination = sprites[i+1].Position;

	// 		//Normalize origin
	// 		//origin = origin.Normalized() * 100;
	// 		//Normalize destination
	// 		//destination = destination.Normalized() * 100;

	// 		//Draw a white line from one sprite to another
	// 		//DrawLine(origin, destination, Color.Color8(1,1,1), 1);
	// 		DrawLine(origin, destination, Colors.White, 3, false);
	// 	}

	// }

	public void createConnections()
	{
		//GREEN CONNECTIONS
		connections.Add("GREEN_1", new List<string> { "GREEN_2", "YELLOW_2", "B9", "B12" }); //CONNECTS ACROSS THE MAP TO BLUE_12 AND BLUE_9
		connections.Add("GREEN_2", new List<string> { "YELLOW_2", "YELLOW_3", "GREEN_3", "GREEN_1", "GREEN_4" });
		connections.Add("GREEN_3", new List<string> { "GREEN_10", "YELLOW_7", "GREEN_2" });
		connections.Add("GREEN_4", new List<string> { "GREEN_2", "GREEN_10", "GREEN_11" });
		connections.Add("GREEN_5", new List<string> { "GREEN_6", "RED_1", "GREEN_11", "YELLOW_6" });
		connections.Add("GREEN_6", new List<string> { "GREEN_5", "GREEN_7", "GREEN_11" });
		connections.Add("GREEN_7", new List<string> { "RED_1", "GREEN_5", "GREEN_6", "GREEN_8", "GREEN_12" });
		connections.Add("GREEN_8", new List<string> { "GREEN_6", "GREEN_7", "GREEN_9", "GREEN_12" });
		connections.Add("GREEN_9", new List<string> { "GREEN_8", "RED_3", "RED_4" });
		connections.Add("GREEN_10", new List<string> { "GREEN_3", "YELLOW_7", "GREEN_4", "GREEN_11" });
		connections.Add("GREEN_11", new List<string> { "GREEN_10", "GREEN_4", "GREEN_5", "GREEN_6" });
		connections.Add("GREEN_12", new List<string> { "GREEN_8", "GREEN_7", "RED_3" });

		//YELLOW CONNECTIONS
		connections.Add("YELLOW_1", new List<string> { "YELLOW_5" });		
		connections.Add("YELLOW_2", new List<string> { "YELLOW_3", "GREEN_1", "GREEN_2", "B10" }); //CONNECTS ACROSS THE MAP TO BLUE_10
		connections.Add("YELLOW_3", new List<string> { "YELLOW_2", "YELLOW_4", "YELLOW_5", "YELLOW_7", "GREEN_2" });
		connections.Add("YELLOW_4", new List<string> { "YELLOW_3", "YELLOW_5", "YELLOW_6", "YELLOW_7", "YELLOW_8" });
		connections.Add("YELLOW_5", new List<string> { "YELLOW_1", "YELLOW_3", "YELLOW_4" });
		connections.Add("YELLOW_6", new List<string> { "YELLOW_4", "YELLOW_8", "YELLOW_9", "GREEN_5" });
		connections.Add("YELLOW_7", new List<string> { "YELLOW_3", "YELLOW_4", "GREEN_3", "GREEN_10" });
		connections.Add("YELLOW_8", new List<string> { "YELLOW_6", "YELLOW_4" });
		connections.Add("YELLOW_9", new List<string> { "YELLOW_6", "YELLOW_10", "YELLOW_11" });
		connections.Add("YELLOW_10", new List<string> { "YELLOW_9", "YELLOW_11", "YELLOW_12" });
		connections.Add("YELLOW_11", new List<string> { "YELLOW_9", "YELLOW_10", "YELLOW_12", "RED_2" });
		connections.Add("YELLOW_12", new List<string> { "YELLOW_10", "YELLOW_11" });

		//BLUE CONNECTIONS
		connections.Add("BLUE_1", new List<string> { "RED_10", "RED_11", "BLUE_2", "BLUE_4" });
		connections.Add("BLUE_2", new List<string> { "RED_10", "BLUE_3", "BLUE_12" });
		connections.Add("BLUE_3", new List<string> { "BLUE_1", "BLUE_2", "BLUE_4", "BLUE_9" });
		connections.Add("BLUE_4", new List<string> { "RED_11", "BLUE_5", "BLUE_8", "BLUE_9", "BLUE_3", "BLUE_1" });
		connections.Add("BLUE_5", new List<string> { "BLUE_4", "BLUE_6", "BLUE_7", "BLUE_8", "BLUE_10"  });
		connections.Add("BLUE_6", new List<string> { "BLUE_5", "BLUE_7" });
		connections.Add("BLUE_7", new List<string> { "BLUE_5", "BLUE_6", "BLUE_10" });
		connections.Add("BLUE_8", new List<string> { "BLUE_4", "BLUE_5", "BLUE_9", "BLUE_11" });
		connections.Add("BLUE_9", new List<string> { "BLUE_3", "BLUE_4", "BLUE_8", "BLUE_12", "G1" });
		connections.Add("BLUE_10", new List<string> { "BLUE_5", "BLUE_7", "BLUE_11", "G1" });
		connections.Add("BLUE_11", new List<string> { "BLUE_8", "BLUE_10" });
		connections.Add("BLUE_12", new List<string> { "BLUE_2", "BLUE_9", "Y1" });

		//RED CONNECTIONS
		connections.Add("RED_1", new List<string> { "GREEN_5", "GREEN_7", "RED_3", "RED_2" });
		connections.Add("RED_2", new List<string> { "RED_1", "RED_3", "RED_5", "RED_6", "YELLOW_11" });
		connections.Add("RED_3", new List<string> { "GREEN_12", "GREEN_9", "RED_1", "RED_2", "RED_5", "RED_4" });
		connections.Add("RED_4", new List<string> { "GREEN_9", "RED_3", "RED_7" });
		connections.Add("RED_5", new List<string> { "RED_3", "RED_2", "RED_6", "RED_8", "RED_7" });
		connections.Add("RED_6", new List<string> { "RED_2", "RED_5", "RED_8" });
		connections.Add("RED_7", new List<string> { "RED_5", "RED_4", "RED_8", "RED_12" });
		connections.Add("RED_8", new List<string> { "RED_5", "RED_6", "RED_7", "RED_9", "RED_12" });
		connections.Add("RED_9", new List<string> { "RED_10", "RED_12", "RED_8" });
		connections.Add("RED_10", new List<string> { "BLUE_2", "BLUE_1", "RED_11", "RED_12", "RED_9" });
		connections.Add("RED_11", new List<string> { "BLUE_4", "BLUE_1", "RED_10", "RED_12" });
		connections.Add("RED_12", new List<string> { "RED_11", "RED_10", "RED_9", "RED_8", "RED_7" });
	}		
}
