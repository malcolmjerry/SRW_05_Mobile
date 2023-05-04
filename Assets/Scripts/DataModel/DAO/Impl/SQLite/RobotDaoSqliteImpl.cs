using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityORM;

namespace DataModel.DAO.Impl.SQLite {

  class RobotDaoSqliteImpl : IRobotDao {

    private DBMapper mapper;

    //private List<RobotInstance> _robotInstanceList = new List<RobotInstance>();
    //public List<RobotInstance> RobotInstanceList { get { return _robotInstanceList; } }
    public List<RobotInstance> RobotInstanceList { get; set; } = new List<RobotInstance>();

    public RobotDaoSqliteImpl() {
      mapper = MyDbSQLite.Instance.Mapper;

    }

    public Robot GetRobotByID( int ID ) {
      Robot robot = mapper.ReadByKey<Robot>( ID );
      robot.WeaponList = mapper.Read<Weapon>( "SELECT * FROM Weapon Where RobotID = " + ID ).OrderBy( w => w.HitPoint ).ToList();
      return robot;
    }

    public RobotSkill GetRobotSkillByID( int? ID ) {
      if (!ID.HasValue) return null;

      RobotSkill robotSkill = mapper.ReadByKey<RobotSkill>( ID );
      return robotSkill;
    }

    public List<RobotSkill> GetRobotSkillsByRobotInstance( RobotInstance robotInstance ) {
      List<RobotSkill> list = new List<RobotSkill>();

      list.Add( robotInstance.Skill1ID == 0 ? null : GetRobotSkillByID( robotInstance.Skill1ID ) );
      list.Add( robotInstance.Skill2ID == 0 ? null : GetRobotSkillByID( robotInstance.Skill2ID ) );
      list.Add( robotInstance.Skill3ID == 0 ? null : GetRobotSkillByID( robotInstance.Skill3ID ) );
      list.Add( robotInstance.Skill4ID == 0 ? null : GetRobotSkillByID( robotInstance.Skill4ID ) );
      list.Add( robotInstance.Skill5ID == 0 ? null : GetRobotSkillByID( robotInstance.Skill5ID ) );
      list.Add( robotInstance.Skill6ID == 0 ? null : GetRobotSkillByID( robotInstance.Skill6ID ) );

      return list;
    }

    private void injectRobotSkills( RobotInstance robotInstance ) {
      robotInstance.RobotSkill1 = GetRobotSkillByID( robotInstance.Skill1ID );
      robotInstance.RobotSkill2 = GetRobotSkillByID( robotInstance.Skill2ID );
      robotInstance.RobotSkill3 = GetRobotSkillByID( robotInstance.Skill3ID );
      robotInstance.RobotSkill4 = GetRobotSkillByID( robotInstance.Skill4ID );
      robotInstance.RobotSkill5 = GetRobotSkillByID( robotInstance.Skill5ID );
      robotInstance.RobotSkill6 = GetRobotSkillByID( robotInstance.Skill6ID );
      return;
    }


    public void SaveRobotInstanceList( int saveSlot ) {
      mapper.DeleteAll<RobotInstance>( "SaveSlot", saveSlot );
      mapper.DeleteAll<WeaponInstance>( "SaveSlot", saveSlot );

      List<WeaponInstance> weaponInstanceList = new List<WeaponInstance>();

      RobotInstanceList.ForEach( r => {
        r.SaveSlot = saveSlot;

        r.WeaponInstanceList.ForEach( w => {
          w.SaveSlot = saveSlot;
          w.RobotInstanceSaveSlot = saveSlot;
          w.RobotInstanceSeqNo = r.SeqNo;
        } );

        //mapper.InsertAll<WeaponInstance>( r.WeaponInstanceList.ToArray() );
        weaponInstanceList.AddRange( r.WeaponInstanceList );
      } );

      try {
        mapper.InsertAll<WeaponInstance>( weaponInstanceList.ToArray() );
        mapper.InsertAll<RobotInstance>( RobotInstanceList.ToArray() );
      }
      catch (Exception e) {
        Debug.Log( "SaveRobotInstanceList exception: " + e.Message );
      }
    }

    public void LoadRobotInstanceList( int saveSlot ) {
      List<RobotInstance> robotInstanceList = mapper.Read<RobotInstance>( "SELECT * FROM RobotInstance Where SaveSlot = " + saveSlot ).ToList();
      robotInstanceList.ForEach( ri => {
        ri.WeaponInstanceList = LoadWeaponInstanceList( ri.SaveSlot, ri.SeqNo, ri );
        //ri.PartsInstanceList =  loadPartsInstanceList( ri.SaveSlot, ri.SeqNo, ri );
        ri.Robot = GetRobotByID( ri.RobotID );
        injectRobotSkills( ri );
        for (int i = 0; i<ri.Robot.PartsSlot; i++) { ri.PartsInstanceList.Add( null ); }
      } );
      RobotInstanceList = robotInstanceList;
    }

    public List<WeaponInstance> LoadWeaponInstanceList( int robotSaveSlot, int robotSeqNo, RobotInstance robotInstance ) {
      List<WeaponInstance> weaponInstanceList = mapper.Read<WeaponInstance>( $"SELECT * FROM WeaponInstance Where SaveSlot = '{robotSaveSlot}' and RobotInstanceSeqNo = '{robotSeqNo}'" ).ToList();
      weaponInstanceList.ForEach( wi => {
        wi.Weapon = mapper.ReadByKey<Weapon>( wi.WeaponID );
        //wi.RobotInstance = robotInstance;
      } );
      return weaponInstanceList;
    }

    /*
    private List<PartsInstance> loadPartsInstanceList( int robotSaveSlot, int robotSeqNo, RobotInstance robotInstance ) {
      List<PartsInstance> instanceList = mapper.Read<PartsInstance>( "SELECT * FROM PartsInstance Where SaveSlot = '{robotSaveSlot}' and '{robotSeqNo}'" ).ToList();
      instanceList.ForEach( inst => {
        inst.Parts = mapper.ReadByKey<Parts>( inst.PartsID );
        //inst.RobotInstance = robotInstance;
      } );
      return instanceList;
    }*/

  }

}
