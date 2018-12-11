using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour {

	public GameObject _combinedObject;

	// Use this for initialization
	void Start () {
		//CombineQuads(gameObject, _combinedObject);
		//CombineMesh();
	}
	
	void CombineQuads(GameObject source, GameObject target)
	{
		Quaternion oldRotation = source.transform.rotation;
		Vector3 oldPosition = source.transform.position;

		source.transform.rotation = Quaternion.identity;
		source.transform.position = Vector3.zero;

		MeshFilter[] filters = source.GetComponentsInChildren<MeshFilter>();

		CombineInstance[] totalMeshes = new CombineInstance[filters.Length];


		//List<CombineInstance> combines = new List<CombineInstance>();

		for (int i=0; i < filters.Length; i++)
		{
			MeshFilter filter = filters[i];

			if (filter.transform.GetInstanceID() == source.transform.GetInstanceID())
				continue;

			MeshRenderer renderer = filter.GetComponent<MeshRenderer>();

			CombineInstance[] combines = new CombineInstance[1];
			combines[0].mesh = filter.sharedMesh;
			//combines[0].transform = filter.transform.localToWorldMatrix;
			combines[0].subMeshIndex = 0;

			Mesh tempMesh = new Mesh();
			tempMesh.CombineMeshes(combines);

			totalMeshes[i].subMeshIndex = 0;
			totalMeshes[i].mesh = tempMesh;
			totalMeshes[i].transform = filter.transform.localToWorldMatrix;
		}

		Mesh combinedMesh = new Mesh();
		combinedMesh.CombineMeshes(totalMeshes, false);

        MeshFilter meshFilterCombine = target.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = target.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = target.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = target.AddComponent<MeshRenderer>();
        }

		target.GetComponent<MeshFilter>().sharedMesh = combinedMesh;

		source.transform.rotation = oldRotation;
		source.transform.position = oldPosition;
	}

    /**
	Source: https://answers.unity.com/questions/196649/combinemeshes-with-different-materials.html (Bunzaga)
	**/
    void CombineMesh()
    {
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        // Get / Create mesh filter & renderer
        MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
        }

        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances);

        // Destroy other meshes
        //foreach (Mesh oldMesh in meshes)
        //{
        //    oldMesh.Clear();
        //    DestroyImmediate(oldMesh);
        //}

        // Assign materials
        //Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        //meshRendererCombine.materials = materialsArray;

        //foreach (MeshFilter meshFilter in meshFilters)
        //{
        //    DestroyImmediate(meshFilter.gameObject);
        //}
    }

    private int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }
}
