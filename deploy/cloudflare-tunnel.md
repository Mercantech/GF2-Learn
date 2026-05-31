# Cloudflare Tunnel — dev (GF2-Learn)

Opsætning styres via **Cloudflare Zero Trust / Dashboard** (web app) — ikke nødvendigvis en lokal `config.yml`.

## Mapping (nuværende)

| Felt i Cloudflare | Værdi |
|-------------------|--------|
| **Public hostname** | `learn-dev.gf2.dk` |
| **Service type** | Published application |
| **URL / origin (cloudflared på host)** | **`http://127.0.0.1:8031`** (eller din `WEB_HOST_PORT`) |
| **URL (cloudflared i Docker)** | **`http://host.docker.internal:8031`** |

På serveren skal Docker køre:

```bash
docker compose up -d
```

Appen er tilgængelig lokalt på **http://127.0.0.1:8031** og offentligt på **https://learn-dev.gf2.dk**.

## Mercantec OAuth

Tilføj redirect URI i Auth Admin:

`https://learn-dev.gf2.dk/signin-mercantec`

Appen læser `X-Forwarded-Proto` / `X-Forwarded-Host` fra tunnelen, så login og cookies virker over HTTPS.

## Dokploy

Deploy via Dokploy (`docker compose -p gf2learn-...`) — se **[dokploy-cloudflare.md](dokploy-cloudflare.md)** for fuld fejlsøgning.

Kort: SSH til serveren → `curl http://127.0.0.1:8031/health` skal give `200` før Cloudflare kan virke. Brug `scripts/verify-tunnel.sh`.

## Fejlsøgning

| Symptom | Tjek |
|---------|------|
| 502 / connection refused | Connector på **samme server** som Dokploy? Origin `http://127.0.0.1:8031` (ikke Traefik, ikke container 8080)? |
| Deploy OK, tunnel nej | Se [dokploy-cloudflare.md](dokploy-cloudflare.md) |
| Login fejler / forkert callback | Redirect URI `https://learn-dev.gf2.dk/signin-mercantec` |
| Redirect loop | `GF2_BEHIND_REVERSE_PROXY=true` i compose (slår HTTPS-redirect fra bag tunnel) |

## Valgfri: CLI-config

Hvis du senere vil køre `cloudflared` manuelt i stedet for connector fra dashboard, se [`cloudflared.config.example.yml`](cloudflared.config.example.yml).
