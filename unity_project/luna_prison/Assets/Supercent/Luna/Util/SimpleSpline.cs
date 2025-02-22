using System;
using System.Collections.Generic;
using UnityEngine;

namespace Supercent.Util
{
    public class SimpleSpline : MonoBehaviour
    {
        [SerializeField] List<Point> points = new List<Point>();
        [SerializeField] List<float> lengths = new List<float>();
        public float Length => lengths.Count < 1 ? 0f : lengths[lengths.Count - 1];


        public Vector3 GetPosition(float t) => transform.TransformVector(GetLocalPosition(t)) + transform.position;
        public Vector3 GetLocalPosition(float t)
        {
            var count = points.Count;
            if (count < 1) return Vector3.zero;
            if (count < 2) return points[0].CalculateCubicBezierPoint(1f);

            if (t < 0f) t = 0f;
            else if (1f < t) t = 1f;

            var resultLen = Length * t;
            int low = 0;
            int mid = 0;
            int high = lengths.Count - 1;
            while (low <= high)
            {
                mid = (low + high) / 2;
                if (lengths[mid] < resultLen)
                    low = mid + 1;
                else
                    high = mid - 1;
            }

            int curIndex = Mathf.Min(count - 1, mid + 1);
            var curLen = lengths[curIndex];

            var point = points[curIndex];
            var prevLen = curLen - point.Length;
            return point.CalculateCubicBezierPoint((resultLen - prevLen) / (curLen - prevLen));
        }


        public void GetPositionAndForward(float t, out Vector3 position, out Vector3 forward)
        {
            GetLocalPositionAndForward(t, out position, out forward);
            position = transform.TransformVector(position) + transform.position;
            forward = transform.TransformDirection(forward);
        }
        public void GetLocalPositionAndForward(float t, out Vector3 position, out Vector3 forward)
        {
            var count = points.Count;
            if (count < 1)
            {
                position = Vector3.zero;
                forward = Vector3.zero;
                return;
            }
            if (count < 2)
            {
                points[0].CalculateCubicBezierPointAndForward(1f, out position, out forward);
                return;
            }

            if (t < 0f) t = 0f;
            else if (1f < t) t = 1f;

            var resultLen = Length * t;
            var curLen = 0f;
            for (int i = 1; i < count; ++i)
            {
                var prevLen = curLen;
                var p = points[i];
                curLen += p.Length;
                if (curLen < resultLen)
                    continue;

                p.CalculateCubicBezierPointAndForward((resultLen - prevLen) / (curLen - prevLen), out position, out forward);
                return;
            }
            points[count - 1].CalculateCubicBezierPointAndForward(1f, out position, out forward);
        }


        void Dirty()
        {
            var length = 0f;
            lengths.Clear();
            var cnt = points.Count;
            if (0 < cnt)
            {
                points[0].tangentIn = Vector3.zero;
                points[cnt - 1].tangentOut = Vector3.zero;
            }

            Point curPoint = null;
            for (int i = 0; i < cnt; ++i)
            {
                var prevPoint = curPoint;
                curPoint = points[i];
                curPoint.Dirty(prevPoint);
                length += curPoint.Length;
                lengths.Add(length);
            }
        }



#if UNITY_EDITOR
        [Header("#Edit")]
        [SerializeField][Range(0f, 1f)] float edit_ratio = 0f;

        void OnValidate()
        {
            if (points.Count < 1)
                points.Add(new Point());

            Dirty();
        }

        void OnDrawGizmos()
        {
            var isSelected = UnityEditor.Selection.activeGameObject == gameObject;

            var prevMat = Gizmos.matrix;
            var prevColor = Gizmos.color;
            {
                if (isSelected)
                {
                    GetPositionAndForward(edit_ratio, out var point, out var forward);
                    var rotation = forward == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(forward);
                    Gizmos.matrix = Matrix4x4.TRS(point, rotation, Vector3.one);
                    Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
                    Gizmos.DrawCube(Vector3.zero, new Vector3(0.1f, 0.1f, 0.3f));
                }
                else
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.color = Color.white;
                    for (int i = 0, cnt = points.Count; i < cnt; ++i)
                        points[i].Edit_DrawLines();
                }
            }
            Gizmos.color = prevColor;
            Gizmos.matrix = prevMat;
        }
#endif// UNITY_EDITOR


        [Serializable]
        public class Point
        {
            public const int NormalSegments = 30;

            [HideInInspector]
            [SerializeField] Point prev = null;
            [SerializeField] public Vector3 position = Vector3.zero;
            [SerializeField] public Vector3 tangentIn = Vector3.zero;
            [SerializeField] public Vector3 tangentOut = Vector3.zero;

            public bool IsHead => prev == null;
            [SerializeField, HideInInspector] float _length = 0;
            public float Length => _length;
            [SerializeField, HideInInspector] Vector3 p1_3 = Vector3.zero;
            [SerializeField, HideInInspector] Vector3 p2_3 = Vector3.zero;
            [SerializeField, HideInInspector] Vector3 p1_p0_3 = Vector3.zero;
            [SerializeField, HideInInspector] Vector3 p2_p1_6 = Vector3.zero;
            [SerializeField, HideInInspector] Vector3 p3_p2_3 = Vector3.zero;


            public void Dirty(Point point)
            {
                prev = point;
                if (IsHead)
                {
                    _length = 0;
                    p1_3 = Vector3.zero;
                    p2_3 = Vector3.zero;
                    p1_p0_3 = Vector3.zero;
                    p2_p1_6 = Vector3.zero;
                    p3_p2_3 = Vector3.zero;
                }
                else
                {
                    var p1 = prev.position + prev.tangentOut;
                    var p2 = position + tangentIn;  
                    p1_3 = 3f * p1;
                    p2_3 = 3f * p2;
                    p1_p0_3 = 3 * prev.tangentOut;
                    p2_p1_6 = 6f * (p2 - p1);
                    p3_p2_3 = 3 * -tangentIn;
                }
                _length = CalculateCubicBezierLength(NormalSegments);
            }

            float CalculateCubicBezierLength(int segments)
            {
                if (IsHead)
                    return 0f;

                float result = 0f;
                var prevPoint = prev.position;
                for (int i = 1; i <= segments; ++i)
                {
                    var t = i / (float)segments;
                    var curPoint = CalculateCubicBezierPoint(t);

                    result += Vector3.Distance(prevPoint, curPoint);
                    prevPoint = curPoint;
                }
                return result;
            }


            public Vector3 CalculateCubicBezierPoint(float t)
            {
                if (IsHead)
                    return position;

                var u = 1f - t;
                var uu = u * u;
                var uuu = uu * u;
                var tt = t * t;
                var ttt = tt * t;

                return uuu * prev.position
                     + uu * t * p1_3
                     + u * tt * p2_3
                     + ttt * position;
            }
            public void CalculateCubicBezierPointAndForward(float t, out Vector3 position, out Vector3 forward)
            {
                if (IsHead)
                {
                    position = this.position;
                    forward = tangentOut.normalized;
                    return;
                }

                var u = 1f - t;
                var uu = u * u;
                var uuu = uu * u;
                var tt = t * t;
                var ttt = tt * t;

                position = uuu * prev.position
                      + uu * t * p1_3
                      + tt * u * p2_3
                      + ttt * this.position;

                forward = uu * p1_p0_3
                        + u * t * p2_p1_6
                        + tt * p3_p2_3;
                forward.Normalize();
            }



#if UNITY_EDITOR
            public void Edit_DrawLines()
            {
                if (IsHead)
                    return;

                var curPoint = CalculateCubicBezierPoint(0f);
                for (int index = 1; index <= NormalSegments; ++index)
                {
                    var prvPoint = curPoint;
                    curPoint = CalculateCubicBezierPoint(index / (float)NormalSegments);
                    Gizmos.DrawLine(prvPoint, curPoint);
                }
            }
#endif// UNITY_EDITOR
        }
    }
}