---
title: "Fejlfinding"
order: 7
topics: [debug, exceptions]
timer: 2
---

# Fejlfinding

Brug **breakpoints** i IDE og `try/catch` til exceptions:

```csharp
try { /* kode */ }
catch (Exception ex) { Console.WriteLine(ex.Message); }
```
