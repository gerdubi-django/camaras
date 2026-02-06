from cryptography.fernet import Fernet
from app.core.settings import get_settings


class CredentialCipher:
    def __init__(self) -> None:
        settings = get_settings()
        self._fernet = Fernet(settings.fernet_key.encode("utf-8"))

    def encrypt(self, plain_text: str) -> str:
        return self._fernet.encrypt(plain_text.encode("utf-8")).decode("utf-8")

    def decrypt(self, cipher_text: str) -> str:
        return self._fernet.decrypt(cipher_text.encode("utf-8")).decode("utf-8")
