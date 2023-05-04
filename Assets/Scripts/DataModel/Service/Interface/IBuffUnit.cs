
public interface IBuffUnit {

  void BuffUnit( MapFightingUnit unit );

}

public interface IConsumable {

  void Consume( MapFightingUnit unit, MapFightingUnit useBy = null );

}

public interface IBuffUnitByPilotSkill {

  void BuffUnit( MapFightingUnit unit, int order );

}
