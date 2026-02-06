from pathlib import Path
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from app.api.routers import nvrs, recordings, streams, ws
from app.core.logging import configure_logging
from app.core.settings import get_settings
from app.infra.db.base import Base
from app.infra.db.session import engine
from app.services.stream_manager import StreamManager
from app.ws.manager import WSManager

settings = get_settings()
configure_logging()
Base.metadata.create_all(bind=engine)

app = FastAPI(title=settings.app_name)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

Path(settings.hls_root).mkdir(parents=True, exist_ok=True)
app.mount("/hls", StaticFiles(directory=settings.hls_root), name="hls")

app.include_router(nvrs.router)
app.include_router(streams.router)
app.include_router(recordings.router)
app.include_router(ws.router)


@app.on_event("startup")
async def on_startup() -> None:
    ws_manager = WSManager()
    app.state.ws_manager = ws_manager
    app.state.stream_manager = StreamManager(ws_manager)


@app.get("/health")
def healthcheck() -> dict[str, str]:
    return {"status": "ok"}
