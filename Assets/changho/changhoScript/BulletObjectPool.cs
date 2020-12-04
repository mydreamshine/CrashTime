
using System.Collections.Generic;
using UnityEngine;

namespace changhoScript
{

    public class BulletObjectPool : MonoBehaviour
    {
        
        public GameObject prefab;
        public int initialSize;
        public GameObject prefabPos;


        private readonly Stack<GameObject> instances = new Stack<GameObject>();

        private void Start()
        {
            for (var i = 0; i < initialSize; i++)
            {
                var obj = CreateInstance(); 
                obj.SetActive(false); 
                instances.Push(obj); 
            }
        }

       
        public GameObject GetObject()  
        {
            var obj = instances.Count > 0 ? instances.Pop() : CreateInstance();

            

            obj.SetActive(true);
            obj.transform.position = prefabPos.transform.position;
            obj.transform.rotation = prefabPos.transform.rotation;
            return obj;
        }

       
        public void ReturnObject(GameObject obj) 
        {
            var pooledObject = obj.GetComponent<PooledObject>();
            
            

            obj.SetActive(false);
            if (!instances.Contains(obj)) 
            {
                instances.Push(obj); 
            }
        }

        public void Reset() 
        { 
            var objectsToReturn = new List<GameObject>();
            foreach (var instance in transform.GetComponentsInChildren<PooledObject>())
            { 
                if (instance.gameObject.activeSelf) 
                {
                    objectsToReturn.Add(instance.gameObject);  
                }
            }
            foreach (var instance in objectsToReturn)   
            {
                ReturnObject(instance); 
            }
        }


        private GameObject CreateInstance() 
        {
            var obj = Instantiate(prefab);   
            var pooledObject = obj.AddComponent<PooledObject>();
            pooledObject.pool = this;
           
         
            return obj;
        }
    }

    public class PooledObject : MonoBehaviour
    {
        public BulletObjectPool pool;
    }


}