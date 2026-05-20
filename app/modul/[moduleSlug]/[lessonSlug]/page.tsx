import { notFound, redirect } from "next/navigation";
import { getAllLessonPaths, getLesson, getModule } from "@/lib/content";
import { getNotionH1Sections } from "@/lib/notion-sections";
import { CourseShell } from "@/components/layout/course-shell";
import { Breadcrumbs } from "@/components/layout/breadcrumbs";
import { LessonView } from "@/components/content/lesson-view";

interface LessonPageProps {
  params: Promise<{ moduleSlug: string; lessonSlug: string }>;
}

export const dynamicParams = true;

export function generateStaticParams() {
  return getAllLessonPaths().filter(({ moduleSlug, lessonSlug }) => {
    const lesson = getLesson(moduleSlug, lessonSlug);
    return !lesson?.frontmatter.splitNotionByH1;
  });
}

export default async function LessonPage({ params }: LessonPageProps) {
  const { moduleSlug, lessonSlug } = await params;
  const mod = getModule(moduleSlug);
  const lesson = getLesson(moduleSlug, lessonSlug);

  if (!mod || !lesson) {
    notFound();
  }

  if (
    lesson.frontmatter.splitNotionByH1 &&
    lesson.frontmatter.notionPageId
  ) {
    try {
      const sections = await getNotionH1Sections(
        lesson.frontmatter.notionPageId
      );
      if (sections.length > 0) {
        redirect(
          `/modul/${moduleSlug}/${lessonSlug}/${sections[0].slug}`
        );
      }
    } catch {
      /* vis MDX-fallback hvis Notion fejler */
    }
  }

  return (
    <CourseShell
      activeModuleSlug={moduleSlug}
      activeLessonSlug={lessonSlug}
    >
      <div className="mx-auto max-w-3xl">
        <Breadcrumbs
          items={[
            { label: "Forside", href: "/" },
            { label: mod.title },
            { label: lesson.frontmatter.title },
          ]}
        />
        <LessonView lesson={lesson} module={mod} />
      </div>
    </CourseShell>
  );
}
