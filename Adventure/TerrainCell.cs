/*
	A terrain cell is a GridMap used to represent one subsection of the overworld's terrain.
*/
using System;
using Godot;
using System.Collections.Generic;

public class TerrainCell : GridMap{
	public int id; // id according to overworld.
	public Vector2 coords;
	public int cellSize;
	public List<TerrainBlock> blockData;
	public Overworld world;

	public List<Actor> packedActors;
	public List<Item> packedItems;

	public TerrainCell(Overworld world, int cellSize = 1){
		BaseInit(world);
		this.cellSize = cellSize;
		this.blockData = new List<TerrainBlock>();
	}

	public TerrainCell(Overworld world, TerrainCellData data, int cellSize = 1){
		BaseInit(world);
		this.cellSize = cellSize;	
		LoadData(data);
	}

	private void BaseInit(Overworld world){
		this.world = world;
		Theme = TerrainBlock.GetTheme();
		CellSize = Overworld.BaseBlockSize();
	}


	// Return width of this  in world unitys
	public float GetWidth(){
		return CellSize.x * cellSize;
	}

	public bool InBounds(Vector3 pos){
		Vector3 min = GetMinBounds();
		Vector3 max = GetMaxBounds();

		return Util.InBounds(min, max, pos);
	}

	public Vector3 GetMinBounds(){
		return Translation;
	}

	public Vector3 GetMaxBounds(){
		Vector3 ret = Translation;
		float scale = CellSize.x * cellSize;
		ret += new Vector3(scale, scale, scale);
		return ret;
	}


	public TerrainCellData GetData(){
		TerrainCellData data = new TerrainCellData();
		data.coords = coords;
		data.id = id;
		data.blocks = blockData;
		return data;
	}
	
	public void LoadData(TerrainCellData data){
		coords = data.coords;
		id = data.id;
		blockData = data.blocks;
		foreach(TerrainBlock block in data.blocks){
			int x = (int)block.gridPosition.x;
			int y = (int)block.gridPosition.y;
			int z = (int)block.gridPosition.z;
			int meshId = (int)block.blockId;
			SetCellItem(x, y, z, meshId);
		}
	}

	public Vector3 CenteredPos(Vector3 pos){
		float effectiveScale = GetWidth();
		Vector3 offset = new Vector3(effectiveScale/2, 0, effectiveScale/2);
		return pos - offset;
	}


	// Returns center of this terrain cell
	public Vector3 GetPos(){
		float effectiveScale = cellSize * CellSize.x; //Assume cubeic grid
		Vector3 offset = new Vector3(effectiveScale/2, 0, effectiveScale/2);
		return Translation + offset;
	}

	// Move cell and contents to new position.
	public void SetPos(Vector3 pos){
		// Move cell
		Vector3 oldPos = Translation; // Old position, not centered
		Vector3 centeredPos = CenteredPos(pos);
		Translation = centeredPos;


		string debug = "TerrainCell.SetPos\n";
		debug += "  Moving cell " + id + " from " + oldPos + " to " + pos + "\n";
		debug += "  Total translation: " + (pos - oldPos);
		//GD.Print(debug);

		// Move contents
		Vector3 translation = pos - oldPos;
		TranslateContents(translation);
	}

	// Translate all items and actors in bounds
	public void TranslateContents(Vector3 translation){
	  Vector3 start = GetPos();
	 	List<Item> items = ItemsInBounds();
	 	List<Actor> actors = ActorsInBounds();
	 	foreach(Item item in items){
	 		//item.Translation = item.Translation + translation;
	 	}

	 	foreach(Actor actor in actors){
	 		//GD.Print("Moving actor from " + actor.Translation + " to " + (actor.Translation + translation) + " by " + translation);
	 		actor.Translation = actor.Translation + translation;
	 	}
	}

	// Translate all items and actors in bounds
	public void TranslatePackedContents(Vector3 translation){
	  Vector3 start = GetPos();
	 	List<Item> items = ItemsInBounds();
	 	List<Actor> actors = ActorsInBounds();
	 	foreach(Item packedItem in packedItems){
	 		//packedItem.Translation = packedItem.Translation + translation;
	 	}

	 	foreach(Actor packedActor in packedActors){
	 		//GD.Print("Moving actor from " + packedActor.Translation + " to " + (packedActor.Translation + translation) + " by " + translation);
	 		packedActor.Translation = packedActor.Translation + translation;
	 	}
	}

	// Preserves position of activePos relative to newPos.
	public Vector3 RecenterActivePos(Vector3 oldPos, Vector3 newPos, Vector3 activePos){
		Vector3 diff = activePos - oldPos;
		Vector3 recenteredPos = newPos + diff;
		return recenteredPos;
	}

	public List<Item> ItemsInBounds(){
		return world.ActiveItemsInBounds(this);
	}

	public List<Actor> ActorsInBounds(){
		return world.ActiveActorsInBounds(this);
	}

	/* Record and pause all contents. */
	public void Pack(){
		packedItems = ItemsInBounds();
		packedActors = ActorsInBounds();
		
		foreach(Item packedItem in packedItems){
			packedItem.Pause();
		}

		foreach(Actor packedActor in packedActors){
			packedActor.Pause();
		}

	}

	/* Move packed contents and then unpause/forget them. */
	public void Unpack(Vector3 pos){
		if(packedItems == null || packedItems == null){
			//GD.Print("TerrainCell.Unpack: Cannot unpack cell before packing it.");
			return;
		}

		Vector3 oldPos = Translation; // Old position, not centered
		Vector3 centeredPos = CenteredPos(pos);
		Translation = centeredPos;


		string debug = "TerrainCell.Unpack\n";
		debug += "  Moving cell " + id +  "(" + coords + ") from " + oldPos + " to " + pos + "\n";
		debug += "  Total translation: " + (pos - oldPos);
		//GD.Print(debug);

		// Move packed contents.
		Vector3 translation = CenteredPos(pos) - CenteredPos(oldPos);
		TranslatePackedContents(translation);

		foreach(Item packedItem in packedItems){
			packedItem.Unpause();
		}

		foreach(Actor packedActor in packedActors){
			packedActor.Unpause();
		}

		packedItems = null;
		packedActors = null;
	}

}