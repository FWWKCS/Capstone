from flask import Blueprint, request, jsonify, current_app
from models import Instance
from extensions import db
import os

instance_bp = Blueprint("instance", __name__, url_prefix="/instance")

# 오브젝트 정보 생성
@instance_bp.route("/create", methods=["POST"])
def create_instance():
    data = request.get_json()
    new_instance = Instance(
        uid=data["uid"],
        name=data["name"],
        bigClass=data["bigClass"],
        smallClass=data["smallClass"],
        abilityType=data["abilityType"],
        sellState=0,
        cost=-1,
        expireCount=data["expireCount"],
        stat=data["stat"],
        grade=data["grade"]
    )

    # 데이터베이스에 인스턴스 추가
    db.session.add(new_instance)
    db.session.commit()

    # 추가된 db 정보의 oid 가져오기
    oid = new_instance.oid

    # 인스턴스의 oid 반환
    return jsonify({"oid":str(oid)}), 201

# fbx 파일 업로드
@instance_bp.route('/upload', methods=['POST'])
def upload_fbx():
    if 'file' not in request.files:
        return jsonify({'error': 'No file part'}), 400
    
    file = request.files['file']
    
    if file.filename == '':
        return jsonify({'error': 'No selected file'}), 400
    
    if not file.filename.lower().endswith('.fbx'):
        return jsonify({'error': 'Only FBX files are allowed'}), 400

    # 현재 Flask 앱의 설정에서 업로드 경로를 읽어옴
    upload_path = current_app.config['UPLOAD_FOLDER']
    
    save_path = os.path.join(upload_path, file.filename)
    file.save(save_path)

    return jsonify({'message': 'File uploaded successfully'}), 200


# 오브젝트 정보 조회
@instance_bp.route('/instances/<int:oid>', methods=['GET'])
def get_instance(oid):
    instance = Instance.get_by_id(oid)
    if not instance:
        return jsonify({'error': 'Not found'}), 404
    
    return jsonify({
        'oid': instance.oid,
        'uid': instance.uid,
        'bigClass': instance.bigClass,
        'smallClass': instance.smallClass,
        'abilityType': instance.abilityType,
        'sellState': instance.sellState,
        'cost': instance.cost,
        'expireCount': instance.expireCount,
        'stat': instance.stat,
        'grade': instance.grade
    })

# 오브젝트 정보 수정
@instance_bp.route('/instances/<int:oid>', methods=['PUT'])
def update_instance(oid):
    instance = Instance.get_by_id(oid)
    if not instance:
        return jsonify({'error': 'Not found'}), 404
    
    data = request.get_json()
    
    # 수정할 필드만 업데이트
    if 'uid' in data:
        instance.uid = data['uid']
    if 'name' in data:
        instance.name = data['name']
    if 'bigClass' in data:
        instance.bigClass = data['bigClass']
    if 'smallClass' in data:
        instance.smallClass = data['smallClass']
    if 'abilityType' in data:
        instance.abilityType = data['abilityType']
    if 'sellState' in data:
        instance.sellState = data['sellState']
    if 'cost' in data:
        instance.cost = data['cost']
    if 'expireCount' in data:
        instance.expireCount = data['expireCount']
    if 'stat' in data:
        instance.stat = data['stat']
    if 'grade' in data:
        instance.grade = data['grade']

    db.session.commit()

    return jsonify({'message': 'Instance updated successfully'})

# 오브젝트 정보 삭제
@instance_bp.route('/instances/<int:oid>', methods=['DELETE'])
def delete_instance(oid):
    instance = Instance.get_by_id(oid)
    if not instance:
        return jsonify({'error': 'Not found'}), 404
    
    db.session.delete(instance)
    db.session.commit()

    return jsonify({'message': 'Instance deleted successfully'})