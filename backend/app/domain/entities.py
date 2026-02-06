from enum import Enum


class NVRBrand(str, Enum):
    HIKVISION = "hikvision"
    DAHUA = "dahua"


class UserRole(str, Enum):
    ADMIN = "admin"
    OPERATOR = "operator"
