"use client";

import { useTheme } from "next-themes";
import { useEffect, useState } from "react";

export function ThemeToggle() {
  const { theme, setTheme, resolvedTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => setMounted(true), []);

  if (!mounted) {
    return (
      <button
        type="button"
        className="rounded-lg border border-border px-3 py-1.5 text-sm text-muted"
        aria-label="Skift tema"
      >
        …
      </button>
    );
  }

  const isDark = (theme === "system" ? resolvedTheme : theme) === "dark";

  return (
    <button
      type="button"
      onClick={() => setTheme(isDark ? "light" : "dark")}
      className="rounded-lg border border-border bg-card px-3 py-1.5 text-sm transition hover:border-accent/50"
      aria-label={isDark ? "Skift til lys tilstand" : "Skift til mørk tilstand"}
    >
      {isDark ? "Lys" : "Mørk"}
    </button>
  );
}
