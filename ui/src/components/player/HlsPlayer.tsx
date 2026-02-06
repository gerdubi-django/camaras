import { useEffect, useRef } from "react";
import Hls from "hls.js";

interface Props {
  url?: string;
}

export function HlsPlayer({ url }: Props) {
  const ref = useRef<HTMLVideoElement>(null);

  useEffect(() => {
    if (!url || !ref.current) return;
    if (Hls.isSupported()) {
      const hls = new Hls();
      hls.loadSource(url);
      hls.attachMedia(ref.current);
      return () => hls.destroy();
    }
    ref.current.src = url;
  }, [url]);

  return <video ref={ref} controls autoPlay muted style={{ width: "100%", background: "black" }} />;
}
