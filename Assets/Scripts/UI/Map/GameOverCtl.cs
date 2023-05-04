using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCtl : MonoBehaviour {

  private Animator animator;
  private Text text;
  Action callback;
  bool canOut;

  private void Awake() {
    animator = GetComponent<Animator>();
    animator.enabled = false;
    text = transform.Find( "Message" ).GetComponent<Text>();
  }

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (canOut && Input.GetButtonDown( "Confirm" )) {
      gameObject.SetActive( false );
      callback();
    }
  }

  public void Do( Action workload, Action callback ) {
    this.callback = callback;
    StartCoroutine( DoJob( workload ) );
  }

  IEnumerator DoJob( Action doProcess ) {
    canOut = false;
    //text.text = phaseName;
    animator.enabled = true;
    //EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/SpCom/Sp_2" ) );
    BGMController.SET_BGM( "F_GameOver" );
    
    Task task = null;
    if (doProcess != null)
      task = Task.Run( doProcess );

    while (GetComponent<Animator>().GetCurrentAnimatorStateInfo( 0 ).normalizedTime < 1)
      yield return null;

    if (task != null) {
      while (!task.IsCompleted) {
        yield return null;
      }
    }

    animator.enabled = false;
    canOut = true;
  }

}
