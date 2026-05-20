import type { ExtendedRecordMap } from "notion-types";
import { getBlockTitle } from "notion-utils";
import { getBlock } from "./notion-blocks";
import { normalizeNotionPageId } from "./notion-page-id";

export function getNotionPageTitle(
  recordMap: ExtendedRecordMap,
  pageId: string
): string {
  const id = normalizeNotionPageId(pageId);
  const block = getBlock(recordMap, id);
  if (!block) return "Notion-side";

  const title = getBlockTitle(block, recordMap);
  return title || "Notion-side";
}
