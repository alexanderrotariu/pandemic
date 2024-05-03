using Godot;
using System;

public partial class location : Sprite2D
{
	string greenPath = "res://Assets/GREEN_SPRITE.png";
	string redPath = "res://Assets/RED_SPRITE.png";
	string yellowPath = "res://Assets/YELLOW_SPRITE.png";
	string bluePath = "res://Assets/BLUE_SPRITE.png";


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Console.WriteLine(GetParent().GetPath());
		NodePath currentNodePath = GetParent().GetPath();

		switch(currentNodePath)
		{
			case "/root/Board/GREEN_GROUP":
				Texture = GD.Load<Texture2D>(greenPath);
				break;
			case "/root/Board/RED_GROUP":
				Texture = GD.Load<Texture2D>(redPath);
				break;
			case "/root/Board/BLUE_GROUP":
				Texture = GD.Load<Texture2D>(bluePath);
				break;
			case "/root/Board/YELLOW_GROUP":
				Texture = GD.Load<Texture2D>(yellowPath);
				break;
			default:
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
