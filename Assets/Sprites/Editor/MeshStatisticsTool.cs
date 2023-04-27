using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshStatisticsTool
{
    [MenuItem("CONTEXT/MeshFilter/Mesh Statistics", false, 49)]
    private static void NewMenuOption(MenuCommand menuCommand)
    {
        //If nothing is selected don't run
        if (Selection.objects.Length <= 0) return;

        //Get a list of all selected objects, checking to make sure they are mesh filters, add their mesh to the list
        List<Mesh> meshes = new List<Mesh>();
        foreach (GameObject obj in Selection.objects)
        {
            if (obj.GetComponent<MeshFilter>())
            {
                meshes.Add(obj.GetComponent<MeshFilter>().sharedMesh);
            }
        }

        //Used for adding multiple meshes
        int totalVertices = 0;
        int totalTris = 0;

        //Get totals from the mesh, and if there are multiple, get their totals
        foreach (Mesh mesh in meshes)
        {
            Debug.Log(mesh.name + " Vertices:" + mesh.vertexCount + " Triangles:" + mesh.triangles.Length / 3);

            if (meshes.Count > 1)
            {
                totalVertices += mesh.vertexCount;
                totalTris += mesh.triangles.Length / 3;
            }
        }

        //If multiple add new line with totals
        if (totalVertices > 0 && totalTris > 0)
        {
            Debug.Log("Total Vertices: " + totalVertices + " Total Triangles: " + totalTris);
        }

        Selection.objects = null;
    }

}
