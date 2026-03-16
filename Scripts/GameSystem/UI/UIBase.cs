using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class UIState
{
    [System.NonSerialized]
    public bool dirty = true;
    public virtual void MarkDirty() {
        dirty = true;
    }
}

public class UIBase<S> : MonoBehaviour where S : UIState, new() {
    public S state;

    public virtual void ApplyNewStateInternal() {

    }

    public void ApplyNewState() {
        if (!gameObject.activeInHierarchy) return;
        if (state == null) {
            Debug.LogWarningFormat(
                "{0} is re-initialized! this could indicate bad setup. Please be careful if this message show up frequently.",
                gameObject.name);
            state = new S();
        }

        if (state.dirty) {
            ApplyNewStateInternal();
        }

        UpdateChildren();
        state.dirty = false;
    }

    public static void DestroyAllChildren(Transform t) {
        int childs = t.childCount;
        for (int i = childs - 1; i >= 0; i--) {
            GameObject.DestroyImmediate(t.GetChild(i).gameObject);
        }
    }

    public virtual void UpdateChildren() {
    }

    // usage:
    // ApplyNewStateArray<KUINoteState, KUINote>(noteTransform, notePrefab, state.noteStates);

    public static void ApplyNewStateArray<T, C>(RectTransform parent, GameObject childPrefab, IEnumerable<T> states)
        where T : UIState, new() where C : UIBase<T> {
        int i = 0;
        foreach (var currentState in states) {
            GameObject obj;
            if (i < parent.childCount) {
                obj = parent.GetChild(i).gameObject;
                obj.SetActive(true);
            } else {
                obj = Instantiate(childPrefab, parent, false);
            }

            var childComponent = obj.GetComponent<C>();
            if (currentState != childComponent.state) {
                childComponent.state = currentState;
                childComponent.state.dirty = true;
            }

            childComponent.ApplyNewState();
            i++;
        }

        for (; i < parent.childCount; i++) {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }
}
