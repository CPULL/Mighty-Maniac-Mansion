using UnityEngine;

public class Door : Item {
  public Room src;
  public Room dst;
  public Vector3 camerapos;
  public Door correspondingDoor;
  public TransitionDirection transition;
  public Dir arrivalDirection;
  public AudioClip OpenSound;
  public AudioClip CloseSound;
  public AudioClip UnlockSound;
  public AudioClip LockSound;
  public AudioSource Audio;

  private void Start() {
    sr.color = normalColor;
    Audio = GetComponent<AudioSource>();
  }
}





