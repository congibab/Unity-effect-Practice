using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrailTut : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3.0f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVerRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            ActivateTrail(activeTime);
        }
    }

    async UniTask ActivateTrail(float timeActive)
    {
        Debug.Log("start");

        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (meshRenderers == null)
            {
                meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                var gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                var mr = gObj.AddComponent<MeshRenderer>();
                var mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                meshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;
                mr.material = mat;

                AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate);

                Destroy(gObj, meshDestroyDelay);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(meshRefreshRate), ignoreTimeScale: false);
        }

        isTrailActive = false;
    }

    async UniTask AnimateMaterialFloat(Material mat, float goal, float rate, float refrehRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVerRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVerRef, valueToAnimate);
            await UniTask.Delay(TimeSpan.FromSeconds(refrehRate), ignoreTimeScale: false);
        }
    }
}
