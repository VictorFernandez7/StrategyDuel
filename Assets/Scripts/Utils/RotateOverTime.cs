using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public class RotateOverTime : MonoBehaviour
    {
        [Title("Parameters")]
        [SerializeField] private bool _rotate;
        [SerializeField] private Directions _direction;
        [SerializeField] private float _speed;

        private enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }

        public bool rotate
        {
            get => _rotate;
            set { _rotate = value; }
        }

        private void Update()
        {
            if (_rotate)
                transform.Rotate(GetDirection() * (_speed * Time.deltaTime));
        }

        private Vector3 GetDirection()
        {
            Vector3 desiredDirection = Vector3.zero;

            switch (_direction)
            {
                case Directions.Up:
                    desiredDirection = transform.up;
                    break;
                case Directions.Down:
                    desiredDirection = -transform.up;
                    break;
                case Directions.Left:
                    desiredDirection = -transform.right;
                    break;
                case Directions.Right:
                    desiredDirection = transform.right;
                    break;
            }

            return desiredDirection;
        }
    }
}