import type { Block, ExtendedRecordMap } from "notion-types";

type BlockEntry = { value?: unknown } | Block;

/** Notion API returnerer ofte { value: { value: Block } } — unwrap til Block. */
export function unwrapNotionBlock(entry: BlockEntry | undefined): Block | null {
  if (!entry) return null;

  let node: unknown = entry;
  for (let depth = 0; depth < 6; depth++) {
    if (!node || typeof node !== "object") return null;

    if ("type" in node && typeof (node as Block).type === "string") {
      return node as Block;
    }

    if ("value" in node && (node as { value?: unknown }).value) {
      node = (node as { value: unknown }).value;
      continue;
    }

    return null;
  }

  return null;
}

export function getBlock(
  recordMap: ExtendedRecordMap,
  blockId: string
): Block | null {
  const compact = blockId.replace(/-/g, "");
  const key =
    recordMap.block[blockId] !== undefined
      ? blockId
      : Object.keys(recordMap.block).find(
          (k) => k.replace(/-/g, "") === compact
        );

  if (!key) return null;
  return unwrapNotionBlock(recordMap.block[key] as BlockEntry);
}

function findPageBlock(
  recordMap: ExtendedRecordMap,
  pageId: string
): Block | null {
  const direct = getBlock(recordMap, pageId);
  if (
    direct?.type === "page" ||
    direct?.type === "collection_view_page"
  ) {
    return direct;
  }

  const compact = pageId.replace(/-/g, "");
  for (const key of Object.keys(recordMap.block)) {
    if (key.replace(/-/g, "") !== compact) continue;
    const block = getBlock(recordMap, key);
    if (
      block &&
      (block.type === "page" || block.type === "collection_view_page")
    ) {
      return block;
    }
  }

  return direct;
}

/** Rod-indhold: side → collection_view_page hvis siden er tom. */
export function getPageContentIds(
  recordMap: ExtendedRecordMap,
  pageId: string
): string[] {
  const page = findPageBlock(recordMap, pageId);
  if (page?.content?.length) {
    return page.content;
  }

  const compact = pageId.replace(/-/g, "");
  for (const key of Object.keys(recordMap.block)) {
    const block = getBlock(recordMap, key);
    if (block?.type !== "collection_view_page") continue;
    if (key.replace(/-/g, "").startsWith(compact.slice(0, 8))) {
      if (block.content?.length) return block.content;
    }
  }

  for (const key of Object.keys(recordMap.block)) {
    const block = getBlock(recordMap, key);
    if (block?.type === "collection_view_page" && block.content?.length) {
      return block.content;
    }
  }

  return [];
}

/** Notion H1: heading_1 eller legacy header. */
export function isNotionH1Block(block: Block | null): boolean {
  if (!block) return false;
  const type = block.type as string;
  return type === "heading_1" || type === "header";
}

/** Notion H2: heading_2 eller legacy sub_header. */
export function isNotionH2Block(block: Block | null): boolean {
  if (!block) return false;
  const type = block.type as string;
  return type === "heading_2" || type === "sub_header";
}

/** Toggle med titel — bruges som kapitel på nogle Mercantec-sider. */
export function isNotionToggleBlock(block: Block | null): boolean {
  if (!block) return false;
  return (block.type as string) === "toggle";
}

export function resolveRecordMapBlockKey(
  recordMap: ExtendedRecordMap,
  blockId: string
): string | null {
  const compact = blockId.replace(/-/g, "");
  if (recordMap.block[blockId]) return blockId;
  return (
    Object.keys(recordMap.block).find(
      (k) => k.replace(/-/g, "") === compact
    ) ?? null
  );
}

export function isAncestorBlock(
  recordMap: ExtendedRecordMap,
  ancestorId: string,
  descendantId: string
): boolean {
  if (ancestorId === descendantId) return false;
  const visited = new Set<string>();
  const stack = [ancestorId];
  while (stack.length > 0) {
    const id = stack.pop()!;
    if (id === descendantId) return true;
    if (visited.has(id)) continue;
    visited.add(id);
    const block = getBlock(recordMap, id);
    for (const childId of block?.content ?? []) {
      stack.push(childId);
    }
  }
  return false;
}

/** Blokke der starter sektionen (ikke under anden blok i samme sektion). */
export function getSectionRootBlockIds(
  recordMap: ExtendedRecordMap,
  sectionBlockIds: string[]
): string[] {
  const unique = [...new Set(sectionBlockIds)];
  return unique.filter(
    (id) =>
      !unique.some(
        (otherId) => otherId !== id && isAncestorBlock(recordMap, otherId, id)
      )
  );
}

/** Sæt page.content på den rigtige nested Notion-blok. */
export function setPageContentIds(
  recordMap: ExtendedRecordMap,
  pageId: string,
  contentIds: string[]
): void {
  const key = resolveRecordMapBlockKey(recordMap, pageId);
  if (!key) return;

  const entry = recordMap.block[key];
  if (!entry || typeof entry !== "object") return;

  const applyContent = (node: Record<string, unknown>) => {
    const type = node.type as string | undefined;
    if (type === "page" || type === "collection_view_page") {
      node.content = contentIds;
    }
  };

  const walk = (node: unknown): void => {
    if (!node || typeof node !== "object") return;

    if ("type" in node && typeof (node as Block).type === "string") {
      applyContent(node as Record<string, unknown>);
    }

    if ("value" in node && (node as { value?: unknown }).value) {
      walk((node as { value: unknown }).value);
    }
  };

  walk(entry);
}
