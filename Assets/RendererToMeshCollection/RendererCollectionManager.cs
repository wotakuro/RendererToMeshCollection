
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RendererCollection
{
    public delegate void DrawImplement(Mesh mesh, Material material, int subMeshIdx, ref Matrix4x4 matrix);

    public class RendererCollectionManager
    {
        public class RendererRoot : System.IDisposable
        {
            public List<RendererData> rendererDatas = new List<RendererData>();
            public bool isEnabled = false;

            public RendererRoot(GameObject rootGameObject)
            {
                var renderers = rootGameObject.GetComponentsInChildren<Renderer>(true);
                foreach( var render in renderers)
                {
                    RendererData renderData = RendererData.Create(render) ;

                    if(renderData != null)
                    {
                        rendererDatas.Add(renderData);
                    }
                }
            }

            public void Dispose()
            {
                foreach (var data in rendererDatas)
                {
                    data.Dispose();
                }
            }
        }

        private static RendererCollectionManager _instance;
        public static RendererCollectionManager Instance
        {
            get
            {
                if( _instance == null)
                {
                    _instance = new RendererCollectionManager();
                }
                return _instance;
            }
        }

        private Dictionary<GameObject, RendererRoot> m_allRegisterObjects = new Dictionary<GameObject, RendererRoot>();


        private RendererCollectionManager()
        {
        }
        
        public void OnAwakeEvent(GameObject rootObject)
        {
            var rootData = new RendererRoot(rootObject);
            m_allRegisterObjects.Add(rootObject, rootData);
        }

        public void OnEnableEvent(GameObject rootGameObject)
        {
            RendererRoot rootData = null;
            if( m_allRegisterObjects.TryGetValue(rootGameObject, out rootData))
            {
                rootData.isEnabled = true;
            }
            else
            {
                Debug.LogError("[OnEnableEvent]Not Register gameObject");
            }
        }
        public void OnDisableEvent(GameObject rootGameObject)
        {
            RendererRoot rootData = null;
            if (m_allRegisterObjects.TryGetValue(rootGameObject, out rootData))
            {
                rootData.isEnabled = false;
            }
            else
            {
                Debug.LogError("[OnDisableEvent]Not Register gameObject");
            }
        }
        public void OnDestroyEvent(GameObject rootGameObject)
        {
            RendererRoot rootData = null;
            if (m_allRegisterObjects.TryGetValue(rootGameObject, out rootData))
            {
                rootData.Dispose();
                m_allRegisterObjects.Remove(rootGameObject);
            }
            else
            {
                Debug.LogError("[OnDestroyEvent]Not Register gameObject");
            }
        }
        

        public void RenderObjects(DrawImplement drawImp)
        {
            if( drawImp == null)
            {
                return;
            }
            foreach( var kvs in m_allRegisterObjects)
            {
                if( kvs.Value.isEnabled ){
                    continue;
                }
                var renderDatas = kvs.Value.rendererDatas;
                foreach( var renderData in renderDatas)
                {
                    renderData.Render(drawImp);
                }
            }

        }

    }
}
