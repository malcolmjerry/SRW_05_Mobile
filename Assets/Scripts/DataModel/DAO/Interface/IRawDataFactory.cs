using System.Collections.Generic;

public interface IRawDataFactory {

  Robot GetRobotByID( int ID );

  Pilot GetPilotByID( int ID );

  RobotSkill GetRobotSkillByID( int ID );

  Parts GetPartsByID( int ID );

  SPCommand GetSPComByID( int ID );

  void InjectSPComPilots( Pilot pilot );

  List<RobotSkill> GetRobotSkillsByRobotInstance( RobotInstance robotInstance );

  RobotInstance CreateRobotInstanceByRobotID( int ID );

  PilotInstance CreatePilotInstanceByPilotID( int ID, int level = 1, int exp = 500, int enable = 0 );

  PartsInstance CreatePartsInstanceByPartID( int ID );

  MapFightingUnit CreateMapFightingUnit( RobotInstance robotInstance, PilotInstance pilotInstance, UnitInfo.TeamEnum team );



}