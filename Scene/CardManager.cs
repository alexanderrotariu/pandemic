using Godot;
using System;

public partial class CardManager : Node2D
{
	Node2D CardSelect = null;
	// Store the initial click offset to maintain relative grab position
	private Vector2 dragOffset = Vector2.Zero;
	// Reference to the camera for coordinate transformations
	private Camera2D boardCamera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AssignCardSprites();

		// Try to find the camera in the scene - needed for proper coordinate transforms
		boardCamera = GetViewport().GetCamera2D();
		if (boardCamera == null)
		{
			GD.Print("Warning: No Camera2D found in scene. Card constraints may not work correctly.");
		}
		else
		{
			GD.Print($"Camera found: {boardCamera.Name}, Position: {boardCamera.Position}, Zoom: {boardCamera.Zoom}");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (CardSelect != null)
		{
			// Get current mouse position in global coordinates
			var mousePosition = GetGlobalMousePosition();

			// Apply the offset for natural dragging
			CardSelect.GlobalPosition = mousePosition - dragOffset;

			// If we need constraints to keep cards on screen:
			if (boardCamera != null)
			{
				// Get visible area bounds in global coordinates
				var viewport = GetViewport();
				var viewportSize = viewport.GetVisibleRect().Size;
				Vector2 cameraCenterPos = boardCamera.GetScreenCenterPosition();

				// Calculate global bounds with zoom factor
				float zoomFactor = boardCamera.Zoom.X; // Assuming uniform zoom
				float halfWidthGlobal = (viewportSize.X / 2) / zoomFactor;
				float halfHeightGlobal = (viewportSize.Y / 2) / zoomFactor;

				// Calculate screen boundaries in global coordinates
				float leftBound = cameraCenterPos.X - halfWidthGlobal;
				float rightBound = cameraCenterPos.X + halfWidthGlobal;
				float topBound = cameraCenterPos.Y - halfHeightGlobal;
				float bottomBound = cameraCenterPos.Y + halfHeightGlobal;

				// Get card dimensions for edge clamping
				Vector2 cardSize = GetCardSize(CardSelect);
				float cardHalfWidth = cardSize.X / 2;
				float cardHalfHeight = cardSize.Y / 2;

				// Apply constraints to keep card fully visible
				CardSelect.GlobalPosition = new Vector2(
					Mathf.Clamp(CardSelect.GlobalPosition.X, leftBound + cardHalfWidth, rightBound - cardHalfWidth),
					Mathf.Clamp(CardSelect.GlobalPosition.Y, topBound + cardHalfHeight, bottomBound - cardHalfHeight)
				);
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				// Try to get the card area
				var cardArea = RaycastCheckForCard();

				// Only proceed if we found a card area
				if (cardArea != null && IsNodeACard(cardArea))
				{
					// Get the parent card node
					var card = (Node2D)cardArea.GetParent();
					if (card != null)
					{
						CardSelect = card;
						// Calculate offset from mouse to card position for natural dragging
						dragOffset = GetGlobalMousePosition() - card.GlobalPosition;
					}
				}
			}
			else
			{
				// Release the card when mouse button is released
				CardSelect = null;
			}
		}
	}

	// Helper method to check if the detected node is actually a card
	private bool IsNodeACard(Node2D node)
	{
		// Check if the node is a card by checking its parent groups or path
		string parentGroup = node.GetParent().GetParent().Name + "";
		if (node.Name == "Area2D" && parentGroup.Contains("CARDS"))
		{
			return true;
		}
		return false;
	}

	// Helper method to determine card size
	private Vector2 GetCardSize(Node2D card)
	{
		// Try to get size from sprite if available
		var sprite = card.GetNodeOrNull<Sprite2D>("CardImage");
		if (sprite != null && sprite.Texture != null)
		{
			return sprite.Texture.GetSize() * sprite.Scale;
		}

		// Fallback: Try to get size from collision shape
		var area = card.GetNodeOrNull<Area2D>("Area2D");
		if (area != null)
		{
			var collisionShape = area.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
			if (collisionShape != null && collisionShape.Shape != null)
			{
				if (collisionShape.Shape is RectangleShape2D rect)
				{
					return rect.Size * 2; // Size is half-extents in Godot
				}
			}
		}

		// Default fallback size if we can't determine the actual size
		return new Vector2(100, 150); // Typical card dimensions
	}

	//Function that when you click on a card gets all the info.
	public Node2D RaycastCheckForCard()
	{
		var spaceState = GetWorld2D().DirectSpaceState;

		var parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
			CollisionMask = 1
		};

		var result = spaceState.IntersectPoint(parameters);

		if (result.Count > 0)
		{
			var colldierResult = (Node2D)result[0]["collider"];
			return colldierResult;
		}
		return null;
	}

	public void AssignCardSprites()
	{
		GD.Print("Starting assignment");

		var groups = new[] { "BLUE_CARDS"/*, "YELLOW_CARDS", "BLACK_CARDS", "RED_CARDS"*/};

		foreach (var groupName in groups)
		{
			var group = GetNode<CanvasGroup>(groupName);

			if (group == null)
			{
				GD.PrintErr($"Group {groupName} not found!");
				continue;
			}

			foreach (Node cityNode in group.GetChildren())
			{
				GD.Print($"Processing card: {cityNode.Name}");

				if (cityNode is Node2D cardNode)
				{
					// Look for the CardImage child node (as per Card.tscn)
					var sprite = cardNode.GetNodeOrNull<Sprite2D>("CardImage");

					if (sprite != null)
					{
						// Build texture path based on card name (matching the filename)
						var texturePath = $"res://Assets/CardFront/{cityNode.Name}.png";
						GD.Print($"Attempting to load texture: {texturePath}");

						var texture = GD.Load<Texture2D>(texturePath);

						if (texture != null)
						{
							sprite.Texture = texture;
							GD.Print($"Successfully assigned texture to {cityNode.Name}");
						}
						else
						{
							GD.PrintErr($"Failed to load texture for {cityNode.Name} at path: {texturePath}");
						}
					}
					else
					{
						GD.PrintErr($"Failed to find CardImage sprite for {cityNode.Name}");
					}
				}
			}
		}
	}
}