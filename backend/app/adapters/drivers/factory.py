from app.adapters.drivers.base import NVRDriver
from app.adapters.drivers.dahua import DahuaDriver
from app.adapters.drivers.hikvision import HikvisionDriver
from app.domain.entities import NVRBrand


def build_driver(brand: NVRBrand, host: str, port: int, username: str, password: str) -> NVRDriver:
    if brand == NVRBrand.HIKVISION:
        return HikvisionDriver(host, port, username, password)
    if brand == NVRBrand.DAHUA:
        return DahuaDriver(host, port, username, password)
    raise ValueError(f"Unsupported brand: {brand}")
