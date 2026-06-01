---
title: Huskeliste
order: 20
difficulty: begynder
kompetencemaal:
  - "Kan bygge menu-drevne konsolprogrammer"
  - "Kan bruge lister til at gemme og filtrere data"
  - "Kan opdele kode i metoder"
timer: 6
related_pensum:
  - 02-variabler-og-datatyper
  - 05-loekker
  - 06-metoder
  - 07-datastrukturer
---

# Huskeliste i konsollen

Lav en **huskeliste** eller **to-do-liste** du selv vil bruge — fx skoleopgaver, indkøb eller GF2-øvelser. Alt kører i konsollen med en simpel menu.

:::callout type="info"
Samme sværhedsgrad som tidsfordrivsspil: fokus på **lister, loops og metoder** — bare uden spil-element.
:::

:::related-pensum
02-variabler-og-datatyper
05-loekker
06-metoder
07-datastrukturer
04-betingelser
:::

## Kernefunktioner

**Implementér:**

1. Menu med mindst: tilføj opgave, vis alle, markér som færdig, slet opgave, afslut
2. Gem opgaver i en `List<string>` (eller lille klasse med titel + `bool` færdig)
3. Vis nummererede opgaver så brugeren kan vælge hvilken der slettes/markeres

*Ekstra:* Filtrér kun åbne opgaver, eller sorter alfabetisk.

## Aflevering

Push til GitHub med README der beskriver menuen. Commit undervejs.

:::git-step
commit: "feat: huskeliste med menu og lister"
branch: main
:::
