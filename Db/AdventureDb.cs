/*

    This class manages File I/O for an adventure, and now serves as a layer
    of abstraction obscuring away how this data is saved or loaded.

    Actor and item data are stored in SQLite, whilst for performance reasons
    cells are serialized directly.
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
            ActorData dat = new ActorData();
            dat.id = System.Convert.ToInt32(rdr["id"]);
            dat.brain = (Actor.Brains)System.Convert.ToInt32(rdr["brain"]);
            
            float posx = (float)System.Convert.ToDecimal(rdr["posx"]);
            float posy = (float)System.Convert.ToDecimal(rdr["posy"]);
            float posz = (float)System.Convert.ToDecimal(rdr["posz"]);
            dat.pos = new Vector3(posx, posy, posz);

            float rotx = (float)System.Convert.ToDecimal(rdr["rotx"]);
            float roty = (float)System.Convert.ToDecimal(rdr["roty"]);
            float rotz = (float)System.Convert.ToDecimal(rdr["rotz"]);
            dat.rot = new Vector3(rotx, roty, rotz);
            GD.Print("Found actor " + dat.ToString());
            actors.Add(dat.id, dat);
        }
        rdr.Close();

        sql = @"
            SELECT * from actors_extra;
        ";

        cmd.CommandText = sql;
        rdr = cmd.ExecuteReader();

        while(rdr.Read()){
            int id = (int)rdr["id"];
            string name = (string)rdr["name"];
            string val = (string)rdr["value"];

            if(actors.ContainsKey(id)){
                ActorData actor = actors[id];

                if(actor.extra.ContainsKey(name)){
                    actor.extra[name] = val;
                }
                else{
                    actor.extra.Add(name, val);
                }
            }
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
            ItemData dat = new ItemData();
            
            dat.id = System.Convert.ToInt32(rdr["id"]);
            dat.type = (Item.Types)System.Convert.ToInt32(rdr["itemType"]);
            dat.name = (string)rdr["name"];
            dat.description = (string)rdr["description"];
            dat.weight = System.Convert.ToInt32(rdr["weight"]);
            dat.held = System.Convert.ToBoolean(rdr["held"]);

            float posx = (float)System.Convert.ToDecimal(rdr["posx"]);
            float posy = (float)System.Convert.ToDecimal(rdr["posy"]);
            float posz = (float)System.Convert.ToDecimal(rdr["posz"]);
            dat.pos = new Vector3(posx, posy, posz);

            float rotx = (float)System.Convert.ToDecimal(rdr["rotx"]);
            float roty = (float)System.Convert.ToDecimal(rdr["roty"]);
            float rotz = (float)System.Convert.ToDecimal(rdr["rotz"]);
            dat.rot = new Vector3(rotx, roty, rotz);
            
            items.Add(dat.id, dat);
        }

        rdr.Close();

        sql = @"
            SELECT * from actors_extra;
        ";

        cmd.CommandText = sql;
        rdr = cmd.ExecuteReader();

        while(rdr.Read()){
            int id = (int)rdr["id"];
            string name = (string)rdr["name"];
            string val = (string)rdr["value"];

            if(items.ContainsKey(id)){
                ItemData item = items[id];

                if(item.extra.ContainsKey(name)){
                    item.extra[name] = val;
                }
                else{
                    item.extra.Add(name, val);
                }
            }
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
            posx FLOAT,
            posy FLOAT,
            posz FLOAT,
            rotx FLOAT,
            roty FLOAT,
            rotz FLOAT,
            name VARCHAR(200),
            itemType INT,
            description TEXT,
            weight INT(2),
            held BOOLEAN
        );

        CREATE TABLE items_extra
        (
            id BIGINT,
            name VARCHAR(200),
            value TEXT
        );

        CREATE TABLE actors
        (
            id BIGINT PRIMARY KEY,
            brain VARCHAR(200),
            posx FLOAT,
            posy FLOAT,
            posz FLOAT,
            rotx FLOAT,
            roty FLOAT,
            rotz FLOAT
        );

        CREATE TABLE actors_extra
        (
            id BIGINT PRIMARY KEY,
            name VARCHAR(200),
            value TEXT
        );
        ";
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public void SaveActor(ActorData data){
        string sql = @"
            INSERT INTO actors(id, brain, posx, posy, posz, rotx, roty, rotz)
            VALUES (@id, @brain, @posx, @posy, @posz, @rotx, @roty, @rotz);
            
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@id", data.id));
        cmd.Parameters.Add(new SqliteParameter("@brain", (int)data.brain));
        cmd.Parameters.Add(new SqliteParameter("@posx", data.pos.x));
        cmd.Parameters.Add(new SqliteParameter("@posy", data.pos.y));
        cmd.Parameters.Add(new SqliteParameter("@posz", data.pos.z));
        cmd.Parameters.Add(new SqliteParameter("@rotx", data.rot.x));
        cmd.Parameters.Add(new SqliteParameter("@roty", data.rot.y));
        cmd.Parameters.Add(new SqliteParameter("@rotz", data.rot.z));
        cmd.ExecuteNonQuery();

        foreach(string key in data.extra.Keys){
            SaveActorExtra(data.id, key, data.extra[key]);
        }

    }

    public void SaveActorExtra(int id, string name, string val){
        string sql = @"
            INSERT OR IGNORE INTO actors_extra(id, name, value)
            VALUES (@id, @name, @value);
            UPDATE settings
            SET value = @value 
            WHERE name = @name
                AND id = @id;
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@id", id));
        cmd.Parameters.Add(new SqliteParameter("@name", name));
        cmd.Parameters.Add(new SqliteParameter("@value", val));
        cmd.ExecuteNonQuery();
    }

    public void SaveItem(ItemData data){
        string sql = @"
            INSERT INTO items
            (
                id, 
                posx, 
                posy, 
                posz, 
                rotx, 
                roty, 
                rotz, 
                name, 
                itemType,
                description,
                weight,
                held
            )
            VALUES 
            (
                @id, 
                @posx, 
                @posy, 
                @posz, 
                @rotx, 
                @roty, 
                @rotz, 
                @name, 
                @itemType,
                @description,
                @weight,
                @held
            );
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@id", data.id));
        cmd.Parameters.Add(new SqliteParameter("@posx", data.pos.x));
        cmd.Parameters.Add(new SqliteParameter("@posy", data.pos.y));
        cmd.Parameters.Add(new SqliteParameter("@posz", data.pos.z));
        cmd.Parameters.Add(new SqliteParameter("@rotx", data.rot.x));
        cmd.Parameters.Add(new SqliteParameter("@roty", data.rot.y));
        cmd.Parameters.Add(new SqliteParameter("@rotz", data.rot.z));
        cmd.Parameters.Add(new SqliteParameter("@name", data.name));
        cmd.Parameters.Add(new SqliteParameter("@itemType", (int)data.type));
        cmd.Parameters.Add(new SqliteParameter("@description", data.description));
        cmd.Parameters.Add(new SqliteParameter("@weight", data.weight));
        cmd.Parameters.Add(new SqliteParameter("@held", data.held));
        cmd.ExecuteNonQuery();

        foreach(string key in data.extra.Keys){
            SaveItemExtra(data.id, key, data.extra[key]);
        }
    }

    public void SaveItemExtra(int id, string name, string val){
        string sql = @"
            INSERT OR IGNORE INTO items_extra(id, name, value)
            VALUES (@id, @name, @value);
            UPDATE settings
            SET value = @value 
            WHERE name = @name
                AND id = @id;
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@id", id));
        cmd.Parameters.Add(new SqliteParameter("@name", name));
        cmd.Parameters.Add(new SqliteParameter("@value", val));
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