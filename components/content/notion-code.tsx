"use client";

import { useEffect, useState } from "react";
import { Code } from "react-notion-x/build/third-party/code";
import { useNotionContext } from "react-notion-x";
import { getBlockTitle } from "notion-utils";
import type { CodeBlock } from "notion-types";
import {
  getNotionCodeLanguage,
  isNotionMermaidBlock,
  loadPrismLanguage,
  withNotionCodeLanguage,
} from "@/lib/notion-code-language";
import { NotionMermaid } from "./notion-mermaid";

interface NotionCodeProps {
  block: CodeBlock;
  defaultLanguage?: string;
  className?: string;
}

export function NotionCode({ block, className }: NotionCodeProps) {
  if (isNotionMermaidBlock(block)) {
    return <NotionMermaidCode block={block} className={className} />;
  }

  return <NotionPrismCode block={block} className={className} />;
}

function NotionMermaidCode({
  block,
  className,
}: Pick<NotionCodeProps, "block" | "className">) {
  const { recordMap } = useNotionContext();
  const content = getBlockTitle(block, recordMap);
  return <NotionMermaid chart={content} className={className} />;
}

function NotionPrismCode({
  block,
  className,
}: Pick<NotionCodeProps, "block" | "className">) {
  const { recordMap } = useNotionContext();
  const notionLanguage = getNotionCodeLanguage(block);
  const [prismLanguage, setPrismLanguage] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;

    loadPrismLanguage(notionLanguage).then((lang) => {
      if (!cancelled) setPrismLanguage(lang);
    });

    return () => {
      cancelled = true;
    };
  }, [block.id, notionLanguage]);

  if (!prismLanguage) {
    const content = getBlockTitle(block, recordMap);
    return (
      <pre className="notion-code rounded-lg bg-sidebar p-4 text-sm overflow-x-auto">
        <code>{content}</code>
      </pre>
    );
  }

  const normalizedBlock = withNotionCodeLanguage(block, prismLanguage);

  return (
    <Code
      key={`${block.id}-${prismLanguage}`}
      block={normalizedBlock}
      defaultLanguage="csharp"
      className={className}
    />
  );
}
