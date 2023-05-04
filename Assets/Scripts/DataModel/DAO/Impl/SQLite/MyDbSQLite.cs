using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityORM;

namespace DataModel.DAO.Impl.SQLite {

  class MyDbSQLite : MyDbContext {

    private static MyDbSQLite instance;
    public static MyDbSQLite Instance {
      get {
        if (instance == null) {
          instance = new MyDbSQLite();
          instance.RobotDao = new RobotDaoSqliteImpl();
          instance.PilotDao = new PilotDaoSqliteImpl();
          //instance.HeroDao = new HeroDaoSqliteImpl();
          instance.PartsDao = new PartsDaoSqliteImpl();
          instance.PilotDialogDao = new PilotDialogDaoSqliteImpl();
          instance.GameDataDao = new GameDataDaoSqliteImpl();
        }
        return instance;
      }
    }

    string dbDestination;
    private DBEvolution Evolution;
    private DBMapper _mapper;
    public DBMapper Mapper { get { return _mapper; } }

    private MyDbSQLite() {
      //Evolution = new DBEvolution( Path.Combine( Application.dataPath, "Data/SRW_05.db" ) );
      copyDbFile( "SRW_05_Encrypted.db", "SRW_05_Encrypted.db" );
      _mapper = new DBMapper( Evolution.Database );
    }

    //override public IRobotDao RobotDao { get; set; }

    //override public IPilotDao PilotDao { get; set; }

    //override public IPartsDao PartsDao { get; set; }

    void copyDbFile( string fromFileName, string toFileName ) {
      dbDestination = Path.Combine( Application.persistentDataPath, "data" );
      dbDestination = Path.Combine( dbDestination, toFileName );

      //Check if the File do not exist then copy it
      if (!File.Exists( dbDestination )) {
        //Where the db file is at
        string dbStreamingAsset = Path.Combine( Application.streamingAssetsPath, fromFileName );

        byte[] result;

        //Read the File from streamingAssets. Use WWW for Android
        /*
        if (dbStreamingAsset.Contains( "://" ) || dbStreamingAsset.Contains( ":///" )) {
          UnityWebRequest www = UnityWebRequest.Get( dbStreamingAsset );
          yield return www.SendWebRequest();
          result = www.downloadHandler.data;
        }
        else {
          result = File.ReadAllBytes( dbStreamingAsset );
        }
        */
        result = File.ReadAllBytes( dbStreamingAsset );
        Debug.Log( $"Loaded db file {dbStreamingAsset}" );

        //Create Directory if it does not exist
        //if (!Directory.Exists( Path.GetDirectoryName( dbDestination ) )) {
          //Directory.CreateDirectory( Path.GetDirectoryName( dbDestination ) );
        //}
        Directory.CreateDirectory( Path.GetDirectoryName( dbDestination ) );

        //Copy the data to the persistentDataPath where the database API can freely access the file
        File.WriteAllBytes( dbDestination, result );
        Debug.Log( $"Copied db file {dbDestination}" );
      }

      Evolution = new DBEvolution( dbDestination );
    }

  }

}
