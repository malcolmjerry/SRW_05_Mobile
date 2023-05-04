using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using System;
using Mono.Data.Sqlite;
using System.Data;
using UnityORM;
using System.IO;
using System.Linq;

public class RawDataFactory : IRawDataFactory {

  private static readonly RawDataFactory instance = new RawDataFactory();

  public List<RobotInstance> PlayerRobotInstance { set; get; }

  private DBEvolution Evolution;
  private DBMapper mapper;

  private RawDataFactory() {
    //GetAllRawRobotData();

    ////Evolution = new DBEvolution( Path.Combine( Application.dataPath, "Data/SRW_05.db" ) );
    
    ////mapper = new DBMapper( Evolution.Database );

    /*
    Robot[] robotList = mapper.Read<Robot>( "SELECT * FROM Robot;" );

    foreach (var robot in robotList) {
      Weapon[] weaponList = mapper.Read<Weapon>( "SELECT * FROM Weapon Where RobotID = " + robot.ID );
      robot.WeaponList = weaponList.ToList();
    }

    RawRobots = robotList.ToList();
    */
  }

  public Robot GetRobotByID( int ID ) {
    Robot robot = mapper.ReadByKey<Robot>( ID );
    robot.WeaponList = mapper.Read<Weapon>( "SELECT * FROM Weapon Where RobotID = " + ID ).OrderBy( w => w.HitPoint ).ToList();
    //robot.RobotSkill1 = mapper.ReadByKey<RobotSkill>( robot.Skill1ID );
    //robot.RobotSkill2 = mapper.ReadByKey<RobotSkill>( robot.Skill2ID );
    //robot.RobotSkill3 = mapper.ReadByKey<RobotSkill>( robot.Skill3ID );
    //robot.RobotSkill4 = mapper.ReadByKey<RobotSkill>( robot.Skill4ID );
    return robot;
  }

  public Pilot GetPilotByID( int ID ) {
    Pilot pilot = mapper.ReadByKey<Pilot>( ID );
    InjectSPComPilots( pilot );
    return pilot;
  }

  public RobotSkill GetRobotSkillByID( int ID ) {
    RobotSkill robotSkill = mapper.ReadByKey<RobotSkill>( ID );
    return robotSkill;
  }

  public Parts GetPartsByID( int ID ) {
    Parts parts = mapper.ReadByKey<Parts>( ID );
    return parts;
  }

  public SPCommand GetSPComByID( int ID ) {
    SPCommand entity = mapper.ReadByKey<SPCommand>( ID );
    return entity;
  }

  public void InjectSPComPilots( Pilot pilot ) {
    pilot.SPComPilots = mapper.Read<SPComPilot>( "SELECT * FROM SPComPilot Where PilotID = " + pilot.ID ).OrderBy( s => s.Level ).ToList();
    foreach (var comp in pilot.SPComPilots) {
      comp.SPCommand = GetSPComByID( comp.SPComID );
    }
  }

  public List<RobotSkill> GetRobotSkillsByRobotInstance( RobotInstance robotInstance ) {
    List<RobotSkill> list = new List<RobotSkill>();

    list.Add( robotInstance.Skill1ID.HasValue ? GetRobotSkillByID( robotInstance.Skill1ID.Value ) : null );
    list.Add( robotInstance.Skill2ID.HasValue ? GetRobotSkillByID( robotInstance.Skill2ID.Value ) : null );
    list.Add( robotInstance.Skill3ID.HasValue ? GetRobotSkillByID( robotInstance.Skill3ID.Value ) : null );
    list.Add( robotInstance.Skill4ID.HasValue ? GetRobotSkillByID( robotInstance.Skill4ID.Value ) : null );
    list.Add( robotInstance.Skill5ID.HasValue ? GetRobotSkillByID( robotInstance.Skill5ID.Value ) : null );
    list.Add( robotInstance.Skill6ID.HasValue ? GetRobotSkillByID( robotInstance.Skill6ID.Value ) : null );

    return list;
  }

  public RobotInstance CreateRobotInstanceByRobotID( int ID/*, int team*/ ) {
    Robot robot = GetRobotByID( ID );

    RobotInstance robotInstance = new RobotInstance() {
      SaveSlot = 0,
      SeqNo = 0,
      Robot = robot,
      RobotID = robot.ID,
      HPLv = 0,
      ENLv = 0,
      HitLv = 0,
      ArmorLv = 0,
      MotilityLv = 0,
      MovePowerLv = 0,
      BGM = robot.BGM,
      Skill1ID = robot.Skill1ID,
      Skill2ID = robot.Skill2ID,
      Skill3ID = robot.Skill3ID,
      Skill4ID = robot.Skill4ID,
      Skill5ID = robot.Skill5ID,
      Skill6ID = robot.Skill6ID
      //Team = team   
    };

    robotInstance.WeaponInstanceList = new List<WeaponInstance>();
    foreach (var weapon in robot.WeaponList.Where( w => w.DefaultEnable )) {
      WeaponInstance weaponInstance = new WeaponInstance() {
        //SaveSlot = 0,
        //SeqNo = 0,
        RobotInstanceSaveSlot = 0,
        RobotInstanceSeqNo = 0,
        Level = 1,
        Enable = 1,
        WeaponID = weapon.ID,
        Weapon = weapon,
        //RobotInstance = robotInstance
      };
      robotInstance.WeaponInstanceList.Add( weaponInstance );
    }

    return robotInstance;
  }

  public PilotInstance CreatePilotInstanceByPilotID( int ID, int level = 1, int exp = 500, int enable = 0 ) {
    Pilot pilot = GetPilotByID( ID );

    PilotInstance pilotInstance = new PilotInstance() {
      SaveSlot = 0,
      SeqNo = 0,
      PilotID = pilot.ID,
      Pilot = pilot,
      ShootAdded = 0,
      MeleeAdded = 0,
      DodgeAdded = 0,
      HitAdded = 0,
      DexAdded = 0,
      DefenseAdded = 0,
      //MaxSp = pilot.MaxSp,
      //RemainSp = (pilot.MaxSp + 1)/2,
      //Level = level,
      Exp = exp,
      Enable = enable
    };

    return pilotInstance;
  }

  public PartsInstance CreatePartsInstanceByPartID( int ID ) {
    Parts parts = GetPartsByID( ID );
    PartsInstance partsInstance = new PartsInstance() {
      PartsID = ID,
      Parts = parts
    };
    return partsInstance;
  }

  public static RawDataFactory Instance { get { return instance; } }

  public MapFightingUnit/*UnitInfo*/ CreateMapFightingUnit( RobotInstance robotInstance, PilotInstance pilotInstance, UnitInfo.TeamEnum team ) {
    MapFightingUnit unit = new MapFightingUnit();
    //UnitInfo unit = new UnitInfo();
    //unit.Team = team;
    //unit.PilotInfo = pilotInstance.CloneInfo();    //駕駛員會對機體或武器有影響, 但相反時不會, 所以先初始化駕駛員
    //unit.RobotInfo = robotInstance.CloneInfo( unit );
    //unit.CurrentRobotInfo = robotInstance.CloneInfo();  
    unit.UpdateInfoByParts();
    return unit;
  }


  /*
  private void GetAllRawRobotData() {
    //TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath<TextAsset>( "Assets/Data/basicData.json" );
    //BasicData basicData = JsonUtility.FromJson<BasicData>( textAsset.text );
    //RawRobots = basicData.RawRobotList;

    string conn = "URI=file:" + Application.dataPath + "/Data/SRW_05.db";

    IDbConnection dbconn;
    dbconn = (IDbConnection)new SqliteConnection( conn );
    dbconn.Open(); //Open connection to the database.
    IDbCommand dbcmd = dbconn.CreateCommand();
    string sqlQuery = "SELECT * " + "FROM Robot";
    dbcmd.CommandText = sqlQuery;
    List<RobotInfo> robotList = new List<RobotInfo>();
    using (IDataReader reader = dbcmd.ExecuteReader()) {
      while (reader.Read()) {
        RobotInfo robot = new RobotInfo() {
          ID = Convert.ToInt32( reader["ID"] ),
          Name = Convert.ToString( reader["Name"] ),
          MovePower = Convert.ToSingle( reader["MovePower"] ),
          FullName = Convert.ToString( reader["FullName"] )
        };
        robotList.Add( robot );
      }
    }
  
    foreach (var robot in robotList) {
      dbcmd.CommandText = "SELECT * " + "FROM Weapon Where RobotID = " + robot.ID;
      List<Weapon> weaponList = new List<Weapon>();
      using (IDataReader reader = dbcmd.ExecuteReader()) {
        while (reader.Read()) {
          Weapon weapon = new Weapon() {
            ID = Convert.ToInt32( reader["ID"] ),
            PlayIndex = Convert.ToInt32( reader["PlayIndex"] ),
            Name = Convert.ToString( reader["Name"] ),
            IsMelee = Convert.ToBoolean( reader["IsMelee"] ),
            HitPoint = Convert.ToInt32( reader["HitPoint"] ),
            MinRange = Convert.ToInt32( reader["MinRange"] ),
            MaxRange = Convert.ToInt32( reader["MaxRange"] ),
            HitRate = Convert.ToInt32( reader["HitRate"] )
          };
          weaponList.Add( weapon );
        }
      }
      robot.WeaponList = weaponList;
    }

    dbcmd.Dispose();
    dbconn.Close();

    RawRobots = robotList;
  }*/

}

/*
[Serializable]
public class BasicData {

  public List<RobotInfo> RawRobotList;

}

*/