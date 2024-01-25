using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Mesh
{
    public class MeshCombiner : EditorWindow
    {
        private static Transform _objectsToBake;
        private MeshFilter[] _meshFilters;
        private string _name = "Past mesh name";
        private string _path = "Assets/Content/MeshCombine";
        private const string CreateButton = "Combine Mesh";

        [MenuItem("Tools/Combine Mesh",false,1)]
        public static void Init()
        {
            GetWindowWithRect(typeof(MeshCombiner), new Rect(0, 0, 512, 256), false, "Combine Mesh");
        }

        public void OnGUI()
        {
            GUILayout.Label("Root of Combine Objects",EditorStyles.boldLabel);
            _objectsToBake = EditorGUILayout.ObjectField(_objectsToBake, typeof(Transform),true) as Transform;
            _name = EditorGUILayout.TextField("Name", _name);
            _path = EditorGUILayout.TextField("Path", _path);
            
            EditorGUILayout.Space();
            var button = GUILayout.Button(CreateButton);
            
            if (button && _objectsToBake)
            {
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
                
                var meshes = _objectsToBake.GetComponentsInChildren<MeshFilter>(true);
                Combine(meshes,_name,_path);
            }
        }

        private static void Combine(MeshFilter[] meshes,string nameMesh,string path)
        {
            var combines = new CombineInstance[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                combines[i].mesh = meshes[i].sharedMesh;
                combines[i].transform = meshes[i].transform.localToWorldMatrix;
            }
            
            var combine = new UnityEngine.Mesh
            {
                name = nameMesh
            };
            combine.CombineMeshes(combines);
            combine.RecalculateBounds();

            var meshPath = $"{path}/{nameMesh}.asset";
            
            AssetDatabase.CreateAsset(combine, meshPath);
            AssetDatabase.ImportAsset(meshPath);
            
            Debug.LogWarning($"Complete {meshPath}");
        }
    }
}
