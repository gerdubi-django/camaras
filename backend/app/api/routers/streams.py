from fastapi import APIRouter, Depends, Request
from sqlalchemy.orm import Session
from app.api.deps import require_role
from app.domain.entities import UserRole
from app.infra.db.session import get_db
from app.schemas.stream import LiveStreamRequest, PlaybackStreamRequest, StreamResponse
from app.services.nvr_service import NVRService
from app.services.repositories import AuditRepository
from app.services.stream_manager import StreamManager

router = APIRouter(prefix="/streams", tags=["streams"])


def get_stream_manager(request: Request):
    return request.app.state.stream_manager


@router.post("/live", response_model=StreamResponse)
async def start_live_stream(
    payload: LiveStreamRequest,
    db: Session = Depends(get_db),
    role: str = Depends(require_role(UserRole.OPERATOR)),
    stream_manager: StreamManager = Depends(get_stream_manager),
):
    rtsp = NVRService(db).get_live_rtsp(payload.nvr_id, payload.channel_id, payload.profile)
    stream_id, hls_url = await stream_manager.start_stream(rtsp, "live")
    AuditRepository(db).log("live_stream_started", role, f"nvr={payload.nvr_id},channel={payload.channel_id}")
    return StreamResponse(stream_id=stream_id, hls_url=hls_url)


@router.post("/playback", response_model=StreamResponse)
async def start_playback_stream(
    payload: PlaybackStreamRequest,
    db: Session = Depends(get_db),
    role: str = Depends(require_role(UserRole.OPERATOR)),
    stream_manager: StreamManager = Depends(get_stream_manager),
):
    rtsp = NVRService(db).get_playback_rtsp(payload.nvr_id, payload.channel_id, payload.start, payload.end)
    stream_id, hls_url = await stream_manager.start_stream(rtsp, "playback")
    AuditRepository(db).log("playback_stream_started", role, f"nvr={payload.nvr_id},channel={payload.channel_id}")
    return StreamResponse(stream_id=stream_id, hls_url=hls_url)
