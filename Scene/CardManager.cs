using Godot;
using System;

public partial class CardManager : Node2D
{
	Node2D CardSelect = null;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (CardSelect != null)
		{
			var mousePosition = GetGlobalMousePosition();
			CardSelect.Position = mousePosition;
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AssignCardSprites();
	}
}