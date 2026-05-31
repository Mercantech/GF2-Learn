# Cloudflare Tunnel — dev (GF2-Learn)

Opsætning styres via **Cloudflare Zero Trust / Dashboard** (web app) — ikke nødvendigvis en lokal `config.yml`.

## Mapping (nuværende)

| Felt i Cloudflare | Værdi |
|-------------------|--------|
| **Public hostname** | `learn-dev.gf2.dk` |
| **Service type** | Published application |
| **URL / origin** | `http://localhost:2020` |

På serveren skal Docker køre:

```bash
docker compose up -d
```

Appen er tilgængelig lokalt på **http://localhost:2020** og offentligt på **https://learn-dev.gf2.dk**.

## Mercantec OAuth

Tilføj redirect URI i Auth Admin:

`https://learn-dev.gf2.dk/signin-mercantec`

Appen læser `X-Forwarded-Proto` / `X-Forwarded-Host` fra tunnelen, så login og cookies virker over HTTPS.

## Fejlsøgning

| Symptom | Tjek |
|---------|------|
| 502 / connection refused | Kører `docker compose`? Er origin **http://localhost:2020** (ikke 8080 på host)? |
| Login fejler / forkert callback | Er redirect URI registreret for `learn-dev.gf2.dk`? Genbyg web efter `UseForwardedHeaders` i `Program.cs`. |
| Kun HTTP lokalt | Forventet — TLS håndteres af Cloudflare foran tunnelen. |

## Valgfri: CLI-config

Hvis du senere vil køre `cloudflared` manuelt i stedet for connector fra dashboard, se [`cloudflared.config.example.yml`](cloudflared.config.example.yml).
