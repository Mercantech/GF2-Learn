"use client";

import dynamic from "next/dynamic";
import { useTheme } from "next-themes";
import { useCallback, useMemo } from "react";
import type { ExtendedRecordMap } from "notion-types";
import { resolveNotionPageHref } from "@/lib/notion-urls";
import { NotionPageLink } from "./notion-page-link";
import "react-notion-x/src/styles.css";

const NotionRenderer = dynamic(
  () => import("react-notion-x").then((m) => m.NotionRenderer),
  { ssr: true }
);

const NotionCode = dynamic(
  () => import("./notion-code").then((m) => m.NotionCode),
  { ssr: true }
);

interface NotionRendererViewProps {
  recordMap: ExtendedRecordMap;
  rootPageId?: string;
  lessonPathsByNotionId?: Record<string, string>;
}

export function NotionRendererView({
  recordMap,
  rootPageId,
  lessonPathsByNotionId,
}: NotionRendererViewProps) {
  const { resolvedTheme } = useTheme();
  const isDark = resolvedTheme === "dark";

  const mapPageUrl = useCallback(
    (pageId: string) =>
      resolveNotionPageHref(pageId, lessonPathsByNotionId),
    [lessonPathsByNotionId]
  );

  const components = useMemo(
    () => ({
      PageLink: NotionPageLink,
      Code: NotionCode,
    }),
    []
  );

  return (
    <div className="notion-page-content">
      <NotionRenderer
        recordMap={recordMap}
        fullPage={false}
        darkMode={isDark}
        mapPageUrl={mapPageUrl}
        components={components}
        rootPageId={rootPageId}
      />
    </div>
  );
}
