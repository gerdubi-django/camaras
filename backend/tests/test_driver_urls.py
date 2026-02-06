from datetime import datetime, UTC
from app.adapters.drivers.dahua import DahuaDriver
from app.adapters.drivers.hikvision import HikvisionDriver


def test_hikvision_rtsp_builder() -> None:
    driver = HikvisionDriver("10.0.0.10", 80, "admin", "secret")
    assert driver.get_live_stream("101", "main") == "rtsp://admin:secret@10.0.0.10:554/Streaming/Channels/10101"


def test_dahua_rtsp_builder() -> None:
    driver = DahuaDriver("10.0.0.20", 80, "admin", "secret")
    assert "realmonitor" in driver.get_live_stream("1", "sub")


def test_playback_contains_timerange() -> None:
    start = datetime(2024, 1, 1, 12, 0, tzinfo=UTC)
    end = datetime(2024, 1, 1, 12, 30, tzinfo=UTC)
    url = HikvisionDriver("x", 80, "u", "p").get_playback_stream("101", start, end)
    assert "starttime=" in url and "endtime=" in url
