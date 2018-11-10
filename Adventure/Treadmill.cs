/*
	Requests and releases cells as they enter and leave a radius around the actor.
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

	public void ReleaseAll(){
		List<Vector2> coordsInRadius = Util.CoordsInRadius(currentCoords, radius);
		foreach(Vector2 coords in coordsInRadius){
			world.ReleaseCell(coords);
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
			FollowActor();
		}
	}

	public void FollowActor(){
		Vector2 actorCoords = Util.CoordsFromPosition(actor.Translation, scale);
		
		if(actorCoords == currentCoords){
			return; // No need to release or request cells
		}

		List<Vector2> oldCells = Util.CoordsInRadius(currentCoords, radius);
		List<Vector2> newCells = Util.CoordsInRadius(actorCoords, radius);

		foreach(Vector2 cell in oldCells){
			if(!newCells.Contains(cell)){
				GD.Print("Releasing " + cell);
				world.ReleaseCell(cell);
			}
		}

		foreach(Vector2 cell in newCells){
			world.RequestCell(cell);
		}
		
		currentCoords = actorCoords;
	}

	// Returns true if cell is in radius
	public bool UsingCell(Vector2 coords){
		foreach(Vector2 coord in Util.CoordsInRadius(currentCoords, radius)){
			if(coords == coord){
				return true;
			}
		}
		
		return false;
	}
}