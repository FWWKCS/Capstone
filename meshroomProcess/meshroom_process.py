import sys
import glob
import subprocess
import logging
from rembg import remove
from PIL import Image
import numpy as np
from pathlib import Path
import shutil

# 실행 경로 설정
exe_dir = Path(sys.executable).parent if getattr(sys, 'frozen', False) else Path(__file__).parent
meshroom_path = exe_dir / "Meshroom" / "meshroom_batch.exe"

# === 로깅 설정 ===
logging.basicConfig(
    filename='app.log',
    level=logging.DEBUG,
    format='%(asctime)s - %(levelname)s - %(message)s'
)

# === 완전 검은 이미지 판별 함수 ===
def is_black_image(image_path: Path, threshold=5):
    try:
        img = Image.open(image_path).convert("L")
        arr = np.array(img)
        avg_brightness = arr.mean()
        return avg_brightness < threshold
    except Exception as e:
        logging.warning(f"이미지 판별 중 오류 발생: {image_path} - {e}")
        return False

# === 배경 제거 함수 ===
def remove_background(input_file: Path, output_file: Path, brightness_threshold=5):
    try:
        img = Image.open(input_file)
        out = remove(img)

        if out.mode == 'RGBA':
            out = out.convert('RGB')

        output_file_jpg = output_file.with_suffix('.jpg')
        out.save(output_file_jpg, format="JPEG", quality=100, subsampling=0, optimize=True)

        if is_black_image(output_file_jpg, threshold=brightness_threshold):
            output_file_jpg.unlink()
            logging.info(f"완전히 검은 이미지로 간주되어 삭제됨: {output_file_jpg}")

    except Exception as e:
        logging.warning(f"배경 제거 실패: {input_file} - {e}")

# === 전체 프로세스 처리 함수 ===
def process(input_dir: Path):
    logging.info("프로세스 시작")
    logging.info(f"입력 디렉토리: {input_dir}")

    rembg_dir = input_dir / "rembg"
    output_dir = input_dir / "output"

    try:
        # 디렉토리 생성 (기존 폴더 제거 후 생성)
        if rembg_dir.exists():
            shutil.rmtree(rembg_dir)
        if output_dir.exists():
            shutil.rmtree(output_dir)

        rembg_dir.mkdir(parents=True, exist_ok=False)
        output_dir.mkdir(parents=True, exist_ok=False)

        image_files = list(input_dir.glob("*.jpg"))
        if not image_files:
            logging.warning("이미지 파일이 없습니다.")
            return

        for img_path in image_files:
            file_name = img_path.name
            out_path = rembg_dir / file_name

            logging.debug(f"배경 제거 시작: {img_path}")
            remove_background(img_path, out_path)
            logging.debug(f"배경 제거 완료: {img_path}")

        logging.info("모든 이미지 배경 제거 완료")

        valid_images = list(rembg_dir.glob("*.jpg"))
        if len(valid_images) < 10:
            logging.error("유효한 이미지 수가 너무 적습니다. 3D 재구성 불가능.")
            return

        # Meshroom 실행
        command = [
            str(meshroom_path.resolve()),
            "--input", str(rembg_dir.resolve()),
            "--output", str(output_dir.resolve())
        ]
        logging.debug(f"Meshroom 실행 명령어: {' '.join(command)}")
        subprocess.run(command, check=True)
        logging.info("3D 모델링 완료")

    except FileNotFoundError as e:
        logging.error(f"파일을 찾을 수 없습니다: {e}")

    except FileExistsError as e:
        logging.error(f"파일이 이미 존재합니다: {e}")

    except subprocess.CalledProcessError as e:
        logging.error(f"Meshroom 실행 실패: {e}")

    except Exception as e:
        logging.exception("예외 발생")
        with open('error.log', 'a') as f:
            f.write(str(e) + '\n')

if __name__ == "__main__":
    if len(sys.argv) < 2:
        logging.error("입력 디렉토리 경로가 제공되지 않았습니다.")
        sys.exit(1)

    input_dir = Path(sys.argv[1])
    process(input_dir)
