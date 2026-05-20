import type { LessonFrontmatter } from "@/lib/types";

export function LessonMeta({ frontmatter }: { frontmatter: LessonFrontmatter }) {
  const hasMeta =
    frontmatter.fag ||
    frontmatter.laereplanRef ||
    frontmatter.kompetencemaal.length > 0 ||
    frontmatter.timer > 0;

  if (!hasMeta) return null;

  return (
    <aside className="mb-6 rounded-lg border border-border bg-sidebar/50 p-4 text-sm">
      <p className="mb-2 font-semibold text-muted">Læreplans-metadata</p>
      <dl className="grid gap-2 sm:grid-cols-2">
        {frontmatter.fag ? (
          <>
            <dt className="text-muted">Fag</dt>
            <dd>{frontmatter.fag}</dd>
          </>
        ) : null}
        {frontmatter.timer > 0 ? (
          <>
            <dt className="text-muted">Timer</dt>
            <dd>{frontmatter.timer}</dd>
          </>
        ) : null}
        {frontmatter.laereplanRef ? (
          <>
            <dt className="text-muted">Læreplan</dt>
            <dd>{frontmatter.laereplanRef}</dd>
          </>
        ) : null}
        {frontmatter.kompetencemaal.length > 0 ? (
          <>
            <dt className="text-muted sm:col-span-1">Kompetencemål</dt>
            <dd className="sm:col-span-1">
              <ul className="list-inside list-disc">
                {frontmatter.kompetencemaal.map((item) => (
                  <li key={item}>{item}</li>
                ))}
              </ul>
            </dd>
          </>
        ) : null}
      </dl>
    </aside>
  );
}
