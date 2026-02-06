export type Brand = "hikvision" | "dahua";

export interface Nvr {
  id: number;
  friendly_name: string;
  host: string;
  port: number;
  brand: Brand;
  username: string;
}

export interface Channel {
  id: number;
  nvr_id: number;
  external_id: string;
  name: string;
  status: string;
}
