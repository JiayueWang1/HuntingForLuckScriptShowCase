using System;
using System.Collections;
using System.Collections.Generic;
using MetroidvaniaTools;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class BoardForNote : MonoBehaviour {
    [Multiline] public string content;

    private void OnTriggerStay2D(Collider2D other) {
        Debug.Log(other);
        if (other.TryGetComponent(out PlayerHealth player)) {
            Debug.Log(Camera.main);
            Debug.Log(Camera.main.WorldToScreenPoint(this.transform.position));
        }
    }

    private void OnTriggerExit2D(Collider2D other) {

        if (other.TryGetComponent(out PlayerHealth player)) {

        }
    }
}
