using System.Collections.Generic;
using System.Linq;
using UnityORM;

namespace DataModel.DAO.Impl.SQLite {

  class PartsDaoSqliteImpl : IPartsDao {

    private DBMapper mapper;

    //private List<PartsInstance> _partsInstanceList = new List<PartsInstance>();
    //public List<PartsInstance> PartsInstanceList { get { return _partsInstanceList; } }

    public List<PartsInstance> PartsInstanceList { get; set; } = new List<PartsInstance>();

    public PartsDaoSqliteImpl() {
      mapper = MyDbSQLite.Instance.Mapper;
    }

    public Parts GetPartsByID( int ID ) {
      Parts parts = mapper.ReadByKey<Parts>( ID );
      return parts;
    }

    public void SavePartsInstanceList( int saveSlot ) {
      mapper.DeleteAll<PartsInstance>( "SaveSlot", saveSlot );
      PartsInstanceList.ForEach( r => { r.SaveSlot = saveSlot; } );
      mapper.InsertAll<PartsInstance>( PartsInstanceList.ToArray() );
    }

    public void LoadPartsInstanceList( int saveSlot ) {
      List<PartsInstance> partsInstanceList = mapper.Read<PartsInstance>( "SELECT * FROM PartsInstance Where SaveSlot = " + saveSlot ).ToList();
      partsInstanceList.ForEach( inst => {
        inst.Parts = mapper.ReadByKey<Parts>( inst.PartsID );
      } );
      PartsInstanceList = partsInstanceList;
    }

  }

}
