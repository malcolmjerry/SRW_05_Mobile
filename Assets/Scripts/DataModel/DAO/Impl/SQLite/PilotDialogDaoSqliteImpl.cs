

using System;
using System.Collections.Generic;
using System.Linq;
using UnityORM;

namespace DataModel.DAO.Impl.SQLite {

  class PilotDialogDaoSqliteImpl : IPilotDialogDao {

    private DBMapper mapper;

    public PilotDialogDaoSqliteImpl() {
      mapper = MyDbSQLite.Instance.Mapper;
    }

    public List<PilotDialog> GetWhenAttackingRival( int pilotID, int otherPilotID ) {
      var pdList = mapper.Read<PilotDialog>( $"SELECT * FROM PilotDialog Where PilotID = {pilotID} and OtherPilotID = {otherPilotID} and IsAttack = 1"  ).ToList();
      return pdList;
    }

    public List<PilotDialog> GetByWeapon( int pilotID, int weaponID, int weaponNameType ) {
      var pdList = mapper.Read<PilotDialog>( $"SELECT * FROM PilotDialog Where PilotID = {pilotID} and (weaponID = {weaponID} or weaponNameType = {weaponNameType} )" ).ToList();
      return pdList;
    }

    public List<PilotDialog> GetByAttack( int pilotID, bool isMelee ) {
      string meleeOrShoot = isMelee ? "IsMelee" : "IsShoot";

      //人物自身攻擊台詞
      List<PilotDialog> pdList = mapper.Read<PilotDialog>( 
                                  $"SELECT * FROM PilotDialog Where PilotID = {pilotID} and " +
                                  $"({meleeOrShoot} = 1 or (IsAttack = 1 and OtherPilotID is null))" ).ToList();
      //公用攻擊台詞
      if (pdList.Count == 0)
        pdList = mapper.Read<PilotDialog>( $"SELECT * FROM PilotDialog Where PilotID = 0 and " +
                                           $"({meleeOrShoot} = 1 or (IsAttack = 1 and OtherPilotID is null))" ).ToList();  
      return pdList;
    }

    public List<PilotDialog> GetWhenDead( int pilotID ) {
      return GetCondition( pilotID, "IsDead" );
    }

    public List<PilotDialog> GetWhenDanger( int pilotID ) {
      return GetCondition( pilotID, "IsDanger" );
    }

    public List<PilotDialog> GetWhenNormal( int pilotID ) {
      return GetCondition( pilotID, "IsNormal" );
    }

    public List<PilotDialog> GetWhenBigDam( int pilotID ) {
      return GetCondition( pilotID, "IsBigDam" );
    }

    public List<PilotDialog> GetWhenSmallDam( int pilotID ) {
      return GetCondition( pilotID, "IsSmallDam" );
    }

    public List<PilotDialog> GetWhenDodge( int pilotID ) {
      return GetCondition( pilotID, "IsDodge" );
    }

    public List<PilotDialog> GetWhenUnable( int pilotID ) {
      return GetCondition( pilotID, "IsUnable" );
    }

    /*
    public List<PilotDialog> GetWhenAny( int pilotID ) {
      return GetCondition( pilotID, "IsAny" );
    }
    */

    private List<PilotDialog> GetCondition( int pilotID, string con ) {
      var pdList = mapper.Read<PilotDialog>( $"SELECT * FROM PilotDialog Where PilotID = {pilotID} and {con} = 1" ).ToList();

      if (pdList.Count == 0) 
        pdList = mapper.Read<PilotDialog>( $"SELECT * FROM PilotDialog Where PilotID = 0 and {con} = 1" ).ToList();

      return pdList;
      //return pdList[new Random().Next( pdList.Count )];
    }



  }

}
