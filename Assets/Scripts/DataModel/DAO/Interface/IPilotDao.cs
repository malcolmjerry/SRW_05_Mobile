using System.Collections.Generic;

public interface IPilotDao {

  Pilot GetPilotBase( int ID );

  Pilot GetPilotByID( int ID );

  SPCommand GetSPComByID( int ID );

  //void InjectSPComPilots( Pilot pilot );

  PilotSkill GetPilotSkillByID( int ID );

  //PilotSkillDefault GetPilotSkillDefaultByKey( int pilotID, int order );

  //List<PilotSkillDefault> GetPilotSkillDefaultList( Pilot pilot );

  //List<PilotSkillInstance> GetPilotSkillInstanceList( PilotInstance pilotInstance );

  void SavePilotInstanceList( int saveSlot );

  void LoadPilotInstanceList( int saveSlot );

  void LoadPilotInstanceList( List<PilotInstance> pilotInstanceList, List<Hero> heroList );

  List<PilotInstance> PilotInstanceList { get; set; }
  
  List<PilotInstance> HeroPilotInstanceList { get; set; }

  List<Hero> HeroList { get; set; }

}

