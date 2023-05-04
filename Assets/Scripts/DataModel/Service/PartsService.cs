
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataModel.Service {

  public class PartsService {

    private MyDbContext db;

    public PartsService( MyDbContext myDbContext ) {
      db = myDbContext;
    }

    public void Reset() {
      db.PartsDao.PartsInstanceList = new List<PartsInstance>();
    }

    private int getNewSeqNo() {
      if (db.PartsDao.PartsInstanceList == null)
        db.PartsDao.LoadPartsInstanceList( 0 );

      return db.PartsDao.PartsInstanceList.Count == 0 ? 1 : db.PartsDao.PartsInstanceList.Max( r => r.SeqNo ) + 1;
    }

    public PartsInstance CreatePartsInstanceByPartsID( int ID, bool isPlayer ) {
      Parts parts = db.PartsDao.GetPartsByID( ID );

      PartsInstance partsInstance = new PartsInstance() {
        SeqNo = getNewSeqNo(),
        Parts = parts,
        PartsID = ID,
        RobotInstanceSeqNo = null
      };

      if (isPlayer) db.PartsDao.PartsInstanceList.Add( partsInstance );

      return partsInstance;
    }

    public PartsInstance AddPartsInstance( PartsInstance partsInstance ) {
      if (partsInstance == null)
        return null;

      partsInstance.SeqNo = getNewSeqNo();
      db.PartsDao.PartsInstanceList.Add( partsInstance );
      return partsInstance;
    }

    public List<PartsInstance> ActivePartsInstanceList {
      get {
        return db.PartsDao.PartsInstanceList;
      }
    }

    public void Load( int saveSlot ) {
      db.PartsDao.LoadPartsInstanceList( saveSlot );
      /*
      var robotList = DIContainer.Instance.RobotService.ActiveRobotInstanceList;

      foreach (var parts in ActivePartsInstanceList) { 

      }
      */
    }

    public void Save( int saveSlot ) {
      //Debug.Log( "start Save Parts: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss" ) );
      db.PartsDao.SavePartsInstanceList( saveSlot );
      //Debug.Log( "finish Save Parts: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss" ) );
    }

    public List<PartsGroup> GetPartsGroupList() {
      List<PartsGroup> partsGroupList = ActivePartsInstanceList.GroupBy( p => p.PartsID ).Select( g => new PartsGroup {
        PartsID = g.Key,
        //Parts = g.Select( p => p.Parts ).FirstOrDefault(),
        PartsIn = g.FirstOrDefault(),
        Count = g.Count( p => !p.RobotInstanceSeqNo.HasValue ),
        Total = g.Count()
      } ).ToList();
      return partsGroupList;      
    }

    /*
    public PartsInstance GetUsableByPartsID( int partsId ) {
      var result = ActivePartsInstanceList.FirstOrDefault( pi => pi.PartsID == partsId && !pi.RobotInstanceSeqNo.HasValue );
      return result;
    }
    */

    public List<PartsInstance> GetAllPartsById( int partsId ) {
      var list = ActivePartsInstanceList.Where( pi => pi.PartsID == partsId ).ToList();
      return list;
    }

  }

  public class PartsGroup {
    public int PartsID;
    public PartsInstance PartsIn;
    public int Count;
    public int Total;
  }


}
