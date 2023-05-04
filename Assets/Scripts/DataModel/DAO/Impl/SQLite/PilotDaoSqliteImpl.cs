

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityORM;

namespace DataModel.DAO.Impl.SQLite {

  class PilotDaoSqliteImpl : IPilotDao {

    private DBMapper mapper;

    //[field: SerializeField]
    public List<PilotInstance> PilotInstanceList { get; set; } = new List<PilotInstance>();

    public List<PilotInstance> HeroPilotInstanceList { get; set; } = new List<PilotInstance>();

    public List<Hero> HeroList { get; set; } = new List<Hero>();

    public PilotDaoSqliteImpl() {
      mapper = MyDbSQLite.Instance.Mapper;
    }

    public Pilot GetPilotBase( int ID ) {
     return mapper.ReadByKey<Pilot>( ID );
    }

    public Pilot GetPilotByID( int ID ) {
      Pilot pilot = mapper.ReadByKey<Pilot>( ID );
      
      pilot.SPComPilots = mapper.Read<SPComPilot>( "SELECT * FROM SPComPilot Where PilotID = " + pilot.ID ).OrderBy( s => s.Level ).ToList();
      pilot.SPComPilots.ForEach( s => {
        s.SPCommand = mapper.ReadByKey<SPCommand>( s.SPComID );
      } );

      pilot.PilotSkillDefaultList = mapper.Read<PilotSkillDefault>( "SELECT * FROM PilotSkillDefault Where PilotID = " + pilot.ID ).OrderBy( d => d.Order ).ToList();

      return pilot;
    }
    /*
    public List<PilotDialog> GetPilotDialogWhenAttackingRival( int pilotID, int otherPilotID ) {
      var pdList = mapper.Read<PilotDialog>( $"SELECT * FROM PilotDialog Where PilotID = {pilotID} and OtherPilotID = {otherPilotID} and IsAttack = 1"  ).ToList();
      return pdList;
    }*/


    public SPCommand GetSPComByID( int ID ) {
      return mapper.ReadByKey<SPCommand>( ID );
    }

    /*
    public void InjectSPComPilots( Pilot pilot ) {
      return;
    }*/

    public PilotSkill GetPilotSkillByID( int ID ) {
      return mapper.ReadByKey<PilotSkill>( ID );
    }
    /*
    public PilotSkillDefault GetPilotSkillDefaultByKey( int pilotID, int order ) {
      return null;
    }*/
    /*
    public List<PilotSkillDefault> GetPilotSkillDefaultList( Pilot pilot ) {
      return null;
    }*/

    public void SavePilotInstanceList( int saveSlot ) {
      mapper.DeleteAll<PilotInstance>( "SaveSlot", saveSlot );
      mapper.DeleteAll<PilotSkillInstance>( "PilotInstanceSaveSlot", saveSlot );
      mapper.DeleteAll<Hero>( "SaveSlot", saveSlot );

      List<PilotSkillInstance> pilotSkillInstanceList = new List<PilotSkillInstance>();
      List<Hero> heroList = new List<Hero>();

      PilotInstanceList.ForEach( pi => {
        pi.SaveSlot = saveSlot;

        pi.PilotSkillInstanceList.ForEach( w => {
          w.PilotInstanceSaveSlot = saveSlot;
          w.PilotInstanceSeqNo = pi.SeqNo;
        } );

        pilotSkillInstanceList.AddRange( pi.PilotSkillInstanceList );
        //mapper.InsertAll( pi.PilotSkillInstanceList.ToArray() );

        if (pi.HeroSeqNo.HasValue && pi.Hero != null) {
          pi.Hero.SaveSlot = saveSlot;
          //Debug.Log( "start save: mapper.InsertAll( new Hero[] { pi.Hero } )" + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );
          //mapper.InsertAll( new Hero[] { pi.Hero } );
          heroList.Add( pi.Hero );
          //Debug.Log( "finish save: mapper.InsertAll( new Hero[] { pi.Hero } )" + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );
        }
      } );

      mapper.InsertAll( heroList.ToArray() );

      //Debug.Log( $"start save: mapper.InsertAll( pilotSkillInstanceList.ToArray(), Count: {pilotSkillInstanceList.Count} )" + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );
      mapper.InsertAll( pilotSkillInstanceList.ToArray() );
      //Debug.Log( "finish save: mapper.InsertAll( pilotSkillInstanceList.ToArray() )" + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );

      mapper.InsertAll<PilotInstance>( PilotInstanceList.ToArray() );
    }

    public void LoadPilotInstanceList( int saveSlot ) {
      List<PilotInstance> pilotInstanceList = mapper.Read<PilotInstance>( "SELECT * FROM PilotInstance Where SaveSlot = " + saveSlot ).ToList();
      HeroList = mapper.Read<Hero>( "SELECT * FROM Hero Where SaveSlot = " + saveSlot ).ToList();

      pilotInstanceList.ForEach( pi => {
        pi.Pilot = GetPilotByID( pi.PilotID );
        pi.PilotSkillInstanceList = loadPilotSkillInstanceList( pi.SaveSlot, pi.SeqNo );

        if (pi.HeroSeqNo.HasValue) {
          pi.Hero = HeroList.FirstOrDefault( h => h.SeqNo == pi.HeroSeqNo );
          HeroPilotInstanceList.Add( pi );
        }
      } );

      PilotInstanceList = pilotInstanceList;
    }

    private List<PilotSkillInstance> loadPilotSkillInstanceList( int saveSlot, int seqNo ) {
      List<PilotSkillInstance> instanceList = mapper.Read<PilotSkillInstance>( $"SELECT * FROM PilotSkillInstance Where PilotInstanceSaveSlot = '{saveSlot}' and  PilotInstanceSeqNo = '{seqNo}'" ).ToList();
      instanceList.ForEach( inst => {
        inst.PilotSkill = mapper.ReadByKey<PilotSkill>( inst.PilotSkillID );
      } );
      return instanceList;
    }

    public void LoadPilotInstanceList( List<PilotInstance> pilotInstanceList, List<Hero> heroList ) {
      HeroList = heroList;

      pilotInstanceList.ForEach( pi => {
        pi.Pilot = GetPilotByID( pi.PilotID );
        //pi.PilotSkillInstanceList = loadPilotSkillInstanceList( pi.SaveSlot, pi.SeqNo );

        if (pi.HeroSeqNo.HasValue) {
          pi.Hero = HeroList.FirstOrDefault( h => h.SeqNo == pi.HeroSeqNo );
          HeroPilotInstanceList.Add( pi );
        }
      } );

      PilotInstanceList = pilotInstanceList;
    }

  }

}
