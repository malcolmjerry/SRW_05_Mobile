using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityORM;

namespace DataModel.DAO.Impl.SQLite {

  class MyDbSQLite : MyDbContext {

    public override string FROM_DB_FILE { protected set; get; } = @"SRW_05_Encrypted.db";
    //public override string FROM_DB_FILE { protected set; get; } = @"SRW_05.db";
    public const string KEY = @"aaaapassword";

    private static MyDbSQLite instance;
    public static MyDbSQLite Instance {
      get {
        if (instance == null) {
          instance = new MyDbSQLite();
        }
        return instance;
      }
    }

    //string dbDestination;
    private DBEvolution Evolution;
    private DBMapper _mapper;
    public DBMapper Mapper { get { return _mapper; } }

    private MyDbSQLite() {
      //Evolution = new DBEvolution( Path.Combine( Application.dataPath, "Data/SRW_05.db" ) );
      /*
      copyDbFile( "SRW_05_Encrypted.db", "SRW_05_Encrypted.db", () => {
        _mapper = new DBMapper( Evolution.Database );
      } );
      */
    }

    public override void Init( string distFile ) {
      Evolution = new DBEvolution( distFile, KEY );
      _mapper = new DBMapper( Evolution.Database );

      instance.RobotDao = new RobotDaoSqliteImpl();
      instance.PilotDao = new PilotDaoSqliteImpl();
      //instance.HeroDao = new HeroDaoSqliteImpl();
      instance.PartsDao = new PartsDaoSqliteImpl();
      instance.PilotDialogDao = new PilotDialogDaoSqliteImpl();
      instance.GameDataDao = new GameDataDaoSqliteImpl();
    }

    //override public IRobotDao RobotDao { get; set; }

    //override public IPilotDao PilotDao { get; set; }

    //override public IPartsDao PartsDao { get; set; }

    /*
    IEnumerator copyDbFile( string fromFileName, string toFileName ) {
    IEnumerator copyDbFile( string fileName ) {
      string dbDestination = Path.Combine( Application.persistentDataPath, "data" );
      dbDestination = Path.Combine( dbDestination, fileName );

      Debug.Log( $"MyDbSQLite dbDestination: {dbDestination}, Exist: {File.Exists( dbDestination )}" );

      byte[] result = new byte[] { };
      //Check if the File do not exist then copy it
      if (!File.Exists( dbDestination )) {
        //Where the db file is at
        string dbStreamingAsset = Path.Combine( Application.streamingAssetsPath, fileName );
        Debug.Log( $"Application.streamingAssetsPath & fromFileName: {dbStreamingAsset}, Exist: {File.Exists( dbStreamingAsset )}" );

        //Read the File from streamingAssets. Use WWW for Android
        if (dbStreamingAsset.Contains( "://" ) || dbStreamingAsset.Contains( ":///" )) {
          UnityWebRequest www = UnityWebRequest.Get( dbStreamingAsset );
          yield return www.SendWebRequest();

          if (www.result == UnityWebRequest.Result.Success)
            result = www.downloadHandler.data;
          else {
            Debug.LogError( $"Failed to load {dbStreamingAsset}: {www.error}" );
            throw new Exception( $"Failed to load {dbStreamingAsset}: {www.error}" );
          }
        }
        else {
          result = File.ReadAllBytes( dbStreamingAsset );
        }

        //result = File.ReadAllBytes( dbStreamingAsset );
        Debug.Log( $"Loaded db file {dbStreamingAsset}" );

        Directory.CreateDirectory( Path.GetDirectoryName( dbDestination ) );

        //Copy the data to the persistentDataPath where the database API can freely access the file
        File.WriteAllBytes( dbDestination, result );
        Debug.Log( $"Copied db file {dbDestination}" );
      }

      System.IO.FileInfo fileInfo = new System.IO.FileInfo( dbDestination );
      long fileSizeInBytes = fileInfo.Length;
      float fileSizeInKB = fileSizeInBytes / 1024f;
      Debug.Log( string.Format( "{0} 的檔案大小為 {1:F2} KB", fileInfo.Name, fileSizeInKB ) );
      yield return dbDestination;
    }
    */
  }

}
