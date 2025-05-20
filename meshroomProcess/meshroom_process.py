import os, sys
import glob
import subprocess
import logging
from rembg import remove
from PIL import Image

exe_dir = os.path.dirname(sys.executable if getattr(sys, 'frozen', False) else __file__)
meshroom_path = os.path.join(exe_dir, "Meshroom", "meshroom_batch.exe")


# === 로깅 설정 ===
logging.basicConfig(
    filename='app.log',
    level=logging.DEBUG,
    format='%(asctime)s - %(levelname)s - %(message)s'
)

# === 배경 제거 함수 ===
def remove_background(input_file, output_file):
    img = Image.open(input_file)
    out = remove(img)
    if out.mode == 'RGBA':
        out = out.convert('RGB')
    out.save(output_file)

# === 전체 프로세스 처리 함수 ===
def process(input_dir):
    logging.info("프로세스 시작")
    logging.info(f"입력 디렉토리: {input_dir}")

    rembg_dir = os.path.join(input_dir, "rembg")
    output_dir = os.path.join(input_dir, "output")

    try:
        # 디렉토리 생성
        os.makedirs(rembg_dir, exist_ok=False)
        os.makedirs(output_dir, exist_ok=False)

        image_files = glob.glob(os.path.join(input_dir, "*.jpg"))
        total = len(image_files)

        if total == 0:
            logging.warning("이미지 파일이 없습니다.")
            return

        for img_path in enumerate(image_files, 1):
            file_name = os.path.basename(img_path)
            out_path = os.path.join(rembg_dir, file_name)

            logging.debug(f"배경제거 시작: {img_path}")
            remove_background(img_path, out_path)
            logging.debug(f"배경제거 완료: {img_path}")

        logging.info("배경제거 완료")

        # Meshroom 실행
        command = [
            meshroom_path,
            "--input", rembg_dir,
            "--output", output_dir
        ]   
        logging.debug(f"실행 명령: {' '.join(command)}")
        subprocess.run(command, check=True)
        logging.info("3D 모델링 완료")

    except FileNotFoundError as e:
        logging.error(f"파일을 찾을 수 없습니다: {e}")

    except FileExistsError as e:
        logging.error(f"파일이 이미 존재합니다: {e}")

    except Exception as e:
        logging.exception("오류 발생")
        with open('error.log', 'a') as f:
            f.write(str(e) + '\n')


if __name__ == "__main__":
    # 유니티에서 명령행인자 받고 프로세스 바로 수행
    process(sys.argv[1])