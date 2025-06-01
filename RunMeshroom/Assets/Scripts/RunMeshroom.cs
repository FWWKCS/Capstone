using System.Diagnostics; // Process 클래스를 사용하기 위해 필요
using UnityEngine;
using UnityEditor; // PrefabUtility를 사용하기 위해 필요
using System.IO;          // 경로 조작을 위해 필요
using System.Threading.Tasks; // async/await와 Task를 위해 필요
using System.Collections;
using System.Text;
using Dummiesman;
using System.Text.RegularExpressions;

public class RunMeshroom : MonoBehaviour
{
    [Header("사진 저장 경로 설정")]
    [Tooltip("메시룸 프로세스에 전달할 사진 디렉토리 경로입니다.")]
    private string pictSavePath = "C:/RecordedFrames/icetea"; // 기본 저장 경로 (인스펙터에서 수정 가능)
    
    [Header("로컬 저장 경로 설정")]
    [Tooltip("로컬 저장 경로입니다.")]
    private string localPath;  

    [Header("프로젝트 경로 설정")]
    [Tooltip("프로젝트의 Assets 폴더 경로입니다.")]
    private string assetsPath = Application.dataPath;

    private Process meshroomProcess; // 외부 프로세스 객체
    private StringBuilder outputBuilder = new StringBuilder();

    // Awake 또는 Start에서 바로 실행하도록 Start() 사용
    void Start()
    {
        localPath = Application.persistentDataPath;
        
        UnityEngine.Debug.Log($"[Unity] Assets 폴더 경로: {assetsPath}");

        // 프로젝트 루트 경로 가져오기 (Assets 폴더의 상위 디렉토리)
        string projectPath = Directory.GetParent(assetsPath).FullName;
        UnityEngine.Debug.Log($"[Unity] 프로젝트 루트 경로: {projectPath}");

        // Debug.Log("[Unity] 스크립트 시작. pictSavePath: " + pictSavePath);
        // Debug.Log("[Unity] persistentDataPath: " + Application.persistentDataPath);

        // 비동기 실행을 위한 함수 호출
        RunMeshroomProcessAsAdminAsync();
    }

    /// <summary>
    /// 메시룸 프로세스를 관리자 권한으로 비동기 실행하고 출력을 캡처하는 함수
    /// </summary>
    public async void RunMeshroomProcessAsAdminAsync()
    {
        string exePath = Path.Combine(localPath, "meshroom_process.exe");
        
        string resourcesPath = Path.Combine(assetsPath, "Resources");

        if (!File.Exists(exePath))
        {
            UnityEngine.Debug.LogError("오류: 'meshroom_process.exe' 파일이 다음 경로에 없습니다: " + exePath);
            UnityEngine.Debug.LogError("파일을 " + localPath + " 경로에 배치해주세요.");
            return;
        }

        if (!Directory.Exists(resourcesPath))
        {
            UnityEngine.Debug.LogError("오류: Resources 폴더가 다음 경로에 없습니다: " + resourcesPath);
            return;
        }

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{pictSavePath}\"",  // 두 번째 매개변수로 Resources 경로 전달
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = false,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Normal
            };

            UnityEngine.Debug.Log($"[Unity] 실행 명령: {exePath} \"{pictSavePath}\"");

            meshroomProcess = Process.Start(startInfo);
            
            if (meshroomProcess == null)
            {
                UnityEngine.Debug.LogError("프로세스 시작 실패");
                return;
            }

            await Task.Run(() => meshroomProcess.WaitForExit());

            if (meshroomProcess.ExitCode == 0)
            {
                UnityEngine.Debug.Log("메시룸 프로세스 종료");
                // 메시룸 완료 후에 obj 경로 탐색
                string modelDirPath = Path.Combine(assetsPath, "Resources", "icetea");
                string[] objFiles = Directory.GetFiles(modelDirPath, "*.obj");

                if (objFiles.Length > 0)
                {
                    string objFilePath = objFiles[0]; // 첫 번째 .obj 사용
                    UnityEngine.Debug.Log($"[Unity] 로드할 .obj 파일: {objFilePath}");
                    // LoadObjAndAssignURPMaterials(objFilePath); // 런타임 오브젝트 임포팅

                }
                else
                {
                    UnityEngine.Debug.LogError("[Unity] obj 파일이 존재하지 않습니다.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("메시룸 프로세스 종료 실패");
            }
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            if (ex.NativeErrorCode == 1223) // 사용자가 UAC 프롬프트를 취소한 경우
            {
                UnityEngine.Debug.LogError("관리자 권한 요청이 취소되었습니다.");
            }
            else
            {
                UnityEngine.Debug.LogError($"[Unity] 메시룸 프로세스 실행 중 Win32 오류 발생: {ex.Message} (Error Code: {ex.NativeErrorCode})");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[Unity] 메시룸 프로세스 실행 실패: {ex.Message}");
        }
    }

    // 앱 종료 시 실행 중인 외부 프로세스를 강제로 종료
    void OnApplicationQuit()
    {
        if (meshroomProcess != null && !meshroomProcess.HasExited)
        {
            UnityEngine.Debug.LogWarning("[Unity] 앱 종료 시 메시룸 프로세스 강제 종료.");
            meshroomProcess.Kill();
            meshroomProcess.Dispose();
        }
    }

    private void LoadObjAndAssignURPMaterials(string objPath)
    {
        if (!File.Exists(objPath))
        {
            UnityEngine.Debug.LogError("[OBJ DEBUG] .obj 파일이 존재하지 않습니다: " + objPath);
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
            UnityEngine.Debug.LogError($"[OBJ DEBUG] 로딩 중 예외 발생: {ex.Message}");
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
        }

        if (loadedObj == null)
        {
            UnityEngine.Debug.LogError("[OBJ DEBUG] Load 실패 - GameObject null");
            return;
        }

        loadedObj.transform.position = Vector3.zero;
        loadedObj.name = "ImportedMesh";

        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null)
        {
            UnityEngine.Debug.LogError("[OBJ DEBUG] URP/Lit 셰이더를 찾을 수 없습니다.");
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
                    UnityEngine.Debug.LogWarning("[OBJ DEBUG] null 머티리얼 감지");
                    continue;
                }

                // material_{숫자} 이름에서 숫자 추출
                Match match = Regex.Match(oldMat.name, @"material_(\d+)");
                if (!match.Success)
                {
                    UnityEngine.Debug.LogWarning($"[OBJ DEBUG] 머티리얼 이름이 형식과 다름: {oldMat.name}");
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
                    UnityEngine.Debug.Log($"[OBJ DEBUG] 텍스처 적용 완료: texture_{id}.png");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[OBJ DEBUG] 텍스처 파일이 존재하지 않음: texture_{id}.png");
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

        UnityEngine.Debug.Log("[OBJ DEBUG] OBJ 로드 및 머티리얼 설정 완료");
    }
}