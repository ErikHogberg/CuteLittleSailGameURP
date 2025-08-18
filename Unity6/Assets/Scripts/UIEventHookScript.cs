using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIEventHookScript : MonoBehaviour, IPointerEnterHandler {

	public UnityEvent MouseEnter;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void OnPointerEnter(PointerEventData eventData) {
        MouseEnter.Invoke();
	}
}
