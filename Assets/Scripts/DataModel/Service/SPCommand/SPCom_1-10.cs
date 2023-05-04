using System.Linq;
using UnityEngine;

//精神 1 必中, 直接在 AttackData 中實現  
public class SPCom_1 {
  //沒有用
  //public void EffectAttacker( AttackData attData ) {
    //attData.FinalHitRateAdd += 100;
    //return;
  //}
}

//精神 2 鐵壁, 直接在 AttackData 中實現  
public class SPCom_2 {
  //沒有用
  //public void EffectAttacker( AttackData attData ) {
    //attData.DamageMultiply *= 0.25f;
    //return;
  //}
}

// 精神 3 熱血, 直接在 AttackData 中實現  
// 精神 4 魂,   直接在 AttackData 中實現  
// 精神 5 不屈, 直接在 AttackData 中實現 
// 精神 6 必閃, 直接在 AttackData 中實現 
// 精神 7 集中, 直接在 AttackData 中實現 
// 精神 8 爆氣 Cri+50  , 直接在 AttackData 中實現 

// 精神 9 加速 使用時加到精神使用列表, Update Unit時檢查, 加到 UnitInfo Move屬性
public class SPCom_9 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    unit.MovePower += 2;
  }
}

// 精神 10 極速 使用時加到精神使用列表, Update Unit時檢查, 加到 UnitInfo Move屬性
public class SPCom_10 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    unit.MovePower += 3;
  }
}
