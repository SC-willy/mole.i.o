using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Supercent.Util.Editor
{
    [CustomEditor(typeof(SimpleSpline))]
    public class SimpleSplineEditor : UnityEditor.Editor
    {
        int selectedPointIndex = -1;
        FieldInfo pointsFieldInfo = null;


        void OnEnable()
        {
            pointsFieldInfo = typeof(SimpleSpline).GetField("points", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        void OnSceneGUI()
        {
            Tools.hidden = -1 < selectedPointIndex;
            var spline = (SimpleSpline)target;
            var points = (List<SimpleSpline.Point>)pointsFieldInfo.GetValue(spline);
            var segments = SimpleSpline.Point.NormalSegments;

            var transform = spline.transform; 
            var posWorld = transform.position;
            var rotWorld = transform.rotation;
            var scaLossy = transform.lossyScale;

            var camForward = SceneView.currentDrawingSceneView.camera.transform.forward;
            var btnRotation = Quaternion.LookRotation(camForward) * Quaternion.Euler(0, 0, 45f);

            var oldColor = Handles.color;
            {
                for (int idPoint = 0, cnt = points.Count; idPoint < cnt; ++idPoint)
                {
                    var point = points[idPoint];
                    var isSelected = selectedPointIndex == idPoint;

                    Handles.color = isSelected ? Color.red : Color.white;
                    var posPoint = transform.TransformPoint(point.position);
                    if (Handles.Button(posPoint, btnRotation, 0.08f, 0.15f, Handles.RectangleHandleCap))
                    {
                        selectedPointIndex = selectedPointIndex == idPoint ? -1 : idPoint;
                        Repaint();
                    }


                    Handles.color = isSelected || (selectedPointIndex + 1) == idPoint ? Color.yellow : Color.green;
                    var curPoint = transform.TransformPoint(point.CalculateCubicBezierPoint(0f));
                    for (int idSegment = 0; idSegment <= segments; ++idSegment)
                    {
                        var prvPoint = curPoint;
                        curPoint = transform.TransformPoint(point.CalculateCubicBezierPoint(idSegment / (float)segments));
                        Handles.DrawLine(prvPoint, curPoint);
                    }

                    Handles.color = isSelected ? Color.red : Color.cyan;
                    if (idPoint != 0)
                    {
                        var potIn = transform.TransformPoint(point.position + point.tangentIn);
                        Handles.DrawLine(posPoint, potIn);
                        if (Handles.Button(potIn, btnRotation, 0.05f, 0.08f, Handles.CircleHandleCap))
                        {
                            selectedPointIndex = idPoint;
                            Repaint();
                        }
                    }
                    if (idPoint != cnt - 1)
                    {
                        var posOut = transform.TransformPoint(point.position + point.tangentOut);
                        Handles.DrawLine(posPoint, posOut);
                        if (Handles.Button(posOut, btnRotation, 0.05f, 0.08f, Handles.CircleHandleCap))
                        {
                            selectedPointIndex = idPoint;
                            Repaint();
                        }
                    }

                    if (isSelected)
                    {
                        var pos = transform.TransformPoint(point.position);
                        var newPosition = Handles.PositionHandle(pos, rotWorld) - pos;
                        if (newPosition != Vector3.zero)
                        {
                            Undo.RecordObject(spline, "Move Point");
                            point.position += newPosition;
                        }

                        if (idPoint != 0)
                        {
                            pos = transform.TransformPoint(point.position + point.tangentIn);
                            var newTangentIn = Handles.PositionHandle(pos, rotWorld) - pos;
                            if (newTangentIn != Vector3.zero)
                            {
                                Undo.RecordObject(spline, "Move Tangent In");
                                point.tangentIn += newTangentIn;
                            }
                        }

                        if (idPoint != cnt - 1)
                        {
                            pos = transform.TransformPoint(point.position + point.tangentOut);
                            var newTangentOut = Handles.PositionHandle(pos, rotWorld) - pos;
                            if (newTangentOut != Vector3.zero)
                            {
                                Undo.RecordObject(spline, "Move Tangent Out");
                                point.tangentOut += newTangentOut;
                            }
                        }
                    }
                }
            }
            Handles.color = oldColor;

            if (GUI.changed)
                EditorUtility.SetDirty(spline);
        }
    }
}