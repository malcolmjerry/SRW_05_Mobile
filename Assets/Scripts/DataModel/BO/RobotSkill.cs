using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class RobotSkill {

  public int ID;

  public string Name;

  public string Desc;

  public bool IsBarrier;
  public bool IsAvatar;
  public bool IsBuffUnit;
  public bool IsPhaseStart;
  public bool IsBattleAttacker;
  public bool IsBattleDefenser;  
  
  /*
  public int Type;    //1.Barrier  2.Avatar  3.Buff  (need class implement)  4. Phase Start  5. Usable  6. Battle Attacker 7. Battle Defenser

  public enum TypeEnum { Barrier = 1, Avatar, Buff, PhaseStart, Usable, BattleAttacker, BattleDefenser }
  */
}

