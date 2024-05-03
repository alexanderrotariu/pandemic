using Godot;
using System;

public partial class Green_Area : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}
	public void _on_Area2D_body_entered(StaticBody2D other)
	{
		Console.WriteLine("Body entered");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
