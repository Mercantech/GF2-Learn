import { unstable_cache } from "next/cache";
import { NotionAPI } from "notion-client";
import type { ExtendedRecordMap } from "notion-types";
import { normalizeNotionPageId } from "./notion-page-id";

export { normalizeNotionPageId } from "./notion-page-id";

function getNotionClient(): NotionAPI | null {
  const token = process.env.NOTION_API_KEY;
  if (!token) return null;
  return new NotionAPI({ authToken: token });
}

async function fetchNotionPageUncached(
  pageId: string
): Promise<ExtendedRecordMap> {
  const client = getNotionClient();
  if (!client) {
    throw new Error("NOTION_API_KEY er ikke konfigureret");
  }
  return client.getPage(normalizeNotionPageId(pageId));
}

export async function getNotionPage(
  pageId: string
): Promise<ExtendedRecordMap> {
  const revalidateSeconds = Number(process.env.NOTION_REVALIDATE_SECONDS ?? 120);

  return unstable_cache(
    () => fetchNotionPageUncached(pageId),
    [`notion-page-${pageId}`],
    { revalidate: revalidateSeconds }
  )();
}

export function isNotionConfigured(): boolean {
  return Boolean(process.env.NOTION_API_KEY);
}
