---
title: "Git — Branches & Merge"
order: 12
topics: [git, branches]
timer: 2
---

# Branches og merge

En **branch** er en parallel version af din kode. Du arbejder på nye features i separate branches, så `main` altid forbliver stabil — især vigtigt i studiegrupper.

## Hvad er en branch?

Forestil dig en tidslinje af commits. `main` er hovedtidslinjen — den version, der "tæller". En branch er en forgrening, hvor du kan eksperimentere uden at påvirke `main`:

```
main:     A --- B --- C --- F (merge)
               \
feature:        D --- E
```

Commit `D` og `E` lever på `feature`-branchen. Når featuren er færdig, **merges** den tilbage til `main` som commit `F`.


:::callout type="info"
En **branch** er en parallel version af koden. **Merge** samler ændringerne tilbage i `main`.
:::

## Oprette og skifte branch

```bash
git checkout -b feature/jobliste   # opret og skift til ny branch
# ... lav ændringer ...
git add .
git commit -m "feat: tilfoej jobliste"

git checkout main                  # skift tilbage til main
git merge feature/jobliste         # merge feature ind i main
```

I GitHub Desktop: **Branch → New branch**, lav ændringer, commit, skift til `main`, klik **Merge into current branch**.

Branch-navne bør beskrive featuren: `feature/login`, `fix/input-validering`, `docs/readme`.


## Merge og merge-konflikter

**Merge** kombinerer to branches. Git forsøger automatisk at flette ændringer — men hvis *samme linje* er ændret i begge branches, opstår en **merge-konflikt**:

```
<<<<<<< HEAD
Console.WriteLine("Hej main");
=======
Console.WriteLine("Hej feature");
>>>>>>> feature/jobliste
```

Løsning:

1. Åbn filen og vælg den rigtige kode (eller kombiner begge)
2. Fjern konflikt-markørerne (`<<<<`, `====`, `>>>>`)
3. `git add .` og `git commit -m "merge: loes konflikt i Program.cs"`

Konflikter er normale i teams — de betyder bare, at Git ikke kan gætte, hvad du vil beholde.


## Feature branch workflow

Standard workflow i GF2 og erhverv:

1. **Opret branch** fra opdateret `main`
2. **Arbejd** på featuren — commit ofte
3. **Push** branchen til GitHub
4. **Merge** til `main` (lokalt eller via Pull Request på GitHub)
5. **Slet** feature-branchen når den er merged

```bash
git checkout main
git pull                           # hent seneste fra GitHub
git checkout -b feature/ny-funktion
# ... arbejde og commits ...
git push -u origin feature/ny-funktion
git checkout main
git merge feature/ny-funktion
git push
```

`-u origin` linker din lokale branch til GitHub — derefter kan du bare `git push`.


## Samarbejde i studiegrupper

Branches gør det muligt at arbejde parallelt uden at overskrive hinanden:

- **Person A** arbejder på `feature/jobliste`
- **Person B** arbejder på `feature/bruger-login`
- Begge merger til `main`, når features er færdige

**Regler:**

- Pull altid seneste `main` før du opretter ny branch
- Kommuniker hvem der arbejder på hvad
- Løs konflikter sammen — det er en læringsmulighed
- Test efter merge — sørg for at `main` stadig kører

Dette workflow er identisk med det, I møder i praktik og job.


:::knowledge-check
---
q: Hvad er en **branch** i Git?
- En backup-fil på din computer
- En parallel version af koden — en forgrening fra hovedtidslinjen
- En commit-besked med flere linjer
correct: 1
explain: En **branch** lader dig arbejde på fx en feature uden at påvirke **`main`**. Når arbejdet er færdigt, **merges** det tilbage.
---
q: Hvad gør **`git checkout -b feature/navn`**?
- Sletter en eksisterende branch
- Opretter en ny branch og skifter til den med det samme
- Merger feature ind i main
correct: 1
explain: **`-b`** opretter branchen; **checkout** skifter til den. Derefter committer du på feature-branchen, ikke på `main`.
---
q: Hvad er en **merge-konflikt**?
- Når Git nægter at oprette en branch
- Når samme linje er ændret i begge branches — Git kan ikke vælge automatisk
- Når `git push` fejler pga. dårlig internetforbindelse
correct: 1
explain: Konflikter vises med markører som `<<<<<<< HEAD`. Du **løser manuelt** — vælg eller kombiner koden, fjern markørerne, `git add` og commit.
---
q: Hvad er **`main`** i feature branch workflow?
- En midlertidig branch der slettes efter hver commit
- Hovedtidslinjen — den stabile version, features merges ind i
- En fil der indeholder alle branches
correct: 1
explain: **`main`** er standard-hovedbranchen. Features udvikles på separate branches og merges til **`main`**, når de er testet og færdige.
---
q: Hvorfor bør du **`git pull`** på `main` før du opretter en ny branch?
- For at slette gamle commits
- For at hente seneste ændringer — så din nye branch starter fra opdateret kode
- Pull opretter automatisk en feature branch
correct: 1
explain: **`git pull`** synkroniserer med GitHub. Uden det kan din branch bygge på **forældet** `main` og give unødvendige konflikter ved merge.
---
q: Hvad gør **`git merge feature/jobliste`** når du står på `main`?
- Sletter feature-branchen
- Fletter ændringerne fra feature-branchen ind i den nuværende branch (`main`)
- Kopierer main til feature-branchen
correct: 1
explain: **Merge** kombinerer to branches. Du skal typisk stå på **`main`** (mål-branchen) og merge feature **ind i** den.
---
q: Hvad betyder **`-u origin`** ved `git push -u origin feature/ny-funktion`?
- Upload kun én fil
- Linker den lokale branch til GitHub — senere kan du bare skrive `git push`
- Tvinger en merge uden konflikt-tjek
correct: 1
explain: **Upstream (`-u`)** husker forbindelsen mellem lokal branch og remote. Efter første push med `-u` er `git push` nok til at sende nye commits.
:::
