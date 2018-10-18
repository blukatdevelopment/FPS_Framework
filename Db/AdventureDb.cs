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

    public SettingsDb(){
        conn = new SqliteConnection("URI=file:" + DefaultFile);
        cmd = conn.CreateCommand();
        conn.Open();
    }

    public void PrintSettings(){
        string sql = @"
            SELECT * from settings;
        ";
        cmd.CommandText = sql;
        IDataReader rdr = cmd.ExecuteReader();
        while(rdr.Read()){
            GD.Print(rdr["name"] + ":" + rdr["value"]);
        }
        rdr.Close();
    }

    public void Close(){
        conn.Close();
    }

    public void CreateTables(){
        string sql = @"
        CREATE TABLE settings 
        (
            name VARCHAR(200) PRIMARY KEY, 
            value VARCHAR(200)
        )
        ";
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        //GD.Print("Created tables.");
    }

    public string SelectSetting(string name){
        string sql = @"
            SELECT value FROM settings
            WHERE name = @name
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter ("@name", name));
        IDataReader rdr = cmd.ExecuteReader();
        string ret = "";
        if(rdr.Read()){
            ret = rdr["value"] as string;
        }
        rdr.Close();
        return ret;
    }

    public static SettingsDb Init(){
        if(System.IO.File.Exists(DefaultFile)){
            //GD.Print(DefaultFile + " already exists. Connecting.");
            return new SettingsDb();
        }
        //GD.Print(DefaultFile + " doesn't exist. Creating.");
        CreateFile(DefaultFile);
        SettingsDb db = new SettingsDb();
        db.CreateTables();
        db.InitSettings();
        db.PrintSettings();
        return db;
    }
    
    public static void CreateFile(string file){
        SQLiteConnection.CreateFile(file);
    }
}