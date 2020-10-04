using System.Collections.Generic;
using UnityEngine;

public class Container : Item {
  public AudioClip OpenSound;
  public AudioClip CloseSound;
  public AudioClip UnlockSound;
  public AudioClip LockSound;
  public AudioSource Audio;
  public List<Item> items;

  private void Start() {
    sr.color = normalColor;
    Audio = GetComponent<AudioSource>();
  }

  internal bool HasItem(Item item) {
    return items.Contains(item);
  }
}





