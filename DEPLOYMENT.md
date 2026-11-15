# Drift af GF2 Learn

## Lokal Docker

Installér Docker med Compose 2.24.4 eller nyere. Kør kommandoerne fra projektroden.

1. Kopiér `.env.example` til `.env` (`Copy-Item .env.example .env` i PowerShell eller `cp .env.example .env` i bash). Vælg en lang, unik `POSTGRES_PASSWORD`, fx 32 tilfældige hextegn. Eksemplet vælger lokalt login.
2. Validér konfigurationen og start web og PostgreSQL.

   ```sh
   docker compose -f docker-compose.yml -f docker-compose.local.yml config --quiet
   docker compose -f docker-compose.yml -f docker-compose.local.yml up -d --build
   ```

3. Åbn [GF2 Learn](http://localhost:8031) og kontrollér [status](http://localhost:8031/health). Ved fejl viser `docker compose -f docker-compose.yml -f docker-compose.local.yml logs --tail=100 web postgres` opstarts- og migrationslogs.

Den lokale port er bundet til `127.0.0.1`. Første build henter containerimages, systempakker og NuGet-pakker. Browseren henter Monaco-editoren og skrifttyper fra CDN. Indlejrede YouTube-videoer og valgfri AI-hjælp bruger deres eksterne tjenester.

## Vælg login i `.env`

- `Auth__Mode=local` bruger installationens PostgreSQL. Brugeren opretter selv en konto med brugernavn, visningsnavn og adgangskode. Der er ingen mailopsætning. En driftsansvarlig kan nulstille en adgangskode fra den kørende webcontainer. Kommandoen spørger skjult efter den nye adgangskode.

  ```sh
  docker compose -f docker-compose.yml -f docker-compose.local.yml exec web dotnet GF2Learn.Web.dll local-auth reset-password BRUGERNAVN
  ```

- `Auth__Mode=central` bruger [Mercantec Auth](https://auth.mercantec.tech) og er standard, når variablen er udeladt. Sæt `MercantecAuth__ClientId` og `MercantecAuth__ClientSecret` for den registrerede klient. Registrér den præcise callback i Auth Admin, fx `https://learn.gf2.dk/signin-mercantec` eller `http://localhost:8031/signin-mercantec` til lokal test. Tilpas ved valg af en anden port eller et andet domæne.

Begge loginvalg logger kun brugeren ud af GF2 Learn. Underviser- og superadminadgang kan tildeles ved at sætte bruger-ID'et fra `/profile` i `AdminAccess__EducatorSubjects` eller `AdminAccess__SuperAdminSubjects`. Lokale bruger-ID'er starter med `local:`. Mercantec Auth-roller understøttes også i central tilstand. Kør opstartskommandoen for dit miljø efter ændringer i `.env`, så webcontaineren får den nye konfiguration.

Et skift af loginmetode giver brugerne adskilte konti og personlig progression. Hold og elevhistorik bliver i installationens database og kan fortsat ses af administratorer med adgang til den. Brug eget Compose-projektnavn (`-p`) og egne volumener pr. miljø. Volumenerne `pgdata` og `dataprotection` bevarer data og login-nøgler. Sikkerhedskopiér dem før ændringer i drift, og bevar dem ved almindelig genstart og opgradering. `POSTGRES_*` initialiserer en tom database. En ændring i `.env` roterer ikke adgangskoden på en eksisterende database.

## Dokploy og øvrige variabler

Brug `docker-compose.yml` i Dokploy med `dokploy-network`. Sæt installationens `.env` i Dokploy. Brug det valgte domæne med HTTPS via Traefik. Web forbinder til PostgreSQL på det private Compose-net. Ved password-reset i Dokploy bruges samme kommando fra Compose-projektet uden `-f docker-compose.local.yml`.

En HTTPS-proxy på værten kan også bruge den lokale variant. Proxyen sender trafik til `127.0.0.1` på `WEB_HOST_PORT` med det offentlige værtsnavn og HTTPS-skema og videresender WebSocket-trafik til Blazor.

| Variabel | Formål |
|---|---|
| `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD` | Database, bruger og adgangskode. Brug en separat, stærk adgangskode i hvert miljø. |
| `WEB_HOST_PORT` | Lokal Docker-port. Standard er `8031`. |
| `APP_DOMAIN`, `TRAEFIK_PREFIX` | Offentligt domæne og unikt Traefik-præfiks pr. Dokploy-app. |
| `VidenscenterBranding__Enabled` | Sæt `true` for VAT-logoer på platformen og slides. |
| `OpenAi__ApiKey`, `OpenAi__Model` | Valgfri AI-hjælp til opgaver. Lad nøglen være tom, hvis funktionen ikke skal bruges. |

Opbevar `.env` i installationen eller Dokploy. Del `.env.example` med afleveringen.
