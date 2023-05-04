using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class PilotSkill {

  public int ID;

  public string Name;

  public string Desc;

  public bool IsLv;

  public bool IsBattleAttacker;    //Ex. 新人類, 底力, 見切, 野生化, 氣力+(命中), 談判專家

  public bool IsBattleAtkHide;   //強運

  public bool IsBattleDefenser;    //Ex. 聖戰士, 底力, 見切, 力"一卜", 氣力+(回避),  強運

  public bool IsBattleDefHide;   //強運

  public bool IsSpiritProtection;  //Ex. 精神耐性, 明鏡止水

  public bool IsBuffUnit;       //Ex. 底力, DASH

  public bool IsPhaseStart;     //戰意高揚, SP回復

  public bool IsAfterAction;    //多回行動, HIT&AWAY
  /*
     Other = 1                //Ex. 多回行動, HIT&AWAY
     BeforeAttacker = 2         //Ex. 新人類 , 底力, 見切
     BeforeDefenser = 4        //Ex. 聖戰士, 底力, 力"一卜",  強運
     SpiritProtection = 8     //Ex. 精神耐性, 明鏡止水
     Buff Unit = 16           //Ex. 底力, DASH
     PhaseStart = 32          //Ex. 戰意高揚, SP回復
     AfterBattle = 64         //Ex. 談判專家, 氣力+(回避), 強運
  */
}

