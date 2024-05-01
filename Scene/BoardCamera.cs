using Godot;
using System;

public partial class BoardCamera : Camera2D
{
	public float movementSpeed = 5.0F;

	public Vector2 getCameraPosition()
	{
		return GetScreenCenterPosition();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 mousePosition = GetGlobalMousePosition();

		Vector2 cameraPosition = GetScreenCenterPosition();

		float positionDiffX = Math.Abs(mousePosition.X - cameraPosition.X);
		float positionDiffY = Math.Abs(mousePosition.Y - cameraPosition.Y);

		float zoomX = Zoom.X;
		float zoomY = Zoom.Y;

		//Console.WriteLine(zoomX + " : " + zoomY);

		//Moves camera
		if( Input.IsActionPressed("move camera") && positionDiffX > 5 && positionDiffY > 5)
		{
			//Console.WriteLine("Move camera");		
			Position = Position.MoveToward(mousePosition, (float)delta * 500);		
		}

		if( Input.IsActionJustPressed("zoom in"))
		{
			if(Zoom.X < 1.25 || Zoom.Y < 1.25)
			{
				Zoom = Zoom.MoveToward(new Vector2(zoomX + 0.10F, zoomY + 0.10F), (float)delta * 5);
			}
		}

		if( Input.IsActionJustPressed("zoom out"))
		{

			Console.WriteLine("yes!");
			if(Zoom.X > 0.2 || Zoom.Y > 0.2)
			{
				Zoom = Zoom.MoveToward(new Vector2(zoomX - 0.10F, zoomY - 0.10F), (float)delta *5) ;
			}
			
		}
		// //Move the camera
		// Offset += (mousePosition - GetViewportRect().Size/2) * movementSpeed * (float)delta;
	}
	
	
}
