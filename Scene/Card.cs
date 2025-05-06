using Godot;
using System;

public partial class Card : Node2D
{
	private bool isHovered = false;
	private bool isDragging = false;
	private Vector2 dragOffset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Update the card position if it's being dragged
		if (isDragging)
		{
			GlobalPosition = GetGlobalMousePosition() - dragOffset;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (!isHovered)
			return;
			
		// Start dragging when the left mouse button is pressed on a hovered card
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				isDragging = true;
				ZIndex = 1;
				dragOffset = GetLocalMousePosition();
				// Disable hover effect while dragging
				Scale = new Vector2(1f, 1f);
			}
			else if (isDragging)
			{
				isDragging = false;
				ZIndex = 0;
				// Apply hover effect again if still hovered after releasing
				if (isHovered && !isDragging)
				{
					Scale = new Vector2(1.2f, 1.2f);
				}
			}
		}
	}

	public void OnMouseEnter()
	{
		isHovered = true;
		// Only apply hover effect if not currently dragging
		if (!isDragging)
		{
			Scale = new Vector2(1.2f, 1.2f);
			
			QueueRedraw();
		}
	}

	public void OnMouseExit()
	{
		isHovered = false;
		// Reset scale when mouse exits
		if (!isDragging)
		{
			Scale = new Vector2(1f, 1f);
			QueueRedraw();
		}
	}
}