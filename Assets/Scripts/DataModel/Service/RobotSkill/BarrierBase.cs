

public abstract class BarrierBase {

  public abstract int BarrierID { get; } // 0 表示沒有防護罩  1 Beam coat  2 I-Field  3 FF-Field  4 PS裝甲  5 念動-Field  6 Pinpoint Barrier  7 歪曲-Field  8 Beam吸收  9 Gravity Barrier
                                  // 10 AT Field  11 Aura Barrier
  public abstract string Name { get; }
  public abstract int EN { get; }
  public abstract int Amount { get; }

  protected void spendEn( MapFightingUnit defUnit, int en ) {
    en = defUnit.RobotInfo.RobotInstance.Bonus == 11 ? en/2 : en;
    defUnit.RobotInfo.EN -= en;
  }

  public abstract bool CanActive( AttackData atkData );

  protected abstract float Damage( float damage );

  public float Effect( AttackData atkData ) {
    if (!CanActive( atkData ))
      return atkData.ExpectedDamage;   //不夠條件發動

    atkData.BarrierID = BarrierID;
    //atkData.BarrierName = Name;
    atkData.ActiveSkillList.Add( Name );
    atkData.ToUseEN += EN;

    float resultDamage = Damage( atkData.ExpectedDamage );

    atkData.ExpectedDamage = resultDamage;

    return resultDamage;
  }

}

