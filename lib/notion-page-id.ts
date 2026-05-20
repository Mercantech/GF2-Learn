/** Accepterer UUID, URL-slug+UUID (fx fra Notion-URL) eller id med bindestreger. */
export function normalizeNotionPageId(pageId: string): string {
  const compact = pageId.trim().replace(/-/g, "");
  const uuidMatch = compact.match(/([a-f0-9]{32})$/i);
  return uuidMatch ? uuidMatch[1] : compact;
}
