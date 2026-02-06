from fastapi import Header, HTTPException, status
from app.domain.entities import UserRole


def require_role(required: UserRole):
    def _checker(x_role: str = Header(default="operator", alias="X-Role")) -> str:
        role = x_role.lower()
        if required == UserRole.ADMIN and role != UserRole.ADMIN:
            raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Admin role required")
        if role not in {UserRole.ADMIN, UserRole.OPERATOR}:
            raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Invalid role")
        return role

    return _checker
