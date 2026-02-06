from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session
from app.api.deps import require_role
from app.domain.entities import UserRole
from app.infra.db.session import get_db
from app.schemas.stream import RecordingItemSchema, RecordingSearchRequest
from app.services.nvr_service import NVRService

router = APIRouter(prefix="/recordings", tags=["recordings"])


@router.post("/search", response_model=list[RecordingItemSchema])
def search_recordings(payload: RecordingSearchRequest, db: Session = Depends(get_db), _=Depends(require_role(UserRole.OPERATOR))):
    return NVRService(db).search_recordings(payload)
