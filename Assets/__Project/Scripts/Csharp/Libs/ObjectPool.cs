using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YoukaiFox.Optimization
{
    public class GenericObjectPool
    {
        private GameObject[] objects;

        public void Create(GameObject prefab, int poolLimit)
        {
            objects = new GameObject[poolLimit];

            for (int i = 0; i < poolLimit; i++)
            {
                objects[i] = GameObject.Instantiate(prefab, null, false);
            }

            DisableAll();
        }
        public GameObject GetPooledObject()
        {
            foreach (GameObject obj in objects)
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }

            return null;
        }
        private void DisableAll()
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }
        }
        public void ClearPool()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject.Destroy(objects[i]);
            }
        }
    }
}
