using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WindZoneScript))]
// [CanEditMultipleObjects]
public class WindZoneEditorScript : Editor {


	private void OnSceneGUI() {

		WindZoneScript windZoneScript = (WindZoneScript)target;

		EditorGUI.BeginChangeCheck();

		Vector3 windDir = windZoneScript.WindDir + windZoneScript.transform.position;

		Handles.DrawLine(windZoneScript.transform.position, windDir);
		windZoneScript.WindDir = Handles.DoPositionHandle(windDir, Quaternion.identity) - windZoneScript.transform.position;

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(this, "moved sailboat force point");
			EditorUtility.SetDirty(this);
		}
	}
}