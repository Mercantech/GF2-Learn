---
title: Projekt 3 — Programmering i et enterprise
order: 3
kompetencemaal:
  - "Kan forbinde til AD via LDAP fra C#"
  - "Kan hente og præsentere brugere og grupper"
  - "Kan automatisere og dokumentere processer i en større virksomhed"
timer: 12
related_pensum:
  - 13-ldap-active-directory
---

# Programmering i et enterprise

I dette projekt er fokus på at **automatisere**, **sikre** og **dokumentere** processer i en større virksomhed — med udgangspunkt i jeres **Active Directory (AD)**.

:::callout type="warning"
Commit **aldrig** rigtige adgangskoder til GitHub. Brug User Secrets eller miljøvariabler — se [LDAP & Active Directory](/curriculum/13-ldap-active-directory).
:::

:::related-pensum
13-ldap-active-directory
:::

## 1. Brugeroversigt i et AD

Lav et C#-program der med **LDAP** forbinder til jeres AD og henter alle jeres brugere.

**Implementér:**

- NuGet-pakke til LDAP (se pensum)
- Forbindelse med test-credentials fra undervisningsmiljøet
- Vis mindst navn, brugernavn og email

*Ekstra:* **Blazor-oversigt** med tabel (navn, email, afdeling) og evt. søgning.

:::git-step
commit: "feat: hent AD-brugere via LDAP"
branch: main
:::

## 2. Gruppeoversigt fra et AD

Udvid programmet til også at hente **alle grupper**.

**Implementér:**

- LDAP-søgning for grupper
- For hver gruppe: navn og medlemmer
- Strukturér data (fx `Dictionary` gruppe → liste af medlemmer)

*Ekstra:* Blazor-oversigt med alle medlemmer per gruppe, eller eksport til CSV.

:::git-step
commit: "feat: AD-grupper og medlemmer"
branch: main
:::

## Ekstra — Stempel ind og ud-system

Brugere **stempler ind/ud** ved at indtaste navn eller ID. Gem tidspunkt og vis hvem der er «på arbejde».

*Ekstra:* Integrér med AD (slå bruger op via LDAP) eller lav ID-kort-simulering.

:::git-step
commit: "feat: stempel ind/ud + dokumentation"
branch: main
:::

## Aflevering

Push til GitHub med README (LDAP-opsætning **uden** secrets) og kørselsvejledning.

:::callout type="success"
Alle tre dele kan ligge i samme solution — konsol eller Blazor efter jeres valg.
:::
