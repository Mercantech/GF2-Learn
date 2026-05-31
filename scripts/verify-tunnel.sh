#!/usr/bin/env sh
# Kør på Dokploy-serveren (SSH) efter deploy.
set -e
PORT="${WEB_HOST_PORT:-8031}"
echo "=== GF2-Learn tunnel verify (port ${PORT}) ==="
echo ""
echo "1) Containers"
docker ps --filter name=gf2learn --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""
echo "2) Lytter host på port ${PORT}?"
if command -v ss >/dev/null 2>&1; then
  ss -tlnp | grep ":${PORT}" || echo "   INGEN proces på :${PORT} — tunnel kan ikke nå appen"
else
  netstat -tlnp 2>/dev/null | grep ":${PORT}" || echo "   (installér ss/netstat for at tjekke)"
fi
echo ""
echo "3) HTTP health"
curl -fsS -o /dev/null -w "   /health → HTTP %{http_code}\n" "http://127.0.0.1:${PORT}/health" || echo "   FEJL: curl til 127.0.0.1:${PORT}/health"
echo ""
echo "4) Forside"
curl -fsS -o /dev/null -w "   / → HTTP %{http_code}\n" "http://127.0.0.1:${PORT}/" || echo "   FEJL: curl til 127.0.0.1:${PORT}/"
echo ""
echo "5) Fra Docker (som cloudflared-container)"
docker run --rm --add-host=host.docker.internal:host-gateway curlimages/curl:latest \
  -fsS -o /dev/null -w "   host.docker.internal:${PORT}/health → HTTP %{http_code}\n" \
  "http://host.docker.internal:${PORT}/health" 2>/dev/null \
  || echo "   host.docker.internal fejlede — brug den URL i Cloudflare hvis cloudflared kører i Docker"
echo ""
echo "Cloudflare Published application skal pege på:"
echo "  host cloudflared:  http://127.0.0.1:${PORT}"
echo "  docker cloudflared: http://host.docker.internal:${PORT}"
