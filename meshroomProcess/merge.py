import os
import trimesh
from PIL import Image
import imageio.v3 as iio  # imageio for .exr

def convert_exr_to_png(exr_path, png_path):
    img = iio.imread(exr_path)
    # img shape: (H, W, C)
    img_8bit = (img * 255).clip(0, 255).astype('uint8')
    Image.fromarray(img_8bit).save(png_path)
    print(f"Converted {exr_path} to {png_path}")

def replace_mtl_texture(mtl_path, old_tex_name, new_tex_name):
    with open(mtl_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    with open(mtl_path, 'w', encoding='utf-8') as f:
        for line in lines:
            if line.strip().startswith("map_Kd") and old_tex_name in line:
                f.write(f"map_Kd {new_tex_name}\n")
            else:
                f.write(line)
    print(f"Updated MTL file: {mtl_path}")

def merge_obj_mtl_to_glb(obj_path, output_path):
    mesh = trimesh.load(obj_path, force='mesh')
    mesh.export(output_path)
    print(f"Exported to {output_path}")

# === 경로 설정 ===
base_dir = "D:/icetea/output"
obj_path = os.path.join(base_dir, "*.obj")
mtl_path = os.path.join(base_dir, "*.mtl")
exr_path = os.path.join(base_dir, "*.exr")
png_path = os.path.join(base_dir, "texture.png")
glb_path = os.path.join(base_dir, "final_model.glb")

# === 1. .exr -> .png 변환 ===
convert_exr_to_png(exr_path, png_path)

# === 2. .mtl에서 텍스처 파일명 수정 ===
replace_mtl_texture(mtl_path, "*.exr", "texture.png")

# === 3. .obj 로드 및 glb로 내보내기 ===
merge_obj_mtl_to_glb(obj_path, glb_path)
