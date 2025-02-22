using UnityEngine;

namespace Supercent.Util
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraScaler : MonoBehaviour
    {
        const float Deg2Rad_Half = Mathf.Deg2Rad / 2f;
        const float Rad2Deg_Double = Mathf.Rad2Deg * 2f;

        [SerializeField] bool orthographic = false;
        [SerializeField] float aspectVertical = 9f / 16f;
        [SerializeField] float viewSizeVertical = 8;
        [SerializeField] float aspectHorizontal = 16f / 9f;
        [SerializeField] float viewSizeHorizontal = 5;


        public bool Orthographic
        {
            get => orthographic;
            set
            {
                if (orthographic == value)
                    return;
                orthographic = value;
                stampAspect = -1f;
            }
        }

        public float AspectVertial
        {
            get => aspectVertical;
            set
            {
                if (aspectVertical == value)
                    return;
                aspectVertical = value;
                stampAspect = -1f;
            }
        }
        public float ViewSizeVertical
        {
            get => viewSizeVertical;
            set
            {
                if (viewSizeVertical == value)
                    return;
                viewSizeVertical = value;
                stampAspect = -1f;
            }
        }
        public float AspectHorizontal
        {
            get => aspectHorizontal;
            set
            {
                if (aspectHorizontal == value)
                    return;
                aspectHorizontal = value;
                stampAspect = -1f;
            }
        }
        public float ViewSizeHorizontal
        {
            get => viewSizeHorizontal;
            set
            {
                if (viewSizeHorizontal == value)
                    return;
                viewSizeHorizontal = value;
                stampAspect = -1f;
            }
        }

        Camera cam;
        float stampAspect = -1f;



        void Awake()
        {
            cam = GetComponent<Camera>();
        }
        void OnEnable()
        {
            stampAspect = -1f;
        }
        void LateUpdate()
        {
            var curAspect = Screen.width / (float)Screen.height;
            float size = 0f;

            if (aspectVertical < curAspect)
            {
                curAspect = curAspect < aspectHorizontal
                          ? aspectVertical
                          : aspectHorizontal;
                if (curAspect == stampAspect) return;

                stampAspect = curAspect;
                size = curAspect < aspectHorizontal
                     ? viewSizeVertical
                     : viewSizeHorizontal;
            }
            else
            {
                curAspect = aspectVertical;
                if (curAspect == stampAspect) return;

                stampAspect = curAspect;
                size = aspectVertical <= curAspect ? viewSizeVertical
                     : orthographic ? aspectVertical / stampAspect * viewSizeVertical
                     : Mathf.Atan(aspectVertical / stampAspect * Mathf.Tan(viewSizeVertical * Deg2Rad_Half)) * Rad2Deg_Double;
            }

            if (orthographic)
                cam.orthographicSize = size;
            else
                cam.fieldOfView = size;
        }



#if UNITY_EDITOR
        void Reset()
        {
            cam = GetComponent<Camera>();
            orthographic = cam.orthographic;
            viewSizeVertical = orthographic
                             ? cam.orthographicSize
                             : cam.fieldOfView;
            viewSizeHorizontal = viewSizeVertical;
        }

        void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            cam = GetComponent<Camera>();
            cam.orthographic = orthographic;
            if (orthographic)
                cam.orthographicSize = viewSizeVertical;
            else
                cam.fieldOfView = viewSizeVertical;
            UnityEditor.EditorUtility.SetDirty(cam);
        }
#endif// UNITY_EDITOR
    }
}
