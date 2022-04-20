using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        public static CameraFollow Instance = null;
        public bool UseFixedUpdate = false;
        public Transform Target = null;
        public Vector3 Offset = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 Speed = Vector3.one;
        [SerializeField] private bool freezeX = false;
        [SerializeField] private bool freezeY = false;
        [SerializeField] private bool freezeZ = false;
        private float zoom = 0;
        private Transform _transform;
        private float initialTargetHeight = 0;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
            {
                DestroyImmediate(gameObject);
                return;
            }
            _transform = transform;

        }
        private void Start()
        {
            initialTargetHeight = Target.position.y;
        }

        private void FixedUpdate()
        {
            if (UseFixedUpdate && Target)
                Follow();
        }

        private void LateUpdate()
        {
            if (!UseFixedUpdate && Target)
                Follow();
        }

        private void Follow()
        {
            Vector3 pos = Target.position + Offset;
            float deltaTime = (UseFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);
            if (freezeX)
                pos.x = _transform.position.x;
            if (freezeY)
                pos.y = _transform.position.y;
            if (freezeZ)
                pos.z = _transform.position.z;
            Vector3 displacement = -3 * _transform.position;
            displacement += Vector3.Lerp(_transform.position, new Vector3(pos.x, _transform.position.y, _transform.position.z), Speed.x * deltaTime);
            displacement += Vector3.Lerp(_transform.position, new Vector3(_transform.position.x, _transform.position.y, pos.z), Speed.z * deltaTime);
            displacement += Vector3.Lerp(_transform.position, new Vector3(_transform.position.x, pos.y, _transform.position.z), Speed.y * deltaTime);
            _transform.position += displacement;
            _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(rotation), deltaTime * 7);
            zoom = Target.position.y - initialTargetHeight;
            if (zoom > 0)
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoom * Vector3.back, Time.deltaTime * 5);
        }

        public void TakePosition()
        {
            Vector3 pos = Target.position + Offset;
            if (freezeX)
                pos.x = transform.position.x;
            if (freezeY)
                pos.y = transform.position.y;
            if (freezeZ)
                pos.z = transform.position.z;
            transform.position = pos;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

}
