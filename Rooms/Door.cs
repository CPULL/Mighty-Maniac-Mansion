using UnityEditor;
using UnityEngine;

public class Door : Item {
  public Room src;
  public Room dst;
  public Vector3 camerapos;
  public Door correspondingDoor;
  public TransitionType transition;
  public Dir arrivalDirection;

  private void Start() {
    sr.color = normalColor;
  }
}





