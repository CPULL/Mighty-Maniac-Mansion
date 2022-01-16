using UnityEngine;

public class Water : MonoBehaviour {
  public Transform onWater;
  Vector3 pos = new Vector3(-6.12f, -1.98f, 0);

  void Update() {
    onWater.localPosition = pos + 
      Vector3.right * Mathf.Sin(Time.time) * .05f +
      Vector3.up * Mathf.Sin(Time.time * Time.time * .1f) * .001f;
  }
}
