"use client";

import Link from "next/link";
import type { ComponentProps } from "react";
import { withAppBasePath } from "@/lib/base-path";

type NotionPageLinkProps = ComponentProps<"a"> & {
  href?: string;
};

/**
 * Erstatter react-notion-x PageLink — bruger next/link til interne stier.
 */
export function NotionPageLink({
  href,
  children,
  className,
  target,
  rel,
  ...rest
}: NotionPageLinkProps) {
  if (!href) {
    return (
      <span className={className} {...rest}>
        {children}
      </span>
    );
  }

  if (href.startsWith("http://") || href.startsWith("https://")) {
    return (
      <a
        href={href}
        className={className}
        target={target ?? "_blank"}
        rel={rel ?? "noopener noreferrer"}
        {...rest}
      >
        {children}
      </a>
    );
  }

  const internalHref = href.startsWith("/gf2")
    ? href.replace(/^\/gf2/, "") || "/"
    : href;

  return (
    <Link
      href={internalHref}
      className={className}
      target={target}
      rel={rel}
      {...rest}
    >
      {children}
    </Link>
  );
}

/** Eksporteret til test — normaliserer href før Link. */
export function toNextLinkHref(href: string): string {
  if (href.startsWith("/gf2")) {
    return href.replace(/^\/gf2/, "") || "/";
  }
  return href;
}

export function toAnchorHref(href: string): string {
  return withAppBasePath(toNextLinkHref(href));
}
