using System.Collections.Generic;

public interface IPilotDialogDao {

  List<PilotDialog> GetWhenAttackingRival( int pilotID, int otherPilotID );

  List<PilotDialog> GetByWeapon( int pilotID, int weaponID, int weaponNameType );

  List<PilotDialog> GetByAttack( int pilotID, bool isMelee );

  List<PilotDialog> GetWhenDead( int pilotID );

  List<PilotDialog> GetWhenDanger( int pilotID );

  List<PilotDialog> GetWhenNormal( int pilotID );

  List<PilotDialog> GetWhenBigDam( int pilotID );

  List<PilotDialog> GetWhenSmallDam( int pilotID );

  List<PilotDialog> GetWhenDodge( int pilotID );

  List<PilotDialog> GetWhenUnable( int pilotID );

  //List<PilotDialog> GetWhenAny( int pilotID );

}

