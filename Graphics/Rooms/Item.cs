using UnityEngine;

public class Item : MonoBehaviour {
  SpriteRenderer img;
  public ItemType type;
  public string ItemName;
  [TextArea(3, 10)]
  public string Description;
  public Vector3 InteractionPosition;
  public Dir preferredDirection;
  public Color32 OverColor = Color.yellow;
  public bool isLocked = false;
  public bool isOpen = false;
  public Sprite[] openSprites;

  private void Awake() {
    img = GetComponent<SpriteRenderer>();
  }


  private void OnMouseEnter() {
    img.color = OverColor;
    Controller.SetCurrentItem(this);
  }

  private void OnMouseExit() {
    img.color = Color.white;
    Controller.SetCurrentItem(null);
  }

  internal bool Open() {
    if (type != ItemType.Openable && type != ItemType.Activable) return false;
    if (isLocked) return true;

    isOpen = !isOpen;
    img.sprite = openSprites[isOpen ? 1 : 0];
    return false;
  }
}
