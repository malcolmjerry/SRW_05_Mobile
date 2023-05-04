using System.Collections.Generic;

public interface IGameDataDao {

  GameData GameData { get; }

  List<GameData> GetAll();

  void Save( int saveSlot );

  void Save( int saveSlot, GameData gameData );

  GameData Load( int saveSlot );

  void Delete( int saveSlot );
}

