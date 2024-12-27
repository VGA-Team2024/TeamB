using UnityEngine;
using R3;
using R3.Triggers;

namespace TeamB.SkitSystem
{
    public class SkitEndMaker : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 1.0f;
        // Start is called before the first frame update
        void Start()
        {
            this.UpdateAsObservable().Subscribe(_ =>
            {
                transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
            }).AddTo(gameObject);
        }
    }
}
