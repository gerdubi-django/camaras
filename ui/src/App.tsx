import { Link, Route, Routes } from "react-router-dom";
import { NvrsPage } from "./pages/NvrsPage";
import { CamerasPage } from "./pages/CamerasPage";

export function App() {
  return (
    <div>
      <nav className="topbar">
        <h1>NVR Multi-Viewer</h1>
        <div>
          <Link to="/">NVRs</Link>
          <Link to="/camaras">Cámaras</Link>
        </div>
      </nav>
      <Routes>
        <Route path="/" element={<NvrsPage />} />
        <Route path="/camaras" element={<CamerasPage />} />
      </Routes>
    </div>
  );
}
