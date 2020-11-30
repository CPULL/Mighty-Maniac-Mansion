using System.Collections.Generic;
using UnityEngine;

public class Triggerer : MonoBehaviour {
  public TriggerID id;
  public List<ActionAndCondition> actions;
  public bool SelfDisable = false;

  private void Start() {
    // Check if we have a collider
    if (GetComponent<BoxCollider2D>() == null && GetComponent<PolygonCollider2D>() == null) {
      Debug.LogError("Missing collider for event trigger: " + name + ": " + transform.parent?.name);
    }
  }

  private void OnTriggerEnter2D(Collider2D col) {
    if (col.gameObject == null) return;
    Actor actor = col.gameObject.GetComponent<Actor>();
    if (actor == null || actor.IAmNPC) return;

    bool done = false;
    foreach (ActionAndCondition ac in actions)
      if (ac.Condition.IsValid(actor, null, null, null, When.Always)) {
        foreach (GameAction a in ac.Actions) {
          a.RunAction(actor, null, false);
        }
        done = true;
      }

    if (done && SelfDisable) gameObject.SetActive(false);
  }

}


public enum TriggerID {
  FredKidnappedDialogue,
  EdnaKitchenStarter,
  NoMapMessage,
}