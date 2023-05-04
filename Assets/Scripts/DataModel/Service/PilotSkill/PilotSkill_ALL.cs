using System.Linq;
using UnityEngine;

//盾防
public class PilotSkill_1 : PilotSkillBase, ISkillChance/*, ISkillOnOff*/ {

  public float GetChance( MapFightingUnit fromUnit, MapFightingUnit toUnit, int order ) {
    int lv = GetSumLv( toUnit.PilotInfo, order );

    float chance = lv / 18f;
    int dexDiff = toUnit.PilotInfo.Dex - fromUnit.PilotInfo.Dex;
    float dexChance = (dexDiff / 4f) / 100f;

    float resultChance = chance + dexChance;
    return Mathf.Min( 0.9f, Mathf.Max( 0.05f, resultChance ) );
  }

  
}
//切彿
public class PilotSkill_2 : PilotSkillBase, ISkillChance /*,ISkillOnOff,*/  {

  public float GetChance( MapFightingUnit fromUnit, MapFightingUnit toUnit, int order ) {
    int lv = GetSumLv( toUnit.PilotInfo, order );

    float chance = lv / 18f;
    int dexDiff = toUnit.PilotInfo.Dex - fromUnit.PilotInfo.Dex;
    float dexChance = (dexDiff / 4f) / 100f;

    float resultChance = chance + dexChance;
    return Mathf.Min( 0.9f, Mathf.Max( 0.05f, resultChance) );
  }

}


//底力
public class PilotSkill_3 : PilotSkillBase, IBattleAttacker, IBattleDefenser /* IBuffUnit, ISkillOnOff*/ {

  private static readonly int[] hpPercentArr = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
  private static readonly int[] defBuffArr = new int[] { 5, 10, 15, 20, 25, 30, 34, 38, 42 };
  private static readonly int[] hitDodgeBuffArr = new int[] { 5, 10, 15, 20, 25, 30, 34, 38, 42 };
  private static readonly int[] criBuffArr = new int[] { 5, 10, 15, 20, 25, 30, 34, 38, 42 };

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    int currLv = activeLv( atkData.FromUnitInfo.MapFightingUnit, level );
    atkData.HitRateAdd += hitDodgeBuffArr[currLv-1];
    atkData.CriAdd += criBuffArr[currLv-1];
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    int currLv = activeLv( atkData.FromUnitInfo.MapFightingUnit, level );
    atkData.HitRateAdd -= hitDodgeBuffArr[currLv-1];
    atkData.DamageMultiply *= ((100 - defBuffArr[currLv-1]) / 100f);
  }

  override protected bool defaultHightlight { get; set; } = true;
  /*
  override public ItemOnOff CheckSkill( MapFightingUnit unit, int order ) {
    string nameWithLv = GetNameWithLv( unit.PilotInfo, order );
    bool isEnabled = IsEnabled( unit, order );
    return new ItemOnOff() { Highlight = isEnabled, Name = nameWithLv };
  }
  */

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    if (unit.RobotInfo == null)
      return false;

    int level = GetSumLv( unit.PilotInfo, order );
    if (level < 1) return false;
    int percent = hpPercentArr[level - 1];
    return ((float)unit.RobotInfo.HP / unit.RobotInfo.MaxHP * 100f) <= percent  && GetSumLv( unit.PilotInfo, order ) > 0;
  }

  private int activeLv( MapFightingUnit unit, int maxLv ) {
    float hpPercent = (float)unit.RobotInfo.HP / unit.RobotInfo.MaxHP * 100f;

    int currLv = 0;
    int i;
    for ( i=0; i<maxLv; i++) {
      if (hpPercent <= hpPercentArr[i]) {
        currLv = maxLv - i;
        break;
      }
    }
    return currLv;
  }

}

//強運
public class PilotSkill_4 : PilotSkillBase, IBattleAttackerHide, IBattleDefenserHide /*IBuffUnit, ISkillOnOff*/ {

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return ;

    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    int p = level * 1;
    bool result = p > UnityEngine.Random.Range( 0, 100 );
    if (result)
      atkData.FinalHitRateAdd += 150;

    atkData.MoneyRateAdd += (level * 3 / 100f);
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;

    int level = GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order );
    int p = level * 1;
    bool result = p > UnityEngine.Random.Range( 0, 100 );
    if (result)
      atkData.FinalHitRateAdd -= 600;
  }

}

//精神耐性
public class PilotSkill_5 : PilotSkillBase /*, ISkillOnOff*/ {

}

//HIT & AWAY
public class PilotSkill_6 : PilotSkillBase/*, ISkillOnOff*/ {

}

//DASH
public class PilotSkill_7 : PilotSkillBase, IBuffUnitByPilotSkill/*, ISkillOnOff*/ {

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (GetSumLv( unit.PilotInfo, order ) < 1) return;

    unit.MovePower++;
    if (unit.PilotInfo.Willpower >= 130)
      unit.MovePower++;
  }

  override protected bool defaultHightlight { get; set; } = true;

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 130 && GetSumLv( unit.PilotInfo, order ) > 0;
  }

}

//極
public class PilotSkill_8 : PilotSkillBase, IBattleAttacker, IBattleDefenser/*, ISkillOnOff*/ {

  override protected bool defaultHightlight { get; set; } = true;

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    atkData.FinalHitRateAdd += 30;
    atkData.FinalCriAdd += 30;
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    atkData.FinalHitRateAdd -= 30;
  }

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 130 && GetSumLv( unit.PilotInfo, order ) > 0;
  }
}

//天才
public class PilotSkill_9 : PilotSkillBase, IBattleAttacker {

  public void EffectAttacker( AttackData atkData, int order ) {
    atkData.ExpRateAdd += .25f;
    atkData.PpRateAdd += .1f;
  }

}

//New Type
public class PilotSkill_10 : PilotSkillBase, IBattleAttacker, IBattleDefenser, IBuffUnitByPilotSkill {

  private static readonly int[] hitDodgeBuffArr = new int[] { 10, 15, 20, 25, 30, 35, 40, 45, 50 };

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd += hitDodgeBuffArr[level-1];
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd -= hitDodgeBuffArr[level-1];
  }

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order )) return;
    int level = GetSumLv( unit.PilotInfo, order );

    foreach (var weaponInfo in unit.WeaponList?.Where( w => w.WeaponInstance.Weapon.IsNT > 0 )) {
      weaponInfo.MaxRange += (level / 2);
    }
  }

  override protected bool defaultHightlight { get; set; } = false;

}

//人工NT
public class PilotSkill_11 : PilotSkillBase, IBattleAttacker, IBattleDefenser, IBuffUnitByPilotSkill {

  private static readonly int[] hitDodgeBuffArr = new int[] { 10, 14, 18, 22, 26, 30, 34, 38, 42 };

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd += hitDodgeBuffArr[level-1];
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd -= hitDodgeBuffArr[level-1];
  }

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order )) return;
    int level = GetSumLv( unit.PilotInfo, order );

    foreach (var weaponInfo in unit.WeaponList?.Where( w => w.WeaponInstance.Weapon.IsNT > 0 )) {
      weaponInfo.MaxRange += (level / 3);
    }
  }

}

//強化人間
public class PilotSkill_12 : PilotSkillBase, IBattleAttacker, IBattleDefenser, IBuffUnitByPilotSkill {

  private static readonly int[] hitDodgeBuffArr = new int[] { 10, 14, 18, 22, 26, 30, 34, 38, 42 };

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd += hitDodgeBuffArr[level-1];
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd -= hitDodgeBuffArr[level-1];
  }

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order )) return;
    int level = GetSumLv( unit.PilotInfo, order );

    foreach (var weaponInfo in unit.WeaponList?.Where( w => w.WeaponInstance.Weapon.IsNT > 0 )) {
      weaponInfo.MaxRange += (level / 3);
    }
  }

}

//防守 力"一卜"
public class PilotSkill_13 : PilotSkillBase, IBattleDefenser/*ISkillOnOff, IBuffBattle*/ {

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    atkData.DamageMultiply *= 0.8f;
    atkData.ActiveSkillList.Add( GetNameWithLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order ) );
  }

  override protected bool defaultHightlight { get; set; } = true;

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 130 && GetSumLv( unit.PilotInfo, order ) > 0;
  }

}

//見切
public class PilotSkill_14 : PilotSkillBase, IBattleAttacker, IBattleDefenser {

  public void EffectAttacker( AttackData atkData, int order ) {
    //int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit, order );
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    atkData.FinalHitRateAdd += 15;
    atkData.FinalCriAdd += 15;
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    //int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit, order );
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    atkData.FinalHitRateAdd -= 15;
  }

  override protected bool defaultHightlight { get; set; } = true;

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 130 && GetSumLv( unit.PilotInfo, order ) > 0;
  }

}

//SP回復
public class PilotSkill_15 : PilotSkillBase, IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.PilotInfo.RemainSp += 5;
  }

}

//鬥爭心
public class PilotSkill_16 : PilotSkillBase {
  //在 MapFightingUnit 的 UpdateInit 裡執行
}

//戰意高揚
public class PilotSkill_17 : PilotSkillBase, IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.PilotInfo.Willpower += 3;
  }

}

//E-SAVE
public class PilotSkill_18 : PilotSkillBase {
  //WeaponInfo 的構造方法裡檢查並處理
}

//19 勇者
public class PilotSkill_19 : PilotSkillBase, IBattleAttacker, IBuffUnitByPilotSkill/*, ISkillOnOff*/ {

  private static readonly int[] BUFF_ARR = new int[] { 6, 9, 12, 15, 18, 21, 24, 27, 30 };

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order )) return;
    int level = GetSumLv( unit.PilotInfo, order );

    unit.PilotInfo.Melee += BUFF_ARR[level-1];
    unit.PilotInfo.Defense += BUFF_ARR[level-1];
  }

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd += BUFF_ARR[level-1];
  }

}

//20 念動力
public class PilotSkill_20 : PilotSkillBase, IBattleAttacker, IBattleDefenser, IPhaseStart {

  private static readonly int[] hitDodgeBuffArr = new int[] { 5, 9, 12, 15, 18, 21, 24, 27, 30 };

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd += hitDodgeBuffArr[level-1];
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd -= hitDodgeBuffArr[level-1];
  }

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.PilotInfo.RemainSp += 2;
  }
}

//21 指揮官
public class PilotSkill_21 : PilotSkillBase {
  //特別處理
}

//22 氣力限界突破
public class PilotSkill_22 : PilotSkillBase, IBuffUnitByPilotSkill {
  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order )) return;
    //int level = GetSumLv( unit, order );

    unit.PilotInfo.MaxWillpower += 10;
  }
}

//23 B-SAVE
public class PilotSkill_23 : PilotSkillBase {
  //RobotInfo Init 時, 生成Weapon List, WeaponInfo 的構造方法裡檢查並處理
}

//24 野生化
public class PilotSkill_24 : PilotSkillBase, IBattleAttacker {

  override protected bool defaultHightlight { get; set; } = true;

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    atkData.DamageMultiply *= 1.1f;
    atkData.CriAdd += 20;
  }

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 125 && GetSumLv( unit.PilotInfo, order ) > 0;
  }
}

//25 談判專家
public class PilotSkill_25 : PilotSkillBase {
  //特殊處理
}

//26 超能力
public class PilotSkill_26 : PilotSkillBase, IBattleAttacker, IBattleDefenser {
  private static readonly int[] hitDodgeBuffArr = new int[] { 4, 8, 12, 16, 20, 24, 28, 32, 36 };

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd += hitDodgeBuffArr[level-1];
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    int level = GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd -= hitDodgeBuffArr[level-1];
  }
}

//27 連結之力
public class PilotSkill_27 : PilotSkillBase {
  //特殊處理
}

//28 預知能力
public class PilotSkill_28 : PilotSkillBase, IBattleDefenser {

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    //int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit, order );
    atkData.HitRateAdd -= 20;
  }
}

//29 SEED
public class PilotSkill_29 : PilotSkillBase, IBattleAttacker, IBattleDefenser, IBuffUnitByPilotSkill {

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order )) return;
    //int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit, order );
    atkData.FinalHitRateAdd += 25;
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;
    //int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit, order );
    atkData.FinalHitRateAdd -= 25;
  }

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 130 && GetSumLv( unit.PilotInfo, order ) > 0;
  }

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order )) return;
    //int level = GetSumLv( unit, order );
    unit.PilotInfo.BuffDex += 40;
    unit.PilotInfo.Dex = unit.PilotInfo.Dex + 40;
  }

}

//30 Half Cut
public class PilotSkill_30 : PilotSkillBase, IBattleDefenserHide {

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order )) return;

    if (atkData.HitRate <= 30)
      atkData.DamageMultiply *= .5f;
  }
}

//31 氣力+ (回避)
public class PilotSkill_31 : PilotSkillBase {
}

//32 氣力+ (命中)
public class PilotSkill_32 : PilotSkillBase/*, ISkillOnOff*/ {
}

//33 氣力+ (被彈)
public class PilotSkill_33 : PilotSkillBase {
}

//34 GUTS
public class PilotSkill_34 : PilotSkillBase {
}

//35 明鏡止水
public class PilotSkill_35 : PilotSkillBase, IBuffUnitByPilotSkill {
  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order ))
      return;

    unit.PilotInfo.BuffDodge += 30;
    unit.PilotInfo.BuffHit += 30;
    unit.PilotInfo.BuffDex += 30;

    unit.PilotInfo.Dodge = unit.PilotInfo.Dodge + 30;
    unit.PilotInfo.Hit = unit.PilotInfo.Hit + 30;
    unit.PilotInfo.Dex = unit.PilotInfo.Dex + 30;
  }

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 130 && GetSumLv( unit.PilotInfo, order ) > 0;
  }

  override protected bool defaultHightlight { get; set; } = true;
}

//36 多回行動
public class PilotSkill_36 : PilotSkillBase, IBuffUnitByPilotSkill {

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order ))
      return;
    int level = GetSumLv( unit.PilotInfo, order );

    unit.ActionCount = 1 + level;
  }

}

//37 藥物強化
public class PilotSkill_37 : PilotSkillBase, IBattleAttacker, IBattleDefenser {
  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order ))
      return;
    atkData.HitRateAdd += 20;
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order ))
      return;
    atkData.HitRateAdd -= 20;
  }
  override protected bool defaultHightlight { get; set; } = false;

  override public bool IsEnabled( MapFightingUnit unit, int order ) {
    return unit.PilotInfo.Willpower >= 120 && GetSumLv( unit.PilotInfo, order ) > 0;
  }
}

//38 聖戰士
public class PilotSkill_38 : PilotSkillBase, IBattleAttacker, IBattleDefenser, IBuffUnitByPilotSkill {
  private static readonly int[] hitDodgeBuffArr = new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45 };

  public void EffectAttacker( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.FromUnitInfo.MapFightingUnit, order ))
      return;
    int level = GetSumLv( atkData.FromUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd += hitDodgeBuffArr[level - 1];
  }

  public void EffectDefenser( AttackData atkData, int order ) {
    if (!IsEnabled( atkData.ToUnitInfo.MapFightingUnit, order ))
      return;
    int level = GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, order );
    atkData.HitRateAdd -= hitDodgeBuffArr[level - 1];
  }

  public void BuffUnit( MapFightingUnit unit, int order ) {
    if (!IsEnabled( unit, order ))
      return;
    int level = GetSumLv( unit.PilotInfo, order );

    foreach (var weaponInfo in unit.WeaponList) {
      weaponInfo.HitPoint = weaponInfo.HitPoint + 50 * level;
    }
  }

  override protected bool defaultHightlight { get; set; } = false;
}

