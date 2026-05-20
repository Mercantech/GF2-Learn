import { withAppBasePath } from "./base-path";
import { normalizeNotionPageId } from "./notion-page-id";

/** Relativ sti til Notion-viewer (uden basePath — til next/link). */
export function getNotionViewerPath(pageId: string): string {
  return `/notion/${normalizeNotionPageId(pageId)}`;
}

/** Fuld sti inkl. /gf2 (til almindelige <a href>). */
export function getNotionViewerHref(pageId: string): string {
  return withAppBasePath(getNotionViewerPath(pageId));
}

/**
 * mapPageUrl til react-notion-x.
 * Returnerer fuld sti med /gf2, da PageLink ofte renderer som <a> uden next/link.
 */
export function resolveNotionPageHref(
  pageId: string,
  lessonPathsByNotionId?: Record<string, string>
): string {
  const id = normalizeNotionPageId(pageId);
  const relative =
    lessonPathsByNotionId?.[id] ?? getNotionViewerPath(id);
  return withAppBasePath(relative);
}
