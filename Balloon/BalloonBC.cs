using UnityEngine;

public class BalloonBC : MonoBehaviour {
  private void OnMouseDown() {
    GD.b.delay = Time.deltaTime;
  }
}