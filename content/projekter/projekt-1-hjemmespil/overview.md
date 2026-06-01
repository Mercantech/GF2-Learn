---
title: Projekt 1 — Tidsfordrivsspil
order: 1
kompetencemaal:
  - "Kan bygge interaktive konsolprogrammer med brugerinput"
  - "Kan bruge variabler, metoder og lister i praksis"
  - "Kan gennemføre projektforløb med Git"
timer: 12
related_pensum:
  - 01-introduktion
  - 02-variabler-og-datatyper
  - 06-metoder
  - 07-datastrukturer
---

# Introduktion til programmering — Små tidsfordrivsspil til hjemmekontoret

Når man arbejder hjemmefra, kan det være rart med en lille pause — og hvad er bedre end at lave sine egne små spil? Her er tre klassiske spil, du kan lave direkte i konsollen. Du må også meget gerne finde på dine egne spil!

:::callout type="info"
Arbejd i dit GitHub-repo og commit undervejs med tydelige beskeder. Brug **variabler, metoder og lister** — det er kernen i dette projekt.
:::

:::related-pensum
01-introduktion
02-variabler-og-datatyper
06-metoder
07-datastrukturer
05-loekker
04-betingelser
:::

## 1. Gæt et tal

Computeren vælger et tilfældigt tal mellem 1 og 100. Du skal gætte tallet. Hver gang du gætter, får du at vide om det rigtige tal er **højere** eller **lavere**. Spillet fortsætter, indtil du gætter rigtigt.

**Implementér:**

- Opret konsolprojekt, fx `Hjemmespil`
- Brug `Random` og en løkke til gæt
- Tæl antal forsøg og vis det til sidst

*Ekstra:* Gem alle dine gæt i en `List<int>` og vis listen, når spillet er slut.

:::git-step
commit: "feat: gæt-et-tal spil"
branch: main
:::

## 2. Sten, saks, papir

Du spiller mod computeren. Hver runde vælger både du og computeren enten **sten**, **saks** eller **papir**. Programmet afgør, hvem der vinder runden, og holder styr på pointene.

**Implementér:**

- Valider brugerens input
- Metode der afgør vinderen af én runde
- Spørg om spilleren vil spille igen

*Ekstra:* Lav spillet så man kan spille flere runder og se den **samlede score** til sidst.

:::git-step
commit: "feat: sten-saks-papir"
branch: main
:::

## 3. Tic-Tac-Toe

To spillere (eller dig mod computeren) skiftes til at sætte **X** og **O** på et 3×3 bræt. Den første der får tre på stribe vinder. Spillet vises i konsollen, og man vælger hvor man vil sætte sit symbol.

**Implementér:**

- Vis 3×3 bræt i konsollen
- Skift tur mellem X og O; afvis ugyldige træk
- Stop ved vinder eller uafgjort

*Ekstra:* Gem brættet som en **liste af lister** (eller 2D-array), og lav metoder til at placere symbol og tjekke for vinder.

:::git-step
commit: "feat: tic-tac-toe"
branch: main
:::

## 4. Find på dit eget spil!

Du må meget gerne finde på dit **eget** lille spil eller tidsfordriv i konsollen — fx quiz, regnespil eller hangman. Brug fantasien!

**Aflevering:** README i repo, push til GitHub og del link. Commit-beskeder skal vise forløbet.

:::git-step
commit: "feat: eget konsolspil + README"
branch: main
:::
