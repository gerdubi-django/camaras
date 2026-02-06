# NVR Multi-Viewer (Base MVP)

Base funcional para operar múltiples NVRs (Hikvision/Dahua) con arquitectura hexagonal y gateway RTSP -> HLS.

## 1) Diseño / arquitectura

- **Patrón**: Clean Architecture / Hexagonal.
- **Core dominio (`backend/app/domain`)**: enums y contratos de negocio.
- **Adapters (`backend/app/adapters`)**: drivers por marca con interfaz común (`NVRDriver`).
- **Services (`backend/app/services`)**: casos de uso (sync, streams, recordings) y orquestación.
- **Infra (`backend/app/infra`)**: SQLAlchemy, seguridad (Fernet), sesión DB.
- **API (`backend/app/api`)**: FastAPI REST + WebSocket para eventos.
- **UI (`ui/src`)**: React + Vite + base Tauri, textos en español.
- **Gateway de streaming**: `StreamManager` inicia procesos FFmpeg async y publica HLS local.

### Drivers iniciales

- `HikvisionDriver`
  - URL RTSP live (main/sub).
  - Búsqueda/listado por **stubs** ISAPI documentados (sin asumir endpoint universal por firmware).
- `DahuaDriver`
  - URL RTSP live (main/sub).
  - Búsqueda/listado por **stubs** CGI/HTTP documentados.

## 2) Árbol de archivos

```text
.
├── backend
│   ├── Dockerfile
│   ├── alembic
│   │   ├── env.py
│   │   ├── script.py.mako
│   │   └── versions
│   │       └── 0001_init.py
│   ├── alembic.ini
│   ├── app
│   │   ├── api
│   │   │   ├── deps.py
│   │   │   └── routers
│   │   │       ├── nvrs.py
│   │   │       ├── recordings.py
│   │   │       ├── streams.py
│   │   │       └── ws.py
│   │   ├── adapters
│   │   │   └── drivers
│   │   │       ├── base.py
│   │   │       ├── dahua.py
│   │   │       ├── factory.py
│   │   │       └── hikvision.py
│   │   ├── core
│   │   │   ├── logging.py
│   │   │   └── settings.py
│   │   ├── domain
│   │   │   └── entities.py
│   │   ├── infra
│   │   │   ├── db
│   │   │   │   ├── base.py
│   │   │   │   ├── models.py
│   │   │   │   └── session.py
│   │   │   └── security
│   │   │       └── crypto.py
│   │   ├── main.py
│   │   ├── schemas
│   │   │   ├── nvr.py
│   │   │   └── stream.py
│   │   ├── services
│   │   │   ├── nvr_service.py
│   │   │   ├── repositories.py
│   │   │   └── stream_manager.py
│   │   └── ws
│   │       └── manager.py
│   ├── pyproject.toml
│   └── tests
│       ├── test_api_smoke.py
│       └── test_driver_urls.py
├── docker-compose.yml
├── shared
│   └── README.md
└── ui
    ├── Dockerfile
    ├── index.html
    ├── package.json
    ├── src
    │   ├── App.tsx
    │   ├── api
    │   │   ├── client.ts
    │   │   └── nvrs.ts
    │   ├── components
    │   │   ├── RecordingsPanel.tsx
    │   │   └── player
    │   │       └── HlsPlayer.tsx
    │   ├── main.tsx
    │   ├── pages
    │   │   ├── CamerasPage.tsx
    │   │   └── NvrsPage.tsx
    │   ├── styles.css
    │   └── types
    │       └── index.ts
    ├── src-tauri
    │   └── tauri.conf.json
    ├── tsconfig.json
    └── vite.config.ts
```

## 3) Endpoints y eventos implementados

### REST
- `POST /nvrs`
- `GET /nvrs`
- `PUT /nvrs/{id}`
- `DELETE /nvrs/{id}`
- `POST /nvrs/{id}/sync`
- `GET /nvrs/{id}/channels`
- `POST /streams/live`
- `POST /streams/playback`
- `POST /recordings/search`

### WebSocket
- `GET ws://host:8000/ws/status`
- Eventos emitidos:
  - `stream_started`
  - `stream_stopped`
  - `stream_error` (hook preparado para extensión)
  - `nvr_online` / `nvr_offline` (hook preparado para health checks periódicos)

## 4) Seguridad

- Credenciales cifradas en DB con `Fernet` (`CredentialCipher`).
- Roles por header `X-Role` con `admin/operator`.
- Auditoría en `audit_logs` para arranque de live/playback.
- No se registran passwords en logs.

## 5) Ejecución paso a paso

### Requisitos
- Docker + Docker Compose.
- Generar `FERNET_KEY`:

```bash
python -c "from cryptography.fernet import Fernet; print(Fernet.generate_key().decode())"
```

### Levantar entorno

```bash
export FERNET_KEY='TU_KEY'
docker compose up --build
```

- API: `http://localhost:8000`
- UI: `http://localhost:5173`
- HLS servido por API en `/hls/<stream_id>/index.m3u8`

### Flujo funcional MVP
1. Crear NVR desde UI (pantalla **NVRs**).
2. Sincronizar canales (`/nvrs/{id}/sync`) con fallback/stub si el endpoint varía.
3. Ir a **Cámaras**, seleccionar canal y abrir live (`/streams/live`).
4. Abrir panel **Grabaciones**, buscar por rango (`/recordings/search`) y reproducir (`/streams/playback`).

## 6) Notas de compatibilidad real

- Los endpoints HTTP de Hikvision/Dahua cambian por firmware/línea; en esta base están modelados como stubs "best effort" + fallback RTSP configurable.
- No se integra Hik-Connect ni Dahua P2P en esta fase (solo LAN/WAN directo).

## 7) Próximas mejoras

1. WebRTC gateway opcional para menor latencia.
2. Descarga de clips (`download_clip`) y gestión de archivos.
3. ONVIF discovery y normalización multi-marca.
4. Polling/scheduler de estado NVR para eventos online/offline reales.
5. Métricas Prometheus + trazas OpenTelemetry.
6. Grupos de cámaras y layouts persistentes por usuario.
7. Hardening de auth (JWT/OAuth2) y RBAC granular.
