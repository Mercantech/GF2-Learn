import Link from "next/link";
import { getCourseNavigation } from "@/lib/navigation";
import { CourseShell } from "@/components/layout/course-shell";

export default async function HomePage() {
  const modules = await getCourseNavigation();

  return (
    <CourseShell>
      <div className="mx-auto max-w-3xl">
        <h1 className="mb-2 text-3xl font-bold tracking-tight">
          GF2 Data — Grundforløb 2
        </h1>
        <p className="mb-8 text-lg text-muted">
          Velkommen til læringsplatformen. Vælg et modul i menuen eller start med
          den første lektion nedenfor.
        </p>

        <div className="space-y-6">
          {modules.map((mod) => (
            <section
              key={mod.slug}
              className="rounded-xl border border-border bg-card p-6 shadow-sm"
            >
              <h2 className="mb-1 text-xl font-semibold">{mod.title}</h2>
              <p className="mb-4 text-muted">{mod.description}</p>
              <ul className="space-y-2">
                {mod.navItems.map((item) => (
                  <li key={`${item.parentLessonSlug ?? ""}-${item.slug}`}>
                    <Link
                      href={item.href}
                      className="text-accent hover:underline"
                    >
                      {item.title}
                    </Link>
                  </li>
                ))}
              </ul>
            </section>
          ))}
        </div>
      </div>
    </CourseShell>
  );
}
