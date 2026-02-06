import { useEffect, useMemo, useState } from "react";
import { channelsByNvr, listNvrs, startLive } from "../api/nvrs";
import { HlsPlayer } from "../components/player/HlsPlayer";
import { RecordingsPanel } from "../components/RecordingsPanel";
import type { Channel, Nvr } from "../types";

export function CamerasPage() {
  const [nvrs, setNvrs] = useState<Nvr[]>([]);
  const [channels, setChannels] = useState<Channel[]>([]);
  const [selectedNvr, setSelectedNvr] = useState<number | undefined>();
  const [selected, setSelected] = useState<Channel | null>(null);
  const [hlsUrl, setHlsUrl] = useState<string>();
  const [grid, setGrid] = useState(4);

  useEffect(() => {
    listNvrs().then((data) => {
      setNvrs(data);
      if (data[0]) setSelectedNvr(data[0].id);
    });
  }, []);

  useEffect(() => {
    if (!selectedNvr) return;
    channelsByNvr(selectedNvr).then(setChannels);
  }, [selectedNvr]);

  const visible = useMemo(() => channels.slice(0, grid), [channels, grid]);

  const openLive = async (channel: Channel) => {
    setSelected(channel);
    const response = await startLive(channel.nvr_id, channel.external_id);
    setHlsUrl(`${import.meta.env.VITE_API_URL ?? "http://localhost:8000"}${response.hls_url}`);
  };

  return (
    <section className="page">
      <h2>Cámaras</h2>
      <div className="toolbar">
        <select onChange={(e) => setSelectedNvr(Number(e.target.value))} value={selectedNvr}>
          {nvrs.map((nvr) => <option key={nvr.id} value={nvr.id}>{nvr.friendly_name}</option>)}
        </select>
        <select onChange={(e) => setGrid(Number(e.target.value))} value={grid}>
          {[1, 4, 9, 16].map((g) => <option key={g} value={g}>{g} vistas</option>)}
        </select>
      </div>
      <div className="layout">
        <div className="grid" style={{ gridTemplateColumns: `repeat(${Math.sqrt(grid)}, 1fr)` }}>
          {visible.map((ch) => (
            <button key={ch.id} className="tile" onClick={() => openLive(ch)}>
              <span>{ch.name}</span>
              <small>{ch.status}</small>
            </button>
          ))}
        </div>
        <div>
          <HlsPlayer url={hlsUrl} />
          {selected && <RecordingsPanel nvrId={selected.nvr_id} channelId={selected.external_id} onPlaybackUrl={setHlsUrl} />}
        </div>
      </div>
    </section>
  );
}
