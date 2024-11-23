using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Sprite2D
{
    private string currentCity = "Atlanta";
    private Vector2 targetPosition;
    private bool isMoving = false;
    private float moveSpeed = 400.0f;
    private BoardLocations boardManager;
    private bool isHovered = false;

    public override void _Ready()
    {
        GD.Print("Player Ready - Initializing...");
        
        // Get reference to the board manager
        boardManager = GetNode<BoardLocations>("/root/Board");
        if (boardManager == null)
        {
            GD.PrintErr("Failed to find BoardLocations manager!");
            return;
        }
        GD.Print("Found BoardLocations manager");

        // Set initial position to Atlanta
        var atlantaNode = GetNode<Node2D>("/root/Board/BLUE_CITIES/Atlanta");
        if (atlantaNode != null)
        {
            Position = atlantaNode.Position;
            targetPosition = Position;
            GD.Print($"Player positioned at Atlanta: {Position}");
        }
        else
        {
            GD.PrintErr("Could not find Atlanta node!");
        }

        Scale = new Vector2(2, 2);
    }

    public override void _Process(double delta)
    {
        if (isMoving)
        {
            // Move towards target position
            Vector2 moveDir = (targetPosition - Position).Normalized();
            Position += moveDir * moveSpeed * (float)delta;

            // Check if we've reached the destination
            if (Position.DistanceTo(targetPosition) < 5.0f)
            {
                Position = targetPosition;
                isMoving = false;
                GD.Print($"Arrived at {currentCity}");
            }
        }
    }

    public bool TryMoveToCity(string cityName, Vector2 cityPosition)
    {
        GD.Print($"Attempting to move from {currentCity} to {cityName}");
        
        if (isMoving)
        {
            GD.Print("Movement rejected: Already moving");
            return false;
        }

        var connectedCities = boardManager.GetConnectedCitiesWithRegions(currentCity);
        GD.Print($"Connected cities to {currentCity}: {string.Join(", ", connectedCities.Select(c => c.cityName))}");
        
        // Check if the selected city is connected to current city
        if (!connectedCities.Any(city => city.cityName == cityName))
        {
            GD.Print($"Movement rejected: {cityName} is not connected to {currentCity}");
            return false;
        }

        // Set movement parameters
        targetPosition = cityPosition;
        isMoving = true;
        currentCity = cityName;
        
        GD.Print($"Movement started to {cityName} at position {cityPosition}");
        return true;
    }

    public void OnMouseEnter()
    {
        isHovered = true;
        Scale = new Vector2(4, 4);
    }

    public void OnMouseExit()
    {
        isHovered = false;
        Scale = new Vector2(2, 2);
    }

    // Helper method to get current city
    public string GetCurrentCity()
    {
        return currentCity;
    }
}