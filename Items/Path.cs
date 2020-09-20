using UnityEngine;

public class Path : MonoBehaviour {
  public float minY;
  public float maxY;
  public bool isStair;

  void Start() {
    GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);

  }

}
