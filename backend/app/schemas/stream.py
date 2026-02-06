from datetime import datetime
from pydantic import BaseModel


class LiveStreamRequest(BaseModel):
    nvr_id: int
    channel_id: str
    profile: str = "main"


class PlaybackStreamRequest(BaseModel):
    nvr_id: int
    channel_id: str
    start: datetime
    end: datetime


class StreamResponse(BaseModel):
    stream_id: str
    hls_url: str


class RecordingSearchRequest(BaseModel):
    nvr_id: int
    channel_id: str
    start: datetime
    end: datetime


class RecordingItemSchema(BaseModel):
    channel_id: str
    start: datetime
    end: datetime
