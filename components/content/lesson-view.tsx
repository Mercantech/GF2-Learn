import type { Lesson, ModuleWithLessons } from "@/lib/types";
import { YouTubeEmbed } from "@/components/media/YouTubeEmbed";
import { MdxContent } from "./mdx-content";
import { NotionPageWrapper } from "./notion-page-wrapper";
import { LessonMeta } from "./lesson-meta";

interface LessonViewProps {
  lesson: Lesson;
  module: ModuleWithLessons;
}

function hasNotionPage(lesson: Lesson): boolean {
  const id = lesson.frontmatter.notionPageId?.trim();
  return Boolean(id);
}

export function LessonView({ lesson, module }: LessonViewProps) {
  const { frontmatter } = lesson;
  const useNotion = hasNotionPage(lesson);

  return (
    <article>
      <header className="mb-6 border-b border-border pb-6">
        <p className="mb-1 text-sm text-muted">{module.title}</p>
        <h1 className="text-3xl font-bold tracking-tight">{frontmatter.title}</h1>
      </header>

      <LessonMeta frontmatter={frontmatter} />

      {frontmatter.youtubeId ? (
        <YouTubeEmbed
          videoId={frontmatter.youtubeId}
          title={frontmatter.title}
        />
      ) : null}

      {useNotion ? (
        <NotionPageWrapper pageId={frontmatter.notionPageId!} />
      ) : (
        <MdxContent source={lesson.content} />
      )}
    </article>
  );
}
