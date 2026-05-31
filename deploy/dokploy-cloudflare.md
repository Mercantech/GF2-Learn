# Dokploy + Cloudflare Tunnel — fejlsøgning

Deploy-loggen viser at **Docker Compose kører OK** (`web-1 Started`). Hvis `https://learn-dev.gf2.dk` stadig fejler, er problemet næsten altid **netværk mellem Cloudflare og container** — ikke selve buildet.

## Arkitektur (skal passe)

```
Browser → Cloudflare (TLS) → cloudflared connector → origin på SAMME server → Docker port 2020 → web:8080
```

| Trin | Krav |
|------|------|
| 1 | **Dokploy** kører compose på serveren (fx `gf2learn-gf2learndev-ku1zfp-web-1`) |
| 2 | **cloudflared-connector** er **Healthy** og kører på **den samme maskine** som Dokploy |
| 3 | **Published application** origin peger på app på **den server** — ikke din lokale PC |

Hvis tunnelen i Cloudflare peger på `http://localhost:2020`, men connector kører på din laptop, mens Dokploy kører på en VPS → **det virker ikke**.

---

## Trin 1 — Test på Dokploy-serveren (SSH)

Kør på serveren hvor Dokploy deployer (ikke på din udviklings-PC):

```bash
curl -sI http://127.0.0.1:2020/health
```

**Forventet:** `HTTP/1.1 200 OK`

| Resultat | Betydning |
|----------|-----------|
| `200 OK` | App OK — fejlen er Cloudflare/connector/origin-URL |
| Connection refused | Port 2020 ikke eksponeret — se Dokploy ports / compose |
| Timeout | Container kører ikke eller crasher — `docker logs gf2learn-gf2learndev-ku1zfp-web-1` |

Tjek container og port:

```bash
docker ps --filter name=gf2learn-gf2learndev
docker port gf2learn-gf2learndev-ku1zfp-web-1
```

Skal vise `0.0.0.0:2020->8080/tcp` (eller `127.0.0.1:2020`).

---

## Trin 2 — Cloudflare Zero Trust (web app)

1. **Networks → Tunnels** → vælg tunnel → **Connectors** skal være **Healthy** på **Dokploy-serverens** hostname/IP.
2. **Published application routes** (eller Public Hostname):
   - Hostname: `learn-dev.gf2.dk`
   - Service: **HTTP**
   - URL: `http://127.0.0.1:2020` (prøv `127.0.0.1` i stedet for `localhost`)

### Hvis cloudflared kører i Docker (ikke som systemd på host)

`localhost` inde i cloudflared-containeren er **ikke** Dokploy-hosten. Brug i stedet:

| Setup | Origin URL |
|-------|------------|
| cloudflared på **host** (systemd-pakke) | `http://127.0.0.1:2020` |
| cloudflared i **Docker** på samme host | `http://host.docker.internal:2020` eller host-IP `http://172.17.0.1:2020` |
| cloudflared i **samme compose-netværk** | `http://web:8080` (service-navn fra `docker-compose.yml`) |

Dokploy-projektnetværk hedder typisk `gf2learn-gf2learndev-ku1zfp_default` — find med:

```bash
docker network ls | grep gf2learn
docker inspect gf2learn-gf2learndev-ku1zfp-web-1 --format '{{range $k,$v := .NetworkSettings.Networks}}{{$k}}{{end}}'
```

---

## Trin 3 — Dokploy-specifikt

- **Environment:** Sæt `MercantecAuth__ClientSecret` og `POSTGRES_PASSWORD` i Dokploy UI (`.env` committes ikke).
- **Postgres-port:** Brug **ikke** `POSTGRES_PORT=5422` i `.env`. Appen forbinder internt på **5432**. `5422` er kun host-port til pgAdmin/DBeaver.
- Fejl `postgres:5422` / `Connection refused` → fjern `POSTGRES_PORT` fra Dokploy-env eller sæt den til `5432`; connection string i compose bruger nu fast `5432`.
- **Ports:** Bekræft at compose-port `2020:8080` ikke overskrives. Nogle setups kræver eksplicit «Publish port 2020» i Dokploy.
- **Gendeploy** efter ændring af `.env` i Dokploy.

---

## Trin 4 — Typiske fejl i browseren

| Fejl | Sandsynlig årsag |
|------|-------------------|
| **502 Bad Gateway** | Connector kan ikke nå origin — forkert URL eller app lytter ikke på 2020 |
| **522 / timeout** | Ingen healthy connector, eller firewall |
| **Redirect loop** | HTTPS-redirect på app — sæt `GF2_BEHIND_REVERSE_PROXY=true` (i `docker-compose.yml`) |
| **DNS_PROBE** | `learn-dev.gf2.dk` ikke CNAME til tunnel i Cloudflare DNS |

---

## App-konfiguration (repo)

| Variabel | Formål |
|----------|--------|
| `GF2_BEHIND_REVERSE_PROXY=true` | Slår `UseHttpsRedirection` fra bag tunnel |
| `UseForwardedHeaders` | Korrekt `https` til OAuth/cookies |
| `GET /health` | Healthcheck for Docker og manuel test |

OAuth redirect: `https://learn-dev.gf2.dk/signin-mercantec`

---

## Hurtig beslutningstræ

```
curl http://127.0.0.1:2020/health på Dokploy-server
├─ Fejler → fix Docker/Dokploy ports & logs
└─ 200 OK → fix Cloudflare (connector på samme host, origin URL, DNS)
```
