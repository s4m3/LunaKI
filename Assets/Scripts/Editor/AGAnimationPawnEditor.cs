//using UnityEditor;
//
//[CustomEditor(typeof(AGAnimationPawn))]
//public class AGAnimationPawnEditor : Editor {
//
//	public override void OnInspectorGUI ()
//	{
//		DrawDefaultInspector ();
//		AGAnimationPawn pawn = (AGAnimationPawn)target;
//		pawn.sameLayer = EditorGUI.IntField (new UnityEngine.Rect (), pawn.sameLayer);
////		int layer = UnityEngine.GUILayout.TextField ("Layer");
//		if (UnityEngine.GUILayout.Button ("Same Layer")) {
//			pawn.Idle.m_Layer = pawn.sameLayer;
//			pawn.Walk.Layer (pawn.sameLayer);
//			pawn.Run.Layer (pawn.sameLayer);
////			pawn.Walk.Layer (pawn.sameLayer);
//		}
//	}
//}