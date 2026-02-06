import os

os.environ["DATABASE_URL"] = "sqlite+pysqlite:///:memory:"
os.environ["FERNET_KEY"] = "QJj5f6sktbyQLaWB0XjYjRleYkEbe9fuGyM7U50D8C0="

from fastapi.testclient import TestClient
from app.main import app


def test_healthcheck() -> None:
    client = TestClient(app)
    response = client.get("/health")
    assert response.status_code == 200
    assert response.json()["status"] == "ok"
