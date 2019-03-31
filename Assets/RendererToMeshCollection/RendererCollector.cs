using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RendererCollection
{
    public class RendererCollector : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            RendererCollectionManager.Instance.OnAwakeEvent(gameObject);
        }

        private void OnEnable()
        {
            RendererCollectionManager.Instance.OnEnableEvent(gameObject);
        }
        private void OnDisable()
        {
            RendererCollectionManager.Instance.OnDisableEvent(gameObject);
        }

        // Update is called once per frame
        void OnDestroy()
        {
            RendererCollectionManager.Instance.OnDestroyEvent(gameObject);
        }
    }
}