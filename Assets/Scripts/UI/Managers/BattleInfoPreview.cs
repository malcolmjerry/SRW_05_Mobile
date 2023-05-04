using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AttackData;

public class BattleInfoPreview : MonoBehaviour {

  void Awake() {

  }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void Setup( UnitInfo fromUnitInfo, UnitInfo toUnitInfo, WeaponInfo fromWeapon, WeaponInfo toWeapon ) {
    var toChannel = transform.Find( "Panel" );

    toChannel.Find( "HpTxt" ).GetComponent<Text>().text = toUnitInfo.RobotInfo.HP + "/" + toUnitInfo.RobotInfo.MaxHP;
    toChannel.Find( "EnTxt" ).GetComponent<Text>().text = toUnitInfo.RobotInfo.EN + "/" + toUnitInfo.RobotInfo.MaxEN;
    toChannel.Find( "HpSlider" ).GetComponent<Slider>().value = (float)toUnitInfo.RobotInfo.HP / toUnitInfo.RobotInfo.MaxHP;
    toChannel.Find( "EnSlider" ).GetComponent<Slider>().value = (float)toUnitInfo.RobotInfo.EN / toUnitInfo.RobotInfo.MaxEN;
    toChannel.Find( "RobotNameTxt" ).GetComponent<Text>().text = toUnitInfo.RobotInfo.RobotInstance.Robot.FullName;
    toChannel.Find( "ArmorTxt" ).GetComponent<Text>().text = toUnitInfo.RobotInfo.Armor.ToString();
    //toChannel.Find( "WeaponNameTxt" ).GetComponent<Text>().text = CounterAttack.AttackType == AttackTypeEnum.Unable ? "反擊不能" : ToWeapon.WeaponInstance.Weapon.Name;
    toChannel.Find( "PilotNameTxt" ).GetComponent<Text>().text = toUnitInfo.PilotInfo.ShortName;
    toChannel.Find( "WillPowerTxt" ).GetComponent<Text>().text = toUnitInfo.PilotInfo.Willpower.ToString();
    toChannel.Find( "LvTxt" ).GetComponent<Text>().text = toUnitInfo.PilotInfo.Level.ToString();

    if (fromUnitInfo != null && fromWeapon != null) {
      var firstAttack = new AttackData( fromUnitInfo, fromWeapon, toUnitInfo, true, AttackTypeEnum.Normal, CounterTypeEnum.Normal );
      toChannel.Find( "HitRateTxt" ).GetComponent<Text>().text = firstAttack.HitRate + "%";
    }
    else {
      toChannel.Find( "HitRateTxt" ).GetComponent<Text>().text = "---";
    }
  }

}
