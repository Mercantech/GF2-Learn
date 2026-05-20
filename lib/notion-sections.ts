import { unstable_cache } from "next/cache";
import type { Block, ExtendedRecordMap } from "notion-types";
import { getBlockTitle } from "notion-utils";
import {
  getBlock,
  getPageContentIds,
  getSectionRootBlockIds,
  isNotionH1Block,
  isNotionH2Block,
  isNotionToggleBlock,
  resolveRecordMapBlockKey,
  setPageContentIds,
} from "./notion-blocks";
import { getNotionPage } from "./notion";
import { normalizeNotionPageId } from "./notion-page-id";

export interface NotionH1Section {
  slug: string;
  title: string;
  /** Alle blok-ID'er i sektionen (flad liste). */
  blockIds: string[];
}

function slugifyTitle(title: string): string {
  const base = title
    .toLowerCase()
    .replace(/æ/g, "ae")
    .replace(/ø/g, "oe")
    .replace(/å/g, "aa")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-|-$/g, "")
    .slice(0, 64);
  return base || "sektion";
}

function uniqueSlug(base: string, used: Set<string>): string {
  let slug = base;
  let n = 2;
  while (used.has(slug)) {
    slug = `${base}-${n}`;
    n += 1;
  }
  used.add(slug);
  return slug;
}

function collectDescendantIds(
  recordMap: ExtendedRecordMap,
  blockId: string,
  into: Set<string>
): void {
  into.add(blockId);
  const block = getBlock(recordMap, blockId);
  if (!block?.content?.length) return;
  for (const childId of block.content) {
    collectDescendantIds(recordMap, childId, into);
  }
}

/** Pre-order gennem hele siden (inkl. kolonner, toggles osv.). */
function collectBlocksInDocumentOrder(
  recordMap: ExtendedRecordMap,
  pageId: string
): string[] {
  const ordered: string[] = [];
  const visit = (blockId: string) => {
    ordered.push(blockId);
    const block = getBlock(recordMap, blockId);
    for (const childId of block?.content ?? []) {
      visit(childId);
    }
  };
  for (const id of getPageContentIds(recordMap, pageId)) {
    visit(id);
  }
  return ordered;
}

function parseSectionsByHeading(
  recordMap: ExtendedRecordMap,
  pageId: string,
  isHeading: (block: Block | null) => boolean
): NotionH1Section[] {
  const normalizedPageId = normalizeNotionPageId(pageId);
  const orderedIds = collectBlocksInDocumentOrder(recordMap, normalizedPageId);
  const usedSlugs = new Set<string>();
  const sections: NotionH1Section[] = [];

  let pendingBeforeFirst: string[] = [];
  let current: { title: string; blockIds: string[] } | null = null;

  const flushCurrent = () => {
    if (!current) return;
    const slug = uniqueSlug(slugifyTitle(current.title), usedSlugs);
    sections.push({ slug, title: current.title, blockIds: current.blockIds });
    current = null;
  };

  for (const blockId of orderedIds) {
    const block = getBlock(recordMap, blockId);
    if (isHeading(block)) {
      flushCurrent();
      const title = block
        ? getBlockTitle(block, recordMap)?.trim() || "Uden titel"
        : "Uden titel";
      current = {
        title,
        blockIds: [...pendingBeforeFirst, blockId],
      };
      pendingBeforeFirst = [];
    } else if (current) {
      current.blockIds.push(blockId);
    } else {
      pendingBeforeFirst.push(blockId);
    }
  }

  flushCurrent();

  if (sections.length === 0) {
    return [];
  }

  if (pendingBeforeFirst.length > 0 && sections[0]) {
    sections[0].blockIds = [...pendingBeforeFirst, ...sections[0].blockIds];
  }

  return sections;
}

/** Udtræk sektioner ved H1; fallback til H2 hvis kun én H1-sektion. */
export function parseH1Sections(
  recordMap: ExtendedRecordMap,
  pageId: string
): NotionH1Section[] {
  const normalizedPageId = normalizeNotionPageId(pageId);
  const h1Sections = parseSectionsByHeading(
    recordMap,
    normalizedPageId,
    isNotionH1Block
  );

  if (h1Sections.length > 1) {
    return h1Sections;
  }

  const h2Sections = parseSectionsByHeading(
    recordMap,
    normalizedPageId,
    isNotionH2Block
  );
  if (h2Sections.length > 1) {
    return h2Sections;
  }

  const toggleSections = parseSectionsByHeading(
    recordMap,
    normalizedPageId,
    isNotionToggleBlock
  );
  if (toggleSections.length > 1) {
    return toggleSections;
  }

  if (h1Sections.length === 1) {
    return h1Sections;
  }

  if (h2Sections.length === 1) {
    return h2Sections;
  }

  if (toggleSections.length === 1) {
    return toggleSections;
  }

  const contentIds = getPageContentIds(recordMap, normalizedPageId);
  const usedSlugs = new Set<string>();
  const pageBlock = getBlock(recordMap, normalizedPageId);
  const title = pageBlock
    ? getBlockTitle(pageBlock, recordMap)?.trim() || "Indhold"
    : "Indhold";

  return [
    {
      slug: uniqueSlug(slugifyTitle(title), usedSlugs),
      title,
      blockIds: contentIds.length ? contentIds : [normalizedPageId],
    },
  ];
}

/**
 * Klargør recordMap til én sektion: behold alle blokke, men begræns
 * page.content til sektionens rod-blokke (fra overskrift til overskrift).
 */
export function sliceRecordMapForSection(
  recordMap: ExtendedRecordMap,
  pageId: string,
  section: NotionH1Section
): ExtendedRecordMap {
  const pageKey = resolveRecordMapBlockKey(recordMap, pageId);
  if (!pageKey) return recordMap;

  const rootIds = getSectionRootBlockIds(recordMap, section.blockIds);
  const contentIds =
    rootIds.length > 0 ? rootIds : [...section.blockIds];

  const sliced: ExtendedRecordMap = {
    ...recordMap,
    block: { ...recordMap.block },
  };

  setPageContentIds(sliced, pageKey, contentIds);

  return sliced;
}

export function resolveNotionRootPageId(
  recordMap: ExtendedRecordMap,
  pageId: string
): string {
  return resolveRecordMapBlockKey(recordMap, pageId) ?? normalizeNotionPageId(pageId);
}

async function fetchNotionH1Sections(pageId: string): Promise<NotionH1Section[]> {
  const recordMap = await getNotionPage(pageId);
  return parseH1Sections(recordMap, pageId);
}

export async function getNotionH1Sections(
  pageId: string
): Promise<NotionH1Section[]> {
  const normalizedId = normalizeNotionPageId(pageId);
  const revalidateSeconds = Number(process.env.NOTION_REVALIDATE_SECONDS ?? 120);

  return unstable_cache(
    () => fetchNotionH1Sections(normalizedId),
    [`notion-h1-sections-${normalizedId}`],
    { revalidate: revalidateSeconds }
  )();
}

export async function getNotionSectionRecordMap(
  pageId: string,
  sectionSlug: string
): Promise<{
  section: NotionH1Section;
  recordMap: ExtendedRecordMap;
  rootPageId: string;
} | null> {
  const normalizedPageId = normalizeNotionPageId(pageId);
  const recordMap = await getNotionPage(normalizedPageId);
  const sections = parseH1Sections(recordMap, normalizedPageId);
  const section = sections.find((s) => s.slug === sectionSlug);
  if (!section) return null;
  const sliced = sliceRecordMapForSection(recordMap, normalizedPageId, section);
  return {
    section,
    recordMap: sliced,
    rootPageId: resolveNotionRootPageId(sliced, normalizedPageId),
  };
}
