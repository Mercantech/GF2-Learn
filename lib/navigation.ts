import { unstable_cache } from "next/cache";
import { buildNotionLessonPathMap, getAllModules } from "./content";
import { normalizeNotionPageId } from "./notion-page-id";
import { getNotionH1Sections } from "./notion-sections";
import type { ModuleWithLessons } from "./types";

export interface NavItem {
  slug: string;
  title: string;
  href: string;
  parentLessonSlug?: string;
  isSection?: boolean;
}

export interface NavigationModule extends ModuleWithLessons {
  navItems: NavItem[];
}

async function buildCourseNavigation(): Promise<NavigationModule[]> {
  const modules = getAllModules();
  const result: NavigationModule[] = [];

  for (const mod of modules) {
    const navItems: NavItem[] = [];

    for (const lesson of mod.lessons) {
      const { splitNotionByH1, notionPageId } = lesson.frontmatter;

      if (splitNotionByH1 && notionPageId) {
        try {
          const sections = await getNotionH1Sections(notionPageId);
          for (const section of sections) {
            navItems.push({
              slug: section.slug,
              title: section.title,
              href: `/modul/${mod.slug}/${lesson.slug}/${section.slug}`,
              parentLessonSlug: lesson.slug,
              isSection: true,
            });
          }
        } catch {
          navItems.push({
            slug: lesson.slug,
            title: lesson.frontmatter.title,
            href: `/modul/${mod.slug}/${lesson.slug}`,
          });
        }
      } else {
        navItems.push({
          slug: lesson.slug,
          title: lesson.frontmatter.title,
          href: `/modul/${mod.slug}/${lesson.slug}`,
        });
      }
    }

    result.push({ ...mod, navItems });
  }

  return result;
}

export async function getCourseNavigation(): Promise<NavigationModule[]> {
  const revalidateSeconds = Number(process.env.NOTION_REVALIDATE_SECONDS ?? 120);

  return unstable_cache(
    buildCourseNavigation,
    ["course-navigation"],
    { revalidate: revalidateSeconds }
  )();
}

/** Udvider mapPageUrl: Notion-side med split → første H1-sektion. */
export async function buildNotionLessonPathMapAsync(): Promise<
  Record<string, string>
> {
  const map = buildNotionLessonPathMap();

  for (const mod of getAllModules()) {
    for (const lesson of mod.lessons) {
      const { splitNotionByH1, notionPageId } = lesson.frontmatter;
      if (!splitNotionByH1 || !notionPageId) continue;

      try {
        const sections = await getNotionH1Sections(notionPageId);
        const pageKey = normalizeNotionPageId(notionPageId);
        if (sections[0]) {
          map[pageKey] = `/modul/${mod.slug}/${lesson.slug}/${sections[0].slug}`;
        }
      } catch {
        /* behold eksisterende mapping */
      }
    }
  }

  return map;
}
