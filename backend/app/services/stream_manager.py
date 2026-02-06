from __future__ import annotations
import asyncio
import os
import shutil
from dataclasses import dataclass
from pathlib import Path
from uuid import uuid4
from app.core.settings import get_settings
from app.ws.manager import WSManager


@dataclass
class RunningStream:
    stream_id: str
    hls_url: str
    process: asyncio.subprocess.Process


class StreamManager:
    def __init__(self, ws_manager: WSManager) -> None:
        self.settings = get_settings()
        self.ws_manager = ws_manager
        self.running: dict[str, RunningStream] = {}
        Path(self.settings.hls_root).mkdir(parents=True, exist_ok=True)

    async def start_stream(self, rtsp_url: str, kind: str) -> tuple[str, str]:
        if len(self.running) >= self.settings.max_concurrent_streams:
            raise RuntimeError("Maximum stream limit reached")
        stream_id = str(uuid4())
        stream_path = Path(self.settings.hls_root) / stream_id
        stream_path.mkdir(parents=True, exist_ok=True)
        playlist = stream_path / "index.m3u8"
        cmd = [
            self.settings.ffmpeg_binary,
            "-rtsp_transport",
            "tcp",
            "-i",
            rtsp_url,
            "-c:v",
            "libx264",
            "-preset",
            "veryfast",
            "-g",
            "48",
            "-hls_time",
            "2",
            "-hls_list_size",
            "6",
            "-hls_flags",
            "delete_segments+append_list",
            str(playlist),
        ]
        process = await asyncio.create_subprocess_exec(*cmd, stdout=asyncio.subprocess.PIPE, stderr=asyncio.subprocess.PIPE)
        hls_url = f"/hls/{stream_id}/index.m3u8"
        self.running[stream_id] = RunningStream(stream_id=stream_id, hls_url=hls_url, process=process)
        await self.ws_manager.broadcast("stream_started", {"stream_id": stream_id, "kind": kind, "hls_url": hls_url})
        return stream_id, hls_url

    async def stop_stream(self, stream_id: str) -> None:
        stream = self.running.get(stream_id)
        if not stream:
            return
        if stream.process.returncode is None:
            stream.process.terminate()
            await stream.process.wait()
        del self.running[stream_id]
        shutil.rmtree(os.path.join(self.settings.hls_root, stream_id), ignore_errors=True)
        await self.ws_manager.broadcast("stream_stopped", {"stream_id": stream_id})
