/** Skal matche basePath i next.config.ts */
export const APP_BASE_PATH = process.env.NEXT_PUBLIC_BASE_PATH ?? "/gf2";

/** Prefix intern sti med basePath (til almindelige <a>-tags). */
export function withAppBasePath(path: string): string {
  if (path.startsWith("http://") || path.startsWith("https://")) {
    return path;
  }
  const base = APP_BASE_PATH.replace(/\/$/, "");
  const normalized = path.startsWith("/") ? path : `/${path}`;
  if (normalized === base || normalized.startsWith(`${base}/`)) {
    return normalized;
  }
  return `${base}${normalized}`;
}
