---
title: Jobsøgning-tracker
order: 30
difficulty: begynder
kompetencemaal:
  - "Kan strukturere data i lister og søge i dem"
  - "Kan validere brugerinput"
  - "Kan dokumentere et lille program i README"
timer: 8
related_pensum:
  - 04-betingelser
  - 06-metoder
  - 07-datastrukturer
---

# Jobsøgning-tracker

Hold styr på **stillingsopslag** du søger — virksomhed, titel, status og evt. deadline. Et praktisk konsolprojekt der træner samme C#-grundlag som spil og huskeliste.

:::related-pensum
04-betingelser
06-metoder
07-datastrukturer
02-variabler-og-datatyper
:::

## Kernefunktioner

**Implementér:**

1. Tilføj stilling (firma, jobtitel, status fx *Ansøgt* / *Afventer* / *Afvist*)
2. Vis alle stillinger i overskuelig liste
3. Søg eller filtrér på firma eller status
4. Opdater status på en eksisterende stilling

*Ekstra:* Gem antal dage siden ansøgning, eller eksportér som simpel tekstfil.

## Aflevering

- **GitHub-repo** med README (skærmbillede eller eksempel-output fra konsollen)
- **Konsol-app** med menu og `Console.ReadLine()` — se [løsning & aflevering](/projects/projekt-jobsoegning/solution) for kørbar demo
- **Blazor** (valgfri/anbefalet ekstra) — samme data i et pænere UI

:::callout type="tip"
Åbn **Se løsning & aflevering** på projektsiden for reference til konsol (med simuleret input) og Blazor-demos.
:::

:::git-step
commit: "feat: jobsøgning-tracker"
branch: main
:::
