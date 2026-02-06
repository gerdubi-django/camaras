import { useState } from "react";
import { searchRecordings, startPlayback } from "../api/nvrs";

interface Props {
  nvrId: number;
  channelId: string;
  onPlaybackUrl: (url: string) => void;
}

export function RecordingsPanel({ nvrId, channelId, onPlaybackUrl }: Props) {
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [result, setResult] = useState<any[]>([]);

  const search = async () => {
    const items = await searchRecordings({ nvr_id: nvrId, channel_id: channelId, start, end });
    setResult(items);
  };

  const play = async () => {
    const response = await startPlayback({ nvr_id: nvrId, channel_id: channelId, start, end });
    onPlaybackUrl(`${import.meta.env.VITE_API_URL ?? "http://localhost:8000"}${response.hls_url}`);
  };

  return (
    <aside className="panel">
      <h3>Grabaciones</h3>
      <input type="datetime-local" value={start} onChange={(e) => setStart(e.target.value)} />
      <input type="datetime-local" value={end} onChange={(e) => setEnd(e.target.value)} />
      <button onClick={search}>Buscar</button>
      <button onClick={play}>Reproducir rango</button>
      <ul>{result.map((item, idx) => <li key={idx}>{item.start} - {item.end}</li>)}</ul>
    </aside>
  );
}
