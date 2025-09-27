# 경매장에 올라온 전체 목록
# sellState 활성화 된거 모조리 Select 해서 보내주면 될듯 
# - (OID, Cost, bigClass, smallClass, stat) 반환
# SELECT OID, Cost, bigClass, smallClass, stat FROM _TABLE_ WHERE sellState = 1
# 이걸 기반으로 경매장 모든 매물 리스트 만들어서 표시

# 내가 올린 전체 목록
# sellState && uid 를 SELECT 해서 보내주면 될듯 
# - (OID, Cost, bigClass, smallClass, stat) 반환
# SELECT OID, Cost, bigClass, smallClass, stat FROM _TABLE_ WHERE sellState = 1 AND uid = myUid
# 이걸 기반으로 내가 올린 경매장 모든 매물 리스트 만들어서 표시, 유저는 올린거 취소할 수도 있음