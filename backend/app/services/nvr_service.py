from datetime import datetime
from sqlalchemy.orm import Session
from fastapi import HTTPException
from app.adapters.drivers.factory import build_driver
from app.domain.entities import NVRBrand
from app.infra.security.crypto import CredentialCipher
from app.schemas.stream import RecordingSearchRequest
from app.services.repositories import ChannelRepository, NVRRepository


class NVRService:
    def __init__(self, db: Session) -> None:
        self.db = db
        self.nvrs = NVRRepository(db)
        self.channels = ChannelRepository(db)
        self.cipher = CredentialCipher()

    def decrypt_password(self, encrypted: str) -> str:
        return self.cipher.decrypt(encrypted)

    def build_driver(self, brand: NVRBrand, host: str, port: int, username: str, encrypted_password: str):
        return build_driver(brand, host, port, username, self.decrypt_password(encrypted_password))

    def sync_channels(self, nvr_id: int) -> list[dict]:
        nvr = self.nvrs.get(nvr_id)
        if not nvr:
            raise HTTPException(status_code=404, detail="NVR not found")
        driver = self.build_driver(nvr.brand, nvr.host, nvr.port, nvr.username, nvr.encrypted_password)
        channels = driver.list_devices_or_channels()
        data = [{"external_id": c.external_id, "name": c.name, "status": c.status} for c in channels]
        self.channels.replace_nvr_channels(nvr_id, data)
        return data

    def search_recordings(self, request: RecordingSearchRequest) -> list[dict]:
        nvr = self.nvrs.get(request.nvr_id)
        if not nvr:
            raise HTTPException(status_code=404, detail="NVR not found")
        driver = self.build_driver(nvr.brand, nvr.host, nvr.port, nvr.username, nvr.encrypted_password)
        items = driver.search_recordings(request.channel_id, request.start, request.end)
        return [{"channel_id": i.channel_id, "start": i.start, "end": i.end} for i in items]

    def get_live_rtsp(self, nvr_id: int, channel_id: str, profile: str) -> str:
        nvr = self.nvrs.get(nvr_id)
        if not nvr:
            raise HTTPException(status_code=404, detail="NVR not found")
        driver = self.build_driver(nvr.brand, nvr.host, nvr.port, nvr.username, nvr.encrypted_password)
        return driver.get_live_stream(channel_id, profile)

    def get_playback_rtsp(self, nvr_id: int, channel_id: str, start: datetime, end: datetime) -> str:
        nvr = self.nvrs.get(nvr_id)
        if not nvr:
            raise HTTPException(status_code=404, detail="NVR not found")
        driver = self.build_driver(nvr.brand, nvr.host, nvr.port, nvr.username, nvr.encrypted_password)
        return driver.get_playback_stream(channel_id, start, end)
