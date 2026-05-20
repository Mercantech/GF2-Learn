import Link from "next/link";
import { notFound } from "next/navigation";
import { CourseShell } from "@/components/layout/course-shell";
import { Breadcrumbs } from "@/components/layout/breadcrumbs";
import { NotionPageWrapper } from "@/components/content/notion-page-wrapper";
import { getLessonByNotionPageId } from "@/lib/content";
import { getNotionPage, isNotionConfigured } from "@/lib/notion";
import { getNotionPageTitle } from "@/lib/notion-title";
import { normalizeNotionPageId } from "@/lib/notion-page-id";

interface NotionViewerPageProps {
  params: Promise<{ pageId: string }>;
}

export const dynamic = "force-dynamic";

export default async function NotionViewerPage({ params }: NotionViewerPageProps) {
  const { pageId: rawPageId } = await params;
  const pageId = normalizeNotionPageId(rawPageId);

  if (!/^[a-f0-9]{32}$/i.test(pageId)) {
    notFound();
  }

  const linkedLesson = getLessonByNotionPageId(pageId);
  let title = linkedLesson?.title ?? "Notion-side";

  if (isNotionConfigured()) {
    try {
      const recordMap = await getNotionPage(pageId);
      title = getNotionPageTitle(recordMap, pageId);
    } catch {
      if (!linkedLesson) notFound();
    }
  } else if (!linkedLesson) {
    notFound();
  }

  return (
    <CourseShell>
      <div className="mx-auto max-w-3xl">
        <Breadcrumbs
          items={[
            { label: "Forside", href: "/" },
            { label: title },
          ]}
        />
        <header className="mb-6 border-b border-border pb-6">
          <h1 className="text-3xl font-bold tracking-tight">{title}</h1>
          {linkedLesson ? (
            <p className="mt-2 text-sm text-muted">
              Denne side findes også som{" "}
              <Link
                href={`/modul/${linkedLesson.moduleSlug}/${linkedLesson.lessonSlug}`}
                className="text-accent hover:underline"
              >
                kursets lektion
              </Link>
              .
            </p>
          ) : null}
        </header>
        <NotionPageWrapper pageId={pageId} />
      </div>
    </CourseShell>
  );
}
