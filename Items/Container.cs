using System.Collections.Generic;
using UnityEngine;

public class Container : Item {
  public AudioClip OpenSound;
  public AudioClip CloseSound;
  public AudioClip UnlockSound;
  public AudioClip LockSound;
  public AudioSource Audio;
  [SerializeField] private List<Item> items;
  public List<ContainedItem> containedItems;

  private void Start() {
    sr.color = normalColor;
    Audio = GetComponent<AudioSource>();
  }

  internal bool ValidFor(Item item) {
    foreach (ContainedItem ci in containedItems)
      if (ci.item.Equals(item)) return true;
    return false;
  }

  internal bool HasItem(Item item) {
    if (item == null) {
      foreach (ContainedItem ci in containedItems)
        if (ci.inContainer) return false;
      return true;
    }
    foreach (ContainedItem ci in containedItems)
      if (ci.item.Equals(item) && ci.inContainer) return true;
    return false;
  }

  internal bool HasItem(ItemEnum type) {
    if (type == ItemEnum.Undefined) {
      foreach (ContainedItem ci in containedItems)
        if (ci.inContainer) return false;
      return true;
    }
    foreach (ContainedItem ci in containedItems)
      if (ci.type == type && ci.inContainer) return true;
    return false;
  }

  internal void ShowItems() {
    foreach (ContainedItem ci in containedItems)
      if (ci.item != null && ci.inContainer) {
        ci.item.gameObject.SetActive(true);
        ci.item.transform.SetParent(transform);
        ci.item.transform.localPosition = ci.localPos;
      }
  }

  internal void HideItems() {
    foreach (ContainedItem ci in containedItems)
      if (ci.item != null && ci.inContainer)
        ci.item.gameObject.SetActive(false);
  }

  internal void Collect(Item item, Actor actor) {
    foreach (ContainedItem ci in containedItems)
      if (ci.item.Equals(item) && ci.inContainer) {
        ci.inContainer = false;
        if (!actor.inventory.Contains(item))
          actor.inventory.Add(ci.item);
        ci.item.gameObject.SetActive(false);
        // FIXME add to the right parent?
      }
  }
  internal void Collect(ItemEnum type, Actor actor) {
    foreach (ContainedItem ci in containedItems)
      if (ci.type == type && ci.inContainer) {
        ci.inContainer = false;
        actor.inventory.Add(ci.item);
        ci.item.gameObject.SetActive(false);
      }
  }

  internal bool Place(Item item, Actor actor) {
    foreach (ContainedItem ci in containedItems)
      if (ci.type == item.Item && !ci.inContainer) {
        ci.inContainer = true;
        if (actor.inventory.Contains(item))
          actor.inventory.Remove(item);
        ci.item.transform.SetParent(transform);
        ci.item.transform.localPosition = ci.localPos;
        ci.item.gameObject.SetActive(IsOpen());
        return false;
      }

    return true; // This should generate a "It does not fit" message
  }
}

[System.Serializable]
public class ContainedItem {
  public ItemEnum type;
  public Item item;
  public Vector2 localPos;
  public bool inContainer = true;
}



