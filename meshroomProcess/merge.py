import OpenImageIO as oiio
import numpy as np
import re


def merge_obj_mtl_png(obj_path, mtl_path, png_filename, output_obj_path):
    # MTL 파일 읽기 및 경로 수정
    with open(mtl_path, 'r') as f:
        mtl_content = f.read()

    # map_Kd 라인을 정확히 교체 (전체 경로 포함)
    mtl_content = re.sub(r'map_Kd\s+.*', f'map_Kd {png_filename}', mtl_content)

    # OBJ 파일 읽기 및 mtllib 참조 제거
    with open(obj_path, 'r') as f:
        obj_lines = f.readlines()
    obj_lines = [line for line in obj_lines if not line.strip().startswith('mtllib')]

    # OBJ 내용에 mtllib 재삽입
    obj_output = []
    obj_output.append('# Merged OBJ + MTL\n')
    obj_output.append('mtllib merged_materials.mtl\n')
    obj_output.extend(obj_lines)

    # 최종 OBJ 저장
    with open(output_obj_path, 'w') as f:
        f.writelines(obj_output)

    # 병합된 MTL 저장
    with open(output_obj_path.replace('.obj', '.mtl'), 'w') as f:
        f.write(mtl_content)

    print(f"[OK] Merged OBJ: {output_obj_path}")
    print(f"[OK] Merged MTL: {output_obj_path.replace('.obj', '.mtl')}")


def exr_to_png(input_path, output_path):
    image = oiio.ImageInput.open(input_path)
    if not image:
        raise RuntimeError(f"Failed to open EXR file: {input_path}")

    spec = image.spec()
    width, height = spec.width, spec.height
    nchannels = spec.nchannels

    # float 형식으로 이미지 읽기
    data = image.read_image(format=oiio.TypeDesc("float"))
    image.close()

    # numpy로 변환 및 정규화
    data = np.asarray(data).reshape((height, width, nchannels))
    data = np.clip(data, 0.0, 1.0)
    data = (data * 255).astype(np.uint8)

    # 새로운 저장용 이미지 스펙 생성
    newspec = oiio.ImageSpec(width, height, nchannels, oiio.TypeDesc("uint8"))

    # PNG 저장
    out = oiio.ImageOutput.create(output_path)
    if not out:
        raise RuntimeError(f"Failed to create image output: {output_path}")
    out.open(output_path, newspec)
    out.write_image(data)
    out.close()



# 실행 예시

exr_to_png(
    "C:/RecordedFrames/icetea/output/texture_1001.exr",
    "C:/RecordedFrames/icetea/output/texture_1001.png"
)

merge_obj_mtl_png(
    "C:/RecordedFrames/icetea/output/texturedMesh.obj",
    "C:/RecordedFrames/icetea/output/texturedMesh.mtl",
    "C:/RecordedFrames/icetea/output/texture_1001.png",
    "C:/RecordedFrames/icetea/output/merged_mesh.obj"
)
