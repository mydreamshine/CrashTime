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

        public void AddTriangles(
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
                Bounds.max = Vector3.Min(Bounds.max, vertex1);
                Bounds.max = Vector3.Min(Bounds.max, vertex2);
                Bounds.max = Vector3.Min(Bounds.max, vertex3);
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
            // etc ...
        }
    }

    public class MeshDestroy : MonoBehaviour
    {
        [Range(1, 100)]
        public int maxSlicing = 1;
        
        private void DestroyMesh()
        {
            var originMesh = GetComponent<MeshFilter>().mesh;
            var originPart = new PartMesh()
            {
                Vertices = originMesh.vertices,
                Normals = originMesh.normals,
                Triangles = new int[originMesh.subMeshCount][],
                UV = originMesh.uv,
                Bounds = originMesh.bounds
            };
            for (var i = 0; i < originMesh.triangles.Length; i++)
                originPart.Triangles[i] = originMesh.GetTriangles(i);
            
            var newParts = new List<PartMesh>();
            
            for (var i = 0; i < maxSlicing; i++)
            {
                // 랜덤한 방향의 평면(slicer) 생성
                var partBounds = originPart.Bounds;
                var planeNormal = Random.onUnitSphere;
                var pointInPlane = new Vector3(
                    Random.Range(partBounds.min.x, partBounds.max.x),
                    Random.Range(partBounds.min.y, partBounds.max.y),
                    Random.Range(partBounds.min.z, partBounds.max.z));
                var slicer = new Plane(planeNormal, pointInPlane);
                
                // 평면(Slicer)을 기준으로 두개의 Part(slicedMesh) 생성
                var subPart1 = GenerateMesh(originPart, slicer, true);
                var subPart2 = GenerateMesh(originPart, slicer, false);
                
                if(subPart1 == null || subPart2 == null) continue;
                newParts.Add(subPart1);
                newParts.Add(subPart2);
            }

            foreach (var newPart in newParts)
                newPart.MakeGameObject(this);
            
            // Object Pool로 관리될 예정
            if(newParts.Count > 0) Destroy(gameObject);
        }

        private PartMesh GenerateMesh(PartMesh original, Plane slicer, bool isUpperOnPlane)
        {
            var partMesh = new PartMesh();
            bool generatedTriangle = true;
            
            for (var i = 0; i < original.Triangles.Length; i++)
            {
                for (var j = 0; j < original.Triangles[i].Length; j += 3)
                    generatedTriangle = CheckAndGenerateTriangle(i, j, original, partMesh, slicer, isUpperOnPlane);
            }

            if (!generatedTriangle) return null;
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
        /// <returns>Triangle is sliced?</returns>
        private bool CheckAndGenerateTriangle(
            int subMesh, int triangle,
            PartMesh original, PartMesh partMesh,
            Plane slicer, bool isUpperOnPlane)
        {
            bool isSliced;

            var index = triangle;
            
            // Triangle의 Vertex들이
            // 평면 위에 존재하는 가
            // 아니면 평면 아래에 존재하는 가
            // 그에 따라 index는 어떻게 변경할 것인가
            if (isUpperOnPlane)
            {
                // Point check upper on plane
                {
                    // 임의의 위치 벡터가 평면 위/아래에 존재하는 가를
                    // 알아내는 방법은?...
                }
                // index = ?
            }

            var pivotPoint = original.Vertices[index];
            var point1 = original.Vertices[(index + 1) % 3];
            var point2 = original.Vertices[(index + 2) % 3];
            
            var pivotNormal = original.Normals[index];
            var normal1 = original.Normals[(index + 1) % 3];
            var normal2 = original.Normals[(index + 2) % 3];

            var pivotUV = original.UV[index];
            var uv1 = original.UV[(index + 1) % 3];
            var uv2 = original.UV[(index + 2) % 3];
            
            var edge1 = new Ray(pivotPoint, point1 - pivotPoint);
            var edge2 = new Ray(pivotPoint, point2 - pivotPoint);

            var edge1IsSliced = slicer.Raycast(edge1, out var enter1);
            var edge2IsSliced = slicer.Raycast(edge2, out var enter2);
            isSliced = edge1IsSliced && edge2IsSliced;

            if (isSliced)
            {
                var lerp1 = enter1 / edge1.direction.magnitude;
                var lerp2 = enter2 / edge2.direction.magnitude;
                
                var interPoint1 = Vector3.Lerp(pivotPoint, point1, lerp1);
                var interPoint2 = Vector3.Lerp(pivotPoint, point2, lerp2);
                                
                // 비등방 노멀맵핑이라는 가정 하에 노멀을 Lerp시켜준다.
                var interNormal1 = Vector3.Lerp(pivotNormal, normal1, lerp1);
                var interNormal2 = Vector3.Lerp(pivotNormal, normal2, lerp2);
                                
                var interUV1 = Vector2.Lerp(pivotUV, uv1, lerp1);
                var interUV2 = Vector2.Lerp(pivotUV, uv2, lerp2);
                                
                partMesh.AddTriangles(subMesh,
                    pivotPoint, interPoint1, interPoint2,
                    pivotNormal, interNormal1, interNormal2,
                    pivotUV, interUV1, interUV2);
            }

            return isSliced;
        }
    }
}
