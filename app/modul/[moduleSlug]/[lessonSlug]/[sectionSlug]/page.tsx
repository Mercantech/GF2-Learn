import { notFound } from "next/navigation";
import { getLesson, getModule } from "@/lib/content";
import { getNotionSectionRecordMap } from "@/lib/notion-sections";
import { CourseShell } from "@/components/layout/course-shell";
import { Breadcrumbs } from "@/components/layout/breadcrumbs";
import { NotionPage } from "@/components/content/notion-page";
import { buildNotionLessonPathMapAsync } from "@/lib/navigation";

interface SectionPageProps {
  params: Promise<{
    moduleSlug: string;
    lessonSlug: string;
    sectionSlug: string;
  }>;
}

/** Notion-sektioner ved request — undgår tung SSG og korrupt .next-cache i dev. */
export const dynamic = "force-dynamic";
export const revalidate = 120;

export default async function SectionPage({ params }: SectionPageProps) {
  const { moduleSlug, lessonSlug, sectionSlug } = await params;
  const mod = getModule(moduleSlug);
  const lesson = getLesson(moduleSlug, lessonSlug);

  if (!mod || !lesson?.frontmatter.notionPageId) {
    notFound();
  }

  if (!lesson.frontmatter.splitNotionByH1) {
    notFound();
  }

  const data = await getNotionSectionRecordMap(
    lesson.frontmatter.notionPageId,
    sectionSlug
  );
  if (!data) {
    notFound();
  }

  const lessonPathsByNotionId = await buildNotionLessonPathMapAsync();

  return (
    <CourseShell
      activeModuleSlug={moduleSlug}
      activeLessonSlug={lessonSlug}
      activeSectionSlug={sectionSlug}
    >
      <div className="mx-auto max-w-3xl">
        <Breadcrumbs
          items={[
            { label: "Forside", href: "/" },
            { label: mod.title },
            { label: data.section.title },
          ]}
        />
        <div className="notion-wrapper rounded-lg border border-border bg-card p-4">
          <NotionPage
            recordMap={data.recordMap}
            rootPageId={data.rootPageId}
            lessonPathsByNotionId={lessonPathsByNotionId}
          />
        </div>
      </div>
    </CourseShell>
  );
}
