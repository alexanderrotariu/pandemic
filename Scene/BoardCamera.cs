using Godot;
using System;

public partial class BoardCamera : Camera2D
{
	public float movementSpeed = 5.0F;
	public float zoomSpeed = 5.0F;
	public float zoomIncrement = 0.10F;
	public float minZoom = 0.2F;
	public float maxZoom = 1.25F;
	
	// Store viewport size
	private Vector2 viewportSize;
	// Camera boundaries (if needed)
	private Vector2 cameraBoundMin = new Vector2(-2000, -1500);
	private Vector2 cameraBoundMax = new Vector2(2000, 1500);
	// Debug mode for troubleshooting
	private bool debugMode = true;
	
	// Track initial mouse position when action is first pressed
	private Vector2 dragStartPosition;
	private Vector2 cameraStartPosition;
	private bool isDragging = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Cache viewport size
		viewportSize = GetViewport().GetVisibleRect().Size;
		
		// Configure the camera
		Position = Vector2.Zero;  // Start at origin
		Zoom = new Vector2(1.0F, 1.0F); // Start at neutral zoom
		
		if (debugMode)
		{
			GD.Print($"Camera initialized at position {Position}");
			GD.Print($"Viewport size: {viewportSize}");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleCameraMovement(delta);
		HandleCameraZoom(delta);
	}
	
	public override void _Input(InputEvent @event)
	{
		// Handle start of camera drag
		if (@event.IsActionPressed("move camera"))
		{
			isDragging = true;
			dragStartPosition = GetViewport().GetMousePosition();
			cameraStartPosition = Position;
			
			if (debugMode)
			{
				GD.Print($"Started dragging at {dragStartPosition}");
			}
		}
		
		// Handle end of camera drag
		if (@event.IsActionReleased("move camera"))
		{
			isDragging = false;
			
			if (debugMode)
			{
				GD.Print($"Stopped dragging");
			}
		}
	}
	
	private void HandleCameraMovement(double delta)
	{
		// Use the drag approach for camera movement
		if (isDragging)
		{
			Vector2 currentMousePos = GetViewport().GetMousePosition();
			Vector2 movement = dragStartPosition - currentMousePos;
			
			// Move camera based on mouse movement (multiplied by zoom to adjust sensitivity)
			Position = cameraStartPosition + movement / Zoom;
			
			if (debugMode)
			{
				GD.Print($"Mouse Position: {currentMousePos}, Camera Position: {Position}");
			}
			
			// Apply camera bounds if needed
			Position = new Vector2(
				Mathf.Clamp(Position.X, cameraBoundMin.X, cameraBoundMax.X),
				Mathf.Clamp(Position.Y, cameraBoundMin.Y, cameraBoundMax.Y)
			);
		}
	}
	
	private void HandleCameraZoom(double delta)
	{
		float zoomX = Zoom.X;
		float zoomY = Zoom.Y;
		
		// Handle zoom in
		if (Input.IsActionJustPressed("zoom in"))
		{
			if (zoomX < maxZoom && zoomY < maxZoom)
			{
				Vector2 targetZoom = new Vector2(zoomX + zoomIncrement, zoomY + zoomIncrement);
				Zoom = Zoom.MoveToward(targetZoom, (float)delta * zoomSpeed);
				
				if (debugMode)
				{
					GD.Print($"Zooming in: {Zoom}");
				}
			}
		}
		
		// Handle zoom out
		if (Input.IsActionJustPressed("zoom out"))
		{
			if (zoomX > minZoom && zoomY > minZoom)
			{
				Vector2 targetZoom = new Vector2(zoomX - zoomIncrement, zoomY - zoomIncrement);
				Zoom = Zoom.MoveToward(targetZoom, (float)delta * zoomSpeed);
				
				if (debugMode)
				{
					GD.Print($"Zooming out: {Zoom}");
				}
			}
		}
	}
	
	// Utility method to get camera's current position for other scripts
	public Vector2 GetCameraPosition()
	{
		return Position;
	}
	
	// Utility method to get camera's current zoom for other scripts
	public Vector2 GetCameraZoom()
	{
		return Zoom;
	}
}