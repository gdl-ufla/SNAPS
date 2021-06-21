using UnityEngine;

namespace YoukaiFox.Tools
{
    public class RollingCredits : MonoBehaviour
    {
        #region Serialized fields
        #pragma warning disable 0649 

        [SerializeField] private Transform _rollingObject;

        [SerializeField] private float _rollingSpeed;

        #pragma warning restore 0649 
        #endregion

        #region Non-serialized fields
        #endregion

        #region Unity events

        private void Update() 
        {
            _rollingObject.localPosition += Vector3.up * Time.deltaTime * _rollingSpeed;
        }

        #endregion

        #region Public methods
        #endregion

        #region Private methods
        #endregion    
    }
}
