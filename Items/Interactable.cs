using System.Collections.Generic;
using UnityEngine;

public class Interactable : IObj {
  SpriteRenderer img;
  public ItemType type;
  public string ItemName;
  [TextArea(3, 10)]
  public string Description;
  public Vector3 InteractionPosition;
  public Dir preferredDirection;
  public Color32 OverColor = Color.yellow;
  public Color32 NotOverColor = Color.white;
  public bool isLocked = false;
  public bool isOpen = false;
  public Sprite[] openSprites;
  public List<GameAction> Actions;

  private void Awake() {
    img = GetComponent<SpriteRenderer>();
  }


  private void OnMouseEnter() {
    img.color = OverColor;
    Controller.SetCurrentItem(this);
  }

  private void OnMouseExit() {
    img.color = NotOverColor;
    Controller.SetCurrentItem(null);
  }

  internal bool Open() {
    if (type != ItemType.Openable && type != ItemType.Activable && type != ItemType.Usable) return false;
    if (isLocked) return true;

    isOpen = !isOpen;
    if (openSprites != null && openSprites.Length > 0)
      img.sprite = openSprites[isOpen ? 1 : 0];
    return false;
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = new Color(1, 0, 1, 0.75F);
    Gizmos.DrawSphere(InteractionPosition, .1f);
  }

  internal void PlayActions() {
    if (Actions == null || Actions.Count == 0) return;

    foreach (GameAction a in Actions)
      Controller.AddAction(a);
  }
}
