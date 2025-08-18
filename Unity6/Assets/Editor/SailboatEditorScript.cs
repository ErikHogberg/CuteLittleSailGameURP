using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SailboatScript))]
// [CanEditMultipleObjects]
public class SailboatEditorScript : Editor {


	private void OnSceneGUI() {

		SailboatScript sailboatScript = (SailboatScript)target;

		EditorGUI.BeginChangeCheck();

		sailboatScript.WeightCenter = sailboatScript.transform.InverseTransformPoint(Handles.DoPositionHandle(sailboatScript.transform.TransformPoint(sailboatScript.WeightCenter), sailboatScript.transform.rotation));
		// sailboatScript.FrontForcePoint = sailboatScript.transform.InverseTransformPoint(Handles.DoPositionHandle(sailboatScript.transform.TransformPoint(sailboatScript.FrontForcePoint), sailboatScript.transform.rotation));
		// sailboatScript.RearForcePoint = sailboatScript.transform.InverseTransformPoint(Handles.DoPositionHandle(sailboatScript.transform.TransformPoint(sailboatScript.RearForcePoint), sailboatScript.transform.rotation));

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(this, "moved sailboat force point");
			EditorUtility.SetDirty(this);
		}
	}
}