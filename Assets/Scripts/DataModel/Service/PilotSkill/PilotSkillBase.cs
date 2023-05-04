using System.Linq;
using UnityEngine;

public class PilotSkillBase {

  static protected int getDefaultLevel( PilotSkillDefault skDef, int lv ) {
    int defaultLv = 0;
    if (skDef.Lv9 <= lv) defaultLv = 9;
    else if (skDef.Lv8 <= lv) defaultLv = 8;
    else if (skDef.Lv7 <= lv) defaultLv = 7;
    else if (skDef.Lv6 <= lv) defaultLv = 6;
    else if (skDef.Lv5 <= lv) defaultLv = 5;
    else if (skDef.Lv4 <= lv) defaultLv = 4;
    else if (skDef.Lv3 <= lv) defaultLv = 3;
    else if (skDef.Lv2 <= lv) defaultLv = 2;
    else if (skDef.Lv1 <= lv) defaultLv = 1;

    return defaultLv;
  }

  static public string GetNameWithLv( PilotInfo pilotInfo, int order ) {
    PilotSkillInstance skInst = pilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( s => s.OrderSort == order );
    PilotSkillDefault skDef = pilotInfo.PilotInstance.Pilot.PilotSkillDefaultList.FirstOrDefault( s => s.Order == order );

    int sumLv = GetSumLv( pilotInfo, order );
    if (sumLv < 0) return "------";

    else if (sumLv == 0) return "??????";

    else {

      if (!skInst.PilotSkill.IsLv) {
        return skInst.PilotSkill.Name;
      }

      if (skDef == null || skDef.PilotSkillID != skInst.PilotSkillID) {
        return skInst.PilotSkill.Name + "    +" + skInst.Level;
      }

      int defaultLv = getDefaultLevel( skDef, pilotInfo.Level );
      string extendLv = "";
      if (skInst.Level > 0)
        extendLv = $"+{skInst.Level}";

      return skInst.PilotSkill.Name + $" L{defaultLv} {extendLv}";

    }
  }

  static public int GetSumLv( PilotInfo pilotInfo, int order ) {
    PilotSkillInstance skInst = pilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( s => s.OrderSort == order );
    if (skInst == null) return -1;

    PilotSkillDefault skDef = pilotInfo.PilotInstance.Pilot.PilotSkillDefaultList.FirstOrDefault( s => s.Order == order );

    if (skDef == null || skDef.PilotSkillID != skInst.PilotSkillID) {
      if (skInst.PilotSkill.IsLv)
        return skInst.Level;
      return 1;
    }

    if (!skInst.PilotSkill.IsLv) {
      if (skDef.Lv0 <= pilotInfo.Level)
        return 1;
      return 0;
    }

    int defaultLv = getDefaultLevel( skDef, pilotInfo.Level );

    return Mathf.Min( defaultLv + skInst.Level, 9 );
  }

  virtual protected bool defaultHightlight { get; set; } = false;

  virtual public ItemOnOff CheckSkill( MapFightingUnit unit, int order ) {
    string nameWithLv = GetNameWithLv( unit.PilotInfo, order );
    //return new ItemOnOff() { Highlight = false, Name = nameWithLv };
    return new ItemOnOff() { Highlight = IsEnabled( unit, order ) && defaultHightlight, Name = nameWithLv };
  }

  virtual public bool IsEnabled( MapFightingUnit unit, int order ) {
    return GetSumLv( unit.PilotInfo, order ) > 0;
  }

  virtual public bool IsLight( MapFightingUnit unit, int order ) {
    return false;
  }

  static public bool IsEnabledBySkillID( PilotInfo pilotInfo, int skillID ) {
    int index = pilotInfo.PilotInstance.PilotSkillInstanceList.FindIndex( psi => psi.PilotSkillID == skillID );
    if (index > -1)
      return GetSumLv( pilotInfo, order: index + 1 ) > 0;
    return false;
  }
}

