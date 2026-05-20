"use client";

import { useEffect, useId, useRef, useState } from "react";
import { useTheme } from "next-themes";

interface NotionMermaidProps {
  chart: string;
  className?: string;
}

export function NotionMermaid({ chart, className }: NotionMermaidProps) {
  const { resolvedTheme } = useTheme();
  const isDark = resolvedTheme === "dark";
  const containerRef = useRef<HTMLDivElement>(null);
  const baseId = useId().replace(/:/g, "");
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    const source = chart.trim();

    if (!source) {
      setError(null);
      if (containerRef.current) containerRef.current.innerHTML = "";
      return;
    }

    async function renderChart() {
      try {
        const mermaid = (await import("mermaid")).default;
        mermaid.initialize({
          startOnLoad: false,
          theme: isDark ? "dark" : "default",
          securityLevel: "strict",
        });

        const { svg } = await mermaid.render(
          `notion-mermaid-${baseId}-${Date.now()}`,
          source
        );

        if (cancelled || !containerRef.current) return;

        containerRef.current.innerHTML = svg;
        setError(null);
      } catch (err) {
        if (cancelled) return;
        setError(
          err instanceof Error ? err.message : "Kunne ikke rendere diagram"
        );
      }
    }

    void renderChart();

    return () => {
      cancelled = true;
    };
  }, [chart, isDark, baseId]);

  if (error) {
    return (
      <div className={`notion-mermaid notion-mermaid-error ${className ?? ""}`}>
        <p className="mb-2 text-sm text-muted">Kunne ikke vise Mermaid-diagram</p>
        <pre className="overflow-x-auto rounded-lg bg-sidebar p-4 text-sm">
          <code>{chart}</code>
        </pre>
      </div>
    );
  }

  return (
    <div
      ref={containerRef}
      className={`notion-mermaid ${className ?? ""}`}
      aria-label="Mermaid-diagram"
    />
  );
}
