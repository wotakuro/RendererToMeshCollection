using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RendererCollection
{
    public class RendererData : System.IDisposable
    {
        public enum RendererType : int
        {
            Mesh,
            SkinnedMesh,
            Particle,
        };
        public Mesh mesh;
        public Material[] materials;
        public Transform meshTransform;
        public RendererType type;

        private MeshRenderer meshRenderer;
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private ParticleSystemRenderer particleSystemRenderer;

        private RendererData(MeshRenderer r, MeshFilter filter)
        {
            this.type = RendererType.Mesh;
            this.mesh = filter.sharedMesh;
            this.materials = r.sharedMaterials;
            this.meshRenderer = r;
        }
        private RendererData(SkinnedMeshRenderer skin)
        {
            this.type = RendererType.SkinnedMesh;
            this.skinnedMeshRenderer = skin;
            this.mesh = new Mesh();
            this.materials = skin.sharedMaterials;
            this.meshTransform = skin.transform;
        }
        private RendererData(ParticleSystemRenderer particle)
        {
            this.type = RendererType.Particle;
            this.mesh = new Mesh();
            this.materials = particle.sharedMaterials;
            this.meshTransform = particle.transform;
        }
        public void Dispose()
        {
            if (type != RendererType.Mesh && this.mesh != null)
            {
                mesh.Clear();
            }
            this.mesh = null;
            this.materials = null;
            this.meshTransform = null;
        }

        public void Render(DrawImplement drawImp)
        {
            UpdateBake();
            Matrix4x4 matrix = this.meshTransform.localToWorldMatrix;
            for (int i = 0; i < this.materials.Length; ++i) {
                drawImp(this.mesh, this.materials[i], i, ref matrix);
             }
        }

        private void UpdateBake()
        {
            switch(type)
            {
                case RendererType.Particle:
                    this.particleSystemRenderer.BakeMesh(mesh, false);
                    break;
                case RendererType.SkinnedMesh:
                    this.skinnedMeshRenderer.BakeMesh(mesh);
                    break;
            }
        }

        public static RendererData Create(Renderer render)
        {
            RendererData renderData = null; 
            var renderType = render.GetType();
            if (renderType == typeof(MeshRenderer))
            {
                var filter = render.GetComponent<MeshFilter>();
                if (filter != null)
                {
                    renderData = new RendererData(render as MeshRenderer, filter);
                }
            }
            else if (renderType == typeof(SkinnedMeshRenderer))
            {
                renderData = new RendererData(render as SkinnedMeshRenderer);
            }
            else if (renderType == typeof(ParticleSystemRenderer))
            {
                renderData = new RendererData(render as ParticleSystemRenderer);
            }
            return renderData;
        }

    }

}