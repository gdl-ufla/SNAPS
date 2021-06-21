using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/SystemDataBase")]
public class SystemDataBase : ScriptableObject
{
    #region Serialized fields
    public SystemData[] SystemData;
    #endregion

    #region Non-serialized fields
    
    #endregion

    #region Unity events
    #endregion

    #region Public methods

    public SystemData[] GetData()
    {
        return SystemData;
    }

    #endregion

    #region Private methods
    #endregion    
}
