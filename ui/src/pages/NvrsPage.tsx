import { useEffect, useState } from "react";
import { createNvr, listNvrs, syncNvr } from "../api/nvrs";
import type { Nvr } from "../types";

export function NvrsPage() {
  const [items, setItems] = useState<Nvr[]>([]);
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({ friendly_name: "", host: "", port: 80, brand: "hikvision", username: "", password: "" });

  const load = async () => {
    setLoading(true);
    setItems(await listNvrs());
    setLoading(false);
  };

  useEffect(() => void load(), []);

  const submit = async () => {
    await createNvr(form);
    setForm({ friendly_name: "", host: "", port: 80, brand: "hikvision", username: "", password: "" });
    await load();
  };

  return (
    <section className="page">
      <h2>NVRs</h2>
      <div className="form-grid">
        <input placeholder="Nombre" value={form.friendly_name} onChange={(e) => setForm({ ...form, friendly_name: e.target.value })} />
        <input placeholder="Host" value={form.host} onChange={(e) => setForm({ ...form, host: e.target.value })} />
        <input placeholder="Puerto" type="number" value={form.port} onChange={(e) => setForm({ ...form, port: Number(e.target.value) })} />
        <select value={form.brand} onChange={(e) => setForm({ ...form, brand: e.target.value })}>
          <option value="hikvision">Hikvision</option>
          <option value="dahua">Dahua</option>
        </select>
        <input placeholder="Usuario" value={form.username} onChange={(e) => setForm({ ...form, username: e.target.value })} />
        <input placeholder="Contraseña" type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
        <button onClick={submit}>Guardar NVR</button>
      </div>
      {loading ? <p>Cargando...</p> : (
        <ul>
          {items.map((nvr) => (
            <li key={nvr.id}>
              {nvr.friendly_name} ({nvr.brand}) - {nvr.host}
              <button onClick={() => syncNvr(nvr.id)}>Sincronizar canales</button>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
