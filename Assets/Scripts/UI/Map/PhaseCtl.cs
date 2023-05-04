using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PhaseCtl : MonoBehaviour {

  private Animator animator;
  private Text text;

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

  }

  public void Do( string phaseName, Action workload, Action callback ) {
    StartCoroutine( DoJob( phaseName, workload, callback ) );
  }

  IEnumerator DoJob( string phaseName, Action doProcess, Action callback ) {
    text.text = phaseName;
    animator.enabled = true;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/SpCom/Sp_2" ) );

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
    CoroutineCommon.CallWaitForSeconds( 1f, () => {
      gameObject.SetActive( false );
      callback();
    } );
  }

}
