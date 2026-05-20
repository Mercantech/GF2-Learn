import type { CodeBlock } from "notion-types";

/** Notion-sprognavn → Prism-sprog-id */
const LANGUAGE_ALIASES: Record<string, string> = {
  "c#": "csharp",
  "c♯": "csharp",
  cs: "csharp",
  "c-sharp": "csharp",
  csharp: "csharp",
  dotnet: "csharp",
  "c++": "cpp",
  cplusplus: "cpp",
  "f#": "fsharp",
  fsharp: "fsharp",
  js: "javascript",
  javascript: "javascript",
  ts: "typescript",
  typescript: "typescript",
  shell: "bash",
  sh: "bash",
  zsh: "bash",
  bash: "bash",
  powershell: "powershell",
  ps: "powershell",
  pwsh: "powershell",
  "plain text": "csharp",
  text: "csharp",
  plaintext: "csharp",
  json: "json",
  css: "css",
  html: "markup",
  xml: "markup",
  sql: "sql",
  python: "python",
  py: "python",
  java: "java",
  kotlin: "kotlin",
  go: "go",
  rust: "rust",
  php: "php",
  yaml: "yaml",
  yml: "yaml",
  markdown: "markdown",
  md: "markdown",
  mermaid: "mermaid",
};

const DEFAULT_LANGUAGE = "csharp";

const MERMAID_LANGUAGE = "mermaid";

export function readNotionCodeLanguage(block: CodeBlock): string | undefined {
  const raw = block.properties?.language?.[0]?.[0];
  return typeof raw === "string" ? raw : undefined;
}

/** Normaliser Notion-sprog til Prism; fallback C#. */
export function normalizeNotionCodeLanguage(raw: string | undefined | null): string {
  const key = (raw ?? "").trim().toLowerCase();
  if (!key) return DEFAULT_LANGUAGE;
  return LANGUAGE_ALIASES[key] ?? key.replace(/\s+/g, "");
}

export function getNotionCodeLanguage(block: CodeBlock): string {
  return normalizeNotionCodeLanguage(readNotionCodeLanguage(block));
}

export function isNotionMermaidLanguage(language: string): boolean {
  return normalizeNotionCodeLanguage(language) === MERMAID_LANGUAGE;
}

export function isNotionMermaidBlock(block: CodeBlock): boolean {
  return isNotionMermaidLanguage(getNotionCodeLanguage(block));
}

export function withNotionCodeLanguage(
  block: CodeBlock,
  language: string
): CodeBlock {
  return {
    ...block,
    properties: {
      ...block.properties,
      language: [[language]],
    },
  };
}

type PrismLoader = () => Promise<unknown>;

async function loadClike(): Promise<void> {
  await import("prismjs/components/prism-clike");
}

/** Prism-grammatikker vi kan indlæse efter behov. */
const PRISM_LOADERS: Record<string, PrismLoader> = {
  csharp: async () => {
    await loadClike();
    await import("prismjs/components/prism-csharp");
  },
  bash: () => import("prismjs/components/prism-bash"),
  powershell: () => import("prismjs/components/prism-powershell"),
  python: () => import("prismjs/components/prism-python"),
  java: async () => {
    await loadClike();
    await import("prismjs/components/prism-java");
  },
  sql: () => import("prismjs/components/prism-sql"),
  cpp: async () => {
    await loadClike();
    await import("prismjs/components/prism-cpp");
  },
  fsharp: async () => {
    await loadClike();
    await import("prismjs/components/prism-fsharp");
  },
  go: async () => {
    await loadClike();
    await import("prismjs/components/prism-go");
  },
  rust: () => import("prismjs/components/prism-rust"),
  kotlin: async () => {
    await loadClike();
    await import("prismjs/components/prism-kotlin");
  },
  php: async () => {
    await loadClike();
    await import("prismjs/components/prism-php");
  },
  yaml: () => import("prismjs/components/prism-yaml"),
  markdown: () => import("prismjs/components/prism-markup"),
  markup: () => import("prismjs/components/prism-markup"),
  json: () => import("prismjs/components/prism-json"),
  javascript: () => import("prismjs/components/prism-javascript"),
  typescript: () => import("prismjs/components/prism-typescript"),
  jsx: () => import("prismjs/components/prism-jsx"),
  tsx: () => import("prismjs/components/prism-tsx"),
  css: () => import("prismjs/components/prism-css"),
};

const loaded = new Set<string>();

/** Indlæs Prism-grammatik for sprog (inkl. clike-afhængigheder via prism). */
export async function loadPrismLanguage(language: string): Promise<string> {
  const lang = normalizeNotionCodeLanguage(language);

  if (isNotionMermaidLanguage(lang)) return lang;

  if (loaded.has(lang)) return lang;

  const loader = PRISM_LOADERS[lang];
  if (loader) {
    await loader();
    loaded.add(lang);
    return lang;
  }

  if (lang !== DEFAULT_LANGUAGE) {
    return loadPrismLanguage(DEFAULT_LANGUAGE);
  }

  await PRISM_LOADERS.csharp();
  loaded.add(DEFAULT_LANGUAGE);
  return DEFAULT_LANGUAGE;
}
