---
title: Bibliotekssystem
order: 50
difficulty: mellem
kompetencemaal:
  - "Kan modellere data med klasser og objekter"
  - "Kan bruge lister af egne typer"
  - "Kan opdele ansvar mellem metoder"
timer: 10
related_pensum:
  - 09-oop-grundlaeg
  - 07-datastrukturer
  - 06-metoder
---

# Bibliotekssystem (OOP)

Lav et lille **bibliotek** i konsollen: bøger kan tilføjes, udlånes og returneres. Her træner du **klasser og objekter** — naturligt mellem binær omformer og enterprise-projektet.

:::related-pensum
09-oop-grundlaeg
07-datastrukturer
06-metoder
14-fejlfinding
:::

## Kernefunktioner

**Implementér:**

1. Klasse `Book` med fx titel, forfatter, `IsAvailable`
2. Klasse `Library` (eller statisk samling) med `List<Book>`
3. Menu: tilføj bog, vis alle, udlån bog, returnér bog
4. Fejlhåndtering: kan ikke udlåne bog der allerede er udlånt

*Ekstra:* Klasse `Loan` med udlånsdato, eller søg bøger på forfatter.

## Aflevering

README med klasse-diagram (kan tegnes i tekst). Git med tydelige commits.

:::git-step
commit: "feat: bibliotek med Book og Library"
branch: main
:::
