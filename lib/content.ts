import fs from "fs";
import path from "path";
import matter from "gray-matter";
import type {
  Lesson,
  LessonFrontmatter,
  ModuleMeta,
  ModuleWithLessons,
} from "./types";
import { normalizeNotionPageId } from "./notion-page-id";

const CONTENT_DIR = path.join(process.cwd(), "content", "modules");

function parseLessonFrontmatter(data: Record<string, unknown>): LessonFrontmatter {
  return {
    title: String(data.title ?? "Uden titel"),
    module: String(data.module ?? ""),
    order: Number(data.order ?? 0),
    fag: String(data.fag ?? ""),
    kompetencemaal: Array.isArray(data.kompetencemaal)
      ? data.kompetencemaal.map(String)
      : [],
    timer: Number(data.timer ?? 0),
    laereplanRef: String(data.laereplanRef ?? ""),
    youtubeId:
      data.youtubeId && String(data.youtubeId).trim()
        ? String(data.youtubeId).trim()
        : undefined,
    notionPageId:
      data.notionPageId && String(data.notionPageId).trim()
        ? String(data.notionPageId).trim()
        : undefined,
    splitNotionByH1: Boolean(data.splitNotionByH1),
  };
}

function readModuleMeta(moduleDir: string, slug: string): ModuleMeta {
  const metaPath = path.join(moduleDir, "module.json");
  const raw = JSON.parse(fs.readFileSync(metaPath, "utf8")) as Record<
    string,
    unknown
  >;
  return {
    slug,
    title: String(raw.title ?? slug),
    description: String(raw.description ?? ""),
    order: Number(raw.order ?? 0),
  };
}

function readLessonsForModule(moduleSlug: string): Lesson[] {
  const lessonsDir = path.join(CONTENT_DIR, moduleSlug, "lessons");
  if (!fs.existsSync(lessonsDir)) return [];

  return fs
    .readdirSync(lessonsDir)
    .filter((file) => file.endsWith(".mdx"))
    .map((file) => {
      const slug = file.replace(/\.mdx$/, "");
      const raw = fs.readFileSync(path.join(lessonsDir, file), "utf8");
      const { data, content } = matter(raw);
      const frontmatter = parseLessonFrontmatter(data as Record<string, unknown>);
      return {
        slug,
        moduleSlug,
        frontmatter: { ...frontmatter, module: moduleSlug },
        content,
      };
    })
    .sort((a, b) => a.frontmatter.order - b.frontmatter.order);
}

export function getAllModules(): ModuleWithLessons[] {
  if (!fs.existsSync(CONTENT_DIR)) return [];

  return fs
    .readdirSync(CONTENT_DIR, { withFileTypes: true })
    .filter((d) => d.isDirectory())
    .map((d) => {
      const moduleSlug = d.name;
      const moduleDir = path.join(CONTENT_DIR, moduleSlug);
      const meta = readModuleMeta(moduleDir, moduleSlug);
      const lessons = readLessonsForModule(moduleSlug);
      return { ...meta, lessons };
    })
    .sort((a, b) => a.order - b.order);
}

export function getModule(moduleSlug: string): ModuleWithLessons | null {
  const modules = getAllModules();
  return modules.find((m) => m.slug === moduleSlug) ?? null;
}

export function getLesson(
  moduleSlug: string,
  lessonSlug: string
): Lesson | null {
  const mod = getModule(moduleSlug);
  if (!mod) return null;
  return mod.lessons.find((l) => l.slug === lessonSlug) ?? null;
}

export function getAllLessonPaths(): { moduleSlug: string; lessonSlug: string }[] {
  return getAllModules().flatMap((mod) =>
    mod.lessons.map((lesson) => ({
      moduleSlug: mod.slug,
      lessonSlug: lesson.slug,
    }))
  );
}

/** Notion page ID → intern lektionssti (til mapPageUrl). */
export function buildNotionLessonPathMap(): Record<string, string> {
  const map: Record<string, string> = {};
  for (const mod of getAllModules()) {
    for (const lesson of mod.lessons) {
      const notionId = lesson.frontmatter.notionPageId;
      if (notionId) {
        map[normalizeNotionPageId(notionId)] =
          `/modul/${mod.slug}/${lesson.slug}`;
      }
    }
  }
  return map;
}

export function getLessonByNotionPageId(
  pageId: string
): { moduleSlug: string; lessonSlug: string; title: string } | null {
  const normalized = normalizeNotionPageId(pageId);
  for (const mod of getAllModules()) {
    for (const lesson of mod.lessons) {
      const notionId = lesson.frontmatter.notionPageId;
      if (notionId && normalizeNotionPageId(notionId) === normalized) {
        return {
          moduleSlug: mod.slug,
          lessonSlug: lesson.slug,
          title: lesson.frontmatter.title,
        };
      }
    }
  }
  return null;
}
