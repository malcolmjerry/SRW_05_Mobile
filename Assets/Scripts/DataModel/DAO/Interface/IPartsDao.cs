using System.Collections.Generic;

public interface IPartsDao {

  Parts GetPartsByID( int ID );

  void SavePartsInstanceList( int saveSlot );

  void LoadPartsInstanceList( int saveSlot );

  List<PartsInstance> PartsInstanceList { get; set; }
}

