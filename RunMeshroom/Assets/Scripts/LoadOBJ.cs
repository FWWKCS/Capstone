using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using Dummiesman;
using UnityEditor; // PrefabUtility를 사용하기 위해 필요

/*
1. 프리팹 만들어서 저장하기
2. 프리팹 이름 OID로 바꾸기
3. 프리팹 저장 위치는 우진이랑 협의
4. 실제 아이템 생성 하고, 인게임에서 소환까지 되는지 확인
*/


public class LoadOBJ : MonoBehaviour
{
    private string objPath = Path.Combine(Application.dataPath, "Resources/icetea/texturedMesh.obj");

    void Start()
    {
        LoadObjAndAssignURPMaterials();
        SaveAsPrefab(objPath);
    }

    private void LoadObjAndAssignURPMaterials()
    {
        if (!File.Exists(objPath))
        {
            Debug.LogError("[OBJ DEBUG] .obj 파일이 존재하지 않습니다: " + objPath);
            return;
        }

        string originalDir = Directory.GetCurrentDirectory();
        string objDir = Path.GetDirectoryName(objPath);
        Directory.SetCurrentDirectory(objDir);

        GameObject loadedObj = null;
        try
        {
            loadedObj = new OBJLoader().Load(objPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[OBJ DEBUG] 로딩 중 예외 발생: {ex.Message}");
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
        }

        if (loadedObj == null)
        {
            Debug.LogError("[OBJ DEBUG] Load 실패 - GameObject null");
            return;
        }

        loadedObj.transform.position = Vector3.zero;
        loadedObj.name = "ImportedMesh";

        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null)
        {
            Debug.LogError("[OBJ DEBUG] URP/Lit 셰이더를 찾을 수 없습니다.");
            return;
        }

        Renderer[] renderers = loadedObj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                Material oldMat = renderer.sharedMaterials[i];
                if (oldMat == null)
                {
                    Debug.LogWarning("[OBJ DEBUG] null 머티리얼 감지");
                    continue;
                }

                // material_{숫자} 이름에서 숫자 추출
                Match match = Regex.Match(oldMat.name, @"material_(\d+)");
                if (!match.Success)
                {
                    Debug.LogWarning($"[OBJ DEBUG] 머티리얼 이름이 형식과 다름: {oldMat.name}");
                    continue;
                }

                string id = match.Groups[1].Value;
                string texturePath = Path.Combine(objDir, $"texture_{id}.png");

                Material newMat = new Material(urpShader);
                newMat.name = $"material_{id}_URP";

                // 1. 텍스처 로드 및 Base Map 설정
                if (File.Exists(texturePath))
                {
                    byte[] texBytes = File.ReadAllBytes(texturePath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(texBytes);
                    newMat.SetTexture("_BaseMap", tex);
                    Debug.Log($"[OBJ DEBUG] 텍스처 적용 완료: texture_{id}.png");
                }
                else
                {
                    Debug.LogWarning($"[OBJ DEBUG] 텍스처 파일이 존재하지 않음: texture_{id}.png");
                }

                // 2. Base Color를 완전 흰색으로 설정
                if (newMat.HasProperty("_BaseColor"))
                {
                    newMat.SetColor("_BaseColor", Color.white);
                }

                // 3. Smoothness = 0.0으로 설정
                if (newMat.HasProperty("_Smoothness"))
                {
                    newMat.SetFloat("_Smoothness", 0.0f);
                }

                newMaterials[i] = newMat;
            }

            renderer.sharedMaterials = newMaterials;
        }

        Debug.Log("[OBJ DEBUG] OBJ 로드 및 머티리얼 설정 완료");
    }

    private void SaveAsPrefab(string objPath)
    {
        string prefabPath = Path.ChangeExtension(objPath, ".prefab");
        // 런타임 중 씬에 임포팅 된 오브젝트를 찾기
        GameObject loadedObj = GameObject.Find("ImportedMesh");
        PrefabUtility.SaveAsPrefabAsset(loadedObj, prefabPath);
        UnityEngine.Debug.Log($"[OBJ DEBUG] 프리팹 저장 경로: {prefabPath}");
    }
}
