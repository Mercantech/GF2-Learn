"use client";

import type { ExtendedRecordMap } from "notion-types";
import { NotionRendererView } from "./notion-renderer";

interface NotionPageProps {
  recordMap: ExtendedRecordMap;
  rootPageId?: string;
  lessonPathsByNotionId?: Record<string, string>;
}

export function NotionPage({
  recordMap,
  rootPageId,
  lessonPathsByNotionId,
}: NotionPageProps) {
  return (
    <NotionRendererView
      recordMap={recordMap}
      rootPageId={rootPageId}
      lessonPathsByNotionId={lessonPathsByNotionId}
    />
  );
}
