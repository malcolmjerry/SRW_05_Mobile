using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataModel.Service {

  public class PilotService {

    private MyDbContext _myDbContext;

    public PilotService( MyDbContext myDbContext ) {
      _myDbContext = myDbContext;
    }

    public void Reset() {
      _myDbContext.PilotDao.HeroList = new List<Hero>();
      _myDbContext.PilotDao.HeroPilotInstanceList = new List<PilotInstance>();
      _myDbContext.PilotDao.PilotInstanceList = new List<PilotInstance>();
    }

    public Pilot LoadPilotBase( int id ) {
      return _myDbContext.PilotDao.GetPilotBase( id );
    }

    public Hero LoadHero( int seqNo ) {
      return _myDbContext.PilotDao.HeroList.FirstOrDefault( h => h.SeqNo == seqNo );
    }

    public PilotInstance LoadHeroPilotInstance( int heroSeqNo ) {
      return _myDbContext.PilotDao.HeroPilotInstanceList.FirstOrDefault( h => h.HeroSeqNo == heroSeqNo );
    }

    public PilotInstance LoadPilotInstance( int seqNo ) {
      return _myDbContext.PilotDao.PilotInstanceList.FirstOrDefault( h => h.SeqNo == seqNo );
    }

    private int getNewSeqNo() {
      if (_myDbContext.PilotDao.PilotInstanceList.Count == 0)
        return 1;
      return _myDbContext.PilotDao.PilotInstanceList.Max( r => r.SeqNo ) + 1;
    }

    public PilotInstance CreatePilotInstance( int ID, int exp = 0, int enable = 1, bool isPlayer = false, int level = 0, Hero hero = null ) {
      Pilot pilot = _myDbContext.PilotDao.GetPilotByID( ID );

      if (level > 0)
        exp = (level - 1) * 500;

      PilotInstance pilotInstance = new PilotInstance() {
        SaveSlot = 0,
        SeqNo = getNewSeqNo(),
        PilotID = pilot.ID,
        Pilot = pilot,
        //MaxSp = pilot.MaxSp,
        //ShortName = pilot.ShortName,
        //FirstName = pilot.FirstName,
        //LastName = pilot.LastName,
        //RemainSp = (pilot.MaxSp + 1)/2,
        //Level = level,
        Exp = exp,
        Enable = enable,
        Hero = hero,
        HeroSeqNo = hero?.SeqNo
      };
      pilotInstance.PilotSkillInstanceList = GetPilotSkillInstanceByDefault( pilotInstance );

      if (isPlayer) _myDbContext.PilotDao.PilotInstanceList.Add( pilotInstance );

      if (hero != null) {
        _myDbContext.PilotDao.HeroList.Add( hero );
        _myDbContext.PilotDao.HeroPilotInstanceList.Add( pilotInstance );
      }

      return pilotInstance;
    }

    public PilotInstance AddPilotInstance( PilotInstance pilotInstance ) {
      if (pilotInstance.HeroSeqNo.HasValue)
        return null;

      pilotInstance.SeqNo = getNewSeqNo();
      pilotInstance.PilotSkillInstanceList.ForEach( psi => psi.PilotInstanceSeqNo = pilotInstance.SeqNo );

      _myDbContext.PilotDao.PilotInstanceList.Add( pilotInstance );
      return pilotInstance;
    }

    public List<PilotSkillInstance> GetPilotSkillInstanceByDefault( PilotInstance pilotInstance ) {
      List<PilotSkillInstance> psInstances = new List<PilotSkillInstance>();
      pilotInstance.Pilot.PilotSkillDefaultList.ForEach( d => {
        PilotSkillInstance psInstance = new PilotSkillInstance() {
          DefaultPilotID = d.PilotID,
          DefaultOrder = d.Order,
          OrderSort = d.Order,
          PilotInstanceSaveSlot = pilotInstance.SaveSlot,
          PilotInstanceSeqNo = pilotInstance.SeqNo,
          Level = 0,
          PilotSkillID = d.PilotSkillID,
          PilotSkill = _myDbContext.PilotDao.GetPilotSkillByID( d.PilotSkillID )
        };
        psInstances.Add( psInstance );
      } );
      return psInstances;
    }

    public List<PilotDialog> GetPilotDialogsWhenAttack( int pilotID, int otherPilotID, int weaponID, int weaponNameType, bool isMelee ) {
      var attRivalList = _myDbContext.PilotDialogDao.GetWhenAttackingRival( pilotID, otherPilotID );
      var byWeaponList = _myDbContext.PilotDialogDao.GetByWeapon( pilotID, weaponID, weaponNameType );
      var byAttackList = _myDbContext.PilotDialogDao.GetByAttack( pilotID, isMelee );
      List<PilotDialog> allList = new List<PilotDialog>();
      allList = allList.Concat( attRivalList ).Concat( byWeaponList ).Concat( byAttackList ).ToList();

      var dialog_0 = allList[UnityEngine.Random.Range( 0, allList.Count )];
      Debug.Log( $"pilotID: {pilotID}, otherPilotID: {otherPilotID}, weaponID: {weaponID}, weaponNameType: {weaponNameType}, dialog_0: {dialog_0?.Text}" );
      return new List<PilotDialog>() { dialog_0 };
    }

    public List<PilotDialog> GetPilotDialogsWhenDef( AttackData attData ) {
      int pilotID = attData.ToUnitInfo.PilotInfo.PilotInstance.PilotID;
      float dmgPercent = (float)attData.TotalDamage / attData.ToUnitInfo.RobotInfo.MaxHP;
      bool isSmallDmg = dmgPercent <= 0.1f;
      bool isBigDmg = dmgPercent>= 0.4f;
      bool isNormal = dmgPercent > 0.1f && dmgPercent < 0.4;
      bool isDead = attData.IsDefeated;
      bool isDanger = (float)(attData.ToUnitInfo.RobotInfo.HP - attData.TotalDamage) / attData.ToUnitInfo.RobotInfo.MaxHP <= 0.2f;
      bool isDodge = attData.IsDodge;

      List<PilotDialog> allList = new List<PilotDialog>();

      // 任何情況下都通用的台詞
      //allList.AddRange( _myDbContext.PilotDialogDao.GetWhenAny( pilotID ) );

      if (isDead) {
        allList.AddRange( _myDbContext.PilotDialogDao.GetWhenDead( pilotID ) );
      }
      else if (isDodge) {
        allList.AddRange( _myDbContext.PilotDialogDao.GetWhenDodge( pilotID ) );
      }
      else {
        if (isNormal) allList.AddRange( _myDbContext.PilotDialogDao.GetWhenNormal( pilotID ) );

        if (isSmallDmg) allList.AddRange( _myDbContext.PilotDialogDao.GetWhenSmallDam( pilotID ) );

        if (isBigDmg) allList.AddRange( _myDbContext.PilotDialogDao.GetWhenBigDam( pilotID ) );

        if (isDanger) allList.AddRange( _myDbContext.PilotDialogDao.GetWhenDanger( pilotID ) );
      }

      var dialog_0 = allList[UnityEngine.Random.Range( 0, allList.Count )];
      return new List<PilotDialog>() { dialog_0 };
    }

    public List<PilotDialog> GetPilotDialogsWhenUnable( int pilotID ) {
      var allList = _myDbContext.PilotDialogDao.GetWhenUnable( pilotID );

      var dialog_0 = allList[UnityEngine.Random.Range( 0, allList.Count )];
      Debug.Log( $"pilotID: {pilotID}, dialog_0: {dialog_0?.Text}" );
      return new List<PilotDialog>() { dialog_0 };
    }

    public List<PilotInstance> ActivePilotInstanceList {
      get {
        return _myDbContext.PilotDao.PilotInstanceList;
      }
    }

    public void LoadActivePilotInstance( int saveSlot ) {
      _myDbContext.PilotDao.LoadPilotInstanceList( saveSlot );
    }

    public void SaveActivePilotInstance( int saveSlot ) {
      //Debug.Log( "start SaveActivePilotInstance: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss" ) );
      _myDbContext.PilotDao.SavePilotInstanceList( saveSlot );
      //Debug.Log( "finish SaveActivePilotInstance: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss" ) );
    }

    public void LoadBySerialize( List<PilotInstance> pilotInstanceList, List<Hero> heroList ) {
      _myDbContext.PilotDao.LoadPilotInstanceList( pilotInstanceList, heroList );
    }

    public SPCommand GetSPComByID( int ID ) {
      return _myDbContext.PilotDao.GetSPComByID( ID );
    }

    public PilotSkill GetPilotSkillByID( int ID ) {
      return _myDbContext.PilotDao.GetPilotSkillByID( ID );
    }

  }

}
