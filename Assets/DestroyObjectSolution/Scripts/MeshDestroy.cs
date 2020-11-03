namespace DestroyObjectSolution.Scripts
{
    using UnityEngine;
    using System.Collections.Generic;

    public class PartMesh
    {
        private List<Vector3> _vertices = new List<Vector3>();
        private List<Vector3> _normals = new List<Vector3>();
        private List<List<int>> _triangles = new List<List<int>>();
        private List<Vector2> _uvs = new List<Vector2>();

        public Vector3[] Vertices;
        public Vector3[] Normals;
        public int[][] Triangles;
        public Vector2[] UV;
        
        // Object Pool로 관리될 예정
        public GameObject GameObject;
        public Bounds Bounds;

        public PartMesh()
        {
            
        }
        
        public PartMesh(Mesh originMesh)
        {
            Vertices = originMesh.vertices;
            Normals = originMesh.normals;
            Triangles = new int[originMesh.subMeshCount][];
            UV = originMesh.uv;
            originMesh.RecalculateBounds();
            Bounds = originMesh.bounds;
            for (var i = 0; i < originMesh.subMeshCount; i++)
                Triangles[i] = originMesh.GetTriangles(i);
        }

        public void AddTriangle(
            int subMesh,
            Vector3 vertex1, Vector3 vertex2, Vector3 vertex3,
            Vector3 normal1, Vector3 normal2, Vector3 normal3,
            Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            if (_triangles.Count - 1 < subMesh)
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
            {
                _uvs.Add(uv1);
                _uvs.Add(uv2);
                _uvs.Add(uv3);
            }
            // Set Bound
            {
                Bounds.min = Vector3.Min(Bounds.min, vertex1);
                Bounds.min = Vector3.Min(Bounds.min, vertex2);
                Bounds.min = Vector3.Min(Bounds.min, vertex3);
                Bounds.max = Vector3.Max(Bounds.max, vertex1);
                Bounds.max = Vector3.Max(Bounds.max, vertex2);
                Bounds.max = Vector3.Max(Bounds.max, vertex3);
            }
        }

        public void FillArrays()
        {
            Vertices = _vertices.ToArray();
            Normals = _normals.ToArray();
            UV = _uvs.ToArray();
            Triangles = new int[_triangles.Count][];
            for (var i = 0; i < _triangles.Count; i++)
                Triangles[i] = _triangles[i].ToArray();
        }
        
        /// <summary>
        /// Mesh Destroy 구성에 따라 오브젝트를 생성.
        /// Mesh Destroy 구성: MeshRenderer, MeshFilter, etc.
        /// </summary>
        /// <param name="original"></param>
        public void MakeGameObject(MeshDestroy original)
        {
            // Object Pool로 관리될 예정
            GameObject = new GameObject(original.name);
            var originTransform = original.transform;
            GameObject.transform.position = originTransform.position;
            GameObject.transform.rotation = originTransform.rotation;
            GameObject.transform.localScale = originTransform.localScale;
            
            // Set Mesh
            {
                var mesh = new Mesh
                {
                    name = original.GetComponent<MeshFilter>().mesh.name,
                    vertices = Vertices,
                    normals = Normals,
                    uv = UV
                };
                for (var i = 0; i < Triangles.Length; i++)
                    mesh.SetTriangles(Triangles[i], i, true);
                Bounds = mesh.bounds;

                var renderer = GameObject.AddComponent<MeshRenderer>();
                renderer.materials = original.GetComponent<MeshRenderer>().materials;

                var filter = GameObject.AddComponent<MeshFilter>();
                filter.mesh = mesh;
            }
            // Set collider
            {
                var collider = GameObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
            // Set rigidbody
            {
                var rigidbody = GameObject.AddComponent<Rigidbody>();
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }
            // Set MeshDestroy
            {
                var meshDestroy = GameObject.AddComponent<MeshDestroy>();
                meshDestroy.maxSlicing = original.maxSlicing;
                meshDestroy.explodeForce = original.explodeForce;
            }
        }

        public void AddForce(float scalar)
        {
            var rigidBody = GameObject.GetComponent<Rigidbody>();
            rigidBody.AddForceAtPosition(Bounds.center * scalar, GameObject.transform.position);
        }
    }

    public class MeshDestroy : MonoBehaviour
    {
        [Range(1, 5)] public int maxSlicing = 3;

        [Range(1.0f, 2000.0f)] public float explodeForce = 1000.0f;
        
        private bool _slicingSectionGenerated;
        private Vector3 _slicingPolygonPivotVertex;
        private Vector2 _slicingSectionPivotUV;
        private Plane _slicingSection;
        
        private void Update()
        {
            // _debug test
            if (Input.GetMouseButtonDown(0)) DestroyMesh();
        }

        public void DestroyMesh()
        {
            var originPart = new PartMesh(GetComponent<MeshFilter>().mesh);
            var newParts = new List<PartMesh> {originPart};
            var subParts = new List<PartMesh>();
            for (var i = 0; i < maxSlicing; i++)
            {
                foreach (var subPart in newParts)
                {
                    // 랜덤한 방향의 평면(slicer) 생성
                    var partBounds = subPart.Bounds;
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
                    
                    if (slicedPart1 == null || slicedPart2 == null) continue;
                    subParts.Add(slicedPart1);
                    subParts.Add(slicedPart2);
                }
                
                newParts = new List<PartMesh>(subParts);
                subParts.Clear();
            }

            foreach (var newPart in newParts)
            {
                newPart.MakeGameObject(this);
                newPart.AddForce(explodeForce);
            }

            // Object Pool로 관리될 예정
            if(newParts.Count > 0) Destroy(gameObject);
        }

        private PartMesh GenerateMesh(PartMesh original, Plane slicer, bool isUpperOnPlane)
        {
            var partMesh = new PartMesh();

            for (var i = 0; i < original.Triangles.Length; i++)
            {
                _slicingSectionGenerated = false;
                for (var j = 0; j < original.Triangles[i].Length; j += 3)
                    GenerateTriangle(i, j, original, partMesh, slicer, isUpperOnPlane);
            }
            
            partMesh.FillArrays();
            return partMesh;
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
            var triangles = original.Triangles[subMesh];
            
            // 아래와 같이 Triangle의 Vertex가
            // 평면(slicer)의 어느 위치(위/아래)에 존재하는 가에 따라
            // 잘려나가는 Triangle의 Pivot Vertex(index)를 결정한다.
            var includePointA = slicer.GetSide(original.Vertices[triangles[index]]) == isUpperOnPlane;
            var includePointB = slicer.GetSide(original.Vertices[triangles[index + 1]]) == isUpperOnPlane;
            var includePointC = slicer.GetSide(original.Vertices[triangles[index + 2]]) == isUpperOnPlane;

            var includeOriginVertexCount = (includePointA ? 1 : 0)
                                             + (includePointB ? 1 : 0)
                                             + (includePointC ? 1 : 0);
            if (includeOriginVertexCount == 0)
            {
                //Debug.Log($"includeOriginVertexCount:{includeOriginVertexCount}");
                return;
            }

            if (includeOriginVertexCount == 3)
            {
                partMesh.AddTriangle(subMesh,
                    original.Vertices[triangles[index]], original.Vertices[triangles[index + 1]], original.Vertices[triangles[index + 2]],
                    original.Normals[triangles[index]], original.Normals[triangles[index + 1]], original.Normals[triangles[index + 2]],
                    original.UV[triangles[index]], original.UV[triangles[index + 1]], original.UV[triangles[index + 2]]);
                //Debug.Log($"includeOriginVertexCount:{includeOriginVertexCount}");
                return;
            }

            var offsetIndex = includePointB == includePointC ? 0 : (includePointA == includePointC ? 1 : 2);

            var pivotPoint = original.Vertices[triangles[index + offsetIndex]];
            var point1 = original.Vertices[triangles[index + ((offsetIndex + 1) % 3)]];
            var point2 = original.Vertices[triangles[index + ((offsetIndex + 2) % 3)]];
            
            var pivotNormal = original.Normals[triangles[index + offsetIndex]];
            var normal1 = original.Normals[triangles[index + ((offsetIndex + 1) % 3)]];
            var normal2 = original.Normals[triangles[index + ((offsetIndex + 2) % 3)]];

            var pivotUV = original.UV[triangles[index + offsetIndex]];
            var uv1 = original.UV[triangles[index + ((offsetIndex + 1) % 3)]];
            var uv2 = original.UV[triangles[index + ((offsetIndex + 2) % 3)]];
            
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
                    interUV2);
                
                // includeOriginVertexCount가 2일 경우 해당 SlicedTriangle의 모양은 사각형이기에
                // Triangle 2개로 구분하여 삼각형 모양을 유지해 준다.
                if (includeOriginVertexCount == 2)
                {
                    partMesh.AddTriangle(subMesh,
                        interPoint1, point1, point2,
                        interNormal1, normal1, normal2,
                        interUV1, uv1, uv2);
                    partMesh.AddTriangle(subMesh,
                        interPoint1, point2, interPoint2,
                        interNormal1, normal2: normal2, interNormal2,
                        interUV1, uv2: uv2, interUV2);
                    //Debug.Log($"includeOriginVertexCount:{includeOriginVertexCount}");
                }
                else
                {
                    partMesh.AddTriangle(subMesh,
                        pivotPoint, interPoint1, interPoint2,
                        pivotNormal, interNormal1, interNormal2,
                        pivotUV, interUV1, interUV2);
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
        private void AddSlicingSection(int subMesh,
            PartMesh partMesh,
            Vector3 normal,
            Vector3 vertex1, Vector3 vertex2,
            Vector2 uv1, Vector2 uv2)
        {
            if (!_slicingSectionGenerated)
            {
                _slicingSectionGenerated = true;
                _slicingPolygonPivotVertex = vertex1;
                _slicingSectionPivotUV = uv1;
            }
            else
            {
                _slicingSection.Set3Points(_slicingPolygonPivotVertex, vertex1, vertex2);
                
                // 잘린 단면의 Normal에 따라 vertex 순서를 변경한다.
                var clockWise = _slicingSection.GetSide(_slicingPolygonPivotVertex + normal);

                partMesh.AddTriangle(subMesh,
                    _slicingPolygonPivotVertex,
                    clockWise ? vertex1 : vertex2,
                    clockWise ? vertex2 : vertex1,
                    normal,
                    normal,
                    normal,
                    _slicingSectionPivotUV,
                    uv1,
                    uv2);
            }
        }
    }
}
