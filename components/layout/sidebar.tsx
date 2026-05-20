import Link from "next/link";
import type { NavigationModule } from "@/lib/navigation";

interface SidebarProps {
  modules: NavigationModule[];
  activeModuleSlug?: string;
  activeLessonSlug?: string;
  activeSectionSlug?: string;
}

export function Sidebar({
  modules,
  activeModuleSlug,
  activeLessonSlug,
  activeSectionSlug,
}: SidebarProps) {
  return (
    <aside className="w-full shrink-0 border-r border-border bg-sidebar md:w-72 md:min-h-[calc(100vh-3.5rem)]">
      <nav className="p-4" aria-label="Kursusnavigation">
        <p className="mb-3 text-xs font-semibold uppercase tracking-wider text-muted">
          Moduler
        </p>
        <ul className="space-y-4">
          {modules.map((mod) => (
            <li key={mod.slug}>
              <p className="mb-1 text-sm font-semibold text-foreground">
                {mod.title}
              </p>
              <ul className="space-y-0.5 border-l border-border pl-3">
                {mod.navItems.map((item) => {
                  const isActive = item.isSection
                    ? activeModuleSlug === mod.slug &&
                      activeLessonSlug === item.parentLessonSlug &&
                      activeSectionSlug === item.slug
                    : activeModuleSlug === mod.slug &&
                      activeLessonSlug === item.slug &&
                      !activeSectionSlug;

                  return (
                    <li key={`${item.parentLessonSlug ?? ""}-${item.slug}`}>
                      <Link
                        href={item.href}
                        className={`block rounded-md px-2 py-1.5 text-sm transition ${
                          isActive
                            ? "bg-accent/15 font-medium text-accent"
                            : "text-muted hover:bg-card hover:text-foreground"
                        }`}
                      >
                        {item.title}
                      </Link>
                    </li>
                  );
                })}
              </ul>
            </li>
          ))}
        </ul>
      </nav>
    </aside>
  );
}
