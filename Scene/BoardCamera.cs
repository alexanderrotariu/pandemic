using Godot;
using System;

public partial class BoardCamera : Camera2D
{
	public float movementSpeed = 5.0F;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 mousePosition = GetGlobalMousePosition();

		Vector2 cameraPosition = GetScreenCenterPosition();

		//Moves camera
		if(mousePosition != cameraPosition)
		{
			Console.WriteLine("Move camera");		
			Position = Position.MoveToward(mousePosition, (float)delta * 100);	
		}
		// //Move the camera
		// Offset += (mousePosition - GetViewportRect().Size/2) * movementSpeed * (float)delta;

		Console.WriteLine(Position);
	}
}
