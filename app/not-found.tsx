import Link from "next/link";
import { CourseShell } from "@/components/layout/course-shell";

export default function NotFound() {
  return (
    <CourseShell>
      <div className="mx-auto max-w-lg text-center">
        <h1 className="mb-2 text-2xl font-bold">Siden findes ikke</h1>
        <p className="mb-6 text-muted">
          Den lektion eller side du leder efter, findes ikke.
        </p>
        <Link href="/" className="text-accent hover:underline">
          Tilbage til forsiden
        </Link>
      </div>
    </CourseShell>
  );
}
