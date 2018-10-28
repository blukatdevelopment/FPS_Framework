/*

    This class manages an SQLite database dedicated
    to saving/loading an adventure mode game.
*/
using Godot;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Mono.Data.Sqlite;

public class AdventureDb{
    const string SaveDirectory = "Saves/";
    public string filePath;
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
    }

    public void Close(){
        conn.Close();
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
            itemType VARCHAR(200),
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

        CREATE TABLE terrain_blocks
        (
            cellId INT(8),
            blockId INT(2),
            orientationx INT(2),
            orientationy INT(2),
            orientationz INT(2),
            posx INT(2),
            posy INT(2),
            posz INT(2)
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

    public void SaveTerrain(TerrainCellData data){
        foreach(TerrainBlock block in data.blocks){
            SaveBlock(data.id, (int)block.blockId, block.gridPosition, block.orientation);
        }
    }

    /* Prepares and caches the query to insert blocks. */
    public IDbCommand GetSaveBlockCommand(){
        if(saveBlockCommand != null){
            return saveBlockCommand;
        }
        saveBlockCommand = conn.CreateCommand();
        string sql = @"
            INSERT INTO terrain_blocks
            (
                cellId,
                blockId, 
                orientationx, 
                orientationy,
                orientationz,
                posx,
                posy,
                posz 
            )
            VALUES (
                @cellId,
                @blockId,
                @orientationx,
                @orientationy,
                @orientationz,
                @posx,
                @posy,
                @posz
            );
        ";
        saveBlockCommand.CommandText = sql;
        saveBlockCommand.Parameters.Add(new SqliteParameter("@cellId", 0));
        saveBlockCommand.Parameters.Add(new SqliteParameter("@blockId", 0));
        saveBlockCommand.Parameters.Add(new SqliteParameter("@orientationx", 0.0f));
        saveBlockCommand.Parameters.Add(new SqliteParameter("@orientationy", 0.0f));
        saveBlockCommand.Parameters.Add(new SqliteParameter("@orientationz", 0.0f));
        saveBlockCommand.Parameters.Add(new SqliteParameter("@posx", 0.0f));
        saveBlockCommand.Parameters.Add(new SqliteParameter("@posy", 0.0f));
        saveBlockCommand.Parameters.Add(new SqliteParameter("@posz", 0.0f));

        saveBlockCommand.Prepare();
        return saveBlockCommand;
    }

    public void SaveBlock(int cellId, int blockId, Vector3 pos, Vector3 rot){
        cmd = GetSaveBlockCommand();

        ((SqliteParameter)cmd.Parameters[0]).Value = cellId;
        ((SqliteParameter)cmd.Parameters[1]).Value = blockId;
        ((SqliteParameter)cmd.Parameters[2]).Value = rot.x;
        ((SqliteParameter)cmd.Parameters[3]).Value = rot.y;
        ((SqliteParameter)cmd.Parameters[4]).Value = rot.z;
        ((SqliteParameter)cmd.Parameters[5]).Value = pos.x;
        ((SqliteParameter)cmd.Parameters[6]).Value = pos.y;
        ((SqliteParameter)cmd.Parameters[7]).Value = pos.z;

        cmd.ExecuteNonQuery();
    }

    /* Save overworld data to current file. */
    public void SaveData(OverworldData dat){
        /* Tear down existing database. */
        
        conn.Close();
        if(System.IO.File.Exists(filePath)){
            System.IO.File.Delete(filePath);
        }

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
            GD.Print("Saving cell " + id);
            SaveTerrain(dat.cellsData[id]);
        }

        trans.Commit();

        conn.Close();
    }

    public static void CreateFile(string file){
        SQLiteConnection.CreateFile(file);
    }
}