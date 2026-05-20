export interface LessonFrontmatter {
  title: string;
  module: string;
  order: number;
  fag: string;
  kompetencemaal: string[];
  timer: number;
  laereplanRef: string;
  youtubeId?: string;
  notionPageId?: string;
  /** Opdel Notion-side i H1-sektioner — hver får eget menupunkt. */
  splitNotionByH1?: boolean;
}

export interface Lesson {
  slug: string;
  moduleSlug: string;
  frontmatter: LessonFrontmatter;
  content: string;
}

export interface ModuleMeta {
  slug: string;
  title: string;
  description: string;
  order: number;
}

export interface ModuleWithLessons extends ModuleMeta {
  lessons: Lesson[];
}
