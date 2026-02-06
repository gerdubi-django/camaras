from sqlalchemy.orm import Session
from app.infra.db import models
from app.schemas.nvr import NVRCreate, NVRUpdate


class NVRRepository:
    def __init__(self, db: Session) -> None:
        self.db = db

    def list(self) -> list[models.NVR]:
        return self.db.query(models.NVR).order_by(models.NVR.id.desc()).all()

    def get(self, nvr_id: int) -> models.NVR | None:
        return self.db.get(models.NVR, nvr_id)

    def create(self, data: NVRCreate, encrypted_password: str) -> models.NVR:
        item = models.NVR(**data.model_dump(exclude={"password"}), encrypted_password=encrypted_password)
        self.db.add(item)
        self.db.commit()
        self.db.refresh(item)
        return item

    def update(self, item: models.NVR, data: NVRUpdate, encrypted_password: str | None) -> models.NVR:
        for key, value in data.model_dump(exclude_unset=True, exclude={"password"}).items():
            setattr(item, key, value)
        if encrypted_password:
            item.encrypted_password = encrypted_password
        self.db.commit()
        self.db.refresh(item)
        return item

    def delete(self, item: models.NVR) -> None:
        self.db.delete(item)
        self.db.commit()


class ChannelRepository:
    def __init__(self, db: Session) -> None:
        self.db = db

    def by_nvr(self, nvr_id: int) -> list[models.Channel]:
        return self.db.query(models.Channel).filter(models.Channel.nvr_id == nvr_id).all()

    def replace_nvr_channels(self, nvr_id: int, channels: list[dict]) -> list[models.Channel]:
        self.db.query(models.Channel).filter(models.Channel.nvr_id == nvr_id).delete()
        created = [models.Channel(nvr_id=nvr_id, **channel) for channel in channels]
        self.db.add_all(created)
        self.db.commit()
        return created


class AuditRepository:
    def __init__(self, db: Session) -> None:
        self.db = db

    def log(self, action: str, actor_role: str, details: str) -> None:
        self.db.add(models.AuditLog(action=action, actor_role=actor_role, details=details))
        self.db.commit()
