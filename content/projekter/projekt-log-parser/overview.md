---
title: Log-parser
order: 60
difficulty: mellem
kompetencemaal:
  - "Kan læse og skrive tekstfiler"
  - "Kan parse linjer og tælle forekomster"
  - "Kan håndtere fejl ved filadgang"
timer: 8
related_pensum:
  - 07-datastrukturer
  - 09-oop-grundlaeg
  - 14-fejlfinding
  - 05-loekker
---

# Log-parser

Virksomheder og servere gemmer **logfiler** med hændelser. Lav et program der læser en logfil (eller en fil I selv opretter) og viser **statistik** — fx antal fejl, advarsler og info-linjer.

:::callout type="tip"
Brug en simpel format I selv definerer, fx `[ERROR] Bruger kunne ikke logge ind` pr. linje — så slipper I for rigtige serverlogs.
:::

:::related-pensum
07-datastrukturer
14-fejlfinding
05-loekker
06-metoder
:::

## Kernefunktioner

**Implementér:**

1. Læs en `.txt`-fil linje for linje (`File.ReadAllLines` eller `StreamReader`)
2. Tæl hvor mange linjer der indeholder fx `ERROR`, `WARN`, `INFO` (eller jeres egne tags)
3. Vis en kort rapport i konsollen
4. `try/catch` hvis filen ikke findes

*Ekstra:* Skriv rapporten til en ny fil, eller vis de 5 seneste fejl-linjer.

## Aflevering

Medlevér en **eksempel-logfil** i repo (uden persondata). Push til GitHub.

:::git-step
commit: "feat: log-parser med fil og statistik"
branch: main
:::
