/*

    This class manages File I/O for an adventure, and now serves as a layer
    of abstraction obscuring away how this data is saved or loaded.

    Actor and item data are stored in SQLite, whilst for performance reasons
    cells are serialized directly into individual files.
*/
using Godot;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class AdventureDb{
    const string SaveDirectory = "Saves/";
    public string filePath;
    public string terrainDirectory;
    IDbConnection conn;
    IDbCommand cmd;

    IDbCommand saveBlockCommand;

    public AdventureDb(string fileName){
        filePath = SaveDirectory + fileName;
        bool initNeeded = false;
        if(!System.IO.File.Exists(filePath)){
            initNeeded = true;
            CreateFile(filePath);
        }
        conn = new SqliteConnection("URI=file:" + filePath);
        cmd = conn.CreateCommand();
        conn.Open();
        
        if(initNeeded){
            CreateTables();
        }
        InitTerrainFolder();
    }

    public void Close(){
        conn.Close();
    }

    public System.Collections.Generic.Dictionary<int, ActorData> LoadActors(){
        System.Collections.Generic.Dictionary<int, ActorData> actors;
        actors = new System.Collections.Generic.Dictionary<int, ActorData>();
        
        string sql = @"
            SELECT * from actors;
        ";

        cmd.CommandText = sql;
        IDataReader rdr = cmd.ExecuteReader();
        while(rdr.Read()){
            string json = (string)rdr["data"];
            ActorData dat = ActorData.FromJson(json);
            actors.Add(dat.id, dat);
        }
        rdr.Close();

        return actors;
    }

    public System.Collections.Generic.Dictionary<int, ItemData> LoadItems(){
        System.Collections.Generic.Dictionary<int, ItemData> items;
        items = new System.Collections.Generic.Dictionary<int, ItemData>();
        
        string sql = @"
            SELECT * from items;
        ";

        cmd.CommandText = sql;
        IDataReader rdr = cmd.ExecuteReader();

        while(rdr.Read()){
            string json = (string)rdr["data"];
            ItemData data = ItemData.FromJson(json);
            items.Add(data.id, data);
        }

        rdr.Close();

        return items;
    }

    public void CreateTables(){
        string sql = @"
        CREATE TABLE adventure 
        (
            name VARCHAR(200) PRIMARY KEY, 
            value VARCHAR(200)
        );

        CREATE TABLE items
        (
            id BIGINT PRIMARY KEY,
            data TEXT
        );

        CREATE TABLE actors
        (
            id BIGINT PRIMARY KEY,
            data TEXT
        );
        ";
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public void SaveActor(ActorData data){
        string sql = @"
            INSERT INTO actors(id, data)
            VALUES (@id, @data);
            
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@id", data.id));
        cmd.Parameters.Add(new SqliteParameter("@data", ActorData.ToJson(data)));
        
        cmd.ExecuteNonQuery();
    }

    public void SaveItem(ItemData data){
        string sql = @"
            INSERT INTO items
            (
                id, 
                data
            )
            VALUES 
            (
                @id, 
                @data
            );
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@id", data.id));
        cmd.Parameters.Add(new SqliteParameter("@data", ItemData.ToJson(data)));

        cmd.ExecuteNonQuery();
    }

    public string CellFileName(int id){
        string ret = terrainDirectory + "/cell" + id;
        return ret;
    }

    /* Save one cell into its own file. */
    public void SaveCell(TerrainCellData data){
        string cellFile = CellFileName(data.id);
        GD.Print("Saving cell " + data.id + " to " + cellFile);

        // Delete an existing cell file.
        System.IO.File.Delete(cellFile);

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(cellFile, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public TerrainCellData LoadCell(int id){
        string cellFile = CellFileName(id);

        IFormatter formatter = new BinaryFormatter();  
        Stream stream = new FileStream(cellFile, FileMode.Open, FileAccess.Read, FileShare.Read);  
        TerrainCellData ret = (TerrainCellData)formatter.Deserialize(stream);  
        stream.Close();

        return ret;
    }

    /* Save overworld data to current file. */
    public void SaveData(OverworldData dat){
        /* Tear down existing database. */
        conn.Close();
        System.IO.File.Delete(filePath);
        

        /* Set up new database for this save. */
        conn = new SqliteConnection("URI=file:" + filePath);
        cmd = conn.CreateCommand();
        conn.Open();

        CreateFile(filePath);

        CreateTables();

        string output = "Saving overworld data for " + dat.saveFile + "\n";

        output += "\tnextActorId: " + dat.nextActorId + "\n";
        output += "\tnextItemId: " + dat.nextItemId + "\n";
        output += "\titems: " + dat.itemsData.Count + "\n";
        output += "\tactors: " + dat.actorsData.Count + "\n";
        output += "\tcells: " + dat.cellsData.Count + "\n";

        GD.Print(output);

        var trans = conn.BeginTransaction();

        foreach(int id in dat.actorsData.Keys){
            GD.Print("Saving actor " + id);
            SaveActor(dat.actorsData[id]);
        }

        foreach(int id in dat.itemsData.Keys){
            GD.Print("Saving item " + id);
            SaveItem(dat.itemsData[id]);
        }

        foreach(int id in dat.cellsData.Keys){
            SaveCell(dat.cellsData[id]);
        }

        trans.Commit();

        conn.Close();
    }

    public void InitTerrainFolder(){
        terrainDirectory = filePath.Replace(".adventure", "");
        System.IO.Directory.CreateDirectory(terrainDirectory);

    }

    public OverworldData LoadData(){
        OverworldData data = new OverworldData();

        data.actorsData = LoadActors();
        data.itemsData = LoadItems();
        return data;
    }

    public static void CreateFile(string file){
        GD.Print("Creating " + file);
        SQLiteConnection.CreateFile(file);
    }
}