using System;
using System.Collections.Generic;
using System.Linq;
using KPU.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Functions.DestroyObjectSolution.Scripts
{
    public class PartMesh
    {
        private List<Vector3> _vertices = new List<Vector3>();
        private List<Vector3> _normals = new List<Vector3>();
        private List<List<int>> _triangles = new List<List<int>>();
        private List<Vector2> _uvs = new List<Vector2>();

        public Vector3[] vertices;
        public Vector3[] normals;
        public int[][] triangles;
        public Vector2[] uv;
        
        // Object Pool로 관리
        private GameObject gameObject;
        
        public Bounds bounds;
        
        public Material[] materials;
        public string meshName;

        public PartMesh Initialize(MeshFilter originMeshFilter, MeshRenderer originMeshRenderer)
        {
            var originMesh = originMeshFilter.mesh;
            vertices = originMesh.vertices;
            normals = originMesh.normals;
            triangles = new int[originMesh.subMeshCount][];
            for (var i = 0; i < originMesh.subMeshCount; i++)
                triangles[i] = originMesh.GetTriangles(i);
            uv = originMesh.uv;

            originMesh.RecalculateBounds();
            bounds = originMesh.bounds;

            materials = originMeshRenderer.materials;
            meshName = originMesh.name;

            return this;
        }
        
        public PartMesh Initialize(SkinnedMeshRenderer originSkin, Transform originTransform)
        {
            var originMesh = originSkin.sharedMesh;
            CalculateAnimData(ref vertices,ref normals, originSkin, originTransform);
            triangles = new int[originMesh.subMeshCount][];
            for (var i = 0; i < originMesh.subMeshCount; i++)
                triangles[i] = originMesh.GetTriangles(i);
            uv = originMesh.uv;
            
            originMesh.RecalculateBounds();
            bounds = originMesh.bounds;
            
            materials = originSkin.materials;
            meshName = originMesh.name;

            return this;
        }

        private static void CalculateAnimData(
            ref Vector3[] destVertices, ref Vector3[] destNormals,
            SkinnedMeshRenderer originSkin, Transform originTransform)
        {
            var originMesh = originSkin.sharedMesh;
            destVertices = new Vector3[originMesh.vertexCount];
            destNormals = new Vector3[originMesh.vertexCount];
            
            
            var boneMatrices = new Matrix4x4[originSkin.bones.Length];

            for (var i = 0; i < boneMatrices.Length; i++)
            {
                boneMatrices[i] = originTransform.worldToLocalMatrix * originSkin.bones[i].localToWorldMatrix *
                                  originMesh.bindposes[i];
            }

            // System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            // sw.Start();
            
            var animMatrix = new Matrix4x4();
            for (var v = 0; v < originMesh.vertexCount; v++)
            {
                var boneWeight = originMesh.boneWeights[v];

                // Matrix 연산 비용이 너무 크다...
                for (var n = 0; n < 16; n++)
                {
                    animMatrix[n] = 
                        boneMatrices[boneWeight.boneIndex0][n] * boneWeight.weight0 
                        + boneMatrices[boneWeight.boneIndex1][n] * boneWeight.weight1
                        + boneMatrices[boneWeight.boneIndex2][n] * boneWeight.weight2
                        + boneMatrices[boneWeight.boneIndex3][n] * boneWeight.weight3;
                }

                destVertices[v] = animMatrix.MultiplyPoint3x4(originMesh.vertices[v]);
                destNormals[v] = animMatrix.MultiplyVector(originMesh.normals[v]);
            }
            
            // sw.Stop();
            // Debug.Log("Time taken to calculate AnimVertices: " + sw.ElapsedMilliseconds.ToString() + "ms");
        }

        public void AddTriangle(
            int subMesh,
            Vector3 vertex1, Vector3 vertex2, Vector3 vertex3,
            Vector3 normal1, Vector3 normal2, Vector3 normal3,
            Vector2 uv1, Vector2 uv2, Vector2 uv3,
            bool existUVs)
        {
            while (_triangles.Count - 1 < subMesh)
                _triangles.Add(new List<int>());

            // Add Vertices
            {
                _triangles[subMesh].Add(_vertices.Count);
                _vertices.Add(vertex1);
                _triangles[subMesh].Add(_vertices.Count);
                _vertices.Add(vertex2);
                _triangles[subMesh].Add(_vertices.Count);
                _vertices.Add(vertex3);
            }
            // Add Normals
            {
                _normals.Add(normal1);
                _normals.Add(normal2);
                _normals.Add(normal3);
            }
            // Add UVs
            if (existUVs)
            {
                _uvs.Add(uv1);
                _uvs.Add(uv2);
                _uvs.Add(uv3);
            }
            // Set Bound
            {
                bounds.min = Vector3.Min(bounds.min, vertex1);
                bounds.min = Vector3.Min(bounds.min, vertex2);
                bounds.min = Vector3.Min(bounds.min, vertex3);
                bounds.max = Vector3.Max(bounds.max, vertex1);
                bounds.max = Vector3.Max(bounds.max, vertex2);
                bounds.max = Vector3.Max(bounds.max, vertex3);
            }
        }

        public void FillArrays()
        {
            vertices = _vertices.ToArray();
            normals = _normals.ToArray();
            uv = _uvs.ToArray();
            triangles = new int[_triangles.Count][];
            for (var i = 0; i < _triangles.Count; i++)
                triangles[i] = _triangles[i].ToArray();
        }

        /// <summary>
        /// Mesh Destroy 구성에 따라 오브젝트를 생성.
        /// Mesh Destroy 구성: MeshRenderer, MeshFilter, etc.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="simulateFactor"></param>
        public void MakeGameObject(MeshDestroy original, float simulateFactor = 1.0f)
        {
            var originTransform = original.transform;
            
            gameObject = ObjectPoolManager.Instance.Spawn(
                "destroyed_object",
                originTransform.position, originTransform.rotation);
            gameObject.name = original.name + "(destructive)";
            gameObject.transform.localScale = originTransform.localScale;
            
            // Set Mesh
            var mesh = new Mesh
            {
                name = meshName,
                vertices = vertices,
                normals = normals,
                uv = uv
            };
            {
                for (var i = 0; i < triangles.Length; i++)
                {
                    mesh.SetTriangles(triangles[i], i, true);
                    mesh.subMeshCount = triangles.Length;
                }
                mesh.RecalculateBounds();
                bounds = mesh.bounds;

                var renderer = gameObject.GetComponent<MeshRenderer>();
                if (renderer == null) renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.materials = materials;
                foreach (var mat in renderer.materials)
                {
                    mat.SetFloat("_Mode", 3f);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    var opaqueColor = mat.color;
                    opaqueColor.a = 1.0f;
                    mat.color = opaqueColor;
                }
                
                var filter = gameObject.GetComponent<MeshFilter>();
                if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
                filter.mesh = mesh;
            }
            // Set collider
            {
                var collider = gameObject.GetComponent<MeshCollider>();
                if (collider == null) collider = gameObject.AddComponent<MeshCollider>();
                collider.convex = true;
                collider.sharedMesh = mesh;
            }
            // Set rigidbody
            {
                var rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }
            // Set Disappear
            {
                var meshDisappear = gameObject.GetComponent<MeshDisappear>();
                if (meshDisappear == null) gameObject.AddComponent<MeshDisappear>();
                meshDisappear.renderer = gameObject.GetComponent<MeshRenderer>();
                meshDisappear.rigidbody = gameObject.GetComponent<Rigidbody>();

                meshDisappear.rigidBodyFullActionScale = simulateFactor;
            }
        }

        public void AddForce(float scalar)
        {
            var rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.AddForceAtPosition(bounds.center * scalar, gameObject.transform.position);
        }
    }

    public class MeshDestroy : MonoBehaviour
    {
        [Range(1, 5)] public int maxSlicing = 3;

        [Range(1.0f, 2000.0f)] public float explodeForce = 1000.0f;

        private bool slicingSectionGenerated;
        private Vector3 slicingPolygonPivotVertex;
        private Vector2 slicingSectionPivotUV;
        private Plane slicingSection;

        public void DestroyMesh(Vector3 impulse, Vector3 impulsePoint, float simulateFactor = 1.0f)
        {
            var originParts = ParsingPartMeshes();
            var newParts = new List<PartMesh>(originParts);
            var subParts = new List<PartMesh>();
            for (var i = 0; i < maxSlicing; i++)
            {
                foreach (var subPart in newParts)
                {
                    // 랜덤한 방향의 평면(slicer) 생성
                    var partBounds = subPart.bounds;
                    // 평면(slicer)의 크기는 mesh보다 살짝 더 크게 해준다.
                    // Plane.Raycast()메소드에서
                    // mesh의 edge가  Plane의 가장자리에 위치할 때에는
                    // 교차판정이 안되기 때문.
                    partBounds.Expand(0.5f);
                    var planeNormal = Random.onUnitSphere;
                    var pointInPlane = new Vector3(
                        Random.Range(partBounds.min.x, partBounds.max.x),
                        Random.Range(partBounds.min.y, partBounds.max.y),
                        Random.Range(partBounds.min.z, partBounds.max.z));
                    var slicer = new Plane(planeNormal, pointInPlane);
                
                    // 평면(slicer)을 기준으로 두개의 Part(slicedMesh) 생성
                    // 잘려나간 Part(slicedMesh)는 볼록 다면체라고 가정한다.
                    // (볼록 다면체일 경우 한번의 Slicing으로 
                    //  잘려나가는 Part(slicedMesh)는 총 2개.)
                    // (※ 오목 다면체일 경우 한번의 Slicing으로
                    //  잘려나가는 Part(slicedMesh)의 개수를 가늠하기가 어렵다...)
                    var slicedPart1 = GenerateMesh(subPart, slicer, true);
                    var slicedPart2 = GenerateMesh(subPart, slicer, false);

                    if (slicedPart1 != null)
                    {
                        if (slicedPart1.vertices.Length > 0) 
                            subParts.Add(slicedPart1);
                    }

                    if (slicedPart2 != null)
                    {
                        if (slicedPart2.vertices.Length > 0)
                            subParts.Add(slicedPart2);
                    }
                }
                
                newParts = new List<PartMesh>(subParts);
                subParts.Clear();
            }

            foreach (var newPart in newParts)
            {
                newPart.MakeGameObject(this, simulateFactor);
                newPart.AddForce(explodeForce);
            }

            // Object Pool로 관리
            if(newParts.Count > 0) gameObject.SetActive(false);
        }

        private IEnumerable<PartMesh> ParsingPartMeshes()
        {
            var originParts = new List<PartMesh>();
            var originMeshObjects = new List<Transform>();

            bool WhereCondition(Component t) 
                => (t.gameObject.GetComponent<MeshFilter>() != null
                    && t.gameObject.GetComponent<MeshRenderer>() != null)
                   || t.gameObject.GetComponent<SkinnedMeshRenderer>() != null;

            // if (WhereCondition(transform))
            //     originMeshObjects.Add(transform);
            // Linq.Enumerable.Where: child가 없을 경우 자기자신을 조건부로 검출한다.
            var enuCollection = GetComponentsInChildren<Transform>().Where(WhereCondition);
            originMeshObjects.AddRange(enuCollection);

            foreach (var obj in originMeshObjects)
            {
                var meshFilter = obj.gameObject.GetComponent<MeshFilter>();
                var meshRenderer = obj.gameObject.GetComponent<MeshRenderer>();

                if (meshFilter != null || meshRenderer != null) 
                    originParts.Add(new PartMesh().Initialize(meshFilter, meshRenderer));
                else
                {
                    var skinnedMeshRenderer = obj.gameObject.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null)
                        originParts.Add(new PartMesh().Initialize(skinnedMeshRenderer, transform));
                    else
                    {
                        if (meshFilter == null)
                            throw new NullReferenceException(
                                "Object does not has MeshFilter and MeshRenderer" +
                                "(Or SkinnedMeshRenderer) component required to create PartMesh.");
                    }
                }
            }
            
            return originParts;
        }
        
        private PartMesh GenerateMesh(PartMesh original, Plane slicer, bool isUpperOnPlane)
        {
            var partMesh = new PartMesh();

            for (var i = 0; i < original.triangles.Length; i++)
            {
                slicingSectionGenerated = false;
                for (var j = 0; j < original.triangles[i].Length; j += 3)
                    GenerateTriangle(i, j, original, partMesh, slicer, isUpperOnPlane);
            }
            
            partMesh.FillArrays();
            partMesh.meshName = original.meshName;
            partMesh.materials = original.materials;
            
            return partMesh;
        }

        private bool CheckAndSetUVs(ref Vector2[] destUVs, Vector2[] originUVs, int[] uvIndices)
        {
            var existUVs = true;
            if (originUVs != null)
            {
                if (originUVs.Length > 0)
                    destUVs = new [] {originUVs[uvIndices[0]], originUVs[uvIndices[1]], originUVs[uvIndices[2]]};
                else
                {
                    destUVs = new [] {Vector2.zero, Vector2.zero, Vector2.zero};
                    existUVs = false;
                }
            }
            else
            {
                destUVs = new [] {Vector2.zero, Vector2.zero, Vector2.zero};
                existUVs = false;
            }

            return existUVs;
        }

        /// <summary>
        /// 평면(Slicer)과 현재 Triangle의 Edge들과의 교차 판정을 통해
        /// 교점이 2개 이상이면
        /// 해당 교점을 기준으로 새로운 Triangle을 생성해준다.
        /// </summary>
        /// <param name="subMesh">is Index on original.triangles</param>
        /// <param name="triangle">is Index on original.triangles[subMesh]</param>
        /// <param name="original"></param>
        /// <param name="partMesh"></param>
        /// <param name="slicer"></param>
        /// <param name="isUpperOnPlane">평면상에서의 Triangle의 위치에 따라 Triangle의 구성이 달라진다.</param>
        private void GenerateTriangle(
            int subMesh, int triangle,
            PartMesh original, PartMesh partMesh,
            Plane slicer, bool isUpperOnPlane)
        {
            var index = triangle;
            var triangles = original.triangles[subMesh];
            
            // 아래와 같이 Triangle의 Vertex가
            // 평면(slicer)의 어느 위치(위/아래)에 존재하는 가에 따라
            // 잘려나가는 Triangle의 Pivot Vertex(index)를 결정한다.
            var includePointA = slicer.GetSide(original.vertices[triangles[index]]) == isUpperOnPlane;
            var includePointB = slicer.GetSide(original.vertices[triangles[index + 1]]) == isUpperOnPlane;
            var includePointC = slicer.GetSide(original.vertices[triangles[index + 2]]) == isUpperOnPlane;

            var includeOriginVertexCount = (includePointA ? 1 : 0)
                                             + (includePointB ? 1 : 0)
                                             + (includePointC ? 1 : 0);
            if (includeOriginVertexCount == 0)
            {
                //Debug.Log($"includeOriginVertexCount:{includeOriginVertexCount}");
                return;
            }

            Vector2[] indexingUVs;
            int[] originUVIndices;
            bool existUVs;
            if (includeOriginVertexCount == 3)
            {
                indexingUVs = null;
                originUVIndices = new[] {triangles[index], triangles[index + 1], triangles[index + 2]};
                existUVs = CheckAndSetUVs(ref indexingUVs, original.uv, originUVIndices);
                
                partMesh.AddTriangle(subMesh,
                    original.vertices[triangles[index]], original.vertices[triangles[index + 1]],
                    original.vertices[triangles[index + 2]],
                    original.normals[triangles[index]], original.normals[triangles[index + 1]],
                    original.normals[triangles[index + 2]],
                    indexingUVs[0], indexingUVs[1], indexingUVs[2],
                    existUVs: existUVs);

                //Debug.Log($"includeOriginVertexCount:{includeOriginVertexCount}");
                return;
            }

            var offsetIndex = includePointB == includePointC ? 0 : (includePointA == includePointC ? 1 : 2);

            var pivotPoint = original.vertices[triangles[index + offsetIndex]];
            var point1 = original.vertices[triangles[index + ((offsetIndex + 1) % 3)]];
            var point2 = original.vertices[triangles[index + ((offsetIndex + 2) % 3)]];
            
            var pivotNormal = original.normals[triangles[index + offsetIndex]];
            var normal1 = original.normals[triangles[index + ((offsetIndex + 1) % 3)]];
            var normal2 = original.normals[triangles[index + ((offsetIndex + 2) % 3)]];

            indexingUVs = null;
            originUVIndices = new[]
            {
                triangles[index + offsetIndex],
                triangles[index + ((offsetIndex + 1) % 3)],
                triangles[index + ((offsetIndex + 2) % 3)]
            };
            existUVs = CheckAndSetUVs(ref indexingUVs, original.uv, originUVIndices);
            var pivotUV = indexingUVs[0];
            var uv1 = indexingUVs[1];
            var uv2 = indexingUVs[2];
            
            var edge1 = new Ray(pivotPoint, point1 - pivotPoint);
            var edge2 = new Ray(pivotPoint, point2 - pivotPoint);

            var edge1IsSliced = slicer.Raycast(edge1, out var enter1);
            var edge2IsSliced = slicer.Raycast(edge2, out var enter2);
            var isSliced = edge1IsSliced && edge2IsSliced;

            if (isSliced)
            {
                var lerp1 = enter1 / edge1.direction.magnitude;
                var lerp2 = enter2 / edge2.direction.magnitude;
                
                // 선형보간을 통해 교점을 구할 경우 오차가 발생하여
                // 간헐적으로 오목 다면체를 유발하게 된다.
                // var interPoint1 = Vector3.Lerp(pivotPoint, point1, lerp1);
                // var interPoint2 = Vector3.Lerp(pivotPoint, point2, lerp2);
                var interPoint1 = edge1.origin + edge1.direction.normalized * enter1;
                var interPoint2 = edge2.origin + edge2.direction.normalized * enter2;
                                
                // 비등방 노멀맵핑이라는 가정 하에 노멀을 Lerp시켜준다.
                var interNormal1 = Vector3.Lerp(pivotNormal, normal1, lerp1);
                var interNormal2 = Vector3.Lerp(pivotNormal, normal2, lerp2);
                                
                var interUV1 = Vector2.Lerp(pivotUV, uv1, lerp1);
                var interUV2 = Vector2.Lerp(pivotUV, uv2, lerp2);
                
                
                AddSlicingSection(subMesh, partMesh,
                    isUpperOnPlane ? slicer.normal*-1.0f : slicer.normal,
                    interPoint1,
                    interPoint2,
                    interUV1,
                    interUV2,
                    existUVs);
                
                // includeOriginVertexCount가 2일 경우 해당 SlicedTriangle의 모양은 사각형이기에
                // Triangle 2개로 구분하여 삼각형 모양을 유지해 준다.
                if (includeOriginVertexCount == 2)
                {
                    partMesh.AddTriangle(subMesh,
                        interPoint1, point1, point2,
                        interNormal1, normal1, normal2,
                        interUV1, uv1, uv2,
                        existUVs);
                    partMesh.AddTriangle(subMesh,
                        interPoint1, point2, interPoint2,
                        interNormal1, normal2: normal2, interNormal2,
                        interUV1, uv2: uv2, interUV2,
                        existUVs);
                    //Debug.Log($"includeOriginVertexCount:{includeOriginVertexCount}");
                }
                else
                {
                    partMesh.AddTriangle(subMesh,
                        pivotPoint, interPoint1, interPoint2,
                        pivotNormal, interNormal1, interNormal2,
                        pivotUV, interUV1, interUV2,
                        existUVs);
                    //Debug.Log($"includeOriginVertexCount:{includeOriginVertexCount}");
                }
            }
        }

        /// <summary>
        /// Slicing된 PartMesh의 단면을 추가
        /// 
        /// </summary>
        /// <param name="subMesh"></param>
        /// <param name="partMesh"></param>
        /// <param name="normal"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="uv1"></param>
        /// <param name="uv2"></param>
        /// <param name="existUVs"></param>
        private void AddSlicingSection(int subMesh,
            PartMesh partMesh,
            Vector3 normal,
            Vector3 vertex1, Vector3 vertex2,
            Vector2 uv1, Vector2 uv2,
            bool existUVs)
        {
            if (!slicingSectionGenerated)
            {
                slicingSectionGenerated = true;
                slicingPolygonPivotVertex = vertex1;
                slicingSectionPivotUV = uv1;
            }
            else
            {
                slicingSection.Set3Points(slicingPolygonPivotVertex, vertex1, vertex2);
                
                // 잘린 단면의 Normal에 따라 vertex 순서를 변경한다.
                var clockWise = slicingSection.GetSide(slicingPolygonPivotVertex + normal);

                partMesh.AddTriangle(subMesh,
                    slicingPolygonPivotVertex,
                    clockWise ? vertex1 : vertex2,
                    clockWise ? vertex2 : vertex1,
                    normal,
                    normal,
                    normal,
                    slicingSectionPivotUV,
                    uv1,
                    uv2,
                    existUVs);
            }
        }
    }
}
