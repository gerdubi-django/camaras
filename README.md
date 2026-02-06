# NVR Desk (Windows, .NET 8, WPF, LibVLCSharp)

## Justificación de UI elegida
Se usa **WPF + MVVM** porque en .NET 8 para escritorio Windows ofrece alta madurez, tooling sólido en Visual Studio 2022, integración estable con `LibVLCSharp.WPF` y menor riesgo para un MVP de video RTSP multivista.

## Arquitectura
- `NvrDesk.Domain`: entidades, enums, contratos.
- `NvrDesk.Application`: casos de uso, DTOs, validación.
- `NvrDesk.Infrastructure`: EF Core SQLite, migración inicial, cifrado DPAPI, drivers Hikvision/Dahua, repositorios.
- `NvrDesk.Presentation`: app WPF MVVM, vistas en español, celdas de video LibVLCSharp.
- `NvrDesk.Tests`: unit tests de builders RTSP y validaciones.

## Árbol de solución
```text
NvrDesk.sln
├─ NvrDesk.Domain
│  ├─ Contracts
│  ├─ Entities
│  ├─ Enums
│  └─ ValueObjects
├─ NvrDesk.Application
│  ├─ Abstractions
│  ├─ Dtos
│  ├─ UseCases
│  └─ Validation
├─ NvrDesk.Infrastructure
│  ├─ Data
│  ├─ Drivers
│  ├─ Http
│  ├─ Migrations
│  ├─ Repositories
│  └─ Security
├─ NvrDesk.Presentation
│  ├─ App.xaml / App.xaml.cs
│  ├─ Controls
│  ├─ Services
│  ├─ ViewModels
│  └─ Views
└─ NvrDesk.Tests
   ├─ Drivers
   └─ Validation
```

## Requisitos (Windows 100%)
1. Windows 10/11 x64.
2. Visual Studio 2022 (17.8+) con workload `.NET desktop development`.
3. .NET 8 SDK.
4. NuGet restore habilitado.
5. VLC runtime instalado (recomendado VLC 3.x) para `LibVLCSharp`.

## Ejecutar
1. Abrir `NvrDesk.sln` en Visual Studio 2022.
2. Restaurar paquetes NuGet.
3. Definir `NvrDesk.Presentation` como startup project.
4. Ejecutar (`F5`).
5. La base SQLite se crea en `%LOCALAPPDATA%\NvrDesk\nvrdesk.db`.

## Cómo agregar un NVR y ver live
1. Ir a pestaña **NVRs**.
2. Pulsar **Agregar NVR** (crea registro base editable en DB para MVP).
3. Pulsar **Probar conexión** para validar endpoint configurable de la marca.
4. Pulsar **Sincronizar canales** para traer canales (stub seguro configurable).
5. Ir a pestaña **Cámaras** y asignar URLs RTSP en flujo de reproducción (extensión siguiente: selector por celda).

## Notas de drivers (sin inventar endpoints seguros)
- Hikvision y Dahua tienen **stubs HTTP configurables** para pruebas/sincronización inicial.
- Las funciones de búsqueda de grabaciones están implementadas como stub documentado y retornan segmentos sintéticos para habilitar UX MVP.
- Los builders RTSP live/playback sí están implementados de forma determinística.

## Extensión futura ONVIF
- Añadir un nuevo driver `OnvifDriver` que implemente `INvrDriver`.
- Mantener compatibilidad gracias a `INvrDriverFactory` y casos de uso existentes.
