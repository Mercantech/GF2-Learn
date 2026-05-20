import { getCourseNavigation } from "@/lib/navigation";
import { Header } from "./header";
import { Sidebar } from "./sidebar";

interface CourseShellProps {
  children: React.ReactNode;
  activeModuleSlug?: string;
  activeLessonSlug?: string;
  activeSectionSlug?: string;
}

export async function CourseShell({
  children,
  activeModuleSlug,
  activeLessonSlug,
  activeSectionSlug,
}: CourseShellProps) {
  const modules = await getCourseNavigation();

  return (
    <div className="min-h-screen">
      <Header />
      <div className="flex min-h-[calc(100vh-3.5rem)] flex-col md:flex-row">
        <Sidebar
          modules={modules}
          activeModuleSlug={activeModuleSlug}
          activeLessonSlug={activeLessonSlug}
          activeSectionSlug={activeSectionSlug}
        />
        <main className="min-w-0 flex-1 p-4 md:p-8">{children}</main>
      </div>
    </div>
  );
}
