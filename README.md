# GF2 Learn

Central læringsplatform til **Grundforløb 2 programmering** på Mercantec — C#, Git, opgaver og projekter.

**Produktion:** [learn.mags.dk](https://learn.mags.dk)

## Stack

- **Blazor Web App** (.NET 10) med Static SSR
- **Markdown + YAML frontmatter** i `content/` (versioneret i Git)
- **Markdig** til rendering med custom directives (`:::git-step`, `:::exercise`, osv.)
- **Mercantec Auth** ([auth.mercantec.tech](https://auth.mercantec.tech)) — OAuth 2.0 + PKCE
- **Docker** + Cloudflare Tunnel til deploy

## Tre sektioner

| Sektion | Indhold |
|---------|---------|
| **Pensum** | C#-teori og Git |
| **Opgaver** | Øvelser (begynder / mellem / avanceret) |
| **Projekter** | 7 forløb i tre niveauer (begynder → mellem → avanceret/AD) |

Pensum, opgaver og projekter er **offentligt tilgængelige**. `/profile` kræver login.

## Lokal udvikling

```bash
cd src/GF2Learn.Web
dotnet user-secrets set "MercantecAuth:ClientSecret" "dit-client-secret"
dotnet run --urls http://localhost:5288
```

Åbn **http://localhost:5083** (eller den port du bruger med `dotnet run --urls`)

## Mercantec Auth opsætning

### 1. Opret OAuth-klient i Auth Admin

Registrér klient `gf2-learn` med **præcis** disse redirect URIs:

| Miljø | Redirect URI |
|-------|----------------|
| Lokal (`dotnet run`) | `http://localhost:5083/signin-mercantec` |
| Docker lokal | `http://localhost:2020/signin-mercantec` |
| Dev (Cloudflare Tunnel) | `https://learn-dev.gf2.dk/signin-mercantec` |
| Produktion | `https://learn.mags.dk/signin-mercantec` |

### 2. Konfiguration

Miljøvariabler (se `.env.example`):

| Variabel | Beskrivelse |
|----------|-------------|
| `MercantecAuth__ClientId` | OAuth client_id (default: `gf2-learn`) |
| `MercantecAuth__ClientSecret` | Client secret (confidential client) |

JWT valideres mod `iss=https://auth.mercantec.tech` og `aud=mercantec-apps`.

### 3. Endpoints i appen

| Sti | Formål |
|-----|--------|
| `/auth/login` | Start OAuth-flow (PKCE S256) |
| `/signin-mercantec` | OAuth callback (automatisk) |
| `/auth/logout` | Log ud + redirect til auth signout |
| `/profile` | Brugerprofil (kræver login) |

### 4. Logout

Logout rydder app-cookie og sender brugeren til `https://auth.mercantec.tech/signout?returnUrl=...` for at nulstille auth-session (SSO).

## Indholdsstruktur

```
content/
  pensum/           # *.md — teori
  opgaver/          # begynder/, mellem/, avanceret/
  projekter/        # projekt-*/overview.md (ét dokument per projekt)
```

Reference-løsninger og afleveringskrav: `/projects/{slug}/solution` (Blazor). Konsol-demo-kode i `src/GF2Learn.Web/ProjectReferences/`.

### Frontmatter

```yaml
title: "Emnetitel"
order: 1
topics: [csharp, git]
kompetencemaal: ["Kan ..."]
timer: 2
difficulty: begynder    # kun opgaver
related_pensum: [02-variabler-og-datatyper]
youtube_id: "..."       # valgfri
```

### Custom directives

| Directive | Formål |
|-----------|--------|
| `:::callout type="info"` | Info/advarsel-boks |
| `:::git-step` | Git commit/branch instruktion |
| `:::exercise` | Opgavebeskrivelse |
| `:::solution` | Fold-ud løsning |
| `:::code-playground` | Interaktiv C#-playground (kører i browser via WASM). Valgfri `refs:` og `expected:` |
| `:::related-pensum` | Links til pensum |

### C# playground (browser)

På `/exercises/*` loades en WebAssembly-runtime (~15–25 MB, caches i browseren). Elever kan redigere og køre top-level C# med `Console.WriteLine` direkte på egen maskine.

Editoren er **Monaco** (samme motor som VS Code) med C#-syntax og snippets — fx skriv `cw` + **Tab** for `Console.WriteLine()`. **Ctrl+Space** viser flere snippets (`for`, `foreach`, `if`, …).

Ekstra NuGet-assemblies whitelistes i [`wwwroot/playground/playground.json`](src/GF2Learn.Web/wwwroot/playground/playground.json) — se [`wwwroot/playground/refs/README.md`](src/GF2Learn.Web/wwwroot/playground/refs/README.md).

```markdown
:::code-playground
refs: System.Text.Json
```csharp
var json = JsonSerializer.Serialize(new { navn = "Ada" });
Console.WriteLine(json);
```
expected: {"navn":"Ada"}
:::
```

Manuel test (efter `dotnet run`):

1. Åbn `/exercises/01-variabler` — gennemgå kapitel 1 og løs opgaverne i jeres konsolprojekt.
2. Indsæt syntaksfejl — compile-fejl vises i output-panelet.
3. Uendelig løkke — timeout efter 3 sekunder.
4. JSON med `System.Text.Json` uden ekstra refs — skal virke via BCL.

## Docker

**Lokal (Windows/macOS/Linux):**

```bash
cp .env.example .env   # udfyld secrets ved behov
docker compose -f docker-compose.yml -f docker-compose.local.yml up -d --build
```

| Tjeneste | URL |
|----------|-----|
| **Web** | http://localhost:8031 |
| **Health** | http://localhost:8031/health |

**Videnscenter-branding:** sæt `VidenscenterBranding__Enabled=true` i `.env` på VC-afleveringsversionen; `false` (standard) internt hos Mercantec uden VAT-logoer.

**Dokploy (prod/dev på server):** kun `docker-compose.yml` — kræver `dokploy-network` og Traefik.

OAuth redirect for lokal Docker (tilføj i Auth Admin hvis du tester login):

`http://localhost:8031/signin-mercantec`

Videnstjek-progression gemmes i Postgres ved login (migrationer kører automatisk ved opstart).

### Cloudflare Tunnel (dev)

Port exposure som andre apps — se [`deploy/dokploy-cloudflare.md`](deploy/dokploy-cloudflare.md).

| Cloudflare (host cloudflared) | `http://localhost:8031` |
| Cloudflare (cloudflared i Docker) | `http://host.docker.internal:8031` |

`WEB_HOST_PORT=8031` i Dokploy skal matche.

OAuth: `https://learn-dev.gf2.dk/signin-mercantec`
