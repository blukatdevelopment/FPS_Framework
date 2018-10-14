/*
	Requests and releases cells as they enter and leave a radius around the player.
*/
using Godot;
using System;
using System.Collections.Generic;

public class Treadmill {

	public float timer = 0f;
	public float timerMax = 0.5f;

	public Overworld world;
	public Actor actor; // Actor to monitor.
	public int radius; // How many layers of Cells should be  modified.
	public bool paused = false;
	public Vector2 currentCoords;
	public float scale;

	public Treadmill(Overworld world, int radius, Actor actor){
		this.world = world;
		this.radius = radius;
		this.actor = actor;
		this.scale = Overworld.GetCellScale();
		this.currentCoords = Util.CoordsFromPosition(actor.Translation, scale);
		InitialRequests();
	}

	public void InitialRequests(){
		List<Vector2> coordsInRadius = Util.CoordsInRadius(currentCoords, radius);
		foreach(Vector2 coords in coordsInRadius){
			world.RequestCell(coords);
		}
	
	}

	public void Pause(){
		paused = true;
	}

	public void Unpause(){
		paused = false;
	}

	public void Update(float delta){
		if(paused){
			return;
		}
		timer += delta;
		if(timer >= timerMax){
			timer = 0;
			CheckPlayerPos();
		}
	}

	public void CheckPlayerPos(){
		// TODO: check if player left cell. If so, release and request some cells.
	}

	// Returns true if cell is in radius
	public bool UsingCell(Vector2 coords){
		// TODO: Check if cell in radius
		return true;
	}
}