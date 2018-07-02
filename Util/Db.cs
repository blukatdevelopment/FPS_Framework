/*
	This class aims to abstract away SQLite and related file operations.
*/
using Godot;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

public class Db{
	const string DefaultFile = @"Db/save.db";
	public string file;
	public SQLiteConnection conn;

	public Db(string file){
		this.file = file;
		conn = new SQLiteConnection("Data Source="+ file + ";Version=3;");
	}

	public void StoreSetting(string name, string value){

	}

	public string SelectSetting(string name){
		return "TODO";
	}



	// Static methods

	public static Db Init(){
		if(!System.IO.File.Exists(DefaultFile)){
			Create(DefaultFile);
		}
		return new Db(DefaultFile);
	}
	
	public static void Create(string file){
		SQLiteConnection.CreateFile(file);

	}



	
}