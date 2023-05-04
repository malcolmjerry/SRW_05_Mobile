using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class SPCommand {

  public int ID;

  public string Name;

  public string  Desc;

  public int SP;

  public PeriodEnum Period;  //1 Attack  2 Be attacked  3 Be Hit  4 Destroy Enermy  5 One phase  6 Action End (Move/Attack)  7 Move 

  public enum PeriodEnum { Attack = 1, BeAttacked, BeHit, DestroyEnermy, OnePhase, ActionEnd, Move };

  public bool IsAtk;

  public bool IsDef;

  public bool IsBuff;

  public int Place;

  public string ShortName;

  /// <summary> //null: Self    2: Player Team (including self)  3: Enemy Team   4: Anyone   5. Dead Ally </summary>
  public int? Target; 

  /*
   BeforeBattle = 1         //Ex. 多回行動, HIT&AWAY
   PreHitDodge = 2          //Ex. 新人類 , 底力
   BeforeAttack = 4         //Ex. 力"一卜", 見切, 強運
   SpiritProtection = 8     //Ex. 精神耐性, 明鏡止水
   Buff Unit = 16           //Ex. 底力
   PhaseStart = 32          //Ex. 戰意高揚, SP回復
   AfterBattle = 64 }       //Ex. 談判專家, 氣力+(回避), 強運
*/
}


