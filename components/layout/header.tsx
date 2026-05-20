import Link from "next/link";
import { ThemeToggle } from "./theme-toggle";

export function Header() {
  return (
    <header className="sticky top-0 z-20 flex h-14 items-center justify-between border-b border-border bg-card/95 px-4 backdrop-blur md:px-6">
      <Link href="/" className="font-semibold tracking-tight text-foreground">
        GF2 Data <span className="text-muted font-normal">· MAGS</span>
      </Link>
      <ThemeToggle />
    </header>
  );
}
