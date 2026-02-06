import { api } from "./client";
import type { Channel, Nvr } from "../types";

export const listNvrs = async () => (await api.get<Nvr[]>("/nvrs")).data;
export const createNvr = async (payload: Record<string, unknown>) => (await api.post<Nvr>("/nvrs", payload)).data;
export const syncNvr = async (id: number) => (await api.post(`/nvrs/${id}/sync`)).data;
export const channelsByNvr = async (id: number) => (await api.get<Channel[]>(`/nvrs/${id}/channels`)).data;
export const startLive = async (nvr_id: number, channel_id: string) => (await api.post("/streams/live", { nvr_id, channel_id })).data;
export const searchRecordings = async (payload: Record<string, unknown>) => (await api.post("/recordings/search", payload)).data;
export const startPlayback = async (payload: Record<string, unknown>) => (await api.post("/streams/playback", payload)).data;
