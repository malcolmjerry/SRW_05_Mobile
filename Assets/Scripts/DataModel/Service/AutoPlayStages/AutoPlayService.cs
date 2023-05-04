using DataModel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AutoPlayService {

  RobotService robotService;
  PilotService pilotService;
  PartsService partsService;
  GameDataService gameDataService;

  public AutoPlayService() {

  }

  public void Play() {
    robotService = DIContainer.Instance.RobotService;
    pilotService = DIContainer.Instance.PilotService;
    partsService = DIContainer.Instance.PartsService;
    gameDataService = DIContainer.Instance.GameDataService;

    playStage1();
    playStage2();
  }

  void playStage1() {
    gameDataService.GenerateNewStage();

    RobotInstance robot1 = robotService.CreateRobotInstance( 16, true, new List<PartsInstance>() {  // 1-WZ-EW  10-鐮刀怪  11-Impulse  13-RZ-02  22-Jefty
      partsService.CreatePartsInstanceByPartsID( 1, true ),
      partsService.CreatePartsInstanceByPartsID( 2, true ),
      partsService.CreatePartsInstanceByPartsID( 3, true )
    } );

    //生成強化部件 放於倉庫
    //partsService.CreatePartsInstanceByPartsID( 1, true );
    //partsService.CreatePartsInstanceByPartsID( 1, true );
    //partsService.CreatePartsInstanceByPartsID( 1, true );

    MapFightingUnit player1 = gameDataService.CreateMapFightingUnit( robot1, pilotService.CreatePilotInstance( 1307, isPlayer: true ) );  // 1307 中山樵

    //增加戰鬥單位至格納庫
    gameDataService.HouseUnits.Add( player1 );

    PilotInstance heroPilotInstance = pilotService.LoadHeroPilotInstance( 1 );
    RobotInstance robot = robotService.CreateRobotInstance( 1, true, new List<PartsInstance>() {  // 1-WZ-EW
      partsService.CreatePartsInstanceByPartsID( 1, true ),  //Chobham裝甲 HP+500; 裝甲+150
      partsService.CreatePartsInstanceByPartsID( 2, true )   //磁層關節 運動性+10
    } );
    MapFightingUnit heroUnit = gameDataService.CreateMapFightingUnit( robot, heroPilotInstance ); //WZ-EW + 主角1
    gameDataService.HouseUnits.Add( heroUnit );

    basePlay();
  }

  void playStage2() {
    gameDataService.GenerateNewStage();

    RobotInstance robot1 = robotService.CreateRobotInstance( 3, true, new List<PartsInstance>() {  // 3: 元祖 
      partsService.CreatePartsInstanceByPartsID( 7, true ),  //Booster
      partsService.CreatePartsInstanceByPartsID( 9, true )  //高性能雷達
    } );
    MapFightingUnit player1 = gameDataService.CreateMapFightingUnit( robot1, pilotService.CreatePilotInstance( 204, level: 10, isPlayer: true ) );  // 204 馬克思

    RobotInstance robot2 = robotService.CreateRobotInstance( 26, true, new List<PartsInstance>() {  // 26: 百式
      partsService.CreatePartsInstanceByPartsID( 24, true ),  //精密照準鏡
      partsService.CreatePartsInstanceByPartsID( 10, true )  //對 Beam Coating
    } );
    MapFightingUnit player2 = gameDataService.CreateMapFightingUnit( robot2, pilotService.CreatePilotInstance( 405, level: 10, isPlayer: true ) );  //Robot 11: V高達  405: 小英


    PilotInstance heroPilotInstance = pilotService.LoadHeroPilotInstance( 2 );
    RobotInstance robot = robotService.CreateRobotInstance( 17, true, new List<PartsInstance>() {  // 17: AZ-0
      partsService.CreatePartsInstanceByPartsID( 1, true ),  //Chobham裝甲 HP+500; 裝甲+150
      partsService.CreatePartsInstanceByPartsID( 2, true )   //磁層關節 運動性+10
    } );
    MapFightingUnit hero2Unit = gameDataService.CreateMapFightingUnit( robot, heroPilotInstance ); //AZ-0 + 主角2
    hero2Unit.PilotInfo.AddExp( 8000 );

    //增加戰鬥單位至格納庫
    gameDataService.HouseUnits.Add( player1 );
    gameDataService.HouseUnits.Add( player2 );
    gameDataService.HouseUnits.Add( hero2Unit );

    basePlay();
  }

  void basePlay() {
    foreach (var unit in gameDataService.HouseUnits.Where( u => u.IsFightable )) {
      unit.PilotInfo.AddExp( 4000 );
      unit.UpdateInit();
    };
    gameDataService.AddMoney( 200000 );
  }

}


