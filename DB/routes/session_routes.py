from flask import Blueprint, request, jsonify
from models import Session
from extensions import db
from datetime import datetime, timedelta

session_bp = Blueprint("session", __name__, url_prefix="/session")

# 세션 생성
@session_bp.route("/create", methods=["POST"])
def create_session():
    data = request.get_json()
    # 만료 시간 가공
    # "YYYY-MM-DDTHH:MM:SS" 형식 (ISO 8601)을 파싱
    # 문자열 -> datetime 객체 변환
    joined_at = datetime.fromisoformat(data["expire"])

    new_session = Session(
        uid=data["uid"], 
        expire=joined_at
    )

    # 데이터베이스에 세션 추가
    db.session.add(new_session)
    db.session.commit()
    return jsonify({"message": "user created"}), 201

# 세션 조회
@session_bp.route("/<int:uid>", methods=["GET"])
def get_session(uid):
    session = Session.query.get(uid)
    if not session:
        return jsonify({"message": "session not found"}), 404

    session_data = {
        "uid": session.uid,
        "expire": session.expire.isoformat()  # datetime 객체를 ISO 형식으로 변환
    }
    return jsonify(session_data), 200

# 세션 갱신
@session_bp.route("/<int:uid>", methods=["PATCH"])
def update_session(uid):
    session = Session.query.get(uid)
    if not session:
        return jsonify({"message": "session not found"}), 404

    # 현재 시간 기준 +3일 계산
    expires_at = datetime.now() + timedelta(days=3)

    # 세션 정보 저장
    session.expire = expires_at

    # 세션 만료 시간 업데이트
    db.session.commit()

    return jsonify({
        "message": "Session expiration updated.",
        "expires_at": expires_at.isoformat()
    }), 200

# 세션 삭제
@session_bp.route("/<int:uid>", methods=["DELETE"])
def delete_session(uid):
    session = Session.query.get(uid)
    if not session:
        return jsonify({"message": "session not found"}), 404

    db.session.delete(session)
    db.session.commit()
    return jsonify({"message": "session deleted"}), 200