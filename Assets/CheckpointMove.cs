using System.Collections;
using UnityEngine;

namespace EHUtil;

public class CheckpointMove : MonoBehaviour {
  void Awake() {
    StartCoroutine(Move());
  }

  private IEnumerator Move() {
    Vector3 p = this.gameObject.transform.position;
    p.y = 0;
    this.gameObject.transform.position = p;
    yield return new WaitForSeconds(0.1f);
    for (float y = 0; y >= -3; y -= 0.025f) {
      p.y = y;
      this.gameObject.transform.position = p;
      yield return new WaitForSeconds(0.005f);
    }
    Destroy(this.gameObject);
  }
}