import { buildNotionLessonPathMapAsync } from "@/lib/navigation";
import { isNotionConfigured } from "@/lib/notion";
import {
  getNotionSectionRecordMap,
  type NotionH1Section,
} from "@/lib/notion-sections";
import { NotionPage } from "./notion-page";

interface NotionSectionViewProps {
  pageId: string;
  sectionSlug: string;
}

export async function NotionSectionView({
  pageId,
  sectionSlug,
}: NotionSectionViewProps) {
  const lessonPathsByNotionId = await buildNotionLessonPathMapAsync();

  if (!isNotionConfigured()) {
    return (
      <div className="rounded-lg border border-amber-500/40 bg-amber-500/10 p-4 text-sm">
        Notion er ikke konfigureret.
      </div>
    );
  }

  try {
    const data = await getNotionSectionRecordMap(pageId, sectionSlug);
    if (!data) {
      return (
        <div className="rounded-lg border border-red-500/40 bg-red-500/10 p-4 text-sm text-red-400">
          Sektionen findes ikke på Notion-siden.
        </div>
      );
    }

    return (
      <div className="notion-wrapper rounded-lg border border-border bg-card p-4">
        <NotionPage
          recordMap={data.recordMap}
          rootPageId={data.rootPageId}
          lessonPathsByNotionId={lessonPathsByNotionId}
        />
      </div>
    );
  } catch {
    return (
      <div className="rounded-lg border border-red-500/40 bg-red-500/10 p-4 text-sm text-red-400">
        Kunne ikke hente sektionen fra Notion.
      </div>
    );
  }
}

export type { NotionH1Section };
