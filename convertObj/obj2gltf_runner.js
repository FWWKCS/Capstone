// obj2gltf_runner.js
const obj2gltf = require('obj2gltf');
const fs = require('fs');
const path = require('path');

async function main() {
    const folderPath = process.argv[2];
    if (!folderPath) {
        console.error("사용법: node obj2gltf_runner.js <작업폴더 경로>");
        process.exit(1);
    }

    const absFolderPath = path.resolve(folderPath);

    // 폴더 존재 여부 확인
    if (!fs.existsSync(absFolderPath)) {
        console.error(`작업 폴더가 존재하지 않습니다: ${absFolderPath}`);
        process.exit(1);
    }

    // 폴더 내 .obj 파일 찾기 (첫 번째 발견된 파일 사용)
    const files = fs.readdirSync(absFolderPath);
    const objFile = files.find(f => f.toLowerCase().endsWith('.obj'));

    if (!objFile) {
        console.error(".obj 파일을 찾을 수 없습니다.");
        process.exit(1);
    }

    const inputObjPath = path.join(absFolderPath, objFile);
    const outputGlbPath = path.join(absFolderPath, path.basename(objFile, '.obj') + '.glb');

    try {
        const glb = await obj2gltf(inputObjPath, {
            binary: true,  // .glb 바이너리 포맷
            embed: true    // 텍스처 임베딩
        });

        fs.writeFileSync(outputGlbPath, glb);
        console.log(`변환 완료: ${outputGlbPath}`);
        process.exit(0);
    } catch (err) {
        console.error("변환 중 오류 발생:", err);
        process.exit(1);
    }
}

main();
