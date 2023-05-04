using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TalkBeforeBattle {

  public List<int> PilotIds = new List<int>();

  public int? HeroSeq = null;

  /// <summary> 0 未使用   1 本回合已用   2 歸檔 </summary>
  public int IsTalked = 0;  //0 未使用   1 本回合已用   2 歸檔

  public Action GetTalkBlock;
}

