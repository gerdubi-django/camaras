from datetime import datetime
import logging
import requests
from app.adapters.drivers.base import DriverChannel, NVRDriver, RecordingItem

logger = logging.getLogger(__name__)


class DahuaDriver(NVRDriver):
    def __init__(self, host: str, port: int, username: str, password: str) -> None:
        self.host = host
        self.port = port
        self.username = username
        self.password = password

    def test_connection(self) -> bool:
        url = f"http://{self.host}:{self.port}/cgi-bin/magicBox.cgi?action=getSystemInfo"
        try:
            response = requests.get(url, auth=(self.username, self.password), timeout=5)
            return response.status_code < 500
        except requests.RequestException:
            return False

    def list_devices_or_channels(self) -> list[DriverChannel]:
        # This endpoint may vary by model/firmware so it is a best-effort stub.
        url = f"http://{self.host}:{self.port}/cgi-bin/configManager.cgi?action=getConfig&name=ChannelTitle"
        try:
            response = requests.get(url, auth=(self.username, self.password), timeout=5)
            if response.ok:
                # Text parsing can be implemented here when variants are known.
                return []
        except requests.RequestException as exc:
            logger.warning("dahua_list_channels_failed", extra={"error": str(exc)})
        return [DriverChannel(external_id="1", name="Canal 1")]

    def get_live_stream(self, channel_id: str, profile: str = "main") -> str:
        subtype = "0" if profile == "main" else "1"
        return (
            f"rtsp://{self.username}:{self.password}@{self.host}:554/"
            f"cam/realmonitor?channel={channel_id}&subtype={subtype}"
        )

    def search_recordings(self, channel_id: str, start: datetime, end: datetime) -> list[RecordingItem]:
        # Possible variant: CGI file find workflow on /cgi-bin/mediaFileFind.cgi.
        return [RecordingItem(channel_id=channel_id, start=start, end=end)]

    def get_playback_stream(self, channel_id: str, start: datetime, end: datetime) -> str:
        start_iso = start.strftime("%Y-%m-%d %H:%M:%S")
        end_iso = end.strftime("%Y-%m-%d %H:%M:%S")
        return (
            f"rtsp://{self.username}:{self.password}@{self.host}:554/"
            f"cam/playback?channel={channel_id}&starttime={start_iso}&endtime={end_iso}"
        )

    def download_clip(self, channel_id: str, start: datetime, end: datetime) -> str | None:
        return None
