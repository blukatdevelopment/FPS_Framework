/*

    This class manages an SQLite database dedicated
    to user preferences.
*/
using Godot;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class SettingsDb{
    const string DefaultFile = @"settings.db";
    const string SavesDirectory = "Saves/";
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

    public void InitSettings(){
        StoreSetting("master_volume", "1.0");
        StoreSetting("sfx_volume", "1.0");
        StoreSetting("music_volume", "1.0");
        StoreSetting("username", "New Player");
        StoreSetting("first_login", DateTime.Today.ToString("MM/dd/yyyy"));
        StoreSetting("mouse_sensitivity_x", "1.0");
        StoreSetting("mouse_sensitivity_y", "1.0");
    }

    public void StoreSetting(string name, string val){
        string sql = @"
            INSERT OR IGNORE INTO settings(name, value)
            VALUES (@name, @value);
            UPDATE settings
            SET value = @value 
            WHERE name = @name;
        ";
        cmd.CommandText = sql;
        cmd.Parameters.Add(new SqliteParameter ("@name", name));
        cmd.Parameters.Add(new SqliteParameter ("@value", val));
        cmd.ExecuteNonQuery();
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

    public static bool SaveExists(string saveName){
        string filePath = SavesDirectory + saveName;
        return System.IO.File.Exists(filePath);
    }

    public static List<string> GetAllSaves(string extension = ""){
        string[] results;
        if(extension == ""){
            results = System.IO.Directory.GetFiles(SavesDirectory); 
        }
        else{
            results = System.IO.Directory.GetFiles(SavesDirectory, "*." + extension);
        }
        
        for(int i = 0; i < results.Length; i++){
            results[i] = results[i].Replace("Saves/", "");
            results[i] = results[i].Replace("." + extension, "");
        }
        List<string> ret = new List<String>(results);
        


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