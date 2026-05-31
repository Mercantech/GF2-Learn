# Dokploy + Cloudflare — port exposure

Containers **Healthy** i Dokploy betyder ikke at **host-port** er åben. GF2-Learn bruger derfor **`network_mode: host`** på `web`, så appen lytter **direkte på serveren** — samme effekt som «exposed port» på dine andre apps.

## Konfiguration

| Variabel | Standard | Brug |
|----------|----------|------|
| `WEB_HOST_PORT` | `8031` | Port på **host** + Cloudflare origin |
| `POSTGRES_HOST_PORT` | `5422` | Postgres kun på localhost (web forbinder via `127.0.0.1`) |

I Dokploy environment:

```env
WEB_HOST_PORT=8031
POSTGRES_HOST_PORT=5422
```

Cloudflare **Published application**:

```text
http://127.0.0.1:8031
```

(hvis `WEB_HOST_PORT` er 8031)

Hvis cloudflared kører **i Docker**:

```text
http://host.docker.internal:8031
```

## Efter deploy — kør på serveren

```bash
chmod +x scripts/verify-tunnel.sh
WEB_HOST_PORT=8031 ./scripts/verify-tunnel.sh
```

Eller manuelt:

```bash
curl -sI http://127.0.0.1:8031/health   # skal være HTTP 200
ss -tlnp | grep 8031                     # skal vise dotnet lytter
```

**Hvis curl giver 200** men browser/tunnel stadig fejler → problemet er **kun Cloudflare** (DNS, connector, forkert port i dashboard).

**Hvis curl fejler** → tjek logs: `docker logs gf2learn-gf2learndev-ku1zfp-web-1`

## Dokploy UI

- Sæt **ikke** en anden exposed port i UI end `WEB_HOST_PORT` — compose styrer host-netværk nu.
- `network_mode: host` kræver at Dokploy tillader det (de fleste self-hosted gør).

## Postgres

- Connection: `127.0.0.1:5422` fra web (host network)
- `password authentication failed` → ens password som ved første volume-oprettelse

## Hvis host network ikke er tilladt

Kontakt os / brug bridge med `ports: "8031:8080"` og find den præcise origin-URL dine **andre** compose-apps bruger i Cloudflare (screenshot af deres Published route).
