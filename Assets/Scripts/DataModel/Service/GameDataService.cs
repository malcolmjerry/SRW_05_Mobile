using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace DataModel.Service {

  public class GameDataService {

    public List<MapFightingUnit> HouseUnits { get; set; } = new List<MapFightingUnit>();
    public List<MapFightingUnit> HouseUnitsFightables { get { return HouseUnits.Where( h => h.IsFightable ).ToList(); } }

    private MyDbContext db;
    private string savePath;
    private string saveBeforeStagePath;

    public GameDataService( MyDbContext myDbContext ) {
      db = myDbContext;
      //savePath = Path.Combine( Application.dataPath, "Data/continue.save" );
      //saveBeforeStagePath = Path.Combine( Application.dataPath, "Data/saveBeforeStage.save" );
      savePath = Path.Combine( Application.persistentDataPath, "Data", "continue.save" );
      saveBeforeStagePath = Path.Combine( Application.persistentDataPath, "Data", "saveBeforeStage.save" );
    }

    public void Reset() {
      GameData.Chapter = 0;
      GameData.Stage = 0;
      GameData.Flags = 0;
      GameData.HistoryMoney = 0;
      GameData.HistoryTurns = 0;
      GameData.Money = 0;
      GameData.Turns = 0;
    }

    public GameData GameData {
      get { return db.GameDataDao.GameData; }
    }

    public GameData UpdateGameData( GameData gameData ) {
      db.GameDataDao.GameData.SaveSlot = gameData.SaveSlot;
      db.GameDataDao.GameData.Money = gameData.Money;
      db.GameDataDao.GameData.HistoryMoney = gameData.HistoryMoney;
      db.GameDataDao.GameData.Turns = gameData.Turns;
      db.GameDataDao.GameData.HistoryTurns = gameData.HistoryTurns;
      db.GameDataDao.GameData.Flags = gameData.Flags;
      db.GameDataDao.GameData.Chapter = gameData.Chapter;  //表面顯示的話數
      db.GameDataDao.GameData.Stage = gameData.Stage;    //實際通關的版號
      db.GameDataDao.GameData.SaveTime = gameData.SaveTime;
      return db.GameDataDao.GameData;
    }

    const long MAX_MONEY = 9999999999;
    public void AddMoney( int money ) {
      db.GameDataDao.GameData.Money += money;
      if (db.GameDataDao.GameData.Money > MAX_MONEY)
        db.GameDataDao.GameData.Money = MAX_MONEY;

      db.GameDataDao.GameData.HistoryMoney += money;
      if (db.GameDataDao.GameData.HistoryMoney > MAX_MONEY)
        db.GameDataDao.GameData.HistoryMoney = MAX_MONEY;
    }

    public void UseMoney( int money ) {
      db.GameDataDao.GameData.Money -= money;
    }

    public void AddTurn() {
      db.GameDataDao.GameData.Turns++;
      db.GameDataDao.GameData.HistoryTurns++;
    }

    public void InitTurn() {
      db.GameDataDao.GameData.Turns = 1;
      db.GameDataDao.GameData.HistoryTurns++;
    }

    public string GetStageName() {
      //return "stage_002";

      string stageNum = GameData.Stage.ToString().PadLeft( 3, '0' );
      return $"stage_{stageNum}";
    }

    public string GetChapterName() {
      return StageMap.StageMapList[db.GameDataDao.GameData.Stage];
    }

    public string GetChapterStr() {
      string stageTitle = StageMap.StageMapList[db.GameDataDao.GameData.Stage];
      return $"第{db.GameDataDao.GameData.Chapter}話 {stageTitle}";
    }

    public void Save( int saveSlot ) {
      try {
        GameData.SaveTime = DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" );
        db.GameDataDao.Save( saveSlot );

        DIContainer.Instance.RobotService.SaveActiveRobotInstance( saveSlot );
        DIContainer.Instance.PilotService.SaveActivePilotInstance( saveSlot );      
        DIContainer.Instance.PartsService.Save( saveSlot );
      }
      catch (Exception e) {
        Debug.Log( "Save exception. " + e.Message );
        throw;
      }
    }

    public void Load( int saveSlot ) {
      HouseUnits = new List<MapFightingUnit>();
      db.GameDataDao.Load( saveSlot );
      //DIContainer.Instance.PartsService.Load( saveSlot );  //包含在 Robot 的讀取方法
      DIContainer.Instance.RobotService.LoadActiveRobotInstance( saveSlot );
      DIContainer.Instance.PilotService.LoadActivePilotInstance( saveSlot );

      //MapFightingUnits = new List<MapFightingUnit>();  //必須先清空

      DIContainer.Instance.RobotService.ActiveRobotInstanceList.ForEach( ri => {
        PilotInstance pilotIn = DIContainer.Instance.PilotService.ActivePilotInstanceList.Where( pi => pi.RobotInstanceSeqNo == ri.SeqNo && pi.SaveSlot == ri.SaveSlot ).FirstOrDefault();
        HouseUnits.Add( CreateMapFightingUnit( ri, pilotIn?.Enable == 1? pilotIn : null ) );
      } );

      DIContainer.Instance.PilotService.ActivePilotInstanceList.Where( pi => !pi.RobotInstanceSeqNo.HasValue ).ToList().ForEach( pi => {
        if (pi.Enable == 1)
          HouseUnits.Add( CreateMapFightingUnit( null, pi ) );
      } );
    }

    public void SaveContinue( SaveContinue saveCon ) {
      saveCon.HouseUnits = HouseUnits;
      saveCon.HeroList = db.PilotDao.HeroList;
      saveCon.PilotInstanceList = db.PilotDao.PilotInstanceList;
      saveCon.HeroPilotInstanceList = db.PilotDao.HeroPilotInstanceList;
      saveCon.PartsInstanceList = db.PartsDao.PartsInstanceList;
      saveCon.RobotInstanceList = db.RobotDao.RobotInstanceList;
      saveCon.GameData = db.GameDataDao.GameData;

      var binaryFormatter = new BinaryFormatter();
      using (var fileStream = File.Create( savePath )) {
        try {
          binaryFormatter.Serialize( fileStream, saveCon );
        }
        catch (Exception e) {
          Debug.LogError( e.Message );
        }
      }
      Debug.Log( "Data Saved" );
    }

    public SaveContinue LoadContinue() {
      SaveContinue saveCon;
      var binaryFormatter = new BinaryFormatter();
      using (var fileStream = File.Open( savePath, FileMode.Open )) {
        saveCon = (SaveContinue)binaryFormatter.Deserialize( fileStream );
      }
      //DIContainer.Instance.PilotService.LoadBySerialize( saveCon.PilotInstanceList, saveCon.HeroList );
      DIContainer.Instance.db.PilotDao.PilotInstanceList = saveCon.PilotInstanceList;
      DIContainer.Instance.db.PilotDao.HeroPilotInstanceList = saveCon.HeroPilotInstanceList;
      DIContainer.Instance.db.PilotDao.HeroList = saveCon.HeroList;
      DIContainer.Instance.db.RobotDao.RobotInstanceList = saveCon.RobotInstanceList;
      DIContainer.Instance.db.PartsDao.PartsInstanceList = saveCon.PartsInstanceList;
      HouseUnits = saveCon.HouseUnits;
      UpdateGameData( saveCon.GameData );

      return saveCon;
      /*
      var a = HouseUnits[0];
      a.PilotInfo.PilotInstance.ShootAdded += 1000;
      var b = DIContainer.Instance.db.PilotDao.PilotInstanceList[0];
      a.PilotInfo.Update();

      Debug.Log( "b.Pilot.Hit" + b.Pilot.Hit );
      */
    }

    public void SaveBeforeStage() {
      SaveBase saveBase = new SaveBase();
      saveBase.HouseUnits = HouseUnits;
      saveBase.HeroList = db.PilotDao.HeroList;
      saveBase.PilotInstanceList = db.PilotDao.PilotInstanceList;
      saveBase.HeroPilotInstanceList = db.PilotDao.HeroPilotInstanceList;
      saveBase.PartsInstanceList = db.PartsDao.PartsInstanceList;
      saveBase.RobotInstanceList = db.RobotDao.RobotInstanceList;
      saveBase.GameData = db.GameDataDao.GameData;

      var binaryFormatter = new BinaryFormatter();
      using (var fileStream = File.Create( saveBeforeStagePath )) {
        try {
          binaryFormatter.Serialize( fileStream, saveBase );
        }
        catch (Exception e) {
          Debug.LogError( e.Message );
        }
      }
    }

    public SaveBase LoadBeforeStage() {
      SaveBase saveCon;
      var binaryFormatter = new BinaryFormatter();
      using (var fileStream = File.Open( saveBeforeStagePath, FileMode.Open )) {
        saveCon = (SaveBase)binaryFormatter.Deserialize( fileStream );
      }

      DIContainer.Instance.db.PilotDao.PilotInstanceList = saveCon.PilotInstanceList;
      DIContainer.Instance.db.PilotDao.HeroPilotInstanceList = saveCon.HeroPilotInstanceList;
      DIContainer.Instance.db.PilotDao.HeroList = saveCon.HeroList;
      DIContainer.Instance.db.RobotDao.RobotInstanceList = saveCon.RobotInstanceList;
      DIContainer.Instance.db.PartsDao.PartsInstanceList = saveCon.PartsInstanceList;
      HouseUnits = saveCon.HouseUnits;
      UpdateGameData( saveCon.GameData );

      return saveCon;
    }

    public bool HasContinueSave() {
      return File.Exists( savePath );
    }

    public void Delete( int saveSlot ) {
      GameData.SaveTime = null;
      db.GameDataDao.Save( saveSlot );
    }

    public List<GameData> GetAll() {
      List<GameData> list = db.GameDataDao.GetAll();

      if (list == null || list.Count == 0) {
        list = new List<GameData>();
        for (int i = 0; i<24; i++) {
          GameData newGameData = new GameData() { SaveSlot = i+1 };
          list.Add( newGameData );
          db.GameDataDao.Save( newGameData.SaveSlot, newGameData );
        }
      }
      return list;
    }

    public MapFightingUnit CreateMapFightingUnit( RobotInstance robotInstance, PilotInstance pilotInstance ) {
      if (robotInstance != null && pilotInstance != null) 
        pilotInstance.RobotInstanceSeqNo = robotInstance.SeqNo;

      MapFightingUnit unit = new MapFightingUnit();

      if (robotInstance != null)
        unit.RobotInfo = new RobotInfo( robotInstance ); //robotInstance?.CloneInfo( unit );

      if (pilotInstance != null) 
        unit.PilotInfo =  new PilotInfo( pilotInstance ); //pilotInstance?.CloneInfo();

      unit.UpdateInit();
      return unit;
    }

    public void CheckAndRemoveUnit( MapFightingUnit unit ) {
      if (unit.RobotInfo == null && unit.PilotInfo == null)
        HouseUnits.Remove( unit );
    }

    public StoryPhaseEnum StoryPhase { set; get; } = 0;  //0 Start  1 After 
    public enum StoryPhaseEnum { Start = 0, After }

    public int GenerateNewStage() {
      //if (StoryPhase > StoryPhaseEnum.Start)
        //return GameData.Stage;
      ++GameData.Chapter;
      switch (GameData.Stage) {
        case 0:
          //做一些檢查, 判斷一些Flag要素, 分支路選擇等, 決定下一個Stage
          return ++GameData.Stage;
        case 1:
          //做一些檢查, 判斷一些Flag要素, 分支路選擇等, 決定下一個Stage
          return ++GameData.Stage;//++GameData.Stage;
        case 2:
          //做一些檢查, 判斷一些Flag要素, 分支路選擇等, 決定下一個Stage
          return ++GameData.Stage;
        case 3:
          return ++GameData.Stage;
        default: return 1;
      }
    }

    public void PrologInitStage() {
      //GameData.Stage = 0;
      //GameData.Chapter = 0;
    }

  }

}
