# Capstone
Real or FaK'e

# MeshroomProcess
    - unity 클라이언트로부터 인자 받음
        1. 모델링 생성 대상 사진 디렉토리 경로
        2. 최종 생성될 glb 파일의 이름(oid)

    - 프로세스 동작 파이프라인
        1. 사진 배경 제거 전처리
        2. meshroom_batch.exe 에서 사진 측량 및 모델링 데이터 생성
        3. obj -> glb 변환 프로세스 호출 (ConvertProcess)

    - 진행되는 과정에 대한 디버깅은 app.log에 기록
    
# ConvertProcess 
    - meshroom_process 로부터 인자 받음
        1. obj 및 mtl, png 텍스처 파일이 있는 output 디렉토리 경로
        2. persistent 저장소의 object 디렉토리 경로
        3. glb 파일로 변환되어 저장될 오브젝트 glb 파일의 이름(oid)
        
    - node.js 런타임에서 obj2gltf 라이브러리 동작

    - 진행되는 과정에 대한 디버깅은 conv.log에 기록
