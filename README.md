# GF2-Learn

Læringsplatform til **Grundforløb 2 på data** — [learn.mags.dk/gf2](https://learn.mags.dk/gf2).

## Stack

- Next.js 15 (App Router) med `basePath: /gf2`
- Pensum i **MDX** i `content/modules/`
- Valgfrit **live Notion** per lektion (`notionPageId` i frontmatter)
- YouTube-embed, sidebar-navigation, mørk/lys tilstand
- Docker (standalone) til Dokploy

## Lokal udvikling

```bash
npm install
cp .env.example .env.local   # udfyld NOTION_API_KEY hvis du tester Notion
npm run dev
```

Åbn **http://localhost:3000/gf2** (ikke kun `/` på roden).

### Fejl i dev (`ENOENT` / `Cannot find module './611.js'`)

Sker ofte hvis `.next` slettes mens `npm run dev` kører. Stop serveren (Ctrl+C), ryd cache, start igen:

```bash
npm run dev:clean
```

Eller manuelt: slet mappen `.next`, kør derefter `npm run dev`.

Åbn [http://localhost:3000/gf2](http://localhost:3000/gf2).

## Indholdsstruktur

```
content/modules/<modul-slug>/
  module.json          # titel, beskrivelse, rækkefølge
  lessons/
    01-velkommen.mdx   # frontmatter + MDX-brødtekst
```

### Frontmatter (GF2-forberedt)

| Felt | Beskrivelse |
|------|-------------|
| `title` | Lektionstitel |
| `module` | Modul-slug |
| `order` | Sortering i modulet |
| `fag` | Fag (udfydes i fase 2) |
| `kompetencemaal` | Liste af mål |
| `timer` | Antal timer |
| `laereplanRef` | Reference til læreplan |
| `youtubeId` | YouTube video-ID (valgfri) |
| `notionPageId` | Notion side-ID — live indhold når sat |

**Én kilde per lektion:** Hvis `notionPageId` er udfyldt, hentes indhold fra Notion; ellers vises MDX.

**Notion-links i indhold:** Interne links til andre Notion-sider peger på `/gf2/notion/<page-id>` (eller på den tilsvarende lektion, hvis den findes i `content/`).

### Opdel Notion-side efter overskrifter (sidebar)

Sæt i lektionens frontmatter:

```yaml
notionPageId: "..."
splitNotionByH1: true
```

Hver **Heading 1** (eller **Heading 2**, hvis der ikke findes H1) i Notion bliver et eget menupunkt under modulet. URL: `/gf2/modul/<modul>/<lektion>/<sektion-slug>`.

Krav i Notion: brug rigtige overskriftsblokke (Heading 1 / Heading 2) — fed tekst eller store fonte tæller ikke.

## Notion-opsætning

1. Opret en [internal integration](https://www.notion.so/my-integrations) i Notion.
2. Kopiér secret til `NOTION_API_KEY` i `.env.local` / Dokploy.
3. Del relevante sider med integrationen (⋯ → Connect to → din integration).
4. Side-ID findes i URL: `notion.so/.../<32-tegn-id>` (med eller uden bindestreger).
5. Sæt `notionPageId` i lektionens frontmatter.

## Docker / Dokploy

```bash
docker build -t gf2-learn .
docker run -p 3000:3000 -e NOTION_API_KEY=din-nøgle gf2-learn
```

I **Dokploy**: peg GitHub-repo, build med Dockerfile, port **3000**, env-vars fra `.env.example`.

**Cloudflare Tunnel**: peg `learn.mags.dk` mod containeren. Appen forventer at være tilgængelig under `/gf2` (via reverse proxy path eller direkte med basePath).

## Deploy-noter

- `output: "standalone"` — `content/` kopieres med i Docker-image, så MDX kan læses ved runtime.
- Auth kommer i en senere fase; platformen er offentlig i v1.

## Fase 2

Udfyld moduler og lektioner efter GF2 Data læreplan, rigtige YouTube-IDs og Notion-sider.
