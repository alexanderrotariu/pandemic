using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Godot;

public partial class BoardLocations : Node2D
{
	Dictionary<string, List<string>> connections = new Dictionary<string, List<string>>();

	//Declaring sprite names being looked for
		// string[] greenNames = { "GREEN_1",
		// 						"GREEN_2",
		// 						"GREEN_3",
		// 						"GREEN_4",			
		// 						"GREEN_5",
		// 						"GREEN_6",
		// 						"GREEN_7",
		// 						"GREEN_8",
		// 						"GREEN_9",
		// 						"GREEN_10",
		// 						"GREEN_11",
		// 						"GREEN_12",};

		// string[] yellowNames = { "YELLOW_1",
		// 						 "YELLOW_2",
		// 						 "YELLOW_3",
		// 						 "YELLOW_4",			
		// 						 "YELLOW_5",
		// 						 "YELLOW_6",
		// 						 "YELLOW_7",
		// 						 "YELLOW_8",
		// 						 "YELLOW_9",
		// 						 "YELLOW_10",
		// 						 "YELLOW_11",
		// 						 "YELLOW_12",};

		// string[] redNames = { 	"RED_1",
		// 						"RED_2",
		// 						"RED_3",
		// 						"RED_4",			
		// 						"RED_5",
		// 						"RED_6",
		// 						"RED_7",
		// 						"RED_8",
		// 						"RED_9",
		// 						"RED_10",
		// 						"RED_11",
		// 						"RED_12",};

		// string[] blueNames = {  "BLUE_1",
		// 						"BLUE_2",
		// 						"BLUE_3",
		// 						"BLUE_4",			
		// 						"BLUE_5",
		// 						"BLUE_6",
		// 						"BLUE_7",
		// 						"BLUE_8",
		// 						"BLUE_9",
		// 						"BLUE_10",
		// 						"BLUE_11",};	

		string[] canvasPaths = { "GREEN_GROUP",
								 "YELLOW_GROUP",
								 "RED_GROUP",
								 "BLUE_GROUP"};

		BoardCamera camera = new BoardCamera();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CanvasGroup[] groups = getAllCanvasGroups(canvasPaths);
		
		//List<string> masterList = getAllLocations(groups);

		// foreach(string locationName in masterList)
		// {
		// 	Console.WriteLine(locationName);
		// }

		//createConnections();
	}



	public override void _Draw()
    {
		createConnections();
		CanvasGroup[] groups = getAllCanvasGroups(canvasPaths);
		List<Node> locationNodes = getAllLocations(groups);

		//For each canvas group get the children contained
		foreach(Node source in locationNodes )
		{
			Sprite2D spriteName = GetNode<Sprite2D>(source.GetPath());

			//For each sprite connected to it
			for(int i = 0; i < connections[spriteName.Name].Count; i++)
			{
				Console.WriteLine("We in here");

				Sprite2D targetNode = new Sprite2D();

				foreach(Node search in locationNodes)
				{
					if(search.Name == connections[spriteName.Name][i])
					{
						targetNode = GetNode<Sprite2D>(search.GetPath());
					}
				}

				try 
				{
					string origin = source.Name.ToString().Substring(0, source.Name.ToString().IndexOf("_"));
					string destination = targetNode.Name.ToString().Substring(0, targetNode.Name.ToString().IndexOf("_"));


					if(origin == destination)
					{
						switch(origin)
						{
							case "GREEN":
								DrawLine(spriteName.Position, targetNode.Position, Colors.Green, 3, true);
								break;

							case "YELLOW":
								DrawLine(spriteName.Position, targetNode.Position, Colors.Yellow, 3, true);
								break;

							case "BLUE":
								DrawLine(spriteName.Position, targetNode.Position, Colors.Blue, 3, true);
								break;

							case "RED":
								DrawLine(spriteName.Position, targetNode.Position, Colors.Red, 3, true);
								break;
						}
					}
					else
					{
						var midpointX = (spriteName.Position.X + targetNode.Position.X) / 2;
						var midpointY = (spriteName.Position.Y + targetNode.Position.Y) / 2;;

						Vector2 halfTarget = new Vector2(midpointX, midpointY);

						switch(origin)
						{
							case "GREEN":
								DrawLine(spriteName.Position, halfTarget, Colors.Green, 3, true);
								break;

							case "YELLOW":
								DrawLine(spriteName.Position, halfTarget, Colors.Yellow, 3, true);
								break;

							case "BLUE":
								DrawLine(spriteName.Position, halfTarget, Colors.Blue, 3, true);
								break;

							case "RED":
								DrawLine(spriteName.Position, halfTarget, Colors.Red, 3, true);
								break;
						}

						switch(destination)
						{
							case "GREEN":
								DrawLine(halfTarget, targetNode.Position, Colors.Green, 3, true);
								break;

							case "YELLOW":
								DrawLine(halfTarget, targetNode.Position, Colors.Yellow, 3, true);
								break;

							case "BLUE":
								DrawLine(halfTarget, targetNode.Position, Colors.Blue, 3, true);
								break;

							case "RED":
								DrawLine(halfTarget, targetNode.Position, Colors.Red, 3, true);
								break;
						}


					}
				}
				catch
				{
					Console.WriteLine("Outer Node");
					// string destination = target.Name.ToString();
					// switch(destination)
					// {
					// 	case "Y1":
					// 		DrawLine(spriteName.Position, target.Position, Colors.Yellow, 3, true);
					// 		break;

					// 	case"G1":
					// 		DrawLine(spriteName.Position, target.Position, Colors.Green, 3, true);
					// 		break;

					// 	case "B9":
					// 	case "B12":
					// 	case "B10":
					// 		DrawLine(spriteName.Position, target.Position, Colors.Blue, 3, true);
					// 		break;
					// }
				}
					
			}
			
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Collision boxes
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
		}

		return sprites;
	}

	public CanvasGroup[] getAllCanvasGroups(string[] paths)
	{
		CanvasGroup[] canvasGroups= new CanvasGroup[paths.Length];
		
		for(int i = 0; i < paths.Length; i++)
		{
			string canvasGroupPath = paths[i];

			NodePath currentNode = new NodePath(canvasGroupPath);

			canvasGroups[i] = GetNode<CanvasGroup>(currentNode);
		}

		return canvasGroups;
	} 

	public List<Node> getAllLocations(CanvasGroup[] group)
	{
		List<Node> output = new List<Node>();

		foreach (CanvasGroup cg in group)
		{
			var childrenList = cg.GetChildren();
			foreach(Node child in childrenList)
			{
				output.Add(child);
			}
		}

		return output;
	}

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
