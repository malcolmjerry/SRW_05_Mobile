using System.Collections.Generic;

public interface IRobotDao {

  Robot GetRobotByID( int ID );

  RobotSkill GetRobotSkillByID( int? ID );

  List<RobotSkill> GetRobotSkillsByRobotInstance( RobotInstance robotInstance );

  //List<Parts> GetPartsByRobotInstance( RobotInstance robotInstance );

  void SaveRobotInstanceList( int saveSlot );

  void LoadRobotInstanceList( int saveSlot );

  List<WeaponInstance> LoadWeaponInstanceList( int robotSaveSlot, int robotSeqNo, RobotInstance robotInstance );

  List<RobotInstance> RobotInstanceList { get; set; }
}

