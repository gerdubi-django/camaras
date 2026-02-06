from fastapi import APIRouter, WebSocket, WebSocketDisconnect, Request
from app.ws.manager import WSManager

router = APIRouter(tags=["ws"])


def get_ws_manager(request: Request):
    return request.app.state.ws_manager


@router.websocket("/ws/status")
async def websocket_status(websocket: WebSocket):
    ws_manager: WSManager = websocket.app.state.ws_manager
    await ws_manager.connect(websocket)
    try:
        while True:
            await websocket.receive_text()
    except WebSocketDisconnect:
        ws_manager.disconnect(websocket)
