# Dokploy + Cloudflare — port exposure (som dine andre apps)

GF2-Learn er sat op til **normal host port mapping** — ikke Traefik-krav.

| Sted | Værdi |
|------|--------|
| Container (ASP.NET) | `8080` |
| Host (standard i compose) | **`8031`** → `8080` |
| Cloudflare Published application | `http://localhost:8031` eller se nedenfor hvis cloudflared kører i Docker |

Sæt i Dokploy environment:

```env
WEB_HOST_PORT=8031
```

Skift til samme port du bruger i Cloudflare og i Dokploy «exposed port».

---

## 1. Tjek på serveren (SSH)

Efter deploy:

```bash
docker ps --filter name=gf2learn --format "table {{.Names}}\t{{.Ports}}"
curl -sI http://127.0.0.1:8031/health
```

**Forventet:** `HTTP/1.1 200 OK` og ports viser `0.0.0.0:8031->8080/tcp`.

Hvis `curl` fejler her, er problemet **Docker/port** — ikke Cloudflare.

---

## 2. Cloudflare origin — afhænger af hvor cloudflared kører

### A) cloudflared på **host** (samme maskine som Dokploy)

Published application URL:

```text
http://127.0.0.1:8031
```

eller `http://localhost:8031` — begge skal virke hvis port er published.

### B) cloudflared i **Docker** (fx cloudflared-container i Dokploy)

`localhost` inde i cloudflared-containeren er **ikke** hostens localhost.

Brug i stedet **én** af disse i Cloudflare:

| URL | Hvornår |
|-----|---------|
| `http://host.docker.internal:8031` | Docker 20.10+ (Linux med `host-gateway`) |
| `http://172.17.0.1:8031` | Klassisk docker0 bridge (tjek med `ip addr show docker0`) |

Test fra en vilkårlig container på serveren:

```bash
docker run --rm --add-host=host.docker.internal:host-gateway curlimages/curl \
  -sI http://host.docker.internal:8031/health
```

Hvis det giver `200`, men `localhost:8031` i tunnel ikke gør — er du i case **B**.

Dine **andre apps** der virker med port exposure bruger sandsynligvis enten host-baseret cloudflared, eller `host.docker.internal` — ikke `localhost` fra en container.

---

## 3. Tjek inde i web-containeren

```bash
docker exec -it gf2learn-gf2learndev-ku1zfp-web-1 curl -sI http://127.0.0.1:8080/health
```

Skal give `200`. Bekræfter at appen lytter inde i containeren.

---

## 4. Dokploy UI

- **Exposed port** i Dokploy skal matche **`WEB_HOST_PORT`** (fx `8031`).
- Compose og Dokploy må ikke mappe **forskellige** host-porte.
- Log «Detected: 0 mounts» er OK — det handler om volumes, ikke ports.

---

## 5. Valgfri: Traefik

Kun hvis du **senere** vil samme mønster som Dokploy-domains via `dokploy-traefik:80`.  
Ikke nødvendigt for simpel port exposure.

---

## Postgres

- App → `postgres:5432` (fast i compose)
- `password authentication failed` → ens `POSTGRES_PASSWORD` som ved volume-oprettelse, eller slet `pgdata` og deploy forfra
