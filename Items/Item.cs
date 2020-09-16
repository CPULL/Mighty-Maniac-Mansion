using System.Globalization;
using UnityEngine;

public class Item : MonoBehaviour {
  public string ID;
  public string Name;
  public NumberFormatInfo Quantity;
  public SpriteRenderer sr;
  // FIXME type?
  // FIXME use with? Like having an Interactable or another Item may produce an action


  public string Interaction;
  public GameAction action;

  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
  }
}
