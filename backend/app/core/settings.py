from functools import lru_cache
from pydantic import Field
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", env_file_encoding="utf-8", extra="ignore")

    app_name: str = "NVR Multi-Viewer API"
    environment: str = "dev"
    host: str = "0.0.0.0"
    port: int = 8000
    database_url: str = "postgresql+psycopg://postgres:postgres@db:5432/nvr"
    fernet_key: str = Field(default="", min_length=1)
    hls_root: str = "./hls"
    ffmpeg_binary: str = "ffmpeg"
    max_concurrent_streams: int = 16
    stream_idle_timeout_sec: int = 30


@lru_cache
def get_settings() -> Settings:
    return Settings()
