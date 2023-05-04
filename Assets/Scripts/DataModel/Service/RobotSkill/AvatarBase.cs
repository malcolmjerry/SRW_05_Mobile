

public abstract class AvatarBase {

  public abstract int AvatarID { get; } // 0 表示沒有防護罩  1 Beam coat  2 I-Field  3 FF-Field  4 PS裝甲  5 念動-Field  6 Pinpoint Barrier  7 歪曲-Field  8 Beam吸收  9 Gravity Barrier
                                  // 10 AT Field  11 Aura Barrier
  public abstract string Name { get; }
  public abstract int EN { get; }

  protected abstract bool CanActive( AttackData atkData );

  public bool Effect( AttackData atkData ) {
    if (!CanActive( atkData ))
      return false;

    atkData.AvatarID = AvatarID;
    //atkData.AvatarName = Name;
    atkData.ActiveSkillList.Add( Name );
    atkData.ToUseEN += EN;

    return true;
  }

}

