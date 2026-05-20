import { buildNotionLessonPathMapAsync } from "@/lib/navigation";
import { getNotionPage, isNotionConfigured } from "@/lib/notion";
import { normalizeNotionPageId } from "@/lib/notion-page-id";
import { NotionPage } from "./notion-page";

interface NotionPageWrapperProps {
  pageId: string;
}

export async function NotionPageWrapper({ pageId }: NotionPageWrapperProps) {
  const normalizedId = normalizeNotionPageId(pageId);
  const lessonPathsByNotionId = await buildNotionLessonPathMapAsync();

  if (!isNotionConfigured()) {
    return (
      <div className="rounded-lg border border-amber-500/40 bg-amber-500/10 p-4 text-sm">
        Notion er ikke konfigureret. Tilføj <code>NOTION_API_KEY</code> i miljøvariabler.
      </div>
    );
  }

  try {
    const recordMap = await getNotionPage(normalizedId);
    return (
      <div className="notion-wrapper rounded-lg border border-border bg-card p-4">
        <NotionPage
          recordMap={recordMap}
          rootPageId={normalizedId}
          lessonPathsByNotionId={lessonPathsByNotionId}
        />
      </div>
    );
  } catch {
    return (
      <div className="rounded-lg border border-red-500/40 bg-red-500/10 p-4">
        <p className="text-sm text-red-400">
          Kunne ikke hente Notion-siden. Tjek at siden er delt med integrationen og at{" "}
          <code>notionPageId</code> er korrekt.
        </p>
        <p className="mt-2 text-sm text-muted">
          Prøv at genindlæse siden om et øjeblik.
        </p>
      </div>
    );
  }
}
