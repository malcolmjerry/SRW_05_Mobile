using System;
using System.Collections.Generic;
using System.Linq;
using UnityORM;

namespace DataModel.DAO.Impl.SQLite {

  class GameDataDaoSqliteImpl : IGameDataDao {

    private DBMapper mapper;
    public GameData GameData { get; private set; } = new GameData();

    public GameDataDaoSqliteImpl() {
      mapper = MyDbSQLite.Instance.Mapper;
      
    }

    public List<GameData> GetAll() {
      List<GameData> dataList = mapper.Read<GameData>( "SELECT * FROM GameData order by SaveSlot" ).ToList();
      return dataList;
    }

    public void Save( int saveSlot ) {
      mapper.DeleteAll<GameData>( "SaveSlot", saveSlot );
      GameData.SaveSlot = saveSlot;
      mapper.InsertAll( new[] { GameData } );
    }

    public void Save( int saveSlot, GameData gameData ) {
      mapper.DeleteAll<GameData>( "SaveSlot", saveSlot );
      gameData.SaveSlot = saveSlot;
      mapper.InsertAll( new[] { gameData } );
    }

    public GameData Load( int saveSlot ) {
      GameData data = mapper.ReadByKey<GameData>( saveSlot );
      GameData = data;
      return data;
    }

    public void Delete( int saveSlot ) {
      mapper.DeleteAll<GameData>( "SaveSlot", saveSlot );
    }

  }

}
