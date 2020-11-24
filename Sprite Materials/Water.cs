using UnityEngine;

public class Water : MonoBehaviour {
  public SpriteRenderer sr;
  public Material WaterMaterial;

  private void Start() {
    WaterMaterial = sr.material;  
  }

  float t = 0;
  void Update() {
    Vector4 pos = Vector4.zero;
    pos.x = Mathf.Sin(t) * .005f;
    pos.y = Mathf.Cos(t*1.001f) * .0025f;
    pos.z = Mathf.Sin((t + 123) * 1.00035f) * .015f;
    pos.w = Mathf.Cos((t + 23) * 1.00025f) * .0075f;
    WaterMaterial.SetVector("_ox", pos);
    t += Time.deltaTime * Random.Range(2.5f, 4f);
  }
}
