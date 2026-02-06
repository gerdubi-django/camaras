from __future__ import annotations
from dataclasses import dataclass
from datetime import datetime
from typing import Protocol


@dataclass
class DriverChannel:
    external_id: str
    name: str
    status: str = "unknown"


@dataclass
class RecordingItem:
    channel_id: str
    start: datetime
    end: datetime


class NVRDriver(Protocol):
    def test_connection(self) -> bool: ...
    def list_devices_or_channels(self) -> list[DriverChannel]: ...
    def get_live_stream(self, channel_id: str, profile: str = "main") -> str: ...
    def search_recordings(self, channel_id: str, start: datetime, end: datetime) -> list[RecordingItem]: ...
    def get_playback_stream(self, channel_id: str, start: datetime, end: datetime) -> str: ...
    def download_clip(self, channel_id: str, start: datetime, end: datetime) -> str | None: ...
