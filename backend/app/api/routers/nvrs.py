from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session
from app.api.deps import require_role
from app.domain.entities import UserRole
from app.infra.db.session import get_db
from app.infra.security.crypto import CredentialCipher
from app.schemas.nvr import ChannelRead, NVRCreate, NVRRead, NVRUpdate
from app.services.nvr_service import NVRService
from app.services.repositories import NVRRepository

router = APIRouter(prefix="/nvrs", tags=["nvrs"])


@router.get("", response_model=list[NVRRead])
def list_nvrs(db: Session = Depends(get_db), _=Depends(require_role(UserRole.OPERATOR))):
    return NVRRepository(db).list()


@router.post("", response_model=NVRRead, status_code=status.HTTP_201_CREATED)
def create_nvr(payload: NVRCreate, db: Session = Depends(get_db), _=Depends(require_role(UserRole.ADMIN))):
    encrypted = CredentialCipher().encrypt(payload.password)
    return NVRRepository(db).create(payload, encrypted)


@router.put("/{nvr_id}", response_model=NVRRead)
def update_nvr(nvr_id: int, payload: NVRUpdate, db: Session = Depends(get_db), _=Depends(require_role(UserRole.ADMIN))):
    repo = NVRRepository(db)
    item = repo.get(nvr_id)
    if not item:
        raise HTTPException(status_code=404, detail="NVR not found")
    encrypted = CredentialCipher().encrypt(payload.password) if payload.password else None
    return repo.update(item, payload, encrypted)


@router.delete("/{nvr_id}", status_code=status.HTTP_204_NO_CONTENT)
def delete_nvr(nvr_id: int, db: Session = Depends(get_db), _=Depends(require_role(UserRole.ADMIN))):
    repo = NVRRepository(db)
    item = repo.get(nvr_id)
    if not item:
        raise HTTPException(status_code=404, detail="NVR not found")
    repo.delete(item)


@router.post("/{nvr_id}/sync")
def sync_nvr_channels(nvr_id: int, db: Session = Depends(get_db), _=Depends(require_role(UserRole.OPERATOR))):
    return NVRService(db).sync_channels(nvr_id)


@router.get("/{nvr_id}/channels", response_model=list[ChannelRead])
def list_channels(nvr_id: int, db: Session = Depends(get_db), _=Depends(require_role(UserRole.OPERATOR))):
    return NVRService(db).channels.by_nvr(nvr_id)
