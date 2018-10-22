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
    public string file;
    IDbConnection conn;
    IDbCommand cmd;

    public AdventureDb(string fileName){
        string filePath = SaveDirectory + fileName;
        bool initNeeded = false;
        if(!System.IO.File.Exists(filePath)){
            initNeeded = true;
            CreateFile(filePath);
        }
        conn = new SqliteConnection("URI=file:" + filePath);
        cmd = conn.CreateCommand();
        if(initNeeded){
            CreateTables();
        }
        conn.Open();
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
            cellId INT(8) PRIMARY KEY,
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
    
    public void SaveFile(Overworld world){
        foreach(int id in world.actorsData.Keys){
            ActorData data = world.actorsData[id];
            SaveActor(data);
        }

        foreach(int id in world.itemsData.Keys){
            ItemData data = world.itemsData[id];
            SaveItem(data);
        }

        foreach(int id in world.cellsData.Keys){
            TerrainCellData data = world.cellsData[id];
            SaveTerrain(data);
        }
    }

    public void SaveActor(ActorData data){
        string sql = @"
            INSERT INTO actor(id, brain, posx, posy, posz, rotx, roty, rotz)
            VALUES (@id, @brain, @posx, @posy, @posz, @rotx, @roty, @rotz);
            
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@id", data.id));
        cmd.Parameters.Add(new SqliteParameter("@brain", (int)data.brain));
        cmd.Parameters.Add(new SqliteParameter("@posx", data.pos.x));
        cmd.Parameters.Add(new SqliteParameter("@posy", data.pos.y));
        cmd.Parameters.Add(new SqliteParameter("@posz", data.pos.z));
        cmd.Parameters.Add(new SqliteParameter("@rotx", data.rot.x));
        cmd.Parameters.Add(new SqliteParameter("@rotx", data.rot.y));
        cmd.Parameters.Add(new SqliteParameter("@rotx", data.rot.z));
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

    public void SaveBlock(int cellId, int blockId, Vector3 pos, Vector3 rot){
        string sql = @"
            INSERT INTO items_extra
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
            );
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter("@cellId", cellId));
        cmd.Parameters.Add(new SqliteParameter("@cellId", blockId));
        cmd.Parameters.Add(new SqliteParameter("@orientationx", rot.x));
        cmd.Parameters.Add(new SqliteParameter("@orientationy", rot.y));
        cmd.Parameters.Add(new SqliteParameter("@orientationz", rot.z));
        cmd.Parameters.Add(new SqliteParameter("@posx", pos.x));
        cmd.Parameters.Add(new SqliteParameter("@posy", pos.y));
        cmd.Parameters.Add(new SqliteParameter("@posz", pos.z));

        cmd.ExecuteNonQuery();
    }

    public static void CreateFile(string file){
        SQLiteConnection.CreateFile(file);
    }
}