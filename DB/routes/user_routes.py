from flask import Blueprint, request, jsonify
from models import User
from extensions import db

user_bp = Blueprint("user", __name__, url_prefix="/user")

# 사용자 생성
@user_bp.route("/create", methods=["POST"])
def create_user():
    data = request.get_json()
    print(data)
    if not data:
        return jsonify({"error": "Invalid input"}), 400
    
    new_user = User(
        name=data["_name"], 
        password=data["_password"], 
        email=data["_email"], 
        level=1, 
        exp=0.0, 
        money=0, 
        boxSize=10
    )
    # level, exp, money, boxSize는 기본값으로 설정
    # uid는 자동 증가로 설정되어 있으므로 명시적으로 지정할 필요 없음

    # 이메일 중복 체크
    existing_user = User.query.filter_by(email=data["_email"]).first()
    if existing_user:
        return jsonify({"error": "Email already exists!"}), 400  # 이메일 중복
    else:
        # 데이터베이스에 사용자 추가
        db.session.add(new_user)
        db.session.commit()
        return jsonify({"message": "user created"}), 201

# uid로 사용자 조회
@user_bp.route("/<int:uid>", methods=["GET"])
def get_user(uid):
    user = User.query.get(uid)
    if not user:
        return jsonify({"message": "user not found"}), 404

    user_data = {
        "uid": user.uid,
        "name": user.name,
        "email": user.email,
        "level": user.level,
        "exp": user.exp,
        "money": user.money,
        "boxSize": user.boxSize
    }
    return jsonify(user_data), 200

# 사용자 정보 수정
@user_bp.route("/<int:uid>", methods=["PUT"])
def update_user(uid):
    data = request.get_json()
    user = User.query.get(uid)
    if not user:
        return jsonify({"message": "user not found"}), 404

    # 수정할 필드만 업데이트
    if "name" in data:
        user.name = data["_name"]
    if "password" in data:
        user.password = data["_password"]
    if "email" in data:
        # 이메일 중복 체크
        existing_user = User.query.filter_by(email=data["_email"]).first()
        if existing_user and existing_user.uid != uid:
            return jsonify({"error": "Email already exists!"}), 400  # 이메일 중복
    if "_level" in data:
        user.level = data["_level"]  
    if "exp" in data:
        user.exp = data["_exp"]
    if "money" in data:
        user.money = data["_money"]
    if "boxSize" in data:
        user.boxSize = data["_boxSize"]
        

    db.session.commit()
    return jsonify({"message": "user updated"}), 200

# 사용자 삭제
@user_bp.route("/<int:uid>", methods=["DELETE"])
def delete_user(uid):
    user = User.query.get(uid)
    if not user:
        return jsonify({"message": "user not found"}), 404

    db.session.delete(user)
    db.session.commit()
    return jsonify({"message": "user deleted"}), 200

