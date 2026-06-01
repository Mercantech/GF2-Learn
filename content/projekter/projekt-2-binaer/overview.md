---
title: Binær kodeomformer
order: 2
kompetencemaal:
  - "Kan konvertere binær ↔ decimal uden indbyggede konverteringsfunktioner"
  - "Kan arbejde med 8-bit grupper og 4 oktetter"
  - "Kan bruge if/else og loops til algoritmer"
timer: 9
related_pensum:
  - 08-binaer-og-ip
  - 10-iteration
  - 05-loekker
  - 04-betingelser
---

# Binær kodeomformer

I har nok noget kendskab til det binære talsystem og hvordan man omregner dem. I dette projekt laver I en **omformer**, der kan tage en binær talgruppe med længde **4** og omdanne den til heltal — og omvendt.

**Eksempel:** `10111011.01001011.10101010.01010101` bliver til **187.75.170.85**

## Krav

- Talgruppe på **længde 4**, med **binære tal af længde 8** (som i eksemplet).
- **Begge veje:** binær → decimal og decimal → binær.
- I må **ikke** bruge indbyggede funktioner, der omformer for jer (fx `Convert.ToString(…, 2)` eller `Convert.ToInt32(…, 2)`).
- Brug **if/else** og **loops**.

:::callout type="info"
**Hjælp**

- [Learn Binary Conversions](https://www.rapidtables.com/convert/number/binary-to-decimal.html)
- [Binary Game](https://learningapps.org/view1941279)
- Pensum: [Binære tal og IP-adresser](/curriculum/08-binaer-og-ip)
:::

:::related-pensum
08-binaer-og-ip
10-iteration
05-loekker
04-betingelser
:::

## Trin 1 — Binær til decimal (én oktet)

Konverter én 8-tegns binær streng til decimal **uden** `Convert`. Validér input (kun `0` og `1`, præcis 8 tegn).

**Test:** `10111011` → `187`, `01001011` → `75`

*Ekstra:* Metode `int BinaerTilDecimal(string binaer)`.

## Trin 2 — Decimal til binær (én oktet)

Konverter 0–255 til 8-bit binær streng **uden** `Convert`. Pad med foranstillede nuller.

*Ekstra:* Metode `string DecimalTilBinaer(int decimal)` og lille menu.

## Trin 3 — Fuld streng (4 oktetter)

Parse `10111011.01001011.10101010.01010101` ↔ `187.75.170.85` med `Split('.')` og jeres metoder fra trin 1–2. Menu for retning. Håndter ugyldigt input.

:::git-step
commit: "feat: fuld 4-oktet omformer begge veje"
branch: main
:::

## Aflevering

README med eksempler, test edge cases (`00000000`, `11111111`). Push til GitHub.

*Valgfri:* Blazor-UI med tekstfelter — stadig med **egne** omregningsmetoder, ikke `Convert`.

:::callout type="tip"
Konsolversionen er **obligatorisk**; Blazor er valgfri udvidelse.
:::
