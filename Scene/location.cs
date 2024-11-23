using Godot;
using System;

public partial class location : Sprite2D
{
    string greenPath = "res://Assets/GREEN_SPRITE.png";
    string redPath = "res://Assets/RED_SPRITE.png";
    string yellowPath = "res://Assets/YELLOW_SPRITE.png";
    string bluePath = "res://Assets/BLUE_SPRITE.png";

    private bool isHovered = false;
    private bool isSelected = false;
    private Color highlightColor = new Color(1, 1, 1, 0.5f);
    private float highlightRadius = 35.0f;
    
    private Label nameLabel;

    public override void _Ready()
    {
        NodePath currentNodePath = GetParent().GetPath();

        switch(currentNodePath)
        {
            case "/root/Board/BLUE_CITIES":
                Texture = GD.Load<Texture2D>(greenPath);
                highlightColor = new Color(0, 0.8f, 0, 0.5f);
                break;
            case "/root/Board/BLACK_CITIES":
                Texture = GD.Load<Texture2D>(redPath);
                highlightColor = new Color(0.8f, 0, 0, 0.5f);
                break;
            case "/root/Board/RED_CITIES":
                Texture = GD.Load<Texture2D>(bluePath);
                highlightColor = new Color(0, 0, 0.8f, 0.5f);
                break;
            case "/root/Board/YELLOW_CITIES":
                Texture = GD.Load<Texture2D>(yellowPath);
                highlightColor = new Color(0.8f, 0.8f, 0, 0.5f);
                break;
            default:
                break;
        }

        nameLabel = new Label();
        nameLabel.Text = Name;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.VerticalAlignment = VerticalAlignment.Center;
        nameLabel.Position = new Vector2(-50, -50);
        nameLabel.CustomMinimumSize = new Vector2(100, 20);
        nameLabel.Visible = false;
        AddChild(nameLabel);
    }

    public override void _Draw()
    {
        if (isHovered || isSelected)
        {
            Color currentColor = isSelected ? 
                new Color(highlightColor.R, highlightColor.G, highlightColor.B, 0.8f) :
                highlightColor;
            
            DrawCircle(Vector2.Zero, highlightRadius, currentColor);
        }
    }

    public void OnMouseEnter()
    {
        isHovered = true;
        Scale = new Vector2(2,2);
        QueueRedraw();
        GD.Print($"Entered {Name}");
    }

    public void OnMouseExit()
    {
        isHovered = false;
        if (!isSelected)
        {
            Scale = new Vector2(1,1);
        }
        QueueRedraw();
        GD.Print($"Exited {Name}");
    }

    public void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                HandleClick();
            }
        }
    }

    private void HandleClick()
    {
        GD.Print($"Location clicked: {Name}");
        
        // Toggle selection state
        isSelected = !isSelected;
        
        // Update visuals
        Scale = isSelected ? new Vector2(2,2) : new Vector2(1,1);
        nameLabel.Visible = isSelected;
        QueueRedraw();

        // If this location is selected, try to move the player here
        if (isSelected)
        {
            var player = GetNode<Player>("/root/Board/Player");
            if (player != null)
            {
                GD.Print($"Found player, current position: {player.Position}");
                GD.Print($"Attempting to move player to {Name} at position {Position}");
                bool moveSuccess = player.TryMoveToCity(Name, Position);
                GD.Print($"Move attempt result: {(moveSuccess ? "Success" : "Failed")}");
            }
            else
            {
                GD.PrintErr("Could not find player node!");
            }

            // Deselect all other locations
            var board = GetNode("/root/Board");
            foreach (var groupName in new[] { "BLUE_CITIES", "YELLOW_CITIES", "BLACK_CITIES", "RED_CITIES" })
            {
                var group = board.GetNode<Node>(groupName);
                if (group != null)
                {
                    foreach (Node node in group.GetChildren())
                    {
                        if (node is location otherLocation && otherLocation != this)
                        {
                            otherLocation.Deselect();
                        }
                    }
                }
            }
        }
    }

    public void Deselect()
    {
        isSelected = false;
        if (!isHovered)
        {
            Scale = new Vector2(1,1);
        }
        nameLabel.Visible = false;
        QueueRedraw();
    }
}