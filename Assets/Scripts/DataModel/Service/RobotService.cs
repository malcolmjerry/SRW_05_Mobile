using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataModel.Service {

  public class RobotService {

    //public List<RobotInstance> ActiveRobotInstanceList { get; set; }

    private MyDbContext db;

    public RobotService( MyDbContext myDbContext ) {
      db = myDbContext;
    }

    public void Reset() {
      db.RobotDao.RobotInstanceList = new List<RobotInstance>();
    }

    private int getNewSeqNo() {
      if (db.RobotDao.RobotInstanceList.Count == 0)
        return 1;
      return db.RobotDao.RobotInstanceList.DefaultIfEmpty().Max( r => r.SeqNo ) + 1;
    }

    public List<RobotInstance> ActiveRobotInstanceList {
      get {
        return db.RobotDao.RobotInstanceList;
      }
    }

    public void LoadActiveRobotInstance( int saveSlot ) {
      db.RobotDao.LoadRobotInstanceList( saveSlot );
      DIContainer.Instance.PartsService.Load( saveSlot );
      setupPartsList();  //必須先讀出所有 Parts
    }

    public void SaveActiveRobotInstance( int saveSlot ) {
      //Debug.Log( "start SaveActiveRobotInstance: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss" ) );
      db.RobotDao.SaveRobotInstanceList( saveSlot );
      //Debug.Log( "finish SaveActiveRobotInstance: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss" ) );
    }

    public RobotInstance CreateRobotInstance( int ID, bool isPlayer, List<PartsInstance> partsInstanceList = null, int startLv = 0, int weaponLv = 0 ) {
      Robot robot = db.RobotDao.GetRobotByID( ID );

      if (partsInstanceList?.Count > robot.PartsSlot) {
        partsInstanceList = partsInstanceList.Take( robot.PartsSlot ).ToList();
      }
      //throw new Exception( $"Robot parts slot only [{robot.PartsSlot}], but init parts count {partsInstanceList.Count}" );

      RobotInstance robotInstance = new RobotInstance() {
        SeqNo = getNewSeqNo(),
        Robot = robot,
        RobotID = robot.ID,
        BGM = robot.BGM,
        Skill1ID = robot.Skill1ID,
        Skill2ID = robot.Skill2ID,
        Skill3ID = robot.Skill3ID,
        Skill4ID = robot.Skill4ID,
        Skill5ID = robot.Skill5ID,
        Skill6ID = robot.Skill6ID,
        Enable = isPlayer? 1 : 0,
        HPLv = 0, ENLv = 0, MotilityLv = 0, ArmorLv = 0, HitLv = 0, MovePowerLv = 0
      };
      for (int i = 0; i<robot.PartsSlot; i++) { robotInstance.PartsInstanceList.Add( null ); }

      robotInstance.WeaponInstanceList = new List<WeaponInstance>();
      int weaponOrder = 0;
      foreach (var weapon in robot.WeaponList.Where( w => w.DefaultEnable )) {
        WeaponInstance weaponInstance = new WeaponInstance() {
          //SaveSlot = 0,
          SeqNo = weaponOrder++,
          //RobotInstanceSaveSlot = 0,
          RobotInstanceSeqNo = robotInstance.SeqNo,
          Level = weaponLv,
          Enable = 1,
          WeaponID = weapon.ID,
          Weapon = weapon,
          //RobotInstance = robotInstance
        };
        robotInstance.WeaponInstanceList.Add( weaponInstance );
      }

      injectRobotSkills( robotInstance );

      int order = 1;
      foreach (var parts in partsInstanceList?? Enumerable.Empty<PartsInstance>()) {
        SetupParts( robotInstance, parts, order++ );
      }

      if (isPlayer) db.RobotDao.RobotInstanceList.Add( robotInstance );

      return robotInstance;
    }

    public RobotInstance AddRobotInstance( RobotInstance robotInstance ) {
      robotInstance.SeqNo = getNewSeqNo();
      robotInstance.WeaponInstanceList.ForEach( wi => wi.RobotInstanceSeqNo = robotInstance.SeqNo );
      robotInstance.PartsInstanceList.ForEach( pi => {
        if (pi != null)
          pi.RobotInstanceSeqNo = robotInstance.SeqNo;
      } );
      db.RobotDao.RobotInstanceList.Add( robotInstance );
      return robotInstance;
    }

    private void injectRobotSkills( RobotInstance robotInstance ) {
      robotInstance.RobotSkill1 = db.RobotDao.GetRobotSkillByID( robotInstance.Skill1ID );
      robotInstance.RobotSkill2 = db.RobotDao.GetRobotSkillByID( robotInstance.Skill2ID );
      robotInstance.RobotSkill3 = db.RobotDao.GetRobotSkillByID( robotInstance.Skill3ID );
      robotInstance.RobotSkill4 = db.RobotDao.GetRobotSkillByID( robotInstance.Skill4ID );
      robotInstance.RobotSkill5 = db.RobotDao.GetRobotSkillByID( robotInstance.Skill5ID );
      robotInstance.RobotSkill6 = db.RobotDao.GetRobotSkillByID( robotInstance.Skill6ID );
      return;
    }

    public void SetupParts( RobotInstance robotInstance, PartsInstance partsInstance, int order ) {
      PartsInstance sourceParts = robotInstance.PartsInstanceList.FirstOrDefault( p => p?.RobotOrder == order );
      if (sourceParts != null) {
        sourceParts.UnplugFromRobot();
        robotInstance.PartsInstanceList[order-1] = null;
      }

      if (partsInstance == null) {
        return;
      }

      if (/*partsInstance.RobotInstanceSaveSlot.HasValue && */partsInstance.RobotInstanceSeqNo.HasValue && partsInstance.RobotOrder.HasValue) {
        RobotInstance oldRobot = db.RobotDao.RobotInstanceList.FirstOrDefault( r => /*r.SaveSlot == partsInstance.RobotInstanceSaveSlot && */ r.SeqNo == partsInstance.RobotInstanceSeqNo );
        oldRobot.PartsInstanceList[partsInstance.RobotOrder.Value - 1] = null;
        partsInstance.UnplugFromRobot();        
      }

      robotInstance.PartsInstanceList[order - 1] = partsInstance;
      partsInstance.RobotOrder = order;
      partsInstance.RobotInstanceSaveSlot = robotInstance.SaveSlot;
      partsInstance.RobotInstanceSeqNo = robotInstance.SeqNo;
      partsInstance.RobotInstance = robotInstance;
      return;
    }

    private void setupPartsList() {
      db.RobotDao.RobotInstanceList.ForEach( ri => {
        List<PartsInstance> partsList = db.PartsDao.PartsInstanceList.Where( p => p.SaveSlot == ri.SaveSlot && p.RobotInstanceSeqNo == ri.SeqNo ).ToList();
        foreach (var partsIn in partsList) {
          int index = partsIn.RobotOrder.HasValue ? (partsIn.RobotOrder.Value - 1) : 0;
          ri.PartsInstanceList[index] = partsIn;
          partsIn.RobotInstance = ri;
        }
      } );
    }

  }

}
