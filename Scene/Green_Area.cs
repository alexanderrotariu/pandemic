using Godot;
using System;

public partial class Green_Area : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

    private void checkOverlap()
	{
		foreach (Node body in GetOverlappingAreas())
		{
			if ( body is StaticBody2D)
			{
				Console.WriteLine("In area");
			}
		}
	}

	public void _on_Area2D_body_entered(StaticBody2D other)
	{
		Console.WriteLine(other.Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		checkOverlap();
	}
}
