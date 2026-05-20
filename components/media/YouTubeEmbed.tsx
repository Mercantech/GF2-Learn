interface YouTubeEmbedProps {
  videoId: string;
  title?: string;
}

const YOUTUBE_ID_PATTERN = /^[a-zA-Z0-9_-]{11}$/;

export function YouTubeEmbed({ videoId, title = "Video" }: YouTubeEmbedProps) {
  const trimmed = videoId.trim();
  if (!trimmed || !YOUTUBE_ID_PATTERN.test(trimmed)) {
    return null;
  }

  return (
    <div className="mb-8 overflow-hidden rounded-xl border border-border bg-card shadow-sm">
      <div className="relative aspect-video w-full">
        <iframe
          className="absolute inset-0 h-full w-full"
          src={`https://www.youtube-nocookie.com/embed/${trimmed}`}
          title={title}
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen
        />
      </div>
    </div>
  );
}
