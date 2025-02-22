using System.Collections;
using System.Collections.Generic;
using Supercent.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Supercent.UI
{
    public class CurrencyEffector : MonoBehaviour
    {
        struct SpreadInfo
        {
            public CurrencyParticle Partial;
            public Vector2          TargetPoint;
            public float            SpraedTimeLength;
            public float            AbsorbDist;
        }

        public delegate void OnPlayAbsorb(float minAbsorbTime, float maxAbsorbTime);



        [Header("Partial Settings")]
        [SerializeField] int                    _particleCount          = 100;  // 입자 최초 생성 개수
        [SerializeField] bool                   _autoGenerate           = true; // 입자가 부족할 경우 자동 생성 여부
        [SerializeField] int                    _autoGenerateCount      = 50;   // 자동 생성 시 입자 생성 개수
        [SerializeField] int                    _autoGenerateMaxCount   = 300;  // 자동 생성 시 입자 생성 한계값
        [SerializeField] CurrencyParticle       _particleOrigin         = null;
        [SerializeField] List<CurrencyParticle> _particleList           = null;

        [Header("Spread Settings")]
        [SerializeField] float          _spread_MinDistance     = 30.0f;    // 입자가 퍼지는 최소 거리
        [SerializeField] float          _spread_MaxDistance     = 130.0f;   // 입자가 퍼지는 최대 거리
        [SerializeField] float          _spread_MinTimeLength   = 0.2f;     // 입자가 최소 거리까지 퍼지는 시간
        [SerializeField] float          _spread_MaxTimeLength   = 0.45f;    // 입자가 최대 거리까지 퍼지는 시간
        [SerializeField] float          _spread_StartScale      = 0.7f;     // 입자 퍼짐 연출 시작 시 크기
        [SerializeField] float          _spread_EndScale        = 0.7f;     // 입자 퍼짐 연출 종료 시 크기
        [SerializeField] AnimationCurve _spread_MoveCurve       = null;
        [SerializeField] AnimationCurve _spread_ScaleCurve      = null;
        [SerializeField] RectTransform  _spread_CenterPoint     = null;

        [Header("Absorb Settings")]
        [SerializeField] float          _absorb_MinTimeLength   = 0.3f;     // 입자 흡수 시 최소 거리에서 흡수되는 시간
        [SerializeField] float          _absorb_MaxTimeLength   = 0.7f;     // 입자 흡수 시 최대 거리에서 흡수되는 시간
        [SerializeField] float          _absorb_EndScale        = 1.0f;     // 입자 흡수 연출 종료 시 입자의 크기
        [SerializeField] AnimationCurve _absorb_MoveCurve       = null;
        [SerializeField] AnimationCurve _absorb_ScaleCurve      = null;
        [SerializeField] RectTransform  _absorb_CenterPoint     = null;

        int _lastUsedPartialIndex = 0;

        UnityEvent _onAbsorbPartial = new UnityEvent();



        public UnityEvent OnAbsorbParticle => _onAbsorbPartial;



        public void Init()
        {
            if (null == _particleOrigin)
                return;

            if (null != _particleList)  _particleList = _particleList.FindAll(fx => null != fx);
            else                        _particleList = new List<CurrencyParticle>(_particleCount);

            var generateCount = Mathf.Max(0, _particleCount - _particleList.Count);
            if (generateCount <= 0)
                return;

            GeneratePartial(generateCount);
        }
        void GeneratePartial(int count)
        {
            while (0 < count)
            {
                var inst = Instantiate(_particleOrigin, transform);
                inst.Init();

                _particleList.Add(inst);

                --count;
            }
        }



        public void HideAllParticle()
        {
            StopAllCoroutines();

            for (int n = 0, cnt = _particleList.Count; n < cnt; ++n)
            {
                var partial = _particleList[n];
                if (null == partial)
                {
                    Debug.LogWarning($"[CurrencyEffector - HideAllParticle] {name}'s ParticleList[{n}] is null !");
                    continue;
                }

                partial.Init();
            }
        }



        /// <summary>
        /// 동일한 캔버스 위에 존재하는 UI의 월드 좌표를 기준으로 연출이 시작되도록 지정
        /// </summary>
        public void SetSpreadPoint_FromUI(Vector3 worldPosition)
        {
            if (null == _spread_CenterPoint)
                return;

            _spread_CenterPoint.position            = worldPosition;
            var anchoredPosition                    = _spread_CenterPoint.anchoredPosition3D;
            anchoredPosition.z                      = 0.0f;
            _spread_CenterPoint.anchoredPosition3D  = anchoredPosition;
        }

        /// <summary>
        /// 동일한 캔버스 위에 존재하는 UI의 월드 좌표를 기준으로 연출이 종료되도록 지정
        /// </summary>
        public void SetAbsorbPoint_FromUI(Vector3 worldPosition)
        {
            if (null == _absorb_CenterPoint)
                return;

            _absorb_CenterPoint.position            = worldPosition;
            var anchoredPosition                    = _absorb_CenterPoint.anchoredPosition3D;
            anchoredPosition.z                      = 0.0f;
            _absorb_CenterPoint.anchoredPosition3D  = anchoredPosition;
        }



        public void Play(int count, OnPlayAbsorb onPlayAbsorb, Transform spreadPoint = null)
        {
            // Icon Sprite 및 Size를 변경하지 않고 생성된 그대로 사용
            Play(null, Vector2.zero, count, onPlayAbsorb, spreadPoint);   
        }
        public void Play(Sprite iconSprite, Vector2 iconSize, int count, OnPlayAbsorb onPlayAbsorb, Transform spreadPoint = null)
        {
            if (count <= 0)
                return;

            if (null != spreadPoint)
                SetSpreadPoint_FromUI(spreadPoint.position);

            StartCoroutine(CoroutinePlay(iconSprite, iconSize, count, onPlayAbsorb));
        }

        IEnumerator CoroutinePlay(Sprite iconSprite, Vector2 iconSize, int count, OnPlayAbsorb onPlayAbsorb)
        {
            Calc_PlayInfos(iconSprite, iconSize, count, out var spreadInfos, 
                                                        out var maxSpreadTime, 
                                                        out var minAbsorbDist, 
                                                        out var maxAbsorbDist);

            for (int n = 0, cnt = spreadInfos.Count; n < cnt; ++n)
                StartCoroutine(CoroutineSpread(spreadInfos[n]));

            if (null == spreadInfos || spreadInfos.Count <= 0)
            {
                if (onPlayAbsorb != null)
                    onPlayAbsorb(0.0f, 0.0f);
                yield break;
            }

            var secDone = Time.time + maxSpreadTime;
            while (Time.time < secDone)
                yield return null;

            var minAbsorbTime = float.MaxValue;
            var maxAbsorbTime = float.MinValue;
            for (int n = 0, cnt = spreadInfos.Count; n < cnt; ++n)
            {
                var info    = spreadInfos[n];
                var partial = info.Partial;
                var absorbTime = MathUtil.Lerp_Percent_Between_A_and_B(minAbsorbDist, maxAbsorbDist,
                                                                       info.AbsorbDist,
                                                                       _absorb_MinTimeLength, _absorb_MaxTimeLength);

                if (absorbTime < minAbsorbTime) minAbsorbTime = absorbTime;
                if (maxAbsorbTime < absorbTime) maxAbsorbTime = absorbTime;

                StartCoroutine(CoroutineAbsorb(partial, absorbTime));
            }

            if (Mathf.Approximately(_absorb_MinTimeLength, float.MaxValue)) minAbsorbTime = _absorb_MinTimeLength;
            if (Mathf.Approximately(_absorb_MaxTimeLength, float.MinValue)) maxAbsorbTime = _absorb_MaxTimeLength;

            if (onPlayAbsorb != null)
                onPlayAbsorb(minAbsorbTime, maxAbsorbTime);
        }

        void Calc_PlayInfos(Sprite iconSprite, Vector2 iconSize, int count, out List<SpreadInfo> finalSpreadInfos, out float finalMaxSpreadTime, out float finalMinAbsorbDist, out float finalMaxAbsorbDist)
        {
            var maxSpreadTime       = float.MinValue;
            var minAbsorbDist       = float.MaxValue;
            var maxAbsorbDist       = float.MinValue;
            var spreadInfos         = new List<SpreadInfo>(count);
            var centerPoint         = _spread_CenterPoint.anchoredPosition;
            var absorbPoint         = _absorb_CenterPoint.anchoredPosition;
            var totalRadian         = 0.0f;
            var additionalMinRadian = Mathf.PI * 0.100f;
            var additionalMaxRadian = Mathf.PI * 0.225f;
            var partialCount        = _particleList.Count;
            var index               = _lastUsedPartialIndex;
            var checkCount          = partialCount;
            var remainCount         = count;

            Calc_Infos();

            finalSpreadInfos        = spreadInfos;
            finalMaxSpreadTime      = maxSpreadTime;
            finalMinAbsorbDist      = minAbsorbDist;
            finalMaxAbsorbDist      = maxAbsorbDist;
            _lastUsedPartialIndex   = index;

            if (remainCount <= 0)                       return;
            if (!_autoGenerate)                         return;
            if (_autoGenerateMaxCount <= partialCount)  return;

            var generateCount = Mathf.CeilToInt(remainCount / (float)_autoGenerateCount) * _autoGenerateCount;
            if (_autoGenerateMaxCount < partialCount + generateCount)
                generateCount = _autoGenerateMaxCount - partialCount;

            GeneratePartial(generateCount);

            partialCount    = _particleList.Count;
            index           = partialCount - generateCount;
            checkCount      = generateCount;

            Calc_Infos();

            finalSpreadInfos        = spreadInfos;
            finalMaxSpreadTime      = maxSpreadTime;
            finalMinAbsorbDist      = minAbsorbDist;
            finalMaxAbsorbDist      = maxAbsorbDist;
            _lastUsedPartialIndex   = index;



            void Calc_Infos()
            {
                while (0 < checkCount && 0 < remainCount)
                {
                    ++index;
                    --checkCount;
                    if (partialCount <= index)
                        index = 0;

                    var partial = _particleList[index];
                    if (null == partial)
                        continue;

                    if (partial.IsUsed)
                        continue;

                    totalRadian += UnityEngine.Random.Range(additionalMinRadian, additionalMaxRadian);

                    var r           = UnityEngine.Random.Range(_spread_MinDistance, _spread_MaxDistance);
                    var targetPoint = new Vector2(centerPoint.x + Mathf.Cos(totalRadian) * r,
                                                  centerPoint.y + Mathf.Sin(totalRadian) * r);
                    var absorbDist  = Vector2.Distance(targetPoint, absorbPoint);
                    var spreadTime  = MathUtil.Lerp_Percent_Between_A_and_B(_spread_MinDistance, _spread_MaxDistance, 
                                                                            r, 
                                                                            _spread_MinTimeLength, _spread_MaxTimeLength);

                    if (maxSpreadTime < spreadTime) maxSpreadTime = spreadTime;
                    if (absorbDist < minAbsorbDist) minAbsorbDist = absorbDist;
                    if (maxAbsorbDist < absorbDist) maxAbsorbDist = absorbDist;

                    if (null != iconSprite)
                    {
                        partial.IconSprite  = iconSprite;
                        partial.SizeDelta   = iconSize;
                    }

                    partial.AnchoredPosition    = centerPoint;
                    partial.LocalScale          = Vector3.zero;
                    partial.Alpha               = 0.0f;
                    partial.Use();

                    spreadInfos.Add(new SpreadInfo
                    {
                        Partial             = partial,
                        TargetPoint         = targetPoint,
                        SpraedTimeLength    = spreadTime,
                        AbsorbDist          = absorbDist,
                    });

                    --remainCount;
                }
            }
        }

        IEnumerator CoroutineSpread(SpreadInfo info)
        {
            var partial     = info.Partial;
            var startPoint  = partial.AnchoredPosition;
            var endPoint    = info.TargetPoint;
            var elapsed     = 0.0f;
            var timeLength  = info.SpraedTimeLength;

            partial.LocalScale  = _spread_StartScale * Vector3.one;
            partial.Alpha       = 1.0f;
            
            while (elapsed < timeLength)
            {
                yield return null;
                elapsed += Time.deltaTime;

                if (!partial.IsUsed)
                    yield break;

                var factor      = elapsed / timeLength;
                var moveFactor  = _spread_MoveCurve .Evaluate(factor);
                var scaleFactor = _spread_ScaleCurve.Evaluate(factor);

                partial.AnchoredPosition    = Vector2.Lerp(startPoint, endPoint, moveFactor);
                partial.LocalScale          = Mathf.Lerp(_spread_StartScale, _spread_EndScale, scaleFactor) * Vector3.one;
            }
        }

        IEnumerator CoroutineAbsorb(CurrencyParticle partial, float timeLength)
        {
            var elapsed     = 0.0f;
            var startPoint  = partial.AnchoredPosition;
            var endPoint    = _absorb_CenterPoint.anchoredPosition;
            while (elapsed < timeLength)
            {
                yield return null;
                elapsed += Time.deltaTime;

                if (!partial.IsUsed)
                    yield break;

                var factor      = elapsed / timeLength;
                var moveFactor  = _absorb_MoveCurve .Evaluate(factor);
                var scaleFactor = _absorb_ScaleCurve.Evaluate(factor);

                partial.AnchoredPosition    = Vector2.Lerp(startPoint, endPoint, moveFactor);
                partial.LocalScale          = Mathf.Lerp(_spread_EndScale, _absorb_EndScale, scaleFactor) * Vector3.one;
            }

            partial.Init();
            if (_onAbsorbPartial != null)
                _onAbsorbPartial.Invoke();
        }
    }
}
