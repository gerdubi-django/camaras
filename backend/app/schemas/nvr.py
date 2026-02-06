from datetime import datetime
from pydantic import BaseModel, Field
from app.domain.entities import NVRBrand


class NVRBase(BaseModel):
    friendly_name: str = Field(min_length=2, max_length=120)
    host: str
    port: int = 80
    brand: NVRBrand
    username: str


class NVRCreate(NVRBase):
    password: str = Field(min_length=1)


class NVRUpdate(BaseModel):
    friendly_name: str | None = None
    host: str | None = None
    port: int | None = None
    brand: NVRBrand | None = None
    username: str | None = None
    password: str | None = None


class NVRRead(NVRBase):
    id: int
    created_at: datetime
    updated_at: datetime

    model_config = {"from_attributes": True}


class ChannelRead(BaseModel):
    id: int
    nvr_id: int
    external_id: str
    name: str
    status: str

    model_config = {"from_attributes": True}
