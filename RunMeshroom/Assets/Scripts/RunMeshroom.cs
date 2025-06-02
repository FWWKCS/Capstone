using System.Diagnostics; // Process 클래스를 사용하기 위해 필요
using UnityEngine;
using System.IO;          // 경로 조작을 위해 필요
using System.Threading.Tasks; // async/await와 Task를 위해 필요
using System.Collections;
using UnityGLTF.Loader;
using UnityGLTF;

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




    // Awake 또는 Start에서 바로 실행하도록 Start() 사용
    void Start()
    {
        string oid = "test";
        localPath = Application.persistentDataPath;
        
        UnityEngine.Debug.Log($"[Unity] Assets 폴더 경로: {assetsPath}");

        // 프로젝트 루트 경로 가져오기 (Assets 폴더의 상위 디렉토리)
        string projectPath = Directory.GetParent(assetsPath).FullName;
        UnityEngine.Debug.Log($"[Unity] 프로젝트 루트 경로: {projectPath}");

        // Debug.Log("[Unity] 스크립트 시작. pictSavePath: " + pictSavePath);
        // Debug.Log("[Unity] persistentDataPath: " + Application.persistentDataPath);

        // 비동기 실행을 위한 함수 호출
        RunMeshroomProcessAsAdminAsync(oid);
    }

    /// <summary>
    /// 메시룸 프로세스를 관리자 권한으로 비동기 실행하고 출력을 캡처하는 함수
    /// </summary>
    public async void RunMeshroomProcessAsAdminAsync(string oid)
    {
        string exePath = Path.Combine(localPath, "meshroom_process.exe");
        
        if (!File.Exists(exePath))
        {
            UnityEngine.Debug.LogError("오류: 'meshroom_process.exe' 파일이 다음 경로에 없습니다: " + exePath);
            UnityEngine.Debug.LogError("파일을 " + localPath + " 경로에 배치해주세요.");
            return;
        }

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{pictSavePath}\" \"{oid}\"",  // 두 번째 매개변수로 Resources 경로 전달
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = false,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Normal
            };

            UnityEngine.Debug.Log($"[Unity] 실행 명령: {exePath} \"{pictSavePath}\" \"{oid}\"");

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
                GLBLoader(oid);
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
            if (meshroomProcess != null)
            {
                if (!meshroomProcess.HasExited)
                {
                    meshroomProcess.Kill();
                }
                meshroomProcess.Dispose();
                meshroomProcess = null;
            }
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

    public void GLBLoader(string oid)
    {   
        // objects 폴더 경로
        string objectsDir = Path.Combine(localPath, "objects");
        string glbFilePath = Path.Combine(objectsDir, $"{oid}.glb");

        UnityEngine.Debug.Log($"[Unity] 로드할 .glb 파일: {glbFilePath}");

        if (!Directory.Exists(objectsDir))
        {
            UnityEngine.Debug.LogError("[Unity] objects 디렉토리를 찾을 수 없습니다: " + objectsDir);
            return;
        }

        if (!File.Exists(glbFilePath))
        {
            UnityEngine.Debug.LogError("[Unity] GLB 파일이 존재하지 않습니다: " + glbFilePath);
            return;
        }

        // GLB 로딩은 코루틴으로 비동기 실행 권장
        StartCoroutine(LoadGLBModelCoroutine(glbFilePath));
    }

    private IEnumerator LoadGLBModelCoroutine(string glbFilePath)
    {
        var loader = new FileLoader(Path.GetDirectoryName(glbFilePath));
        var importer = new GLTFSceneImporter(
            Path.GetFileName(glbFilePath),
            new ImportOptions { DataLoader = loader }
        );
        importer.SceneParent = new GameObject("LoadedGLB").transform;

        UnityEngine.Debug.Log("GLB 로딩 시작: " + glbFilePath);

        var loadSceneTask = importer.LoadSceneAsync();
        while (!loadSceneTask.IsCompleted)
        {
            yield return null;
        }

        if (loadSceneTask.Exception != null)
        {
            UnityEngine.Debug.LogError("GLB 로딩 실패: " + loadSceneTask.Exception.Message);
        }
        else
        {
            UnityEngine.Debug.Log("GLB 로딩 완료");
        }
    }

}