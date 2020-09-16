using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
  public string ID;
  public string RoomName;
  public float orientation;
  public float minL;
  public float maxR;
  public float CameraGround;

  public Camera roomCamera;
  public List<GameObject> Doors; // FIXME use something more complex, with an actual link between the rooms


  private void Start() {
    roomCamera = GetComponentInChildren<Camera>();
  }
}
